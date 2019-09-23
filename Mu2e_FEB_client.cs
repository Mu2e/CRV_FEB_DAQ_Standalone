using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using System.Threading.Tasks;
using System.Net.Sockets;
using System.IO;
using System.Threading;
using System.ComponentModel;
using ZedGraph;

namespace TB_mu2e
{
    public class Mu2e_FEB_client : IStenComms
    {
        #region InterfaceStenComms
        public string m_prop { get { return m; } set { m = value; } }
        public string name { get { return _logical_name; } set { _logical_name = value; } }
        public string host_name_prop { get { return _host_name; } set { _host_name = value; } }
        public TcpClient client_prop { get { return client; } }
        public Socket TNETSocket_prop { get { return TNETSocket; } }
        public NetworkStream stream_prop { get { return stream; } }
        public StreamReader SR_prop { get { return SR; } }
        public StreamWriter SW_prop { get { return SW; } }
        public int max_timeout_prop { get { return max_timeout; } set { max_timeout = value; } }
        public int timeout_prop { get { return timeout; } set { timeout_prop = timeout; } }
        bool IStenComms.ClientOpen { get { return _ClientOpen; } set { } }

        public void DoOpen() { Open(); }
        #endregion InterfaceStenComms

        public string m;
        private string _host_name;
        private string _logical_name;
        private bool _ClientOpen = false;
        private bool _ClientBusy = true;
        private string _TNETname;

        public int _TNETsocketNum = 5000;
        public TcpClient client;
        public Socket TNETSocket;
        public NetworkStream stream;
        public StreamReader SR;
        public StreamWriter SW;
        public int max_timeout;
        public int timeout;
        public List<Mu2e_Register> arrReg;
        public int clientNum;

        // events 
        //public delegate void cOpening();
        //public event cOpening ClientOpening;

        public Mu2e_FEB_client() //construct
        {
            AddRegisters.Add_FEB_reg(out arrReg);
        }

        public bool ClientOpen { get { return _ClientOpen; } }
        public bool ClientBusy { get { return _ClientBusy; } set { _ClientBusy = value; } }
        public int TNETsocketNum { get { return _TNETsocketNum; } set { _TNETsocketNum = value; } }


        public void Open()
        {
            if (!_ClientOpen)
            {
                try
                {
                    client = new TcpClient();//_host_name, _TNETsocketNum + 5000);
                    client.ReceiveBufferSize = 8192 * 8192; //magic number, seems to be maximum receive size with current hardware //5242880 * 4 - 1;
                    client.NoDelay = true;
                    TNETSocket = client.Client;
                    TNETSocket.ReceiveBufferSize = 8192 * 8192; //magic number, seems to be maximum receive size with current hardware //5242880 * 4 - 1;
                    TNETSocket.SendBufferSize = 32000;
                    TNETSocket.ReceiveTimeout = 200;
                    TNETSocket.SendTimeout = 200;
                    TNETSocket.NoDelay = true;
                    //TNETSocket.Connect(_host_name, _TNETsocketNum);
                    IAsyncResult sockResult = TNETSocket.BeginConnect(_host_name, _TNETsocketNum, null, null);
                    sockResult.AsyncWaitHandle.WaitOne(2500);
                    if(sockResult.IsCompleted && TNETSocket.Connected)
                    {
                        _ClientOpen = true;
                        m = "connected " + _logical_name + " to " + _host_name + " on port " + (TNETsocketNum);
                        stream = client.GetStream();
                    }
                    else
                    {
                        _TNETsocketNum++;
                        if (_TNETsocketNum < 5002)
                        {
                            this.Open();
                        }
                        else
                        {
                            _ClientOpen = false;
                            return;
                        }
                    }
                }
                catch
                {
                    _ClientOpen = false;
                    _TNETsocketNum++;
                    if (_TNETsocketNum < 5002)
                    {
                        this.Open();
                    }
                    else
                    {
                        _ClientOpen = false;
                        return;
                    }
                }
            }

        }

        public void Close()
        {
            if (stream != null) { stream.Close(); }
            if (client != null) { client.Close(); }
            _ClientOpen = false;
        }


