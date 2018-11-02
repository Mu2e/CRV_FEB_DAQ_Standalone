using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Windows.Forms;
using System.IO;
using System.Threading;
using System.Net.Sockets;

namespace TB_mu2e
{

    interface IStenComms
    {
        string m_prop { get; set; }
        string host_name_prop { get; set; }
        string name { get; set; }
        TcpClient client_prop { get; }
        Socket TNETSocket_prop { get; }
        NetworkStream stream_prop { get; }
        StreamReader SR_prop { get; }
        StreamWriter SW_prop { get; }
        int max_timeout_prop { get; set; }
        int timeout_prop { get; set; }
        bool ClientOpen { get; set; }
        void DoOpen();
    }


    public class Run
    {
        public int num;
        public string InFileName = "";
        public string OutFileName = "";
        public string run_name;
        public FileStream file;
        public StreamReader sr;
        public StreamWriter sw;
        public int start_evt_num;
        public int stop_evt_num;
        public DateTime SpillStart;
        public DateTime SpillEnd;
        public double[] max_adc;
        public long[] total_trig;
        public long[] total_spill;
        public long[] this_trig;
        public long[] this_bytes_written;
        public long[] total_bytes_written;
        public string myStatus;
        public DateTime created;
        public DateTime timeLastSpill;
        public DateTime timeLastUpdate;
        public Queue<string> RunStatus;
        public bool ACTIVE;
        public bool spill_complete;
        public long num_bytes;
        public bool fake;
        public bool OneSpill;
        public string[] RunParams;
        private List<byte[]> data_buffers;

        public LinkedList<SpillData> Spills;

        public bool SaveAscii { get; internal set; }

        public Run()
        {
            num = 0;
            ACTIVE = false;
            spill_complete = false;
            start_evt_num = 0;
            stop_evt_num = 0;
            if (Spills != null) { Spills.Clear(); }
            else { Spills = new LinkedList<SpillData>(); }
            max_adc = new double[8];
            total_trig = new long[3];
            total_spill = new long[3];
            this_trig = new long[3];
            this_bytes_written = new long[3];
            total_bytes_written = new long[3];
            myStatus = "Run Object Created";//"Starting new run";
            RunStatus = new Queue<string>();
            UpdateStatus(myStatus);
            int last_num = 0;
            StreamReader sr;
            sr = null;
            fake = false;
            OneSpill = false;

            RunParams = new string[10];

            for (int i = 0; i < 10; i++)
            {
                RunParams[i] = "";
            }

            for (int i = 0; i < 3; i++)
            {
                total_bytes_written[i] = 0;
                total_trig[i] = 0;
                total_spill[i] = 0;
            }

            for (int i = 0; i < 8; i++)
            {
                max_adc[i] = 0;
            }

            try
            {
                if (File.Exists("c:\\data\\run_list.txt"))
                {
                    using (sr = File.OpenText("c:\\data\\run_list.txt"))
                    {
                        string l = "";
                        while (!sr.EndOfStream) //read to the last line
                            l = sr.ReadLine();

                        last_num = Convert.ToInt32(l.Substring(0, l.IndexOf(" ")));
                    }
                }

                if (File.Exists("c:\\data\\run_param.txt"))
                {
                    using (StreamReader sr2 = File.OpenText("c:\\data\\run_param.txt"))
                    {
                        string l2 = "";
                        while (!sr2.EndOfStream)
                        {
                            l2 = sr2.ReadLine();
                        }
                        string[] words = l2.Split(' ');
                        RunParams = words;
                    }
                }
            }
            catch(Exception e)
            {
                System.Console.Write("Caught Exception {0} in Program.cs!", e);
                last_num = 0;
            }
            num = last_num + 1;
            run_name = "Run_" + num.ToString("0000");
            //try
            //{
            //    using (StreamWriter sw = File.AppendText("c:\\data\\run_list.txt"))
            //    {
            //        sw.WriteLine(num.ToString() + " starting at: " + DateTime.Now);
            //    }
            //}
            //catch (Exception e)
            //{
            //    System.Console.Write("Caught Exception {0} in Program.cs!", e);
            //}
        }

        public void UpdateStatus(string m)
        {
            TimeSpan since_last_update = DateTime.Now.Subtract(this.timeLastUpdate);
            if (since_last_update.TotalSeconds > 0.9)
            {
                RunStatus.Enqueue(DateTime.Now + ": " + m);
                this.timeLastUpdate = DateTime.Now;
            }
        }

