using System;
using System.Linq;

namespace TB_mu2e
{
    class HistoHelper
    {
        //Datamembers here
        private Mu2e_FEB_client febClient;
        public byte Ext_mode { get; set; }
        private ushort accumulation_interval;
        private static ushort[] base_controls = { 0x10, 0x11, 0x14, 0x15, 0x16, 0x17 }; //bases for the histogram registers
        private Mu2e_Register[] histo_controls; //histo_control, histo_accum_inter, afe0_memRdPtr0, afe1_memRdPtr1, afe0_dataPort, afe1_dataPort

        public HistoHelper()
        {
        }

        public HistoHelper(ref Mu2e_FEB_client febClient, ushort accumulation_interval = 0xFFF,  bool ext_mode = false)
        {
            this.febClient = febClient;
            this.accumulation_interval = accumulation_interval;
            Ext_mode = (ext_mode) ? (byte)0x10 : (byte)0x0;//if the mode is set to be qualified by an external gate, turn on the mode bit for the histo control register
            histo_controls = new Mu2e_Register[base_controls.Length];
        }

        public ROOTNET.NTH1I[] GetHistogram(uint channel, byte binning)
        {
            //Get the registers for the requested channel
            GetRegisters(channel);
            
            //Get the read/write addresses which are important for starting and reading the histograms 
            uint[,] channels = AFE_ChannelAddresses(channel);  
            //0x60
            uint histCntrl_Dark_Base = 0x60 + (uint) Ext_mode + channels[0,4]; // This turns on the histogramming with the defined mode
            //binning should only be 1, 2, 4, or 8, since those are the only supported histogram bin widths
            switch (binning)
            {
                case 1:
                    histCntrl_Dark_Base += 0x0;
                    break;
                case 2:
                    histCntrl_Dark_Base += 0x500;
                    break;
                case 4:
                    histCntrl_Dark_Base += 0xA00;
                    break;
                case 8:
                    histCntrl_Dark_Base += 0xF00;
                    break;
                default:
                    histCntrl_Dark_Base += 0x0;
                    break;
            }

            //Get the registers specific to the requested channel
            SetRegisters();

            //Start the histogramming and wait for it to finish (very quick)
            Mu2e_Register.WriteReg(histCntrl_Dark_Base, ref histo_controls[0], ref febClient.client);
            System.Threading.Thread.Sleep(accumulation_interval + 250); //small bit of time buffer added to the interval
            
            //Get and unpack the data
            UInt32[][] histogramBinContents = ParseData(ReceiveData(channel));

            //Fill the histograms
            ROOTNET.NTH1I[] histo = new ROOTNET.NTH1I[2];
            for (uint i = 0; i < 2; i++)
            {
                try
                {
                    histo[i] = new ROOTNET.NTH1I("Ch" + channels[i, 3].ToString(), channels[i, 3].ToString(), 512, 0, binning * 512);
                    for (uint binIndex = 0; binIndex < histogramBinContents[i].Length; binIndex++)
                    {
                        histo[i].SetBinContent((int)binIndex, histogramBinContents[i][binIndex]);
                    }
                    histo[i].GetXaxis().SetTitle("ADC");
                    histo[i].GetYaxis().SetTitle("N");
                }
                catch(NullReferenceException)
                {
                    //leave it as an empty histogram
                    histo[i].GetXaxis().SetTitle("ADC");
                    histo[i].GetYaxis().SetTitle("N");

                }
            }
            
            //Return the histograms
            return histo;
        }

        private byte[][] ReceiveData(uint requested_channel)
        {
            byte[][] packs = new byte[2][];
            uint[,] addresses = AFE_ChannelAddresses(requested_channel); //first 'address' is the fpga register address for readout of the histogram, second is the fpga number, third is the channel number on that fpga
            System.Net.Sockets.Socket febSocket = febClient.TNETSocket_prop;

            for (int packNum = 0; packNum < 2; packNum++)
            {
                int lret, old_available = 0;
                do
                {
                    old_available = 0;
                    if (febSocket.Available > 0)
                        febSocket.Receive(new byte[febSocket.Available], febSocket.Available, System.Net.Sockets.SocketFlags.None); //Receive any junk it was previously trying to send
                    ResetReadAddresses(); //Set the read-pointer addresses back to 0
                    febSocket.Send(PP.GetBytes("rdbr " + addresses[packNum, 0].ToString("X") + " 400\r\n")); //This is a request command for the FEB to perform 1024 reads at the specified address, and send that data in binary 
                    System.Threading.Thread.Sleep(100); //Pause for a moment to allow the data to show up in the send buffer
                    while (febSocket.Available > old_available) //Wait until the FEB has all the data to send
                    {
                        old_available = febSocket.Available;
                        System.Threading.Thread.Sleep(50);
                    }
                    packs[packNum] = new byte[febSocket.Available];
                    lret = febSocket.Receive(packs[packNum], packs[packNum].Length, System.Net.Sockets.SocketFlags.None);
                    System.Console.WriteLine("Read: {0} / {1} bytes", lret, old_available);
                } while (lret != old_available || old_available < 2048 || old_available > 2060); //2048 is the minimum byte length for an incoming histogram; each one is actually >=2049 due to the '>' sent by the board, also need to check if >2048 because sometimes the borad likes to send both histograms in one packet
            }

            return packs;
        }

