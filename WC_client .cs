using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.IO;
using System.Threading;


namespace TB_mu2e
{
    public class WC_client : IStenComms
    {
        #region InterfaceStenComms
        public string m_prop { get { return m; } set { m = value; } }
        public string name { get { return _logical_name; } set { _logical_name = value; } }
        public string host_name_prop { get { return _host_name; } set { _host_name = value; } }
        public TcpClient client_prop { get { return client; } }
        public Socket TNETSocket_prop { get { return WCsocket; } }
        public NetworkStream stream_prop { get { return stream; } }
        public StreamReader SR_prop { get { return SR; } }
        public StreamWriter SW_prop { get { return SW; } }
        public int max_timeout_prop { get { return _max_timeout; } set { _max_timeout = value; } }
        public int timeout_prop { get { return _timeout; } set { timeout_prop = _timeout; } }
        bool IStenComms.ClientOpen { get { return _ClientOpen; } set { } }
        public void DoOpen() { Open(); }
        #endregion InterfaceStenComms

        TcpClient client = null;
        NetworkStream stream = null;
        StreamReader SR = null;
        StreamWriter SW = null;

        private static string _logical_name = "WC";
        private static int _max_timeout = 100;
        private static int _timeout = 500;
        private static string _host_name;
        private static bool _ClientOpen = false;
        public static string controller_name;// = "FTBFWC01.FNAL.GOV";
        public static Socket WCsocket;
        public static string m;
        static string lastSpillState = "0";
        public int WCsocketNum = 0;
        public bool busy;
        public bool in_spill;




        public bool ClientOpen { get { return _ClientOpen; } set { _ClientOpen = value; } }

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

        public void Open()
        {
            //_ClientOpen = false;
            //try
            //{
            //    WCsocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            //    WCsocket.Blocking = true;
            //    WCsocket.ReceiveBufferSize = 1024000;
            //    WCsocket.Connect(__host_name, 5001);
            //    WCsocket.ReceiveTimeout = _timeout;
            //    Thread.Sleep(100);
            //    _ClientOpen = true;
            //}
            //catch { throw new NotImplementedException("failed to connect to FTBFWC01"); }
            _ClientOpen = false;
            try
            {
                client = new TcpClient();
                WCsocket = client.Client;
                WCsocket.Blocking = true;
                WCsocket.ReceiveBufferSize = 2047;
                WCsocket.SendBufferSize = 32;
                WCsocket.ReceiveTimeout = 10;
                WCsocket.SendTimeout = 10;
                WCsocket.NoDelay = true;
                WCsocket.Connect(_host_name, WCsocketNum + 5000);
                //Thread.Sleep(100);
                _ClientOpen = true;

                m = "connected " + _logical_name + " to " + _host_name + " on port " + (WCsocketNum + 5000);
            }
            catch
            {
                WCsocketNum++;
                if (WCsocketNum < 2)
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

        public static void ForeverFake()
        {
            //byte[] b = new byte[9];
            //byte[] buf = GetBytes("wr 1 c\r\n");
            //WCsocket.Send(buf);
        }

        public static void DisableFake()
        {
            if (_ClientOpen)
            {
                byte[] b = new byte[9];
                byte[] buf = GetBytes("wr 1 0\r\n");
                WCsocket.Send(buf);
            }
        }

        public static void FakeSpill()
        {
            if (_ClientOpen)
            {
                byte[] b = new byte[9];
                byte[] buf = GetBytes("wr 1 1c\r\n");
                WCsocket.Send(buf);
            }
        }

        public static void DisableTrig()
        {
            try
            {
                byte[] buf = GetBytes("wr 88 2\r\n");
                WCsocket.Send(buf);
                byte[] rec_buf = new byte[1000];
                Thread.Sleep(100);
            }
            catch
            {
                //Exception e = new Exception("you should not be messing with WC trigger!");

                //string m = "Did you mean to have the Fake It box checked?";
                // Exception e = new Exception(m);
                // throw e;

            }
        }

        public static void EnableTrig()
        {
            //try
            //{
            byte[] buf = GetBytes("wr 88 1\r\n");
            WCsocket.Send(buf);
            Thread.Sleep(10);
            WCsocket.Send(buf);
            Thread.Sleep(10);
            WCsocket.Send(buf);
            byte[] rec_buf = new byte[1000];
            Thread.Sleep(50);
            buf = GetBytes("rd 88\r\n");
            WCsocket.Send(buf);
            rec_buf = new byte[100];
            Thread.Sleep(50);
            int ret_len = WCsocket.Receive(rec_buf);
            string s = GetString(rec_buf, ret_len);
            if (PP.glbDebug) { Console.WriteLine("Read 88=" + s); }

            Exception e = new Exception("you should not be messing with WC trigger!");
            //}
            //catch { }
        }

        public static void check_status(out bool in_spill, out string num_trig, out string time)
        {
            in_spill = false;
            num_trig = "0";
            time = "0";
            if (WCsocket != null)
            {
                while (WCsocket.Available > 0)
                {
                    byte[] rec_buf = new byte[500000];
                    int ret_len = WCsocket.Receive(rec_buf);
                }
                try
                {
                    byte[] buf = GetBytes("rd 5\r\n");
                    WCsocket.Send(buf);
                    byte[] rec_buf = new byte[50];
                    Thread.Sleep(10);
                    int ret_len = 0;
                    if (WCsocket.Available > 0) { ret_len = WCsocket.Receive(rec_buf); }
                    string s = GetString(rec_buf, ret_len);
                    num_trig = s.Substring(0, 4);
                }
                catch
                { num_trig = "xxx"; }
                try
                {
                    byte[] buf = GetBytes("time\r\n");
                    WCsocket.Send(buf);
                    byte[] rec_buf = new byte[100];
                    Thread.Sleep(10);
                    int ret_len = 0;
                    if (WCsocket.Available > 0) { ret_len = WCsocket.Receive(rec_buf); }
                    string s = GetString(rec_buf, ret_len);
                    time = s.Substring(0, s.IndexOf("\r"));
                }
                catch
                { time = " time unknown"; }

                try
                {
                    byte[] buf = GetBytes("rd b\r\n");
                    WCsocket.Send(buf);
                    byte[] rec_buf = new byte[1000];
                    Thread.Sleep(10);
                    int ret_len = WCsocket.Receive(rec_buf);
                    string s = GetString(rec_buf, ret_len);
                    string sss = s.Substring(3, 1);
                    if (sss != lastSpillState)
                    {
                        if (PP.glbDebug) { Console.WriteLine(); }
                        lastSpillState = sss;
                        if (PP.glbDebug) { Console.Write("SpillState= "); }
                    }
                    if (PP.glbDebug) { Console.Write(sss); }
                    if (sss == "8") { in_spill = true; } else { in_spill = false; }
                }
                catch
                { in_spill = false; }
            }

        }

        public static byte[] read_TDC(out int len)
        {
            byte[] b = new byte[5];
            byte[] buf = GetBytes("rdb\r\n");
            WCsocket.Send(buf);
            byte[] rec_buf = new byte[8000000];
            Thread.Sleep(500);
            try
            {
                int ret_len = WCsocket.Receive(rec_buf);
                len = ret_len;
                return rec_buf;
            }
            catch
            {
                len = 0;
                byte[] fake = new byte[1];
                return fake;
            }
        }


        public static void Close()
        {
            WCsocket.Close();
            _ClientOpen = false;
        }
    }

}