        public void ActivateRun()
        {
            string hName = "";
            string dirName = "c://data//";
            hName = run_name;
            RunName rn = new RunName();
            rn.ShowDialog();

            //hName += "_ch" + this.chan.ToString();
            //hName += "_" + DateTime.Now.Year.ToString("0000");
            //hName += DateTime.Now.Month.ToString("00");
            //hName += DateTime.Now.Day.ToString("00");
            //hName += "_" + DateTime.Now.Hour.ToString("00");
            //hName += DateTime.Now.Minute.ToString("00");
            //hName += DateTime.Now.Second.ToString("00");
            created = DateTime.Now;
            hName = run_name;//PP.myRun.run_name;
            hName = dirName + hName + ".data";
            OutFileName = hName;
            try
            {
                using (sw = new StreamWriter(OutFileName, true))
                {
                    sw.WriteLine("-- START OF RUN -- " + DateTime.Now.ToString());

                    using (StreamWriter sw1 = File.AppendText("c:\\data\\run_list.txt"))
                    {
                        sw1.WriteLine(this.num.ToString() + " ACTIVE at: " + DateTime.Now);
                        sw1.WriteLine(this.num.ToString() + " Name: " + hName);
                    }

                    using (StreamWriter sw2 = File.AppendText("c:\\data\\run_param.txt"))
                    {
                        sw2.WriteLine(this.num.ToString() + " " + rn.textEbeam.Text + " " + rn.textIbeam.Text + " " + rn.BIASVtextBox.Text + " " + rn.GainTextBox.Text + " " + rn.comboPID.Text + " " + rn.textAngle.Text + " " + rn.textXpos.Text + " " + rn.textZpos.Text + " " + rn.textPressure.Text);
                    }

                    //If all goes well above, we are good to go for taking data.
                    UpdateStatus("RUN STARTED");
                    TCP_receiver.SaveEnabled = true;
                    ACTIVE = true;
                }
            }
            catch (Exception e)
            {
                System.Console.Write("Caught Exception {0} in Program.cs!", e);
            }

        }

        public void DeactivateRun()
        {
            try
            {
                using (sw = new StreamWriter(OutFileName, true))
                {
                    sw.WriteLine("-- END OF RUN -- " + DateTime.Now.ToString());
                }

                using (StreamWriter sw1 = File.AppendText("c:\\data\\run_list.txt"))
                {
                    sw1.WriteLine(this.num.ToString() + " STOPPED at: " + DateTime.Now);
                }
            }
            catch { }

            UpdateStatus("RUN STOPPED");
            TCP_receiver.SaveEnabled = false;
            ACTIVE = false;
        }


        public void RecordSpill()
        {
            if (ACTIVE)
            {
                if ((PP.FEB1.client != null) || (PP.FEB2.client != null))
                {
                    TCP_receiver.SaveEnabled = true;
                    num_bytes = 0;
                    if (PP.FEB1.client != null)
                    {
                        TCP_receiver.ReadFeb(ref PP.FEB1, /*PP.FEB1.client PP.FEB1.TNETSocket_prop,*/ out num_bytes);
                        this_bytes_written[0] = num_bytes;
                        total_bytes_written[0] += num_bytes;
                        Application.DoEvents();
                    }
                    if (PP.FEB2.client != null)
                    {
                        TCP_receiver.ReadFeb(ref PP.FEB2, /*PP.FEB1.client PP.FEB2.TNETSocket_prop,*/ out num_bytes);
                        this_bytes_written[1] = num_bytes;
                        total_bytes_written[1] += num_bytes;
                        Application.DoEvents();
                    }
                    //                        TCP_reciever.ReadFeb("WC", PP.WC.TNETSocket_prop, out num_bytes);
                    //spill_complete = true;
                }
            }
        }

