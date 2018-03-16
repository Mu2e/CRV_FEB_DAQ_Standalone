using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
//using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.IO;

namespace TB_mu2e
{
    class DAQ_server
    {
        static TcpListener listener;
        static IPEndPoint ipend;
        const int LIMIT = 2;
        static int g_commanded_max_trig = 1000;
        public static bool gEnable_Remote_Service = true;

        public void StartRC()
        {
            //ipend = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 5000);
            ////ipend = new IPEndPoint(IPAddress.Any, 5000);
            ////ipend = new IPEndPoint(IPAddress.Parse("131.156.224.146"), 5000);
            //listener = new TcpListener(ipend);
            //listener.Start();

            //for (int i = 0; i < LIMIT; i++)
            //{
            //    Thread t = new Thread(new ThreadStart(Service));
            //    t.Start();
            //}

        }

        static bool ArmAll()
        {

            return true;
        }

        static bool DisArmAll()
        {

            return true;
        }

        static bool Trig()
        {

            return true;
        }

        static bool ClearAll()
        {
            return true;
        }

        static bool ReadAll()
        {
            return true;
        }

        static string ReportStatus()
        {
            string message = "";
            return message;
        }

        public static void Service()
        {
            while (gEnable_Remote_Service)
            {
                Socket soc = listener.AcceptSocket();
                
                try
                {
                    string[] delimeter = new string[64];
                    string[] tokens = new string[64];

                    delimeter[0] = "<=";
                    delimeter[1] = "//";
                    delimeter[2] = ";";
                    delimeter[3] = "=";
                    delimeter[4] = "set ";
                    delimeter[5] = "dec";
                    delimeter[6] = "0x";
                    delimeter[7] = "(";
                    delimeter[8] = ")";
                    delimeter[9] = " ";
                    Stream s = new NetworkStream(soc);
                    StreamReader sr = new StreamReader(s);
                    StreamWriter sw = new StreamWriter(s);
                    sw.AutoFlush = true; // enable automatic flushing
                    string RCcmd = "";
                    sw.WriteLine("Hello NIU!");
                    while (RCcmd != "quit")
                    {
                        RCcmd = sr.ReadLine();
                        RCcmd = RCcmd.Trim().ToLower();
                        //sw.WriteLine("ok, I hear you.");
                        //crappy short cuts
                        if (RCcmd == "hello") { WC_client.FakeSpill(); sw.WriteLine("GO"); }
                        if (RCcmd == "done") { WC_client.FakeSpill(); sw.WriteLine("GO"); }
                        if (RCcmd == "help") { sw.WriteLine("I know nothing!"); }
                        if (RCcmd == "disarm") { DisArmAll(); sw.WriteLine(RCcmd); }
                        if (RCcmd == "clear") { ClearAll(); sw.WriteLine(RCcmd); }
                        if (RCcmd == "status") { string m = ReportStatus(); sw.WriteLine(m); }
                        if (RCcmd.Contains("maxtrig"))
                        {
                            int val = 1000;
                            string[] tok = RCcmd.Split(delimeter, StringSplitOptions.None);
                            if (tok.Length > 1)
                            {
                                try
                                {
                                    val = Convert.ToInt32(tok[1]);
                                    if (val < 1) { val = 1; }
                                    if (val > 1000) { val = 1000; }
                                }
                                catch
                                { val = 1000; }
                                g_commanded_max_trig = val;
                            }
                        }

                        if (RCcmd.Contains("read"))
                        {
                            string[] tok = RCcmd.Split(delimeter, StringSplitOptions.None);
                            if (tok.Length > 1)
                            {
                                if (tok[1].Contains("all"))
                                {
                                    ReadAll();
                                    sw.WriteLine(RCcmd);
                                }
                                else
                                {
                                }
                            }
                        }
                    }
                    s.Close();
                }
                catch (Exception e)
                {
                    if (PP.glbDebug){ Console.WriteLine("oops, connection lost"); }
                }

                soc.Close();
            }
        }
    }
}
