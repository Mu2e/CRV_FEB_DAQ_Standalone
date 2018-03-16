using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using System.Threading.Tasks;
using System.Net.Sockets;
using System.IO;
using System.Threading;

namespace TB_mu2e
{
    public class Mu2e_FECC_client : IStenComms
    {
        #region InterfaceStenComms
        public string m_prop { get { return m; } set { m = value; } }
        public string name { get { return _logical_name; } set { _logical_name = value; } }
        public string host_name_prop { get { return host_name; } set { host_name = value; } }
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

        public string m = "";
        public string host_name;
        private static bool _ClientOpen = false;
        private static bool _ClientBusy = true;
        private static string _TNETname;
        
        public int _TNETsocketNum = 0;
        public TcpClient client;
        public Socket TNETSocket;
        public NetworkStream stream;
        public StreamReader SR;
        public StreamWriter SW;
        private  int max_timeout;
        private  int timeout;

        // events 
        //public delegate void cOpening();
        //public event cOpening ClientOpening;

        private static string _logical_name = "FECC";


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

        public bool ClientOpen { get { return _ClientOpen; } set { _ClientOpen = value; } }
        public bool ClientBusy { get { return _ClientBusy; } set { _ClientBusy = value; } }
        public string FEBname { get { return _TNETname; } set { _TNETname = value; } }
        public int TNETsocketNum { get { return _TNETsocketNum; } set { _TNETsocketNum = value; } }
        public void Open()
        {
            ClientOpen = false;
            try
            {
                TNETSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                TNETSocket.Blocking = true;
                TNETSocket.ReceiveBufferSize = 1024000;
                TNETSocket.Connect(FEBname, TNETsocketNum + 5000);
                Thread.Sleep(100);
                _ClientOpen = true;
                m = "conn to " + FEBname + " on port " + (TNETsocketNum + 5000);
            }
            catch
            {

                TNETsocketNum++;
                if (TNETsocketNum < 3)
                {
                    this.Open();
                }
                else
                {
                    _ClientOpen = false;
                }
            }

        }
    }
}