        public double ZeroFrac(int minX, int maxX, int minY, int maxY, int num_events)
        {
            double zf = -1;
            double numz = 0;
            double num_e = 0;
            //int maxCh = Events.Last.Value.max_ch;
            //int[,] ch = new int[maxCh, 266];
            //ch = Events.Last.Value.Channels;
            //{
            //    Events.Last.Value.isZero = true;
            //    for (int i = 0; i < maxCh; i++)
            //    {
            //        for (int j = minX; j < maxX; j++)
            //        {
            //            if (ch[i, j] < minY) { Events.Last.Value.HitCount = 0; }
            //            else { Events.Last.Value.HitCount = 1; }

            //            if (ch[i, j] > maxY)
            //            {
            //                Events.Last.Value.isZero = false;
            //                i = maxCh++;
            //                j = maxX++; //force exit
            //            }
            //        }
            //    }
            //}

            //if (this.Events.Count > num_events)
            //{
            //    if (Events.First.Value.HitCount > 0)
            //    {
            //        num_e--;
            //        if (Events.First.Value.isZero) { numz--; }
            //    }
            //    Events.RemoveFirst();
            //    foreach (mu2e_Event e in this.Events)
            //    {
            //        if (e.HitCount > 0)
            //        {
            //            num_e++;
            //            if (e.isZero) { numz++; }
            //        }
            //    }
            //}

            //if (num_e > 0) { zf = numz / num_e; } else { zf = -1; }
            return zf;
        }
    }

    public class CurrentMeasurements
    {
        private ConcurrentDictionary<int, double> currentMeasurements; //concurrent dictionary is thread-safe
        private Mu2e_FEB_client feb;
        private string filename;

        public CurrentMeasurements(Mu2e_FEB_client feb_client, string _filename)
        {
            feb = feb_client;
            filename = _filename;
            currentMeasurements = new ConcurrentDictionary<int, double>();
        }

        public void Purge()
        {
            currentMeasurements.Clear();
        }

        public void TurnOffBias()
        {
            feb.SetVAll(0); //turns off the bias
        }

        public double TakeMeasurement(int channel) //takes a current measurement for the same channel on all FEBs
        {
            int fpga = channel / 16;
            int chan_fpga = channel % 16;
            double measurement = Convert.ToDouble(feb.ReadA0(fpga, chan_fpga));
            currentMeasurements.TryAdd(channel, measurement);
            return measurement;
        }

        public double GetMeasurement(int channel)
        {
            double measurement = -128;
            currentMeasurements.TryGetValue(channel, out measurement);
            return measurement;                
        }

        public void WriteMeasurements(string dicounter, double temperature)
        {
            using (StreamWriter writer = File.AppendText(filename)) //The output file
            {
                writer.Write("{0}\t{1}\t{2}\t", DateTime.Now.ToString("MM/dd/yy HH:mm:ss\t"), dicounter, temperature); //Write current time, name of module, which side, and current temperature
                //foreach (KeyValuePair<int, double> channel in currentMeasurements)
                for (int chan = 0; chan < 64; chan++)
                {
                    if (currentMeasurements.TryGetValue(chan, out double current))
                        writer.Write("{0}\t", current.ToString("0.0000"));
                }
                //writer.Write("{0}\t", channel.Value.ToString("0.0000")); //write the measured current to file
                writer.WriteLine();
            }
        }
    }

    //TODO: FIX THIS CLASS - To QA a module current measurements from 4 different FEBs are needed
    // Should be fixed by code below, accepts any number of FEBs, so if it is 2 or 4 or however many, it should work fine.
    public class ModuleQACurrentMeasurements
    {
        //private List<SortedDictionary<int, double>> currentMeasurements;
        private List<ConcurrentDictionary<int, double>> currentMeasurements; //concurrent dictionary should be thread safe
        private string moduleName, side;
        List<Mu2e_FEB_client> febs;

        public ModuleQACurrentMeasurements(params Mu2e_FEB_client[] feb_clients)
        {
            febs = new List<Mu2e_FEB_client>();
            //currentMeasurements = new List<SortedDictionary<int, double>>();
            currentMeasurements = new List<ConcurrentDictionary<int, double>>();
            foreach (Mu2e_FEB_client feb in feb_clients)
            {
                febs.Add(feb);
                //currentMeasurements.Add(new SortedDictionary<int, double>()); //Add a sorted dictionary for each FEB given  
                currentMeasurements.Add(new ConcurrentDictionary<int, double>());
            }
        }

        public void Purge()
        {
            //moduleName = ""; side = "";
            //foreach(SortedDictionary<int, double> feb in currentMeasurements)
            foreach(ConcurrentDictionary<int, double> feb in currentMeasurements)
                feb.Clear();
        }

