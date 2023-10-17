using System;

namespace TB_mu2e
{

    class HistoHelper
    {
        //----------------------------------------------
        // TODO: 
        //----------------------------------------------

        //In getting histograms, you get 8 histograms simultaneously (1 for each AFE, which is 2 per FPGA)
        //They will always arrive in ascending order in the array that is returned, i.e. if asked for ch. 16, it will return [0, 8, 16, 24, 32, 40, 48, 56], so it will be 16/8 = 2nd in the array

        //Datamembers here
        private const int num_bins = 1024;
        private Mu2e_FEB_client febClient;
        public byte Ext_mode { get; set; }
        private ushort accumulation_interval;
        private readonly ushort[] base_addrs = { 0x311 /*0x10*/, //histo_control,  
                                                    0x11,           //histo_accum_inter (fpga0),
                                                    0x411,          //histo_accum_inter (fpga1), 
                                                    0x811,          //histo_accum_inter (fpga2),
                                                    0xC11 };        //histo_accum_inter (fpga3)

        private readonly ushort[] read_addrs = { 0x16,  0x17,    // afe0_dataPort (fpga0), afe1_dataPort (fpga0),
                                                 0x416, 0x417,   // afe0_dataPort (fpga1), afe1_dataPort (fpga1),                                                     
                                                 0x816, 0x817,   // afe0_dataPort (fpga2), afe1_dataPort (fpga2),                                                     
                                                 0xC16, 0xC17 }; // afe0_dataPort (fpga3), afe1_dataPort (fpga3)                                                      

        private readonly ushort[] write_addrs = { 0x14,  0x15,    // afe0_memRdPtr0 (fpga3), afe1_memRdPtr1 (fpga3)
                                                  0x414, 0x415,   // afe0_memRdPtr0 (fpga0), afe1_memRdPtr1 (fpga0),
                                                  0x814, 0x815,   // afe0_memRdPtr0 (fpga1), afe1_memRdPtr1 (fpga1),
                                                  0xC14, 0xC15 }; // afe0_memRdPtr0 (fpga2), afe1_memRdPtr1 (fpga2),
                                                                                                                         
        private Mu2e_Register[] histo_controls; 
        private Mu2e_Register[] write_regs;

        public HistoHelper()
        {
        }

        public HistoHelper(ref Mu2e_FEB_client febClient, ushort accumulation_interval = 0xFFF,  bool ext_mode = false)
        {
            this.febClient = febClient;
            this.accumulation_interval = accumulation_interval;
            Ext_mode = (ext_mode) ? (byte)0x10 : (byte)0x0;//if the mode is set to be qualified by an external gate, turn on the mode bit for the histo control register
            histo_controls = new Mu2e_Register[base_addrs.Length];//(base_addrs.Length * 4) - 4]; //*4 for the 4 fpgas, -4 because 0x311 is a broadcast to all fpga histo control registers
            write_regs = new Mu2e_Register[write_addrs.Length];
            GetRegisters();
        }

