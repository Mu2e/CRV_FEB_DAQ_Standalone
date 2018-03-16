using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.IO;
using System.Threading;


namespace TB_mu2e
{
    public class RC_client
    {
        public static bool ClientOpen = false;
        private static TcpClient client;
        private static Socket FEBSocket;
        private static NetworkStream stream;
        private static StreamReader SR;
        private static StreamWriter SW;
        private static int max_timeout;
        private static int timeout;

        static byte[] GetBytes(string str)
        {
            byte[] bytes = new byte[str.Length];
            for (int i = 0; i < str.Length; i++)
            {
                bytes[i] = (byte)(str[i]);
            }
            return bytes;
        }

        static string GetString(byte[] bytes, int len)
        {
            char[] chars = new char[len];
            for (int i = 0; i < len; i++)
            {
                chars[i] = (char)bytes[i];
            }
            return new string(chars);
        }

        public static void Open()
        {
            ClientOpen = false;

            {
                Console.WriteLine("Running on host: " + System.Environment.MachineName);
                //if (System.Environment.MachineName.Contains("shashlik-daq"))
                //{
                //    Console.WriteLine("Connecting to remote PADE Scope");
                //    client = new TcpClient("192.168.1.225", 23);
                //}
                //else

                int my_port = 5001;
                Console.WriteLine("Connecting to HLSWH5 on port 5000");

                //("hlswh5", 5000);
                try
                {
                    FEBSocket = new Socket(SocketType.Stream, ProtocolType.Tcp);
                    FEBSocket.Blocking = true;
                    FEBSocket.ReceiveBufferSize = 4000;
                    FEBSocket.ReceiveTimeout = 200;
                    FEBSocket.SendTimeout = 10;
                    FEBSocket.Connect("hlswh5", my_port);

                    stream = new NetworkStream(FEBSocket);

                    SW = new StreamWriter(stream);
                    SR = new StreamReader(stream);
                    SW.AutoFlush = true;
                    max_timeout = 500;
                    ClientOpen = true;
                }
                catch (Exception)
                {

                    throw;
                }


            }

        }

        public static void SetV(double V)
        {
            UInt32 counts;
            double t=V/5.38*256;
            t=System.Math.Round(t);
            try {counts=Convert.ToUInt32(t);}
            catch {counts=0;}
            SendStr("wr 44 "+Convert.ToString(counts,16));
        }

        public static double ReadA0()
        {
            int dt;
            string t;
            SendStr("wr 20 10");
            Thread.Sleep(20);
            SendStr("gain 8");
            Thread.Sleep(20);
            ReadStr(out t, out dt, 100);
            SendStr("A0 10");
            Thread.Sleep(1500);
            ReadStr(out t, out dt,100);
            char[] sep = new char[3];
            sep[0] = ' ';
            sep[1] = '\r';
            sep[2] = '\n';
            string[] tok = t.Split(sep, StringSplitOptions.RemoveEmptyEntries);
            double adc;
            try { adc = Convert.ToDouble(tok[10]); }
            catch { adc = -1; }
            if (adc > 4.096) { adc = 8.192 - adc; }
            double I = adc/ 8 * 250;
            //for (int i = 0; i < tok.Length; i++)
            return I;
        }
        
        public static bool Arm()
        {
            SW.WriteLine("arm");
            timeout = 0;
            while ((SR.ReadLine() != "arm") && (timeout < max_timeout)) { System.Threading.Thread.Sleep(1); timeout++; }
            if (timeout < max_timeout) { return true; } else { return false; }
        }

        public static bool Disarm()
        {
            string t = "WR 2e 08\r\n";
            if (ClientOpen)
            {

                if (FEBSocket.Available > 0)
                {
                    Thread.Sleep(1);
                    byte[] buf = new byte[FEBSocket.Available];
                    FEBSocket.Receive(buf);
                }
                SW.WriteLine(t);
            }
            //SW.WriteLine("disarm");
            //timeout = 0;
            //while ((SR.ReadLine() != "disarm") && (timeout < max_timeout)) { System.Threading.Thread.Sleep(1); timeout++; }
            //if (timeout < max_timeout) { return true; } else { return false; }
            return true;
        }

        public static bool SoftwareTrig()
        {
            SW.WriteLine("trig");
            timeout = 0;
            while ((SR.ReadLine() != "trig") && (timeout < max_timeout)) { System.Threading.Thread.Sleep(1); timeout++; }
            if (timeout < max_timeout) { return true; } else { return false; }
        }