        public void TurnOffBias()
        {
            foreach(Mu2e_FEB_client feb in febs)
            {
                feb.SetVAll(0); //turns off the bias for all FEBs
            }
        }
        
        public void SetSide(string side_in)
        {
            side = side_in;
        }

        public void TakeMeasurement(int channel) //takes a current measurement for the same channel on all FEBs
        {
            int fpga = channel / 16;
            int chan_fpga = channel % 16;
            for(int i = 0; i < febs.Count; i++)
                currentMeasurements[i].TryAdd(channel, Convert.ToDouble(febs[i].ReadA0(fpga, chan_fpga)));
        }

        public void WriteMeasurements(string filename, double temperature, int dicounter)
        {
            using (StreamWriter writer = File.AppendText(filename)) //The output file
            {
                writer.Write("{0}\t{1}\t{2}\t{3}\t{4}\t", DateTime.Now.ToString("MM/dd/yy HH:mm\t"), moduleName, side, temperature, dicounter); //Write current time, name of module, which side, and current temperature
               //foreach(SortedDictionary<int, double> feb in currentMeasurements)
               foreach(ConcurrentDictionary<int, double> feb in currentMeasurements)
                    //foreach (KeyValuePair<int, double> channel in feb)
                    for(int chan = 0; chan < 64; chan++)
                    {
                        if(feb.TryGetValue(chan, out double current))
                            writer.Write("{0}\t", current.ToString("0.0000"));
                        //writer.Write("{0}\t", channel.Value.ToString("0.0000")); //write the measured current to file
                    }
                writer.WriteLine();
            }
        }

        public string GetgCodeDicounterPosition(int dicounter, int feedrate)
        {
            int feed = feedrate;
            if (dicounter > 100 || dicounter < 0)
                return "G1 X0 F1000";
            if (feedrate <= 0 || feedrate > 3000)
                feed = 1000;
            const double width_dicounter = 102.63,
                         width_bar = 51.33,
                         offset = 42.00;
            const string basegCode = "G1 X";
            int layer = dicounter / 8, pos_layer = dicounter % 8;
            //double position = (pos_layer * width_dicounter) + (layer * offset);
            double position = dicounter * 5.0; //5mm
            string dicounterPositionCommand = basegCode + position + " F" + Convert.ToString(feed);
            return dicounterPositionCommand;
        }


    }

    static class PP
    {
        public static bool glbDebug=false;
        public static Run myRun;
        public static CurrentMeasurements qaDicounterMeasurements;
        public static ModuleQACurrentMeasurements moduleQACurrentMeasurements;
        //        public static frmTelnet myTelnet;
        //        public static Plot0 myPlot;
        public static frmMain myMain;

        public static List<HISTO_curve> FEB1Histo = new List<HISTO_curve>();
        public static List<HISTO_curve> FEB2Histo = new List<HISTO_curve>();
        public static List<IV_curve> FEB1IVs = new List<IV_curve>();
        public static List<IV_curve> FEB2IVs = new List<IV_curve>();
        public static Mu2e_FEB_client FEB1;
        public static Mu2e_FEB_client FEB2;
        public static WC_client WC;
        public static Mu2e_FECC_client FEC;

        public static TcpClient active_TcpClient = null;
        public static NetworkStream active_Stream = null;
        public static Socket active_Socket = null;
        public static int Random_Number = 0;

        public static double[] lightCheckChanThreshs =
            {   0.25, 0.25, 0.25, 0.25, //0
                0.25, 0.25, 0.25, 0.25, //1
                0.25, 0.25, 0.25, 0.25, //2
                0.25, 0.25, 0.25, 0.25, //3
                0.25, 0.25, 0.25, 0.25, //4
                0.25, 0.25, 0.25, 0.25, //5
                0.25, 0.25, 0.25, 0.25, //6
                0.25, 0.25, 0.25, 0.25, //7
                0.25, 0.25, 0.25, 0.25, //8
                0.25, 0.25, 0.25, 0.25, //9
                0.25, 0.25, 0.25, 0.25, //10
                0.25, 0.25, 0.25, 0.25, //11
                0.25, 0.25, 0.25, 0.25, //12
                0.25, 0.25, 0.25, 0.25, //13
                0.25, 0.25, 0.25, 0.25, //14
                0.25, 0.25, 0.25, 0.25  //15
            }; //default thresholds at 0.25 for light tight check