        public ROOTNET.NTH1I[] GetHistogram(uint channel, string suffix = "")
        {
            channel %= 8; //requested channel mod 8 (since each AFE only cares about the 8 channels connected to it)

            SetRegisters(); //Preps the FEB (re-sets accumulation interval and resets the read pointers)
            uint histCntrl_Dark_Base = 0x60 + (uint)Ext_mode + channel; // This is the "on" command for the histogramming and setting for gated histograms and the channel requested

            //Start the histogramming and wait for it to finish (very quick)
            Mu2e_Register.WriteReg(histCntrl_Dark_Base, ref histo_controls[0], ref febClient.client); 
            System.Threading.Thread.Sleep(accumulation_interval + 250); //small bit of time buffer added to the interval
            
            //Get and unpack the data
            UInt32[][] histogramBinContents = ParseData(ReceiveData(channel));

            //Fill the histograms
            ROOTNET.NTH1I[] histo = new ROOTNET.NTH1I[read_addrs.Length];
            for (uint i = 0; i < read_addrs.Length; i++)
            {
                try
                {
                    histo[i] = new ROOTNET.NTH1I("Ch" + ((8 * i) + channel).ToString() + suffix, ((8 * i) + channel).ToString(), num_bins, 0, num_bins); //First bin is underflow, last bin is overflow
                    for (uint binIndex = 0; binIndex < histogramBinContents[i].Length; binIndex++)
                    {
                        histo[i].SetBinContent((int)binIndex+1, histogramBinContents[i][binIndex]);
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
            byte[][] packs = new byte[read_addrs.Length][];
            //uint[,] addresses = AFE_ChannelAddresses(requested_channel); //first 'address' is the fpga register address for readout of the histogram, second is the fpga number, third is the channel number on that fpga
            System.Net.Sockets.Socket febSocket = febClient.TNETSocket_prop;

            for (int packNum = 0; packNum < read_addrs.Length; packNum++)
            {
                int lret, old_available = 0;
                do
                {
                    old_available = 0;
                    if (febSocket.Available > 0)
                        febSocket.Receive(new byte[febSocket.Available], febSocket.Available, System.Net.Sockets.SocketFlags.None); //Receive any junk it was previously trying to send
                    ResetReadAddresses(); //Set the read-pointer addresses back to 0

                    //400 to 800 for new "Big histogramming"
                    febSocket.Send(PP.GetBytes("rdbr " + read_addrs[packNum].ToString("X") + " 800\r\n")); //This is a request command for the FEB to perform 2048 (1024) reads at the specified address, and send that data in binary 
                    System.Threading.Thread.Sleep(10); //Pause for a moment to allow the data to show up in the send buffer
                    while (febSocket.Available > old_available) //Wait until the FEB has all the data to send
                    {
                        old_available = febSocket.Available;
                        System.Threading.Thread.Sleep(10);
                    }
                    packs[packNum] = new byte[febSocket.Available];
                    lret = febSocket.Available;
                    IAsyncResult rResult = febSocket.BeginReceive(packs[packNum], 0, packs[packNum].Length, System.Net.Sockets.SocketFlags.None, null, null);
                    rResult.AsyncWaitHandle.WaitOne(1000);
                    if (!rResult.IsCompleted)
                        Console.WriteLine("Reached timeout when reading histogram");
                    System.Console.WriteLine("Expected {0} bytes for hitogram", lret);
                    //lret = febSocket.Receive(packs[packNum], packs[packNum].Length, System.Net.Sockets.SocketFlags.None);
                    //System.Console.WriteLine("Read: {0} / {1} bytes", lret, old_available);
                } while (lret != old_available || old_available != 4*num_bins); //The 4 is because 32-bit bins -> 4 bytes per bin
            }

            return packs;
        }

        private UInt32[][] ParseData(byte[][] incomingData)
        {
            UInt32[][] data = new UInt32[read_addrs.Length][]; //32-bit numbers are 4-bytes, so the resulting array will be a factor of 4 shorter 
            for (int packNum = 0; packNum < read_addrs.Length; packNum++)
            {
                //while (incomingData[packNum][0] == 0x3e)
                //    incomingData[packNum] = incomingData[packNum].Skip(1).ToArray(); //Get rid of any 3e (>) at the beginning
                //if(incomingData[packNum].Last() == 0x3e)
                //    incomingData[packNum] = incomingData[packNum].Take(incomingData[packNum].Length - 1).ToArray(); //Get rid of the 3e (>) at the end

                if (incomingData[packNum].Length == 4096/*2048*//*512*/) //Since the histogram memory is 512 deep by 32 bits wide, we should quickly check that the data length is really be 512 integers... you know, just in case....
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

        public ushort GetAccumulation_interval() { return accumulation_interval; }

        public void SetAccumulation_Interval(ushort value)
        {
            accumulation_interval = value;
            for(int i = 1; i < histo_controls.Length; i++)
                Mu2e_Register.WriteReg(accumulation_interval, ref histo_controls[i], ref febClient.client); //if we update the accumulation interval in the class, we need to update the registers on the FEB
        }

        //Fills the histogram-related register list 
        private void GetRegisters()
        {
            for (int i = 0; i < base_addrs.Length; i++)
                Mu2e_Register.FindAddr(base_addrs[i], ref febClient.arrReg, out histo_controls[i]); //Histogram controls
            for (int k = 0; k < write_addrs.Length; k++)
                Mu2e_Register.FindAddr(write_addrs[k], ref febClient.arrReg, out write_regs[k]); //write registers
        }

        private void SetRegisters()
        { 
            for(int i = 1; i < histo_controls.Length; i++)
                Mu2e_Register.WriteReg(accumulation_interval, ref histo_controls[i], ref febClient.client); //Set the accumulation interval
            ResetReadAddresses();
        }

        private void ResetReadAddresses()
        {
            for (int j = 0; j < write_regs.Length; j++)
                Mu2e_Register.WriteReg(0x0, ref write_regs[j], ref febClient.client); //Set the read pointers back to 0
        }

    }
}