        private UInt32[][] ParseData(byte[][] incomingData)
        {
            UInt32[][] data = new UInt32[2][]; //32-bit numbers are 4-bytes, so the resulting array will be a factor of 4 shorter 
            for (int packNum = 0; packNum < 2; packNum++)
            {
                while (incomingData[packNum][0] == 0x3e)
                    incomingData[packNum] = incomingData[packNum].Skip(1).ToArray(); //Get rid of any 3e (>) at the beginning
                incomingData[packNum] = incomingData[packNum].Take(incomingData[packNum].Length - 1).ToArray(); //Get rid of the 3e (>) at the end

                if (incomingData[packNum].Length == 2048/*512*/) //Since the histogram memory is 512 deep by 32 bits wide, we should quickly check that the data length is really be 512 integers... you know, just in case....
                {
                    data[packNum] = new UInt32[incomingData[packNum].Length / 4];
                    uint byteIndex = 0;
                    while (byteIndex < incomingData[packNum].Length)
                    {   //Due to the goofy way the data is delivered, this shifts the bytes into their respective positions within the 32-bit integer (Dword)
                        data[packNum][byteIndex / 4] = (UInt32)((incomingData[packNum][byteIndex++] * 0x10000)  //Lower half of the upper word
                                                     + (incomingData[packNum][byteIndex++] * 0x1000000)         //Upper half of the upper word
                                                     + (incomingData[packNum][byteIndex++])                     //Lower half of the lower word
                                                     + (incomingData[packNum][byteIndex++] * 0x100));           //Upper half of the lower word
                    }
                }
            }

            return data;
        }


        //Returns the histogram readout address for the requested channel and its complement on a given FPGA.
        //Also returns the FPGA and 'local FPGA channel'
        private uint[,] AFE_ChannelAddresses(uint requested_channel)
        {
            uint[,] addresses = new uint[2,5]; //first row is for the requested channel, second row is for the channel complement
            uint mapped_channel = requested_channel % 16,
                fpga = requested_channel / 16;
            if (mapped_channel > 7)
            {
                addresses[0, 0] = histo_controls[5].addr; //afe1_dataPort.addr; AFE read address for requested channel
                addresses[1, 0] = histo_controls[4].addr; //afe0_dataPort.addr; AFE read address for complement channel
                addresses[1, 2] = mapped_channel - 8; //The local FPGA channel number for the complement channel
            }
            else
            {
                addresses[0, 0] = histo_controls[4].addr; //afe0_dataPort.addr; AFE read address for requested channel
                addresses[1, 0] = histo_controls[5].addr; //afe1_dataPort.addr; AFE read address for complement channel
                addresses[1, 2] = mapped_channel + 8; //The local FPGA channel number for the complement channel
            }

            addresses[0, 1] = addresses[1,1] = fpga; //FPGA the channels are on
            addresses[0, 2] = mapped_channel; //The 'local FPGA channel'
            addresses[0, 3] = requested_channel; //Giving back the requested channel
            addresses[1, 3] = (fpga * 16) + addresses[1, 2]; //Actual complement channel
            addresses[0, 4] = addresses[1, 4] = mapped_channel % 8; //AFE channel mapping

            return addresses;
        }

        public ushort GetAccumulation_interval() { return accumulation_interval; }

        public void SetAccumulation_Interval(ushort value)
        {
            accumulation_interval = value;
            Mu2e_Register.WriteReg(accumulation_interval, ref histo_controls[1], ref febClient.client); //if we update the accumulation interval in the class, we need to update the register on the FEB
        }

        //Fills the histogram-related register list 
        private void GetRegisters(uint requested_channel)
        {
            uint fpga = requested_channel / 16;
            for (int i = 0; i < base_controls.Length; i++)
                Mu2e_Register.FindAddrFPGA(base_controls[i], fpga, ref febClient.arrReg, out histo_controls[i]); //Histogram control
        }

        private void SetRegisters()
        { 
            Mu2e_Register.WriteReg(accumulation_interval, ref histo_controls[1], ref febClient.client); //Set the accumulation interval
            Mu2e_Register.WriteReg(0x0, ref histo_controls[2], ref febClient.client); //Set the read pointers back to 0
            Mu2e_Register.WriteReg(0x0, ref histo_controls[3], ref febClient.client); //Set the read pointers back to 0
        }

        private void ResetReadAddresses()
        {
            Mu2e_Register.WriteReg(0x0, ref histo_controls[2], ref febClient.client); //Set the read pointers back to 0
            Mu2e_Register.WriteReg(0x0, ref histo_controls[3], ref febClient.client); //Set the read pointers back to 0
        }

    }
}