        //public static FEBC_client FEBC;

        //public static List<Plot0> PlotList = new List<Plot0>();
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]


        //public static void ReadSpill()
        //{
        //    myRun.Events.Clear();

        //    mu2e_Event e = new mu2e_Event();
        //    myRun.Events.AddLast(e);

        //    //try

        //}
        public static void ReadRun()
        {

        }



        /// <summary>
        /// EvNum TimeStamp FrameNum HitCount MaxAmp[ch]
        /// </summary>
        /// <param name="e"></param>
        //public static void Outputevent_ROOT(Event e)
        //{
        //    float fts = (float)e.TimeStamp / 75;
        //    float sum1;

        //    //string t = e.EvNum + " " + e.TimeStamp.ToString() + " " + e.FrameNum.ToString()+" "+ e.HitCount.ToString();
        //    string t = e.EvNum + " " + fts.ToString("F3") + " " + e.FrameNum.ToString() + " " + e.HitCount.ToString();
        //    for (int ch = 0; ch < 16; ch++)
        //    {
        //        int max = 0; sum1 = 0;
        //        for (int sample = 1; sample < 8; sample++)
        //        {
        //            //if (e.Channels[ch, sample] > max) { max = e.Channels[ch, sample]; }
        //            if (e.Channels[ch, sample] > 100) { sum1 += (e.Channels[ch, sample] - 100); }
        //        }
        //        //Nov9: oops I think there is a mess in the data. I will use only 2 and 3 for now
        //        //max = e.Channels[ch, 2] + e.Channels[ch, 3];
        //        t += " " + sum1.ToString();
        //    }
        //    myRun.sw.WriteLine(t);

        //    t = e.EvNum + " " + fts.ToString("F3") + " " + e.FrameNum.ToString() + " " + (e.HitCount + 1).ToString();
        //    for (int ch = 0; ch < 16; ch++)
        //    {
        //        int max = 0; sum1 = 0;
        //        for (int sample = 9; sample < 15; sample++)
        //        {
        //            //if (e.Channels[ch, sample] > max) { max = e.Channels[ch, sample]; }
        //            if (e.Channels[ch, sample] > 100) { sum1 += (e.Channels[ch, sample] - 100); }
        //        }
        //        //Nov9: oops I think there is a mess in the data. I will use only 2 and 3 for now
        //        //max = e.Channels[ch, 2] + e.Channels[ch, 3];
        //        t += " " + sum1.ToString();
        //    }
        //    myRun.sw.WriteLine(t);
        //}


        public static byte[] GetBytes(string str)
        {
            byte[] bytes = new byte[str.Length];
            for (int i = 0; i < str.Length; i++)
            {
                bytes[i] = (byte)(str[i]);
            }
            return bytes;
        }

        public static string GetString(byte[] bytes, int len)
        {
            char[] chars = new char[len];
            for (int i = 0; i < len; i++)
            {
                chars[i] = (char)bytes[i];
            }
            return new string(chars);
        }

        static void Main()
        {
            FEB1Histo = new List<HISTO_curve>();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            //myRun = new Run();
            FEB1 = new Mu2e_FEB_client
            {
                name = "FEB1",
                //host_name_prop = "131.225.52.181";
                //host_name_prop = "128.143.196.218";
                //host_name_prop = "128.143.196.54";
                //host_name_prop = "131.225.52.177";
                host_name_prop = "crvfeb01.fnal.gov"
                //host_name_prop = "131.225.176.32"
                //host_name_prop = "131.225.52.182"
            };

            FEB2 = new Mu2e_FEB_client
            {
                name = "FEB2",
                //host_name_prop = "DCRC5";
                host_name_prop = "crvfeb02.fnal.gov"
                //host_name_prop = "131.225.176.34"
                //host_name_prop = "131.225.52.181"
            };

            //FEC = new Mu2e_FECC_client();

            WC = new WC_client()
            {
                host_name_prop = "FTBFWC01.FNAL.GOV",
                name = "WC"
            };

            //DAQ_server myDAQ_server = new DAQ_server();
            //myDAQ_server.StartRC();

            myMain = new frmMain();
            Application.Run(myMain);
        }


    }
}