        //Read the temperatures from all the CMBs on a given FPGA
        public double[] ReadTempFPGA(int fpga = 0)
        {
            double[] cmb_temps = { 0, 0, 0, 0 }; //new double[4];

            //ReadStr(out string junk, out int junkL);

            SendStr("cmb");
            ReadStr(out string a, out int r);
            if (a.Length > 32) //If it got 'all' the info (32 is just the header: "Ch    DegC     ROM_ID  (Errs=0)")
            {
                string[] tok = a.Split(new string[] { " ", Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                if (String.Equals(tok[1], "DegC")) //Preproduction FEB 'cmb' format
                {
                    for (int i = 4; i < tok.Length; i += 3)
                    {
                        int cmb = Convert.ToInt16(tok[i]) - 1; //start numbering at 0 instead of 1
                        if (cmb / 4 == fpga)
                            cmb_temps[cmb % 4] = Convert.ToDouble(tok[i + 1]);
                    }
                }
                else if (String.Equals(tok[1], "Cnts_TEMP_DegC")) //Prototype FEB 'cmb' format
                {
                    for (int cmb = 0; cmb < 4; cmb++)
                        cmb_temps[cmb] = Convert.ToDouble(tok[(cmb * 4) + (12 * fpga) + 5]); //Starting at index 6, every 4th string is the cmb temperature
                }
                else { for (int cmb = 0; cmb < 4; cmb++) cmb_temps[cmb] = -1; } //unknown format
            }
            else { for (int cmb = 0; cmb < 4; cmb++) cmb_temps[cmb] = -1; } //Didn't receive the info

            return cmb_temps;
        }

        //Read the temperature from a single CMB on a given FPGA
        public double ReadTemp(int cmb, int fpga = 0)
        {
            double cmb_temp = -1;
            if (cmb > 3 && cmb < 16) //if the user gives the CMB number (0-16)
            { fpga = cmb / 4; cmb %= 4; } //set the FPGA and % the cmb number
            else if (cmb < 0) //if the user enters a negative number, return bad value
                return cmb_temp;
            //Else we can assume the user has given a cmb # between 0 and 3, and provided the FPGA number

            SendStr("cmb");
            ReadStr(out string a, out int r);
            if (a.Length > 32) //If it got 'all' the info (32 is just the header: "Ch    DegC     ROM_ID  (Errs=0)")
            {
                string[] tok = a.Split(new string[] { " ", Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                try
                {
                    if (String.Equals(tok[1], "DegC")) //Preproduction FEB 'cmb' format
                    {
                        for (int i = 4; i < tok.Length; i += 3) //Scan through the returned list for the CMB we are interested in
                        {
                            int cmb_num = Convert.ToInt16(tok[i]) - 1; //start numbering at 0 instead of 1
                            if ((cmb_num/4 == fpga) && (cmb_num % 4 == cmb)) //If the cmb number is on the correct fpga and the one that was requested (0-3), then return the temperature
                                cmb_temp = Convert.ToDouble(tok[i + 1]);
                        }
                    }
                    else if (String.Equals(tok[1], "Cnts_TEMP_DegC")) //Prototype FEB 'cmb' format
                    {
                        cmb_temp = Convert.ToDouble(tok[(cmb * 4) + (12 * fpga) + 5]); //Starting at index 5, every 4th string s the cmb temperature
                    }
                    else { cmb_temp = -1; } //unknown format
                }
                catch //if any error occurs, just set the temperature to -1
                {
                    cmb_temp = -1;
                }
            } //else didn't get the info so cmb_temp is left at -1

            return cmb_temp;
        }

        public void SetV(double V, int fpga = 0, short cmb_group = 3) //cmb_group is in binary (00 = first group, 10 = second group, 11 = both groups)
        {
            UInt32 counts;
            try { counts = Convert.ToUInt32(System.Math.Round(V / 5.12/*5.38*/ * 256)); }
            catch { counts = 0; }
            if((cmb_group & 0x1) == 1)
                SendStr("wr " + Convert.ToString(4 * fpga, 16) + "44 " + Convert.ToString(counts, 16));
            if((cmb_group & 0x2) == 1)
                SendStr("wr " + Convert.ToString(4 * fpga, 16) + "45 " + Convert.ToString(counts, 16));
            Thread.Sleep((int)(Math.Ceiling(V / 10.0)*1000) + 500); //Wait for the bias to come up
        }

        public void SetVAll(double V)
        {
            UInt32 counts;
            try { counts = Convert.ToUInt32(System.Math.Round(V / 5.12/*5.38*/ * 256)); }
            catch { counts = 0; }

            for (int fpga = 0; fpga < 4; fpga++)
            {
                SendStr("wr " + Convert.ToString(4 * fpga, 16) + "44 " + Convert.ToString(counts, 16));
                SendStr("wr " + Convert.ToString(4 * fpga, 16) + "45 " + Convert.ToString(counts, 16));
            }
            Thread.Sleep((int)(Math.Ceiling(V / 10.0) * 1000) + 500); //Wait for the bias to come up
        }

        public double ReadV(int fpga = 0)
        {
            if (fpga > 3) { fpga = 0; }
            //UInt32 counts;
            double V;
            SendStr("rd " + Convert.ToString(4 * fpga, 16) + "44");
            ReadStr(out string a, out int r);
            if (a.Length > 4)
            { a = a.Substring(0, 4); }

            try { V = (double)Convert.ToInt32(a, 16); }
            catch { V = 0; }
            double t = V * 5.12/*5.38*/ / 256;
            //t = System.Math.Round(t*1000)/1000;

            return t;
        }

        public double ReadA0(int fpga = 0, int ch = 0)
        {
            if (ch < 0 || ch > 15 || fpga < 0 || fpga > 3) //stop any bad values from reaching the code below
                return 0;

            string t;

            SendStr("mux " + fpga); //Set the mux

            if (fpga > 0)
            {
                //determine CMB
                //change channel on CMB
                int fpga_cmb = ((int) ch / 4)*4; //similar to the fpga's, apparently this value needs to be 1 4 8 C
                int cmb_ch = ch % 4;
                SendStr("wr " + Convert.ToString(4 * fpga, 16) + "20 1" + Convert.ToString(fpga_cmb, 16)); //write to the proper FPGA mux register to change which cmb is being read
                SendStr("wr 20 " + Convert.ToString(cmb_ch, 16)); //write to the mux register to change which channel on the given cmb is being read
            }
            else
                SendStr("wr 20 1" + Convert.ToString(ch, 16));
            SendStr("gain 8");
            SendStr("A0 5");
            while (ReadStr(out t, out int dt) || !t.Contains("avg")) { Thread.Sleep(5); } //If after """reading""" we don't have the current, READ IT AGAIN DAMMIT
            string[] tok = t.Split(new string[] { " ", "\r\n", Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            double adc = -4.096;
            try { adc = Convert.ToDouble(tok[tok.Length-2]); }
            catch { adc = -4.096; }
            if (adc > 4.096) { adc = 8.192 - adc; }
            double I = adc / 8 * 250;
            if (fpga > 0)
                SendStr("wr " + Convert.ToString(4 * fpga, 16) + "20 0"); //disable mux for fpga
            SendStr("wr 20 0"); //disable mux

            return I;
        }

        public bool GetReady()
        {
            string t = "";
            if (_ClientOpen)
            {
                if (TNETSocket.Available > 0)
                {
                    Thread.Sleep(1);
                    byte[] buf = new byte[TNETSocket.Available];
                    TNETSocket.Receive(buf);
                }
                //t = "DREC\r\n";
                //SendStr(t);
                //t = "TRIG 0\r\n";
                //SendStr(t);
                //t = "WR 0 3C\r\n";
                //SendStr(t);
                t = "WR 0 4\r\n"; //reset the AFE seserializer logic
                SendStr(t);
                Thread.Sleep(1);
                t = "WR 400 4\r\n"; //reset the AFE seserializer logic
                SendStr(t);
                Thread.Sleep(1);
                t = "WR 0 20\r\n"; //reset the trigger counter and whatnot
                SendStr(t);
                Thread.Sleep(1);
                t = "WR 400 20\r\n"; //reset the trigger counter and whatnot
                SendStr(t);
                Thread.Sleep(1);

            }
            return true;
        }

        public bool Arm()
        {
            bool sent_arm_success = false;
            string t = "wr 303 32\r\n";
            try
            {
                if (_ClientOpen)
                {
                    if (TNETSocket.Available > 0)
                    {
                        byte[] buf = new byte[TNETSocket.Available];
                        TNETSocket.Receive(buf);
                    }
                    SendStr(t);
                    sent_arm_success = true;
                }
            }
            catch { }
            return sent_arm_success;

        }

        public bool Disarm()
        {
            bool sent_disarm_success = false;
            string t = "wr 303 40\r\n";
            try
            {
                if (_ClientOpen)
                {
                    if (TNETSocket.Available > 0)
                    {
                        byte[] buf = new byte[TNETSocket.Available];
                        TNETSocket.Receive(buf);
                    }
                    SendStr(t);
                    sent_disarm_success = true;
                }
            }
            catch { }
            return sent_disarm_success;
        }

        //public bool SoftwareTrig()
        //{
        //    SW.WriteLine("trig");
        //    timeout = 0;
        //    while ((SR.ReadLine() != "trig") && (timeout < max_timeout)) { System.Threading.Thread.Sleep(1); timeout++; }
        //    if (timeout < max_timeout) { return true; } else { return false; }
        //}

        public bool CheckStatus(out uint spill_state, out uint spill_num, out uint trig_num)
        {
            Mu2e_Register.FindAddr(0x76, ref arrReg, out Mu2e_Register reg_spill_state);
            Mu2e_Register.FindAddr(0x68, ref arrReg, out Mu2e_Register reg_spill_num);
            Mu2e_Register.FindAddr(0x67, ref arrReg, out Mu2e_Register reg_trig_count);

            Mu2e_Register.ReadReg(ref reg_spill_state, ref client);
            Mu2e_Register.ReadReg(ref reg_spill_num, ref client);
            Mu2e_Register.ReadReg(ref reg_trig_count, ref client);

            spill_state = reg_spill_state.val;
            spill_num=reg_spill_num.val;
            trig_num=reg_trig_count.val;
            return true;
        }

        public bool Clear()
        {
            SW.WriteLine("clear");
            timeout = 0;
            while ((SR.ReadLine() != "clear") && (timeout < max_timeout)) { System.Threading.Thread.Sleep(1); timeout++; }
            if (timeout < max_timeout) { return true; } else { return false; }
        }

        public bool ReadCMB_SN(int FPGAnum, int CMB_num, out string CMB_ROM)
        {
            CMB_ROM = "";
            try
            {
                if (_ClientOpen && FPGAnum < 4)
                {
                    //CMB_set.fpga_index = Convert.ToUInt16(FPGAnum);
                    //CMB_cmnd.fpga_index = Convert.ToUInt16(FPGAnum);
                    //CMB_read.fpga_index = Convert.ToUInt16(FPGAnum);
                    //addr25
                    Mu2e_Register.FindAddrFPGA(0x25, (uint)FPGAnum, ref this.arrReg, out Mu2e_Register CMB_set);
                    //addr24
                    Mu2e_Register.FindAddrFPGA(0x24, (uint)FPGAnum, ref this.arrReg, out Mu2e_Register CMB_cmnd);
                    //addr26
                    Mu2e_Register.FindAddrFPGA(0x26, (uint)FPGAnum, ref this.arrReg, out Mu2e_Register CMB_read);


                    switch (CMB_num)
                    {
                        case 0:
                            Mu2e_Register.WriteReg(1, ref CMB_set, ref this.client);
                            break;
                        case 1:
                            Mu2e_Register.WriteReg(2, ref CMB_set, ref this.client);
                            break;
                        case 2:
                            Mu2e_Register.WriteReg(4, ref CMB_set, ref this.client);
                            break;
                        case 3:
                            Mu2e_Register.WriteReg(8, ref CMB_set, ref this.client);
                            break;
                        default:
                            Mu2e_Register.WriteReg(1, ref CMB_set, ref this.client);
                            break;
                    }
                    Mu2e_Register.WriteReg(0x200, ref CMB_cmnd, ref this.client);

                    SendStr("rdi 26"); //This only reads the first FPGA, is this intended?
                    ReadStr(out string t, out int rt);
                    CMB_ROM = t;
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch { return false; }
        }

        public string SendRead(string lin)
        {
            string lout;
            if (_ClientOpen)
            {
                SW.WriteLine(lin);
                System.Threading.Thread.Sleep(50);
                if (TNETSocket.Available > 0)
                {
                    byte[] rec_buf = new byte[TNETSocket.Available];
                    Thread.Sleep(10);
                    int ret_len = TNETSocket.Receive(rec_buf);
                    lout = PP.GetString(rec_buf, ret_len);
                    return lout;
                }
                else
                {
                    return ("!error! timeout");
                }
            }
            else { return null; }
        }

        public void SendStr(string t)
        {
            if (_ClientOpen)
            {
                try
                {
                    if (TNETSocket.Available > 0)
                    {
                        byte[] buf = new byte[TNETSocket.Available];
                        TNETSocket.Receive(buf);
                    }
                    // ? why does this not work? SW.WriteLine(t);
                    //byte[] b = PP.GetBytes(t + Convert.ToChar((byte)0x0d));
                    byte[] b = PP.GetBytes(t + "\r\n");
                    IAsyncResult sendResult = TNETSocket.BeginSend(b, 0, b.Length, SocketFlags.None, null, null);
                    sendResult.AsyncWaitHandle.WaitOne();
                    
                    //TNETSocket.Send(b);
                    //Thread.Sleep(20);
                }
                catch(System.IO.IOException)
                {
                    _ClientOpen = false;
                }
            }
        }

        public bool ReadStr(out string t, out int ret_time, int tmo = 250)
        {
            t = "";
            bool tmo_reached = false;
            int this_t = 0;
            if (_ClientOpen)
            {
                DateTime timeLimit = DateTime.Now.AddMilliseconds(tmo);
                while (TNETSocket.Available == 0 && !tmo_reached)
                {
                    Thread.Sleep(5);
                    this_t += 5;
                    if (DateTime.Now > timeLimit) { tmo_reached = true; }
                }
                if (!tmo_reached)
                {
                    while (TNETSocket.Available > 0)
                    {
                        byte[] rec_buf = new byte[TNETSocket.Available];
                        int ret_len = TNETSocket.Receive(rec_buf);
                        t += PP.GetString(rec_buf, ret_len);
                        Thread.Sleep(100); //Wait a little before it checks the socket again
                    }
                    t = t.Trim(new char[] { '>' }); //remove >
                    t = t.Trim(Environment.NewLine.ToCharArray());
                }
            }
            ret_time = this_t;
            return tmo_reached;
        }

        //public  static int GetStatus(out string[] status)
        //{
        //    string[] n = new string[10];
        //    n[0] = "";
        //    n[1] = "";
        //    n[2] = "";
        //    n[3] = "st=";
        //    n[4] = "ARM=";
        //    n[5] = "t in mem=";
        //    n[6] = "err reg=";
        //    n[7] = "last trig=";
        //    n[8] = "Ptemp=";
        //    n[9] = "Stemp=";
        //    int lines = 0;
        //    int num_pade = 0;
        //    string[] tok = new string[1];

        //    SW.WriteLine("status");

        //    string t = SR.ReadLine();
        //    lines++;
        //    if (t.ToUpper().Contains("MASTER") || t.ToUpper().Contains("SLAVE"))
        //    {
        //        string[] delim = new string[1];
        //        delim[0] = " ";
        //        tok = t.Split(delim, StringSplitOptions.RemoveEmptyEntries);
        //        num_pade = Convert.ToInt32(tok[0]);
        //    }


        //    lines = num_pade;
        //    string[] s = new string[lines + 1];
        //    if ((tok.Length < 9 * num_pade) || (num_pade == 0))
        //    {
        //        for (int i = 0; i < s.Length; i++)
        //        {
        //            s[i] = "error";
        //        }
        //        if (num_pade == 0) { s = new string[1]; s[0] = "error, 0 PADE"; }
        //    }
        //    else
        //    {
        //        int j = 0;
        //        int k = 0;
        //        s[k] = "";
        //        for (int i = 0; i < tok.Length; i++)
        //        {

        //            if (j > 0)
        //            {
        //                s[k] += n[j - 9 * k] + tok[j] + " ";
        //            }
        //            j++;
        //            if ((j - 1) >= (9 * (k + 1))) { k++; s[k] = ""; }
        //        }
        //    }
        //    status = s;
        //    return lines;
        //}

        public void CheckFEB_connection()
        {
            if (!client.Connected)
            {
                client.Connect(_host_name, _TNETsocketNum);
            }
            else
            { }
        }

        public static int Count_bits(int val)
        {
            int count = 0;
            try
            {
                for (int i = 0; i < 31; i++)
                {
                    if ((Convert.ToInt32(Math.Pow(2, i)) & val) == Convert.ToInt32(Math.Pow(2, i))) { count++; }
                }
                return count;
            }
            catch { return 0; }
        }

        //static byte[] GetBytes(string str)
        //{
        //    byte[] bytes = new byte[str.Length];
        //    for (int i = 0; i < str.Length; i++)
        //    {
        //        bytes[i] = (byte)(str[i]);
        //    }
        //    return bytes;
        //}

        //static string GetString(byte[] bytes, int len)
        //{
        //    char[] chars = new char[len];
        //    for (int i = 0; i < len; i++)
        //    {
        //        chars[i] = (char)bytes[i];
        //    }
        //    return new string(chars);
        //}

    }

    struct CMB
    {
        public int num;
        public string rom_id;
        public double temp;
        public ROOTNET.NTH1I[] ledHisto;
        public ROOTNET.NTH1I[] flasherHisto;
        public ROOTNET.NTH1I[] gateHisto;
        public bool flagged;  
        public enum Failure { NoFail=0, TempRom, SiPMResp, Flashgate, LED, TailCancellation, LEDUntested};
        public int failureType;

        public string FailType()
        {
            string[] failures = { "UNKNWN", "TEMP/ROM", "SiPMRESP", "FLASHGATE", "LED", "TAILCAN", "LEDUNTEST" };
            return failures[failureType];
        }
    };
}
