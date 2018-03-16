using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace TB_mu2e
{
    public partial class frmGUI : Form
    {
        public static bool G_Spill_Active;
        public static bool G_Reading_Out;
        public static int G_SPILL_NUM;
        public static bool gStopThis;
        public static bool gFake;

        Int32 listenPort = 21331;    // Receive a message and write it to the console.
        IPEndPoint e;
        UdpClient u;
        //public static List<Event> event_list;
        public static bool event_complete;
        public static int packet_counter;

        public StreamWriter fs;
        public StreamWriter WC_fs;
        public DateTime time_of_last_spill;
        public bool flgGotStatus;

        public frmGUI()
        {
            InitializeComponent();
            e = new IPEndPoint(IPAddress.Any, listenPort);
            u = new UdpClient(e);
            event_complete = true;
            G_SPILL_NUM = 0;
            flgGotStatus = false;
        }

        private void btn_FileInChoose_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "TB4 Data files|*.dat";
            if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                txtInFile.Text = openFileDialog1.FileName;
            }
        }

        private void btn_FileOutChoose_Click(object sender, EventArgs e)
        {
            saveFileDialog1.Filter = "TB4 Data files|*.dat";
            if (saveFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                txtOutFile.Text = saveFileDialog1.FileName;
            }
        }

        private void btn_READ_Click(object sender, EventArgs e)
        {
            //PP.ReadRun();
            PP.ReadSpill();
        }

        private void btnPLOT_Click(object sender, EventArgs e)
        {
            //PP.OpenPlot();
        }

        private void ud_Stop_ValueChanged(object sender, EventArgs e)
        {
            PP.myRun.stop_evt_num = Convert.ToInt32(ud_Stop.Value);
        }

        private void udStart_ValueChanged(object sender, EventArgs e)
        {
            PP.myRun.start_evt_num = Convert.ToInt32(udStart.Value);
        }

        void backgroundWorker1_DoWork(object sender, DoWorkEventArgs evArg)
        {
            //if (PP.myRun.Events.Count == 0)
            //{ Event newEvt = new Event(); PP.myRun.Events.AddLast(newEvt); }

            Event thisEvent = new Event();
            bool messageReceived = false;
            bool messageQuit = false;


            u.Client.ReceiveBufferSize = 8388607; //8meg
            DateTime n = System.DateTime.Now;


            Console.WriteLine("listening for messages on " + listenPort.ToString());

            int waiting_count = 0;
            string t = "";
            string d = "";
            while (!messageQuit)
            {
                //Thread.Sleep(0);
                while (u.Available > 0)
                {
                    Byte[] receiveBytes = u.Receive(ref e); //u.EndReceive(ar, ref e);
                    packet_counter++;
                    t = "";
                    PP.ParseInput(receiveBytes, ref thisEvent, ref event_complete);

                    if (receiveBytes[0] == 1) //data
                    {
                        int ch = Convert.ToInt32(receiveBytes[6]);
                        thisEvent.BoardId = (int)receiveBytes[2];
                        thisEvent.Ticks[ch] = DateTime.Now.Ticks;
                        for (int i = 0; i < 266; i++)
                        {
                            if (i < receiveBytes.Length) { thisEvent.RawBytes[ch, i] = receiveBytes[i]; }
                            else { thisEvent.RawBytes[ch, i] = 0; }
                        }
                        //object o = new object();
                    }
                    //backgroundWorker1.ReportProgress(packet_counter, null);
                    if (event_complete)
                    {
                        thisEvent.EvNum = PP.myRun.Events.Count;
                        PP.myRun.Events.AddLast(thisEvent);
                        thisEvent = new Event();
                    }
                }
            }
        }

        private void backgroundWorker1_ProgressChanged(object sender, System.ComponentModel.ProgressChangedEventArgs e)
        {
            //string t = (string)e.UserState;
            //Console.WriteLine(t);
            this.lblNumEvts.Text = "Packets = " + e.ProgressPercentage.ToString();

        }

        void backgroundWorker1_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {

        }

        private void OpenFiles()
        {
            DateTime n = System.DateTime.Now;
            string fn;
            if (System.Environment.MachineName.Contains("shashlik-daq"))
            {
                fn = "rec_capture_" + n.Year.ToString() + n.Month.ToString("00") + n.Day.ToString("00") + "_" + n.Hour.ToString("00") + n.Minute.ToString("00") + n.Second.ToString("00") + ".txt";
            }
            else
            {
                fn = "c:\\data\\rec_capture_" + n.Year.ToString() + n.Month.ToString("00") + n.Day.ToString("00") + "_" + n.Hour.ToString("00") + n.Minute.ToString("00") + n.Second.ToString("00") + ".txt";
            }
            this.txtOutFile.Text = fn;
            //fs = new StreamWriter(fn);
            fs = new StreamWriter(fn,false,Encoding.ASCII,0x10000);
            //string wc_fn = "c:\\data\\WC_capture_" + n.Year.ToString() + n.Month.ToString("00") + n.Day.ToString("00") + "_" + n.Hour.ToString("00") + n.Minute.ToString("00") + n.Second.ToString("00") + ".txt";
            //WC_fs = new StreamWriter(wc_fn);
        }

        private void btn_Listen_Click(object sender, EventArgs e)
        {
            G_Spill_Active = false;
            btnPLOT.Visible = true;
            btnStartSpill.Visible = true;
            btnStopSpill.Visible = true;
            object o = (object)u;
            //e = (EventArgs)o;
            backgroundWorker1.RunWorkerAsync(o);
            btn_Listen.Visible = false;

            if (!RC_client.ClientOpen) { RC_client.Open(); }
            //if (!WC_client.ClientOpen) { WC_client.Open(); }
            Application.DoEvents();
            RC_client.Clear();
            OpenFiles();
            btnAutomatic.Visible = true;
            G_Spill_Active = false;
            btnPLOT.Visible = true;
            btnStartSpill.Visible = true;
            btnStopSpill.Visible = true;
            btnUpdateStatus.Visible = true;
            btnSetMaxTrig.Visible = true;
            ud_Stop.Visible = true;

            label2.Visible = true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int TotalNumEvents = 0;
            bool res = true;
            gStopThis = false;
            PP.myRun.Events.Clear();
            res = res && RC_client.Arm();
            btnStopThis.Visible = true;
            gFake = true;
            for (int i = 0; i < 2000; i++)
            {
                PP.myRun.Events.Clear();
                packet_counter = 0;

                RC_client.Arm();
                for (int j = 0; j < 128; j++)
                {
                    Thread.Sleep(5);
                    RC_client.SoftwareTrig();
                    if (gStopThis) { i = 2001; btnStopThis.Visible = false; gFake = false; }
                }


                lblDaqMessage2.Text = "fake spill started for spill # " + i.ToString() + " at " + DateTime.Now.TimeOfDay + "with " + TotalNumEvents;
                if (TotalNumEvents > 40000) { i = 2001; btnStopThis.Visible = false; }
                Application.DoEvents();
                for (int j = 0; j < 50; j++)
                {
                    Thread.Sleep(5);
                    if (gStopThis) { i = 2001; btnStopThis.Visible = false; gFake = false; }
                }

                btnStopSpill_Click(null, null);
                lblDaqMessage2.Text = "fake spill ended for spill # " + i.ToString() + " at " + DateTime.Now.TimeOfDay + " with " + PP.myRun.Events.Count;
                TotalNumEvents += PP.myRun.Events.Count;
                Thread.Sleep(1000);
                Application.DoEvents();
                for (int j = 0; j < 10; j++)
                {
                    Thread.Sleep(5);
                    Application.DoEvents();
                    lblDaqMessage.Text = "waiting for " + (10 - j) * 5 / 1000;
                    if (gStopThis) { i = 2001; btnStopThis.Visible = false; gFake = false; }
                }
            }
        }

        private void btnStartSpill_Click(object sender, EventArgs e)
        {
            PP.myRun.Events.Clear();
            packet_counter = 0;

            RC_client.Arm();
        }

        private void btnStopSpill_Click(object sender, EventArgs e)
        {
            bool events_reading = true;
            int old_count = 0;
            int timeout = 0;
            int timeout_max = 20;
            string[] spill_stat;
            RC_client.Disarm();
            RC_client.GetStatus(out spill_stat);
            PP.myRun.spill_status = spill_stat;
            RC_client.ReadAll();

            while (events_reading)
            {
                this.lblDaqMessage.Visible = true;
                this.lblDaqMessage.Text = "looking for events : timeout=" + timeout.ToString();
                //this.lblDaqMessage2.Text = "last event from " + PP.myRun.Events.Last.Value.RawBytes[0, 6].ToString();
                Thread.Sleep(50);
                Application.DoEvents();
                if (PP.myRun.Events.Count > old_count)
                {
                    events_reading = true; old_count = PP.myRun.Events.Count; timeout = 0;
                    this.lblDaqMessage.Text = old_count + " events read so far";
                }
                else
                {
                    timeout++;
                    if (timeout < timeout_max) { }
                    else { events_reading = false; }
                }
            }
            this.lblDaqMessage.Text = "all done";

            Event this_evt = new Event();
            LinkedListNode<Event> this_node = PP.myRun.Events.First;
            bool in_spill;
            string sz_num_trig;
            string sz_WC_time;
            WC_client.check_status(out in_spill, out sz_num_trig, out sz_WC_time);
            G_SPILL_NUM++;
            fs.WriteLine("*** starting spill num " + G_SPILL_NUM.ToString() + " *** at " + DateTime.Now.ToString() + " WC says: num trig= " + sz_num_trig + " time = " + sz_WC_time);
            if (gFake) { fs.WriteLine("*** this spill is fake ***"); }
            for (int i = 0; i < PP.myRun.spill_status.Length - 1; i++)
            {
                fs.WriteLine("*** spill status " + i + " " + PP.myRun.spill_status[i] + " ***");
            }
            for (int i = 0; i < PP.myRun.Events.Count; i++)
            {
                if (this_node != null)
                {
                    this_evt = this_node.Value;
                    for (int j = 0; j < 32; j++)
                    {
                        if (this_evt.RawBytes[j, 0] < 32)
                        {
                            string t = this_evt.Ticks[j].ToString();
                            for (int k = 0; k < 9; k++)
                            {
                                t += " " + this_evt.RawBytes[j, k].ToString("X2");
                            }
                            for (int k = 0; k < 120; k++)
                            {
                                t += " " + this_evt.Channels[j, k].ToString("X3");
                            }
                            fs.WriteLine(t);
                            lblDaqMessage2.Text = i.ToString();
                        }

                    }
                }
                this_node = this_node.Next;
            }
            RC_client.Clear();
            fs.Flush();
            G_Spill_Active = false;
            G_Reading_Out = false;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {

            string sz_trig;
            string sz_WC_time;
            bool in_spill;
            WC_client.check_status(out in_spill, out  sz_trig, out sz_WC_time);

            lblTrigEn.Text = "";
            if (G_Spill_Active) { lblTrigEn.Text += "TRIG ARMED"; }
            if (G_Reading_Out) { lblTrigEn.Text += " and READING OUT"; }

            if (in_spill)
            { lblSpillStatus.Text = "in spill"; time_of_last_spill = DateTime.Now; }
            else
            { lblSpillStatus.Text = "waiting..." + G_SPILL_NUM.ToString() + " spills so far"; }

            //t = WC_client.ReadTrigEn();
            //lblTrigEn.Text = t;
            if (sz_trig.Length < 1) { }
            else
            {
                lblLastTime.Text = DateTime.Now.Subtract(time_of_last_spill).TotalSeconds.ToString("F0");

                if (!G_Spill_Active)
                {
                    if ((DateTime.Now.Subtract(time_of_last_spill).TotalSeconds > 40) && (DateTime.Now.Subtract(time_of_last_spill).TotalSeconds < 44))
                    {
                        G_Spill_Active = true;
                        btnStartSpill_Click(null, null);
                    }
                }
                else
                {
                    if (!G_Reading_Out)
                    {
                        if ((DateTime.Now.Subtract(time_of_last_spill).TotalSeconds < 4) && (DateTime.Now.Subtract(time_of_last_spill).TotalSeconds > 1))
                        {
                            //G_SPILL_NUM++;
                            //WC_fs.WriteLine("*** starting spill num " + G_SPILL_NUM.ToString() + " *** at " + DateTime.Now.ToString());
                            //fs.WriteLine("*** starting spill num " + G_SPILL_NUM.ToString() + " *** at " + DateTime.Now.ToString());
                            timer1.Enabled = false;
                            //timer1.Enabled = false;
                            int ret_len = 0;
                            //byte[] data = WC_client.read_TDC(out ret_len);
                            //string status = "Read " + ret_len + " bytes from WC";
                            //lblWC_status.Text = status;
                            //int j = 0;
                            //for (int i = 0; i < ret_len; i++)
                            //{
                            //    WC_fs.Write(data[i].ToString("X2") + " ");
                            //    j++;
                            //    if (j == 8)
                            //    {
                            //        j = 0; WC_fs.WriteLine();
                            //    }
                            //}
                            //WC_fs.WriteLine();
                            flgGotStatus = false;
                            timer1.Enabled = true;
                            if ((DateTime.Now.Subtract(time_of_last_spill).TotalSeconds < 38) && (DateTime.Now.Subtract(time_of_last_spill).TotalSeconds > 1))
                            {
                                timer1.Enabled = false;
                                G_Reading_Out = true;
                                btnStopSpill_Click(null, null);
                                timer1.Enabled = true;
                            }

                        }
                        else
                        {
                            if (!flgGotStatus)
                            {
                                string[] tt;
                                flgGotStatus = true;
                                int num_pade = RC_client.GetStatus(out tt);
                                if (tt.Length > 0) { lblStat1.Text = tt[0]; } else { lblStat1.Text = ""; }
                                if (tt.Length > 1) { lblStat2.Text = tt[1]; } else { lblStat2.Text = ""; }
                                if (tt.Length > 2) { lblStat3.Text = tt[2]; } else { lblStat3.Text = ""; }
                                if (tt.Length > 3) { lblStat4.Text = tt[3]; } else { lblStat4.Text = ""; }
                            }
                        }
                    }
                    else
                    {
                        lblStat1.Text = "currently reading..." + PP.myRun.Events.Count + " so far";

                    }
                }
            }

        }

        private void button4_Click(object sender, EventArgs e)
        {
            //if (!RC_client.ClientOpen) { RC_client.Open(); }
            //if (!WC_client.ClientOpen) { WC_client.Open(); }
            //fs.Close();
            ////WC_fs.Close();
            //OpenFiles();
            //timer1.Interval = 100;
            //timer1.Enabled = true;

        }

        private void btnAutomatic_Click(object sender, EventArgs e)
        {
            if (btnAutomatic.Text.Contains("to stop"))
            {
                if ((timer1.Enabled == true) && (!G_Spill_Active) && (!G_Reading_Out))
                {
                    timer1.Interval = 999;
                    timer1.Enabled = false;
                    fs.Close();
                    //WC_fs.Close();
                    RC_client.Close();
                    WC_client.Close();
                    btnAutomatic.Text = "ACTIVATE AUTO RUN";
                    btnUpdateStatus.Visible = false;
                }
                else
                {
                    btnAutomatic.Text = "waiting to stop. Click again!";
                    Application.DoEvents();
                }
            }
            else
            {
                timer1.Interval = 1000;
                button4_Click(null, null);
                btnAutomatic.Text = "AUTO ACTIVE (click to stop)";
                G_SPILL_NUM = 0;

                Thread.Sleep(100);
                //try
                //{
                //    if (lblLastTime.Text.Substring(1, 2) == "10") { WC_client.EnableTrig(); }
                //}
                //catch { }
            }
        }

        private void btnSetMaxTrig_Click(object sender, EventArgs e)
        {
            //RC_client.SetMaxTrig(Convert.ToInt32(ud_Stop.Value));
        }

        private void btnUpdateStatus_Click(object sender, EventArgs e)
        {
            string[] tt;
            int num_pade = RC_client.GetStatus(out tt);
            if (tt.Length > 0) { lblStat1.Text = tt[0]; } else { lblStat1.Text = "no status returned"; }
            if (tt.Length > 1) { lblStat2.Text = tt[1]; } else { lblStat2.Text = "no status returned"; }
            if (tt.Length > 2) { lblStat3.Text = tt[2]; } else { lblStat3.Text = "no status returned"; }
            if (tt.Length > 3) { lblStat3.Text = tt[3]; } else { lblStat4.Text = "no status returned"; }
        }

        private void btnStopThis_Click(object sender, EventArgs e)
        {
            gStopThis = true;
        }
    }
}