        public static bool ReadN(int n)
        {
            SW.WriteLine("read " + n.ToString());
            timeout = 0;
            while ((SR.ReadLine().Contains("read") == false) && (timeout < max_timeout)) { System.Threading.Thread.Sleep(1); timeout++; }
            if (timeout < max_timeout) { return true; } else { return false; }

        }

        public static bool ReadAll()
        {
            SW.WriteLine("read all");
            timeout = 0;
            while ((SR.ReadLine().Contains("read") == false) && (timeout < max_timeout)) { System.Threading.Thread.Sleep(1); timeout++; }
            if (timeout < max_timeout) { return true; } else { return false; }
        }

        public static bool Clear()
        {
            SW.WriteLine("clear");
            timeout = 0;
            while ((SR.ReadLine() != "clear") && (timeout < max_timeout)) { System.Threading.Thread.Sleep(1); timeout++; }
            if (timeout < max_timeout) { return true; } else { return false; }
        }

        public static string SendRead(string lin)
        {
            string lout;
            if (ClientOpen)
            {
                SW.WriteLine(lin);
                System.Threading.Thread.Sleep(50);
                if (FEBSocket.Available > 0)
                {
                    byte[] rec_buf = new byte[FEBSocket.Available];
                    Thread.Sleep(10);
                    int ret_len = FEBSocket.Receive(rec_buf);
                    lout = GetString(rec_buf, ret_len);
                    return lout;
                }
                else
                {
                    return ("!error! timeout");
                }
            }
            else { return null; }
        }

        public static void SendStr(string t)
        {
            if (ClientOpen)
            {
                
                if (FEBSocket.Available > 0)
                {
                    Thread.Sleep(1);
                    byte[] buf=new byte[FEBSocket.Available];
                    FEBSocket.Receive(buf);
                }
                SW.WriteLine(t);
            }
        }

        public static void ReadStr(out string t, out int ret_time, int tmo = 100)
        {
            t = "";
            bool tmo_reached = false;
            int this_t = 0;
            if (ClientOpen)
            {
                DateTime s = DateTime.Now;
                while (FEBSocket.Available == 0 && !tmo_reached)
                {
                    Thread.Sleep(5);
                    this_t += 5;
                    if (this_t > tmo) { tmo_reached = true; }
                }
                if (!tmo_reached)
                {
                    byte[] rec_buf = new byte[FEBSocket.Available];
                    Thread.Sleep(10);
                    int ret_len = FEBSocket.Receive(rec_buf);
                    t = GetString(rec_buf, ret_len);
                }
            }
            ret_time = this_t;
        }

        public static int GetStatus(out string[] status)
        {
            string[] n = new string[10];
            n[0] = "";
            n[1] = "";
            n[2] = "";
            n[3] = "st=";
            n[4] = "ARM=";
            n[5] = "t in mem=";
            n[6] = "err reg=";
            n[7] = "last trig=";
            n[8] = "Ptemp=";
            n[9] = "Stemp=";
            int lines = 0;
            int num_pade = 0;
            string[] tok = new string[1];

            SW.WriteLine("status");

            string t = SR.ReadLine();
            lines++;
            if (t.ToUpper().Contains("MASTER") || t.ToUpper().Contains("SLAVE"))
            {
                string[] delim = new string[1];
                delim[0] = " ";
                tok = t.Split(delim, StringSplitOptions.RemoveEmptyEntries);
                num_pade = Convert.ToInt32(tok[0]);
            }


            lines = num_pade;
            string[] s = new string[lines + 1];
            if ((tok.Length < 9 * num_pade) || (num_pade == 0))
            {
                for (int i = 0; i < s.Length; i++)
                {
                    s[i] = "error";
                }
                if (num_pade == 0) { s = new string[1]; s[0] = "error, 0 PADE"; }
            }
            else
            {
                int j = 0;
                int k = 0;
                s[k] = "";
                for (int i = 0; i < tok.Length; i++)
                {

                    if (j > 0)
                    {
                        s[k] += n[j - 9 * k] + tok[j] + " ";
                    }
                    j++;
                    if ((j - 1) >= (9 * (k + 1))) { k++; s[k] = ""; }
                }
            }
            status = s;
            return lines;
        }



        public static void Close()
        {
            stream.Close();
            client.Close();
            ClientOpen = false;
        }

    }

}
