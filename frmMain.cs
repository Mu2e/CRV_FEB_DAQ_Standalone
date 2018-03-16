using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Reflection;
using System.Text;
//using System.Threading.Tasks;
using System.Windows.Forms;
using ZedGraph;

namespace TB_mu2e
{

    public partial class frmMain : Form
    {
        private int _ActiveFEB = 0;
        private int reg_set = 0;
        private bool flgBreak = false;

        private string msg1Conn = "";
        private string msg2Conn = "";
        private uConsole console_label;
        private List<Button> sel_list;

        private static int num_reg = 15;
        private TextBox[] txtREGISTERS = new TextBox[num_reg];
        private static int num_spill_reg = 5;
        private TextBox[] txtSPILL = new TextBox[num_spill_reg];
        CheckBox[] chkChEnable = new CheckBox[64];
        private string[] rnames = new string[50];
        private System.Windows.Forms.Label[] lblREG = new System.Windows.Forms.Label[num_reg + num_spill_reg];
        private bool _IntegralScan = false;

        private SpillData DispSpill;
        private Mu2e_Event DispEvent;
        private bool DebugLogging;
        private int[] spill_trig_num;

        private const int num_chans = 16;
        private System.Windows.Forms.Label[] BDVoltLabels = new System.Windows.Forms.Label[num_chans];

        private SerialPort comPort;

        public void AddConsoleMessage(string mess)
        {
            console_label.add_messg(mess);
        }

        public frmMain()
        {
            InitializeComponent();
            btnFEB1.BackColor = SystemColors.Control;
            lblMessage.Text = msg1Conn + "\n" + msg2Conn;

            btnFEB1.Click += new System.EventHandler(this.Button1_Click);
            btnFEB1.Tag = PP.FEB1; btnFEB1.Text = PP.FEB1.host_name_prop;
            btnFEB2.Click += new System.EventHandler(this.Button1_Click);
            btnFEB2.Tag = PP.FEB2; btnFEB2.Text = PP.FEB2.host_name_prop;

            btnWC.Click += new System.EventHandler(this.Button1_Click);
            btnWC.Tag = PP.WC; btnWC.Text = PP.WC.host_name_prop;

            console_label = new uConsole();


            spill_trig_num = new int[3];
            for (int i = 0; i < 3; i++)
            {
                spill_trig_num[i] = 0;
            }


            #region Registers

            rnames[0] = "CONTROL_STATUS";
            rnames[1] = "SDRAM_WritePointer";
            rnames[2] = "SDRAM_ReadPointer";
            rnames[3] = "TEST_PULSE_FREQ";//pipe_del
            rnames[4] = "TEST_PULSE_DURATION";//num_samp
            rnames[5] = "TEST_PULSE_INTERSPILL";//mux_ctrl
            rnames[6] = "CHAN_MASK";//trig_ctrl
            rnames[7] = "MUX_SEL";//afe_fifo
            rnames[8] = "TRIG_CONTROL";//mig_stat
            rnames[9] = "SPILL_TRIG_COUNT";//mig_fifo
            ///
            rnames[10] = "SPILL_NUMBER";
            rnames[11] = "SPILL_STATE";
            rnames[12] = "SPILL_TRIG_COUNT";
            rnames[13] = "SPILL_WORD_COUNT";
            rnames[14] = "UPTIME";

            for (int i = 0; i < num_reg; i++)
            {
                txtREGISTERS[i] = new TextBox()
                {
                    Location = new System.Drawing.Point(240, 16 + 24 * i),
                    Size = new System.Drawing.Size(100, 15),
                    TextAlign = HorizontalAlignment.Right,
                    Tag = i
                };
                lblREG[i] = new System.Windows.Forms.Label()
                {
                    Location = new System.Drawing.Point(1, 20 + 24 * i),
                    Size = new System.Drawing.Size(230, 20),
                    TextAlign = ContentAlignment.MiddleRight,
                    Text = rnames[i]
                };
            }

            for (int k = 0; k < 2; k++)
            {
                foreach (Control c in tabControl.TabPages[2 + k].Controls)
                {
                    for (int i = 0; i < num_reg; i++)
                    {
                        if (c.Name.Contains("groupBoxREG"))
                        {
                            c.Controls.Add(txtREGISTERS[i]);
                            c.Controls.Add(lblREG[i]);
                        }
                    }
                }

            }
            for (int i = 0; i < num_spill_reg; i++)
            {
                txtSPILL[i] = new TextBox()
                {
                    Location = new System.Drawing.Point(125, 20 * (i + 1)),
                    Size = new System.Drawing.Size(120, 17),
                    TextAlign = HorizontalAlignment.Right,
                    Tag = i
                };
            }

            for (int k = 0; k < 2; k++)
            {
                foreach (Control c in tabControl.TabPages[2 + k].Controls)
                {
                    for (int i = 0; i < num_reg; i++)
                    {
                        if (c.Name == "groupBoxSPILL")
                        { c.Controls.Add(txtSPILL[i]); }
                    }
                }

            }
            #endregion Registers

            #region ch_select

            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 16; j++)
                {
                    chkChEnable[j * 4 + i] = new CheckBox()
                    {
                        Location = new System.Drawing.Point(6 + 45 * j, 20 * i),
                        Size = new System.Drawing.Size(45, 20),
                        Text = (j * 4 + i + 1).ToString(),
                        Tag = j * 4 + i
                    };
                }
            }
            for (int k = 0; k < 2; k++)
            {
                foreach (Control c in tabControl.TabPages[k].Controls)
                {
                    for (int i = 0; i < 32; i++)
                    {
                        if (c.Name == "groupBoxEvDisplay")
                        { c.Controls.Add(chkChEnable[i]); }
                    }
                }

            }
            #endregion ch_select

            #region voltLabels
            BDVoltLabels[0] = BDVoltsLabel0;
            BDVoltLabels[1] = BDVoltsLabel1;
            BDVoltLabels[2] = BDVoltsLabel2;
            BDVoltLabels[3] = BDVoltsLabel3;
            BDVoltLabels[4] = BDVoltsLabel4;
            BDVoltLabels[5] = BDVoltsLabel5;
            BDVoltLabels[6] = BDVoltsLabel6;
            BDVoltLabels[7] = BDVoltsLabel7;
            BDVoltLabels[8] = BDVoltsLabel8;
            BDVoltLabels[9] = BDVoltsLabel9;
            BDVoltLabels[10] = BDVoltsLabel10;
            BDVoltLabels[11] = BDVoltsLabel11;
            BDVoltLabels[12] = BDVoltsLabel12;
            BDVoltLabels[13] = BDVoltsLabel13;
            BDVoltLabels[14] = BDVoltsLabel14;
            BDVoltLabels[15] = BDVoltsLabel15;
            #endregion
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            Button mySender = (Button)sender;
            string myName = mySender.Text;
            IStenComms comm = (IStenComms)mySender.Tag;
            if (!comm.ClientOpen) { comm.DoOpen(); }
            if (!comm.ClientOpen)
            {
                //label1.Text = comm.m_prop;
                mySender.BackColor = Color.Red;
                if (comm.name.Contains("FEB1")) { btnFEB1.BackColor = Color.Red; }
                if (comm.name.Contains("FEB2")) { btnFEB2.BackColor = Color.Red; }
                if (comm.name.Contains("WC")) { btnWC.BackColor = Color.Red; }
                if (comm.name.Contains("FECC")) { lblFEB1.BackColor = Color.Red; }
            }
            else
            {
                mySender.BackColor = Color.Green;
                if (comm.name.Contains("FEB1"))
                {
                    lblFEB1.BackColor = Color.Green;
                    btnFEB1.BackColor = Color.LightSeaGreen;
                    btnFEB2.BackColor = Color.Green;
                    _ActiveFEB = 1;
                    lblActive.Text = " FEB1";
                    PP.active_Stream = PP.FEB1.stream;
                    PP.active_Socket = PP.FEB1.TNETSocket;
                    dbgFEB1.BackColor = Color.Green;
                    dbgFEB2.BackColor = Color.Gray;
                    dbgWC.BackColor = Color.Gray;
                    BtnBiasREAD_Click(null, null);
                    BtnRegREAD_Click(null, null);
                }
                if (comm.name.Contains("FEB2"))
                {
                    lblFEB2.BackColor = Color.Green;
                    btnFEB2.BackColor = Color.LightSeaGreen;
                    btnFEB1.BackColor = Color.Green;
                    _ActiveFEB = 2;
                    lblActive.Text = " FEB2";
                    PP.active_Stream = PP.FEB2.stream;
                    PP.active_Socket = PP.FEB2.TNETSocket;
                    dbgFEB2.BackColor = Color.Green;
                    dbgFEB1.BackColor = Color.Gray;
                    dbgWC.BackColor = Color.Gray;
                    BtnBiasREAD_Click(null, null);
                    BtnRegREAD_Click(null, null);
                }
                if (comm.name.Contains("WC"))
                {
                    lblWC.BackColor = Color.Green;
                    _ActiveFEB = 0;
                    lblActive.Text = "NONE";
                    PP.active_Stream = PP.WC.stream_prop;
                    PP.active_Socket = PP.WC.TNETSocket_prop;
                    dbgFEB2.BackColor = Color.Gray;
                    dbgFEB1.BackColor = Color.Gray;
                    dbgWC.BackColor = Color.Green;
                }
                if (comm.name.Contains("FECC")) { }
                lblMessage.Text = DateTime.Now + " -> " + comm.m_prop;
            }
        }

        public void Select_module_Click(object sender, EventArgs e)
        {

        }

        private void TextBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (((Control.ModifierKeys & Keys.Control) == Keys.Control))
            {
                if (e.KeyChar.ToString() == " ")
                {
                    if (dbgFEB2.BackColor == Color.Green)
                    { DbgFEB_Click((object)dbgWC, null); textBox1.Text = ""; return; }
                    if (dbgFEB1.BackColor == Color.Green)
                    { DbgFEB_Click((object)dbgFEB2, null); textBox1.Text = ""; return; }
                    if (dbgWC.BackColor == Color.Green)
                    { DbgFEB_Click((object)dbgFEB1, null); textBox1.Text = ""; return; }
                }
                if (e.KeyChar.ToString() == "1")
                { DbgFEB_Click((object)dbgWC, null); }
                if (e.KeyChar.ToString() == "2")
                { DbgFEB_Click((object)dbgWC, null); }
            }
        }

        private void TextBox1_TextChanged(object sender, EventArgs e)
        {
            string l = "";
            bool this_is_a_write = false;
            string tt = "";

            if (textBox1.Text.Contains("\n"))
            {
                try
                {
                    if (textBox1.Text.Contains("$")) //this is a comment
                    { }
                    else
                    {
                        byte[] buf = PP.GetBytes(textBox1.Text);
                        tt = textBox1.Text;
                        //textBox1.Text = textBox1.Text.Substring(0, textBox1.Text.Length - 2);
                        if (textBox1.Text.ToLower().Contains("wr")) { this_is_a_write = true; }
                        textBox1.Text = "";
                        while (PP.active_Socket.Available > 0)
                        {
                            byte[] rbuf = new byte[PP.active_Socket.Available];
                            PP.active_Socket.Receive(rbuf);
                        }
                        PP.active_Socket.Send(buf);
                        System.Threading.Thread.Sleep(1);
                        int max_timeout = 50; int timeout = 0;
                        if (this_is_a_write) { max_timeout = 3; }
                        while ((PP.active_Socket.Available == 0) && (timeout < max_timeout))
                        {
                            timeout++; System.Threading.Thread.Sleep(2);
                        }
                        if (timeout < max_timeout)
                        {
                            byte[] rec_buf = new byte[PP.active_Socket.Available];
                            int ret_len = PP.active_Socket.Receive(rec_buf);
                            string t = PP.GetString(rec_buf, ret_len);
                            t = tt.Substring(0, tt.Length - 2) + ": " + t;
                            l = console_label.add_messg(t);
                        }
                        else
                        {
                            if (this_is_a_write)
                            { l = console_label.add_messg(tt); }//there is no response to a write
                            else
                            { l = console_label.add_messg("timeout!"); }
                        }
                        lblConsole_disp.Text = l;
                        Application.DoEvents();
                    }
                }
                catch
                {
                    string m = "Exception caught. Do yu have a module selected?";
                    lblConsole_disp.Text = m;
                    Application.DoEvents();
                }
            }
        }

        private void BtnRegREAD_Click(object sender, EventArgs e)
        {
            Mu2e_FEB_client myFEB = null;
            int i = this.tabControl.SelectedIndex;
            if (tabControl.SelectedTab.Text.Contains("FEB"))
            {
                if (_ActiveFEB == 1)
                { myFEB = PP.FEB1; }
                else if (_ActiveFEB == 2)
                { myFEB = PP.FEB2; }
                else
                { MessageBox.Show("No FEB active"); return; }

                myFEB.checkFEB_connection();

                ushort fpga_num = Convert.ToUInt16(udFPGA.Value);
                for (int j = 0; j < num_reg; j++)
                {
                    Mu2e_Register.FindName(rnames[j], ref myFEB.arrReg, out Mu2e_Register r1);
                    r1.fpga_index = fpga_num;
                    Mu2e_Register.ReadReg(ref r1, ref myFEB.client);
                    if (!r1.pref_hex)
                    { txtREGISTERS[j].Text = r1.val.ToString(); }
                    else
                    { txtREGISTERS[j].Text = "0x" + Convert.ToString(r1.val, 16); }
                }
            }
        }

        private void BtnRegMONITOR_Click(object sender, EventArgs e)
        {

        }

        private void BtnRegWRITE_Click(object sender, EventArgs e)
        {
            Mu2e_FEB_client myFEB = null;
            int i = this.tabControl.SelectedIndex;
            if (tabControl.SelectedTab.Text.Contains("FEB"))
            {
                if (_ActiveFEB == 1)
                { myFEB = PP.FEB1; }
                if (_ActiveFEB == 2)
                { myFEB = PP.FEB2; }

                ushort fpga_num = Convert.ToUInt16(udFPGA.Value);
                for (int j = 0; j < num_reg; j++)
                {
                    Mu2e_Register.FindName(rnames[j], ref myFEB.arrReg, out Mu2e_Register r1);
                    r1.fpga_index = fpga_num;
                    if (txtREGISTERS[j].Text.Contains("x"))
                    {
                        try
                        {
                            UInt32 v = Convert.ToUInt32(txtREGISTERS[j].Text, 16);
                            Mu2e_Register.WriteReg(v, ref r1, ref myFEB.client);
                        }
                        catch
                        { txtREGISTERS[j].Text = "?"; }
                    }
                    else
                    {
                        try
                        {
                            UInt32 v = Convert.ToUInt32(txtREGISTERS[j].Text);
                            Mu2e_Register.WriteReg(v, ref r1, ref myFEB.client);
                        }
                        catch
                        { txtREGISTERS[j].Text = "?"; }
                    }
                }
            }
        }

        private void DbgFEB_Click(object sender, EventArgs e)
        {
            Button mySender = (Button)sender;
            string myName = mySender.Text;
            if (myName.Contains("FEB1"))
            {
                Button1_Click((object)btnFEB1, e);
                lblConsole_disp.Text = console_label.add_messg("---- FEB1 ----\r\n");
            }
            if (myName.Contains("FEB2"))
            {
                Button1_Click((object)btnFEB2, e);
                lblConsole_disp.Text = console_label.add_messg("---- FEB2 ----\r\n");
            }
            if (myName.Contains("WC"))
            {
                Button1_Click((object)btnWC, e);
                lblConsole_disp.Text = console_label.add_messg("----  WC  ----\r\n");
            }
            if (myName.Contains("FECC")) { }
        }

        private void BtnSpillREAD_Click(object sender, EventArgs e)
        {
            Mu2e_FEB_client myFEB = null;
            int i = this.tabControl.SelectedIndex;
            if (tabControl.SelectedTab.Text.Contains("FEB"))
            {
                if (_ActiveFEB == 1)
                { myFEB = PP.FEB1; }
                if (_ActiveFEB == 2)
                { myFEB = PP.FEB2; }


                ushort fpga_num = Convert.ToUInt16(udFPGA.Value);
                for (int j = 0; j < num_spill_reg; j++)
                {
                    Mu2e_Register.FindName(rnames[j], ref myFEB.arrReg, out Mu2e_Register r1);
                    r1.fpga_index = fpga_num;
                    Mu2e_Register.ReadReg(ref r1, ref myFEB.client);
                    if (!r1.pref_hex)
                    { txtSPILL[j].Text = r1.val.ToString(); }
                    else
                    { txtSPILL[j].Text = "0x" + Convert.ToString(r1.val, 16); }
                }
            }
        }

        private void BtnSpillWRITE_Click(object sender, EventArgs e)
        {
            Mu2e_FEB_client myFEB = null;
            int i = this.tabControl.SelectedIndex;
            if (tabControl.SelectedTab.Text.Contains("FEB"))
            {
                if (_ActiveFEB == 1)
                { myFEB = PP.FEB1; }
                if (_ActiveFEB == 2)
                { myFEB = PP.FEB2; }


                ushort fpga_num = Convert.ToUInt16(udFPGA.Value);
                for (int j = 0; j < num_spill_reg; j++)
                {
                    Mu2e_Register.FindName(rnames[j], ref myFEB.arrReg, out Mu2e_Register r1);
                    r1.fpga_index = fpga_num;
                    if (txtSPILL[j].Text.Contains("x"))
                    {
                        try
                        {
                            UInt32 v = Convert.ToUInt32(txtREGISTERS[j].Text, 16);
                            Mu2e_Register.WriteReg(v, ref r1, ref myFEB.client);
                        }
                        catch
                        { txtSPILL[j].Text = "?"; }
                    }
                    else
                    {
                        try
                        {
                            UInt32 v = Convert.ToUInt32(txtSPILL[j].Text);
                            Mu2e_Register.WriteReg(v, ref r1, ref myFEB.client);
                        }
                        catch
                        { txtSPILL[j].Text = "?"; }
                    }
                }
            }
        }

        private void BtnSpillMON_Click(object sender, EventArgs e)
        {

        }

        private void BtnBiasREAD_Click(object sender, EventArgs e)
        {
            double[] cmb_temp = new double[5];
            string name = tabControl.SelectedTab.Text;
            if (name.Contains("FEB"))
            {
                //Console.WriteLine("NEW READING");
                //for (int t = 0; t < 300; t++)
                //{
                //    for (int i = 0; i < 8; i++)
                //    {
                //        double I1 = 0;
                //        int nGoodReads = 0;
                //        for (int k = 0; k < nreads.Value; k++)
                //        {
                //            double I1now = PP.FEB1.ReadA0((int)udFPGA.Value, i);
                //            if (I1now > 0)
                //            {
                //                I1 = I1 + I1now;
                //                nGoodReads++;
                //            }
                //        }
                //        Console.Write(" " + I1/nGoodReads);
                //    }
                //    Console.WriteLine("");
                //    System.Threading.Thread.Sleep(60000);
                //}

                switch (_ActiveFEB)
                {
                    case 1:
                        txtV.Text = PP.FEB1.ReadV((int)udFPGA.Value).ToString("0.000");
                        txtI.Text = PP.FEB1.ReadA0((int)udFPGA.Value, (int)udChan.Value).ToString("0.0000");

                        double I1 = 0;
                        int nGoodReads = 0;
                        for (int i = 0; i < nreads.Value; i++)
                        {
                            double I1now = PP.FEB1.ReadA0((int)udFPGA.Value, (int)udChan.Value);
                            if (I1now > 0)
                            {
                                I1 = I1 + I1now;
                                nGoodReads++;
                            }
                            //                            Console.WriteLine(PP.FEB1.ReadA0((int)udFPGA.Value, (int)udChan.Value));
                        }
                        txtI.Text = (I1 / (double)(nGoodReads)).ToString("0.0000");

                        PP.FEB1.ReadTemp(out cmb_temp[1], out cmb_temp[2], out cmb_temp[3], out cmb_temp[4], (int)udFPGA.Value);
                        break;
                    case 2:
                        txtV.Text = PP.FEB2.ReadV((int)udFPGA.Value).ToString("0.000");
                        txtI.Text = PP.FEB2.ReadA0((int)udFPGA.Value, (int)udChan.Value).ToString("0.0000");
                        PP.FEB2.ReadTemp(out cmb_temp[1], out cmb_temp[2], out cmb_temp[3], out cmb_temp[4], (int)udFPGA.Value);
                        break;

                    default:
                        break;
                }
            }
            txtCMB_Temp1.Text = cmb_temp[1].ToString("0.0");
            txtCMB_Temp2.Text = cmb_temp[2].ToString("0.0");
            txtCMB_Temp3.Text = cmb_temp[3].ToString("0.0");
            txtCMB_Temp4.Text = cmb_temp[4].ToString("0.0");
        }

        private void BtnBiasWRITE_Click(object sender, EventArgs e)
        {
            string name = tabControl.SelectedTab.Text;
            if (name.Contains("FEB"))
            {
                switch (_ActiveFEB)
                {
                    case 1:
                        PP.FEB1.SetV(Convert.ToDouble(txtV.Text), (int)udFPGA.Value);
                        txtI.Text = PP.FEB1.ReadA0((int)udFPGA.Value, (int)udChan.Value).ToString("0.0000");
                        break;
                    case 2:
                        PP.FEB2.SetV(Convert.ToDouble(txtV.Text));
                        txtI.Text = PP.FEB2.ReadA0((int)udFPGA.Value, (int)udChan.Value).ToString("0.0000");
                        break;

                    default:
                        break;
                }
            }
        }

        private void BtnBIAS_MON_Click(object sender, EventArgs e)
        {

        }

        private void TabFEB2_Click(object sender, EventArgs e)
        {

        }

        private void BtnScan_Click(object sender, EventArgs e)
        {


            //zedFEB1.GraphPane.CurveList.Clear();
            Mu2e_FEB_client FEB = new Mu2e_FEB_client();
            switch (_ActiveFEB)
            {
                case 1:
                    FEB = PP.FEB1;
                    break;
                case 2:
                    FEB = PP.FEB2;
                    break;
            }

            if (btnScan.Text != "SCAN")
            { flgBreak = true; return; }

            #region Broken code - needs to be made compatible with new register map
            //if (ShowSpect.Visible)
            //{
            //    IV_curve myIV = new IV_curve();
            //    UdChan_ValueChanged(null, null);
            //    myIV.min_v = (double)udStart.Value;
            //    myIV.max_v = (double)udStop.Value;
            //    myIV.chan = (Int32)udChan.Value;
            //    myIV.board = _ActiveFEB;
            //    decimal v = udStart.Value;
            //    if (!scanAllChanBox.Checked) //Normal usage
            //    {
            //        while ((v < udStop.Value) & !flgBreak)
            //        {
            //            txtV.Text = v.ToString("0.000");
            //            BtnBiasWRITE_Click(null, null);
            //            Application.DoEvents();
            //            double I = Convert.ToDouble(txtI.Text);
            //            myIV.AddPoint((double)v, (double)I);


            //            //plot
            //            UpdateDisplay_IV(myIV);
            //            btnScan.Text = v.ToString("0.00") + "V";
            //            v += udInterval.Value / 1000;
            //            System.Threading.Thread.Sleep(10);
            //            Application.DoEvents();
            //        }

            //        //Cole's Determination of Breakdown Voltage
            //        List<double> voltages = new List<double>();
            //        List<double> logCurrents = new List<double>();
            //        int ptCount = 0;
            //        foreach (PointPair p in myIV.loglist)
            //        {
            //            if (p.Y < 0.000001 || p.Y > 120)
            //            {
            //                continue;
            //            }
            //            voltages.Add(p.X);
            //            logCurrents.Add(p.Y);
            //            ptCount++;
            //        }
            //        //Take derivatives, find max
            //        List<double> dLogCurrents = new List<double>();
            //        for (int n = 0; n < ptCount - 2; n++)
            //        {
            //            dLogCurrents.Add((logCurrents[n + 2] - logCurrents[n]) / (voltages[n + 2] - voltages[n]));
            //        }
            //        double breakDownVolts = voltages[dLogCurrents.IndexOf(dLogCurrents.Max()) + 1];
            //        BDVoltLabels[myIV.chan].Text = breakDownVolts.ToString() + " V";

            //        if (myIV.saved == false) { myIV.Save(); }
            //        myIV.saved = true;
            //        myIV.saved_time = DateTime.Now;
            //    }
            //    else //Find breakdown voltage of all 16 channels
            //    {
            //        List<double>[] chanVolts = new List<double>[num_chans];
            //        List<double>[] chanCurrents = new List<double>[num_chans];
            //        List<double>[] chanLogCurrents = new List<double>[num_chans];
            //        //Initialize all the lists
            //        for (int i = 0; i < num_chans; i++)
            //        {
            //            chanVolts[i] = new List<double>();
            //            chanCurrents[i] = new List<double>();
            //            chanLogCurrents[i] = new List<double>();
            //        }

            //        //Perform the scan
            //        while ((v < udStop.Value) & !flgBreak)
            //        {
            //            txtV.Text = v.ToString("0.000");
            //            BtnBiasWRITE_Click(null, null);
            //            Application.DoEvents();

            //            //Read in all voltages/currents
            //            for (int i = 0; i < num_chans; i++)
            //            {
            //                double chanI = PP.FEB1.ReadA0((int)udFPGA.Value, i);
            //                //Console.WriteLine("Current of Chan {0}: {1}", i.ToString(), chanI.ToString("0.00000"));
            //                if (chanI < 0.00001 || chanI > 120) //Ignore weird spikes in current
            //                {
            //                    continue;
            //                }
            //                chanVolts[i].Add((double)v);
            //                chanCurrents[i].Add(chanI);
            //                chanLogCurrents[i].Add(Math.Log(chanI));
            //            }

            //            btnScan.Text = v.ToString("0.00") + "V";

            //            v += udInterval.Value / 1000;

            //            System.Threading.Thread.Sleep(10);
            //            Application.DoEvents();
            //        }

            //        //Calculate and display all the breakdown voltages
            //        for (int i = 0; i < num_chans; i++)
            //        {
            //            List<double> voltages = chanVolts[i];
            //            List<double> logCurrents = chanLogCurrents[i];
            //            List<double> dLogCurrents = new List<double>();
            //            for (int n = 0; n < voltages.Count - 2; n++)
            //            {
            //                dLogCurrents.Add((logCurrents[n + 2] - logCurrents[n]) / (voltages[n + 2] - voltages[n]));
            //            }
            //            double breakDownVolts = voltages[dLogCurrents.IndexOf(dLogCurrents.Max()) + 1];
            //            BDVoltLabels[i].Text = breakDownVolts.ToString() + " V";
            //        }

            //        //Save data to file
            //        string fileName = "C:\\data\\IV_FEB";
            //        fileName += _ActiveFEB.ToString();
            //        fileName += "_allChs";
            //        DateTime date = DateTime.Now;
            //        fileName += "_" + date.Year.ToString("0000");
            //        fileName += date.Month.ToString("00");
            //        fileName += date.Day.ToString("00");
            //        fileName += "_" + date.Hour.ToString("00");
            //        fileName += date.Minute.ToString("00");
            //        fileName += date.Second.ToString("00") + ".IV";
            //        StreamWriter sw = new StreamWriter(fileName);

            //        sw.Write("-- created_time "); sw.WriteLine(date);
            //        sw.Write("-- num_point "); sw.WriteLine(chanVolts[0].Count);
            //        sw.Write("-- min_V "); sw.WriteLine(((double)udStart.Value).ToString());
            //        sw.Write("-- max_V "); sw.WriteLine(((double)udStop.Value).ToString());
            //        sw.Write("-- board "); sw.WriteLine(_ActiveFEB.ToString());
            //        sw.WriteLine("-----------------------");
            //        for (int i = 0; i < chanVolts[0].Count; i++)
            //        {
            //            sw.Write(chanVolts[0][i].ToString("0.000") + ",");
            //            for (int j = 0; j < num_chans; j++)
            //            {
            //                sw.Write(chanCurrents[j][i].ToString("0.000000"));
            //                if (j != num_chans - 1)
            //                {
            //                    sw.Write(",");
            //                }
            //            }
            //            sw.WriteLine();
            //        }
            //        sw.Close();
            //    }
            //    //End Cole's Stuff
            //    //This resets to voltage to 0 after, don't touch
            //    if (_ActiveFEB == 1) { PP.FEB1IVs.Add(myIV); }
            //    if (_ActiveFEB == 2) { PP.FEB2IVs.Add(myIV); }
            //    v = udStart.Value;
            //    txtV.Text = v.ToString("0.000");
            //    BtnBiasWRITE_Click(null, null);
            //}

            //if (ShowIV.Visible)
            //{
            //    #region ShowSpect
            //    //create new histo
            //    HISTO_curve myHisto = new HISTO_curve()
            //    {
            //        list = new PointPairList(),
            //        loglist = new PointPairList(),
            //        created_time = DateTime.Now,
            //        min_thresh = (int)udStart.Value,
            //        max_thresh = (int)udStop.Value,
            //        interval = (int)udInterval.Value,
            //        chan = (int)udChan.Value,
            //        integral = _IntegralScan,
            //        board = _ActiveFEB
            //    };
            //    UdChan_ValueChanged(null, null);
            //    System.Threading.Thread.Sleep(100);
            //    BtnBIAS_MON_Click(null, null);
            //    Application.DoEvents();
            //    myHisto.V = Convert.ToDouble(txtV.Text);
            //    myHisto.I = Convert.ToDouble(txtI.Text);
            //    //Fetch registers
            //    int FPGA_index = (int)udFPGA.Value;

            //    Mu2e_Register.FindAddr(0x20, ref FEB.arrReg, out Mu2e_Register r_mux);
            //    Mu2e_Register.FindName("HISTO_CHAN_SEL", ref FEB.arrReg, out Mu2e_Register r_ch);
            //    Mu2e_Register.FindName("HISTO_COUNT_INTERVAL", ref FEB.arrReg, out Mu2e_Register r_interval);
            //    Mu2e_Register.FindName("HISTO_THRESH_AFE0", ref FEB.arrReg, out Mu2e_Register r_th0);
            //    Mu2e_Register.FindName("HISTO_COUNT0", ref FEB.arrReg, out Mu2e_Register r_count0);
            //    Mu2e_Register.FindName("HISTO_THRESH_AFE1", ref FEB.arrReg, out Mu2e_Register r_th1);
            //    Mu2e_Register.FindName("HISTO_COUNT1", ref FEB.arrReg, out Mu2e_Register r_count1);

            //    r_ch.fpga_index = (ushort)FPGA_index;
            //    r_interval.fpga_index = (ushort)FPGA_index;
            //    r_th0.fpga_index = (ushort)FPGA_index;
            //    r_count0.fpga_index = (ushort)FPGA_index;
            //    r_th1.fpga_index = (ushort)FPGA_index;
            //    r_count1.fpga_index = (ushort)FPGA_index;

            //    //set interval & ch & stop
            //    UInt32 v = (UInt32)(myHisto.chan);
            //    if (v > 7) { v = v - 8; }
            //    if (_IntegralScan) { v = v + 8; }
            //    Mu2e_Register.WriteReg(0, ref r_mux, ref FEB.client);
            //    Mu2e_Register.WriteReg(v, ref r_ch, ref FEB.client);
            //    v = (UInt32)(myHisto.interval);
            //    Mu2e_Register.WriteReg(v, ref r_interval, ref FEB.client);

            //    myHisto.min_count = 0;

            //    //loop over th
            //    zedFEB1.GraphPane.XAxis.Scale.Max = myHisto.max_thresh;
            //    zedFEB1.GraphPane.XAxis.Scale.Min = myHisto.min_thresh;
            //    zedFEB1.GraphPane.YAxis.Scale.Max = 100;
            //    zedFEB1.GraphPane.YAxis.Scale.Min = 0;

            //    for (int i = myHisto.min_thresh; i < myHisto.max_thresh; i++)
            //    {
            //        if (flgBreak) { i = myHisto.max_thresh; }
            //        UInt32 th = (UInt32)i;
            //        Mu2e_Register.WriteReg(th, ref r_th0, ref FEB.client);
            //        System.Threading.Thread.Sleep(1);
            //        Mu2e_Register.WriteReg(th, ref r_th1, ref FEB.client);
            //        System.Threading.Thread.Sleep(2 * myHisto.interval);

            //        Mu2e_Register.ReadReg(ref r_count0, ref FEB.client);
            //        System.Threading.Thread.Sleep(1);
            //        Mu2e_Register.ReadReg(ref r_count1, ref FEB.client);
            //        UInt32 count = 0;
            //        if (myHisto.chan > 7) { count = r_count1.val; }
            //        else { count = r_count0.val; }

            //        if (count > myHisto.max_count) { myHisto.max_count = count; }

            //        myHisto.AddPoint((int)th, (int)count);
            //        //myCurve.AddPoint(th, count);

            //        btnScan.Text = i.ToString();
            //        Application.DoEvents();

            //    }
            //    if (_ActiveFEB == 1) { PP.FEB1Histo.Add(myHisto); }
            //    if (_ActiveFEB == 2) { PP.FEB2Histo.Add(myHisto); }

            //    Application.DoEvents();
            //    UpdateDisplay();
            //    #endregion ShowSpect
            //}
            #endregion Broken code - needs to be made compatible with new register map

            btnScan.Text = "SCAN";
            flgBreak = false;
            Application.DoEvents();
        }

        private void UpdateDisplay_IV(IV_curve myIV)
        {
            zedFEB1.GraphPane.CurveList.Clear();
            if (chkLogY.Checked)
            {
                if (Math.Round((double)(Math.Log10(myIV.min_I))) < -2)
                { zedFEB1.GraphPane.YAxis.Scale.Min = -2; }
                else
                { zedFEB1.GraphPane.YAxis.Scale.Min = Math.Round((double)(Math.Log10(myIV.min_I))) - .1; }
                zedFEB1.GraphPane.YAxis.Scale.Max = Math.Round((double)(Math.Log10(myIV.max_I * 1000)), 0);
                if (zedFEB1.GraphPane.YAxis.Scale.Max < 0.1) { zedFEB1.GraphPane.YAxis.Scale.Max = 0.1; }
                zedFEB1.GraphPane.AddCurve(myIV.chan.ToString(), myIV.loglist, Color.DarkRed, SymbolType.None);
                zedFEB1.GraphPane.YAxis.Scale.MajorStep = .1 * (double)(Math.Log10(myIV.max_I * 1000));
            }
            else
            {
                zedFEB1.GraphPane.YAxis.Scale.Min = 0.0;
                zedFEB1.GraphPane.YAxis.Scale.Max = Math.Round((double)(myIV.max_I + 0.1 * (myIV.max_I - myIV.min_I)), 0);
                zedFEB1.GraphPane.AddCurve(myIV.chan.ToString(), myIV.list, Color.DarkRed, SymbolType.None);
            }
            double s = Math.Round((double)(myIV.max_v - myIV.min_v) / 10.0, 0);
            if (zedFEB1.GraphPane.XAxis.Scale.MajorStep < s) { zedFEB1.GraphPane.XAxis.Scale.MajorStep = s; }
            zedFEB1.GraphPane.XAxis.Scale.MinorStep = zedFEB1.GraphPane.XAxis.Scale.MajorStep / 4;
            zedFEB1.GraphPane.XAxis.Scale.Min = (double)udStart.Value - .2;
            zedFEB1.GraphPane.XAxis.Scale.Max = (double)udStop.Value + .2;

            s = Math.Round((myIV.max_I - myIV.min_I) / 10.0, 0);
            zedFEB1.GraphPane.YAxis.Scale.MinorStep = zedFEB1.GraphPane.YAxis.Scale.MajorStep / 4;

            zedFEB1.Invalidate(true);
            Application.DoEvents();
        }
        private void UpdateDisplay()
        {
            Color[] this_color = new Color[12];
            Histo_helper.InitColorList(ref this_color);

            List<HISTO_curve> myHistoList = null;
            zedFEB1.GraphPane.CurveList.Clear();

            if (_ActiveFEB == 1) { myHistoList = PP.FEB1Histo; }
            if (_ActiveFEB == 2) { myHistoList = PP.FEB2Histo; }

            foreach (HISTO_curve h1 in myHistoList)
            {
                if (chkLogY.Checked)
                {
                    zedFEB1.GraphPane.YAxis.Scale.Max = Math.Round((double)(Math.Log10(h1.max_count + 0.1 * (h1.max_count - h1.min_count))), 0);
                    zedFEB1.GraphPane.AddCurve(h1.chan.ToString(), h1.loglist, Color.DarkRed, SymbolType.None);
                }
                else
                {
                    zedFEB1.GraphPane.YAxis.Scale.Max = Math.Round((double)(h1.max_count + 0.1 * (h1.max_count - h1.min_count)), 0);
                    zedFEB1.GraphPane.AddCurve(h1.chan.ToString(), h1.list, this_color[h1.chan % 16], SymbolType.None);
                }
                double s = 0;
                s = Math.Round((double)(h1.max_thresh - h1.min_thresh) / 10.0, 0);
                if (zedFEB1.GraphPane.XAxis.Scale.MajorStep < s) { zedFEB1.GraphPane.XAxis.Scale.MajorStep = s; }
                zedFEB1.GraphPane.XAxis.Scale.MinorStep = zedFEB1.GraphPane.XAxis.Scale.MajorStep / 4;

                s = Math.Round((h1.max_count - h1.min_count) / 10.0, 0);
                if (zedFEB1.GraphPane.YAxis.Scale.MajorStep < s) { zedFEB1.GraphPane.YAxis.Scale.MajorStep = s; }
                zedFEB1.GraphPane.YAxis.Scale.MinorStep = zedFEB1.GraphPane.YAxis.Scale.MajorStep / 4;
            }
            zedFEB1.Invalidate(true);
            Application.DoEvents();
        }

        private void BtnCHANGE_Click(object sender, EventArgs e)
        {
            string[,] s = new string[6, num_reg];
            int max_index = 5;
            int index = Convert.ToInt32(btnCHANGE.Tag);
            if (index + 1 > max_index) { btnCHANGE.Tag = Convert.ToString(0); }
            else { btnCHANGE.Tag = Convert.ToString(index + 1); }

            s[0, 0] = "CONTROL_STATUS";
            s[0, 1] = "TRIG_CONTROL";
            s[0, 2] = "CHAN_MASK";
            s[0, 3] = "TEST_PULSE_FREQ";
            s[0, 4] = "TEST_PULSE_DURATION";
            s[0, 5] = "EVENT_WORD_COUNT";
            s[0, 6] = "CHAN_MASK";
            s[0, 7] = "MUX_SEL";
            s[0, 8] = "TRIG_CONTROL";
            s[0, 9] = "SPILL_TRIG_COUNT";
            s[0, 10] = "SPILL_NUMBER";
            s[0, 11] = "SPILL_STATE";
            s[0, 12] = "SPILL_ERROR";
            s[0, 13] = "SPILL_WORD_COUNT";
            s[0, 14] = "SPILL_TRIG_COUNT";

            s[1, 0] = "CONTROL_STATUS";
            s[1, 1] = "HISTO_CONTROL";
            s[1, 2] = "HISTO_ACCUMULATION_INTERVAL";
            s[1, 3] = "HISTO_MEM_PTR_0";
            s[1, 4] = "HISTO_MEM_PTR_1";
            s[1, 5] = "HISTO_READ_0";
            s[1, 6] = "HISTO_READ_1";
            s[1, 7] = "MUX_SEL";
            s[1, 8] = "TRIG_CONTROL";
            s[1, 9] = "SPILL_STATE";
            s[1, 10] = "CHAN_MASK";
            s[1, 11] = "AFE_VGA0";
            s[1, 12] = "AFE_VGA1";
            s[1, 13] = "TRIG_CONTROL";
            s[1, 14] = "UPTIME";

            s[2, 0] = "CONTROL_STATUS";
            s[2, 1] = "MIG_STATUS";
            s[2, 2] = "MIG_FIFO_COUNT";
            s[2, 3] = "LVDS_TRANSMIT_FIFO";
            s[2, 4] = "NUM_SAMPLE_REG";
            s[2, 5] = "TRIG_CONTROL";
            s[2, 6] = "AFE_INPUT_FIFO_EMPTY_FLAG";
            s[2, 7] = "MUX_SEL";
            s[2, 8] = "BIAS_BUS_DAC0";
            s[2, 9] = "BIAS_BUS_DAC1";
            s[2, 10] = "AFE_VGA0";
            s[2, 11] = "AFE_VGA1";
            s[2, 12] = "SPILL_NUMBER";
            s[2, 13] = "SPILL_WORD_COUNT";
            s[2, 14] = "SPILL_TRIG_COUNT";

            s[3, 0] = "TRIG_CONTROL";
            s[3, 1] = "NUM_SAMPLE_REG";
            s[3, 2] = "HIT_DEL_REG";
            s[3, 3] = "TEST_PULSE_DURATION";
            s[3, 4] = "TEST_PULSE_INTERSPILL";
            s[3, 5] = "TEST_PULSE_DELAY";
            s[3, 6] = "SPILL_NUMBER";
            s[3, 7] = "SPILL_TRIG_COUNT";
            s[3, 8] = "SPILL_WORD_COUNT";
            s[3, 9] = "CHAN_MASK";
            s[3, 10] = "SPILL_STATE";
            s[3, 11] = "CONTROL_STATUS";
            s[3, 12] = "SDRAM_ReadPointer";
            s[3, 13] = "SDRAM_WritePointer";
            s[3, 14] = "UPTIME";

            s[4, 0] = "BIAS_DAC_CH0";
            s[4, 1] = "BIAS_DAC_CH1";
            s[4, 2] = "BIAS_DAC_CH2";
            s[4, 3] = "BIAS_DAC_CH3";
            s[4, 4] = "BIAS_DAC_CH4";
            s[4, 5] = "BIAS_DAC_CH5";
            s[4, 6] = "BIAS_DAC_CH6";
            s[4, 7] = "BIAS_DAC_CH7";
            s[4, 8] = "BIAS_DAC_CH8";
            s[4, 9] = "BIAS_DAC_CH9";
            s[4, 10] = "BIAS_DAC_CH10";
            s[4, 11] = "BIAS_DAC_CH11";
            s[4, 12] = "BIAS_DAC_CH12";
            s[4, 13] = "BIAS_DAC_CH13";
            s[4, 14] = "BIAS_DAC_CH14";

            s[5, 0] = "TEST_COUNTER";
            s[5, 1] = "ONE_WIRE_COMMAND";
            s[5, 2] = "ONE_WIRE_CONTROL";
            s[5, 3] = "ONE_WIRE_READ0";
            s[5, 4] = "ONE_WIRE_READ1";
            s[5, 5] = "ONE_WIRE_REA2";
            s[5, 6] = "ONE_WIRE_READ3";
            s[5, 7] = "ONE_WIRE_READ4";
            s[5, 8] = "BIAS_DAC_CH8";
            s[5, 9] = "BIAS_DAC_CH9";
            s[5, 10] = "BIAS_DAC_CH10";
            s[5, 11] = "BIAS_DAC_CH11";
            s[5, 12] = "BIAS_DAC_CH12";
            s[5, 13] = "BIAS_DAC_CH13";
            s[5, 14] = "BIAS_DAC_CH14";

            for (int i = 0; i < num_reg; i++)
            {
                rnames[i] = s[index, i];
                txtREGISTERS[i].Tag = i;

                Mu2e_Register.FindName(rnames[i], ref PP.FEB1.arrReg, out Mu2e_Register r1);
                lblREG[i].Text = rnames[i] + "(x" + Convert.ToString(r1.addr, 16) + ")";
            }

            BtnRegREAD_Click(null, null);
        }

        private void BtnErase_Click(object sender, EventArgs e)
        {
            zedFEB1.GraphPane.CurveList.Clear();
            zedFEB1.Invalidate(true);
            List<HISTO_curve> myHistoList = null;
            //List<HISTO_curve> EraseHistoList = null;
            if (_ActiveFEB == 1) { myHistoList = PP.FEB1Histo; }
            if (_ActiveFEB == 2) { myHistoList = PP.FEB2Histo; }


            //foreach (HISTO_curve h1 in myHistoList)
            //{
            //    if (h1.saved) { myHistoList.Remove(h1); }
            //}
            myHistoList.Clear();
        }

        private void BtnTestSpill_Click(object sender, EventArgs e)
        {
            UInt32 num_trig;
            //--->            mu2e_Event ev = new mu2e_Event();

            WC_client.check_status(out bool in_spill, out string s_num_trig, out string s_WC_time);
            try { num_trig = Convert.ToUInt32("0x0" + s_num_trig, 16); }
            catch { num_trig = 0; }


            Mu2e_Register.FindName("TRIG_CONTROL", ref PP.FEB1.arrReg, out Mu2e_Register r2);
            // Mu2e_Register.WriteReg(0x0350, ref r2, ref PP.FEB1.client);
            Mu2e_Register.FindName("SPILL_STATE", ref PP.FEB1.arrReg, out Mu2e_Register r3);
            System.Threading.Thread.Sleep(250);
            Mu2e_Register.ReadReg(ref r3, ref PP.FEB1.client);



            //if (_ActiveFEB == 1)
            //{
            //    while (r3.val < 6)
            //    {
            //        System.Threading.Thread.Sleep(250);
            //        Application.DoEvents();
            //        Mu2e_Register.ReadReg(ref r3, ref PP.FEB1.client);
            //        Console.WriteLine("waiting for spill to start");
            //    }

            //    while (r3.val == 6)
            //    {
            //        System.Threading.Thread.Sleep(250);
            //        Mu2e_Register.ReadReg(ref r3, ref PP.FEB1.client);
            //        if (PP.glbDebug){ Console.WriteLine("still in spill");
            //    }
            //    PP.FEB1.ReadAll(ref ev, ref PP.FEB1.client);
            //}
            //else if (_ActiveFEB == 2)
            {

                if (!in_spill) { WC_client.FakeSpill(); in_spill = true; }

                while (in_spill)
                {
                    System.Threading.Thread.Sleep(250);
                    WC_client.check_status(out in_spill, out s_num_trig, out s_WC_time);

                    if (PP.glbDebug) { Console.Write(in_spill.ToString() + " "); }
                    Mu2e_Register.ReadReg(ref r3, ref PP.FEB1.client);
                    if (PP.glbDebug) { Console.Write(r3.val.ToString() + " "); }
                    Mu2e_Register.ReadReg(ref r3, ref PP.FEB2.client);
                    if (PP.glbDebug) { Console.Write(r3.val.ToString() + " "); }
                }
                //PP.FEB2.ReadAll(ref ev, ref PP.FEB2.client);
                //PP.FEB1.ReadAll(ref ev, ref PP.FEB1.client);
                WC_client.read_TDC(out int len);
                if (PP.glbDebug) { Console.WriteLine(" "); }
                if (PP.glbDebug) { Console.WriteLine(len); }
                if (PP.glbDebug) { Console.WriteLine("done "); }

            }

            //Mu2e_Register r2;
            //Mu2e_Register.FindName("SPILL_TRIG_COUNT", ref PP.FEB2.arrReg, out r2);
            //r2.fpga_index = 0;
            //Mu2e_Register.ReadReg(ref r2, ref PP.FEB2.client);
            //if (r2.val == 0)
            //{
            //    MessageBox.Show("0 trig found in  FEB2.FPGA0  ");
            //}
            ////else if (r2.val != num_trig)
            ////{
            ////    MessageBox.Show("trig count mismatch: WC=" + num_trig + " FEB2.FPGA0=  " + r2.val);
            ////}
            //else
            //{
            //    if (_ActiveFEB == 2)
            //    {
            //        PP.FEB2.ReadAll(ref ev);
            //    }
            //}
        }

        private void ChkLogY_CheckedChanged(object sender, EventArgs e)
        {
            if (ShowIV.Visible)
            { UpdateDisplay(); }
            if (ShowSpect.Visible)
            {
                if (_ActiveFEB == 1)
                {
                    if (PP.FEB1IVs.Count > 0) { UpdateDisplay_IV(PP.FEB1IVs.Last()); }
                }
                if (_ActiveFEB == 2)
                {
                    if (PP.FEB2IVs.Count > 0) { UpdateDisplay_IV(PP.FEB2IVs.Last()); }
                }
            }
        }

        private void ChkIntegral_CheckedChanged(object sender, EventArgs e)
        {
            if (chkIntegral.Checked) { _IntegralScan = true; }
            else { _IntegralScan = false; }
            UpdateDisplay();
        }

        private void BtnSaveHistos_Click(object sender, EventArgs e)
        {
            if (ShowIV.Visible)
            {
                List<HISTO_curve> myHistoList = null;
                if (_ActiveFEB == 1) { myHistoList = PP.FEB1Histo; }
                if (_ActiveFEB == 2) { myHistoList = PP.FEB2Histo; }

                foreach (HISTO_curve h1 in myHistoList)
                {
                    if (h1.saved) { }
                    else { h1.Save(); }
                }
            }
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            if (PP.FEB1.ClientOpen) { PP.FEB1.Close(); dbgFEB1.BackColor = Color.LightGray; btnFEB1.BackColor = Color.LightGray; }
            if (PP.FEB2.ClientOpen) { PP.FEB2.Close(); dbgFEB2.BackColor = Color.LightGray; btnFEB2.BackColor = Color.LightGray; }
        }

        private void ShowIV_Click(object sender, EventArgs e)
        {
            ShowIV.Visible = false;
            ShowSpect.Visible = true;
            chkIntegral.Visible = false;
            udStart.DecimalPlaces = 1;
            udStart.Minimum = 0;
            udStart.Maximum = 80;
            udStart.Value = 60;
            udStart.Increment = (decimal)0.1;
            udStop.DecimalPlaces = 1;
            udStop.Minimum = 1;
            udStop.Maximum = 80;
            udStop.Value = 70;
            udStop.Increment = (decimal)0.1;
            lblInc.Text = "Step (mv)";
            udInterval.Minimum = (decimal)50;
            udInterval.Maximum = 1000;
            udInterval.Value = (decimal)100;
            udInterval.Increment = (decimal)50;
            Application.DoEvents();
        }

        private void ShowSpect_Click(object sender, EventArgs e)
        {
            ShowIV.Visible = true;
            ShowSpect.Visible = false;
            udStart.DecimalPlaces = 0;
            udStop.DecimalPlaces = 0;
            chkIntegral.Visible = true;
            udStart.Minimum = 0;
            udStart.Maximum = 4000;
            udStart.Value = 2000;
            udStart.Increment = (decimal)100;
            udStop.Minimum = 0;
            udStop.Maximum = 4000;
            udStop.Value = 2200;
            udStop.Increment = (decimal)100;
            lblInc.Text = "Time (ms)";
            udInterval.Minimum = (decimal)1;
            udInterval.Maximum = 255;
            udInterval.Value = (decimal)25;
            udInterval.Increment = (decimal)1;
            Application.DoEvents();
        }

        private void BtnConnectAll_Click(object sender, EventArgs e)
        {
            Application.DoEvents();
            if (chkWC.Checked)
            { Button1_Click((object)btnWC, e); }
            Application.DoEvents();
            if (chkFEB1.Checked)
            { Button1_Click((object)btnFEB1, e); }
            Application.DoEvents();
            if (chkFEB2.Checked)
            { Button1_Click((object)btnFEB2, e); }
            Application.DoEvents();


            //-------------------------------------------------------
            ChkFakeIt_CheckedChanged(null, null);
            chkFakeIt.Visible = false;
            //------DANGER: Fake always off--------------
            Application.DoEvents();
        }

        private void UdChan_ValueChanged(object sender, EventArgs e)
        {
            int FPGA_index = (int)udFPGA.Value;
            uint chan = 0;
            Mu2e_Register mux_reg = new Mu2e_Register();
            Mu2e_FEB_client myFEB = new Mu2e_FEB_client();
            if (_ActiveFEB == 1) { myFEB = PP.FEB1; }
            if (_ActiveFEB == 2) { myFEB = PP.FEB2; }
            chan = (uint)udChan.Value;
            mux_reg.fpga_index = (ushort)FPGA_index;
            if (myFEB != null)
            {
                Mu2e_Register.FindAddr(0x020, ref myFEB.arrReg, out mux_reg);
                if (chan < 16)
                {
                    uint v = (uint)(0x10 + chan);
                    Mu2e_Register.WriteReg(v, ref mux_reg, ref myFEB.client);
                }
            }

        }

        private void Timer1_Tick(object sender, EventArgs e)
        {
            bool in_spill;
            if (PP.glbDebug) { Console.WriteLine("tick"); }
            if (tabControl.SelectedIndex == 0)
            {
                //if (PP.glbDebug){ Console.WriteLine("timer");}
                try
                {
                    TimeSpan since_last_spill = DateTime.Now.Subtract(PP.myRun.timeLastSpill);
                    lblSpillTime.Text = since_last_spill.TotalSeconds.ToString("0.0");
                    if ((since_last_spill.TotalSeconds > 2) && (PP.myRun.spill_complete))
                    {
                        Mu2e_Register.FindName("TRIG_CONTROL", ref PP.FEB1.arrReg, out Mu2e_Register r_spill);
                        if (!PP.myRun.OneSpill)
                        { }// Mu2e_Register.WriteReg(0x02, ref r_spill, ref PP.FEB1.client); }
                    }
                    if (PP.myRun != null)
                    {
                        lblFEB1_TotTrig.Text = PP.myRun.total_trig[0].ToString();
                        lblFEB2_TotTrig.Text = PP.myRun.total_trig[1].ToString();
                        lblWC_TotTrig.Text = PP.myRun.total_trig[2].ToString();
                        if (PP.myRun.ACTIVE)
                        {
                            lblRunTime.Text = DateTime.Now.Subtract(PP.myRun.created).Seconds.ToString("0.0");
                            if ((since_last_spill.Seconds > 2) & (since_last_spill.Seconds < 10))
                            {
                                if (PP.myRun.spill_complete == false)
                                {

                                    PP.myRun.RecordSpill();
                                    if (PP.myRun.OneSpill)
                                    {
                                        //display it
                                        PP.myRun.DeactivateRun();
                                    }
                                    else
                                    {


                                    }
                                    PP.myRun.total_trig[0] += spill_trig_num[0];
                                    PP.myRun.total_trig[1] += spill_trig_num[1];
                                    PP.myRun.total_trig[2] += spill_trig_num[2];


                                    DispSpill = PP.myRun.Spills.Last();
                                    for (LinkedListNode<Mu2e_Event> it = DispSpill.SpillEvents.First; it != null; it = it.Next)
                                    {
                                        DispEvent = it.Value;

                                        Mu2e_Ch[] cha = DispEvent.ChanData.ToArray();
                                        double[] y = new double[cha[0].data.Count() - 1];
                                        for (int i = 0; i < 4; i++)
                                        {
                                            double ped = cha[i].data.Skip(1).Take(50).Average();
                                            double maxADC = cha[i].data.Max() - ped;
                                            PP.myRun.max_adc[i] += maxADC;
                                        }

                                    }

                                    for (int i = 0; i < 4; i++)
                                    {
                                        double meanADC = PP.myRun.max_adc[i] / PP.myRun.total_trig[0];
                                        //Console.WriteLine(i.ToString() + " " + meanADC.ToString() + " " + PP.myRun.total_trig[0]);
                                    }
                                    string _lblmaxadc0 = string.Format("{0:N2}", PP.myRun.max_adc[0] / PP.myRun.total_trig[0]);
                                    lblMaxADC0.Text = _lblmaxadc0;

                                    string _lblmaxadc1 = string.Format("{0:N2}", PP.myRun.max_adc[1] / PP.myRun.total_trig[0]);
                                    lblMaxADC1.Text = _lblmaxadc1;

                                    string _lblmaxadc2 = string.Format("{0:N2}", PP.myRun.max_adc[2] / PP.myRun.total_trig[0]);
                                    lblMaxADC2.Text = _lblmaxadc2;

                                    string _lblmaxadc3 = string.Format("{0:N2}", PP.myRun.max_adc[3] / PP.myRun.total_trig[0]);
                                    lblMaxADC3.Text = _lblmaxadc3;

                                }
                                else
                                {
                                    if ((btnDisplaySpill.Text == "DONE") && (since_last_spill.Seconds > 8))
                                    {
                                        timer1.Enabled = false;
                                        this.BtnDisplaySpill_Click(null, null);
                                        for (int i = 0; i < 10; i++)
                                        {
                                            Application.DoEvents();
                                            System.Threading.Thread.Sleep(50);
                                        }
                                        this.BtnDisplaySpill_Click(null, null);
                                        timer1.Enabled = true;
                                    }
                                }
                            }

                        }
                    }
                }
                catch (Exception ex) { timer1.Enabled = false; if (PP.glbDebug) { Console.WriteLine("timer off " + ex.Message); } }

                if (PP.myRun != null) //update log
                {
                    lblRunName.Text = "Run_" + PP.myRun.num.ToString();
                    lblRunLog.Text = "";
                    string[] all_m = PP.myRun.RunStatus.ToArray<string>();
                    int l = all_m.Length;
                    if (PP.myRun.RunStatus.Count > 13)
                    {
                        for (int i = l - 13; i < l; i++)
                        { lblRunLog.Text += all_m[i] + "\r\n"; }
                    }
                    else
                    {
                        for (int i = 0; i < all_m.Length; i++)
                        { lblRunLog.Text += all_m[i] + "\r\n"; }
                    }


                    if (PP.myRun.fake)
                    {
                        if ((PP.FEB1.ClientOpen) && (chkFEB1.Checked))
                        {

                            PP.FEB1.CheckStatus(out uint spill_status, out uint spill_num, out uint trig_num);
                            if (spill_status > 2) { in_spill = true; } else { in_spill = false; }
                            lblSpillFEB1.Text = spill_status.ToString();
                            lblFEB1TrigNum.Text = trig_num.ToString();
                            spill_trig_num[0] = (int)trig_num;
                            if (in_spill)
                            {
                                if (PP.myRun != null)
                                {
                                    PP.myRun.timeLastSpill = DateTime.Now;
                                    PP.myRun.UpdateStatus("Detected spill. Run file is " + PP.myRun.OutFileName);
                                    PP.myRun.spill_complete = false;
                                }
                            }

                        }
                    }

                    else //if (!PP.myRun.fake)
                    {
                        if (PP.FEB1.ClientOpen)
                        {
                            PP.FEB1.CheckStatus(out uint spill_status, out uint spill_num, out uint trig_num);
                            lblSpillFEB1.Text = spill_status.ToString();
                            lblFEB1TrigNum.Text = trig_num.ToString();
                            spill_trig_num[0] = (int)trig_num;
                        }

                        if (PP.FEB2.ClientOpen)
                        {
                            PP.FEB2.CheckStatus(out uint spill_status, out uint spill_num, out uint trig_num);
                            lblSpillFEB2.Text = spill_status.ToString();
                            lblFEB2TrigNum.Text = trig_num.ToString();
                            spill_trig_num[1] = (int)trig_num;
                        }

                        if (PP.WC.ClientOpen)
                        {

                            WC_client.check_status(out in_spill, out string num_trig, out string mytime);
                            lblSpillWC.Text = in_spill.ToString();
                            //spill_trig_num[2] = Convert.ToInt32(num_trig);

                            if (in_spill)
                            {
                                if (PP.myRun != null)
                                {
                                    PP.myRun.timeLastSpill = DateTime.Now;
                                    PP.myRun.UpdateStatus("Detected spill. Run file is " + PP.myRun.OutFileName);
                                    PP.myRun.spill_complete = false;
                                }
                            }


                            lblWCTrigNum.Text = num_trig;
                        }
                    }
                }
            }
        }

        private void BtnPrepare_Click(object sender, EventArgs e)
        {
            if ((!PP.FEB1.ClientOpen && chkFEB1.Checked) || (!PP.FEB2.ClientOpen && chkFEB2.Checked) || (!PP.WC.ClientOpen && chkWC.Checked))
            { MessageBox.Show("Are all clients open?"); }
            timer1.Enabled = false;
            PP.myRun = new Run();
            if (chkFakeIt.Checked) { PP.myRun.fake = true; }
            else { PP.myRun.fake = false; }

            if (PP.myRun.fake == false)
            #region notfake
            {
                WC_client.DisableTrig();
                PP.myRun.UpdateStatus("waiting for spill to disable WC");
                if (!PP.WC.in_spill) { WC_client.FakeSpill(); }
                int spill_timeout = 0;
                int big_count = 0;
                bool inspill = false;
                string X = "";
                string Y = "";
                lblRunTime.Text = "not running";
                PP.myRun.ACTIVE = false;
                while (!inspill)
                {
                    if (PP.glbDebug) { Console.WriteLine("waiting for spill"); }
                    System.Threading.Thread.Sleep(200);
                    Application.DoEvents();
                    WC_client.check_status(out inspill, out X, out Y);
                    spill_timeout++;
                    if (spill_timeout > 500) { WC_client.FakeSpill(); spill_timeout = 0; big_count++; }
                    if (big_count > 3) { MessageBox.Show("can't get a spill..."); return; }
                }
                PP.myRun.UpdateStatus("in spill....");
                while (PP.WC.in_spill)
                {
                    if (PP.glbDebug) { Console.WriteLine("waiting for spill to end"); }
                    System.Threading.Thread.Sleep(200);
                    WC_client.check_status(out inspill, out X, out Y);
                    Application.DoEvents();
                }
                PP.myRun.UpdateStatus("Prepairing FEB1 and FEB2");
                //            PP.FEB1.GetReady();
                //            PP.FEB2.GetReady();
                PP.myRun.UpdateStatus("Arming WC");
                if (!PP.WC.in_spill) { WC_client.EnableTrig(); }
                lblRunName.Text = PP.myRun.run_name;
                timer1.Enabled = true;
            }
            #endregion notfake
            else
            {
                PP.myRun.UpdateStatus("Fake Run- sending spills to FEB1");
                lblRunName.Text = PP.myRun.run_name;
                timer1.Enabled = true;
            }
        }

        private void BtnStartRun_Click(object sender, EventArgs e)
        {
            if (PP.myRun != null)
            {
                PP.myRun.ActivateRun(); PP.myRun.UpdateStatus("Run STARTING");
                PP.myRun.OneSpill = false;
            }

        }

        private void BtnOneSpill_Click(object sender, EventArgs e)
        {
            Mu2e_Register.FindName("TRIG_CONTROL", ref PP.FEB1.arrReg, out Mu2e_Register r_spill);
            Mu2e_Register.WriteReg(0x300, ref r_spill, ref PP.FEB1.client);
            if (PP.myRun != null)
            {
                PP.myRun.ActivateRun(); PP.myRun.UpdateStatus("Run STARTING in ONE SPILL MODE");
                Mu2e_Register.WriteReg(0x300, ref r_spill, ref PP.FEB1.client);
                PP.myRun.OneSpill = true;
            }
        }

        private void BtnStopRun_Click(object sender, EventArgs e)
        {
            if (PP.myRun != null)
            {
                PP.myRun.DeactivateRun();
                PP.myRun.UpdateStatus("Run STOPPING");
                timer1.Enabled = false;
            }
        }

        private void ChkFakeIt_CheckedChanged(object sender, EventArgs e)
        {
            WC_client.DisableFake();
            //if (PP.WC.ClientOpen)
            //{
            //    if (chkFakeIt.Checked) { WC_client.ForeverFake(); }
            //    else { WC_client.DisableFake(); }
            //}
            //if (chkFakeIt.Checked) { btnOneSpill.Visible = true; } else { btnOneSpill.Visible = false; }
            btnOneSpill.Visible = false;
        }

        private void BtnChangeName_Click(object sender, EventArgs e)
        {
            ClientNameChange frmChange = new ClientNameChange();
            frmChange.ShowDialog();
            btnFEB1.Text = PP.FEB1.host_name_prop;
            btnFEB2.Text = PP.FEB2.host_name_prop;
        }

        private void BtnDisplaySpill_Click(object sender, EventArgs e)
        {
            if (btnDisplaySpill.Text == "DISPLAY")
            {
                if (PP.myRun != null)
                {
                    if (PP.myRun.Spills != null)
                    {
                        if (PP.myRun.Spills.Count > 0)
                        {

                            DispSpill = PP.myRun.Spills.Last();
                            for (LinkedListNode<Mu2e_Event> it = DispSpill.SpillEvents.First; it != null; it = it.Next)
                            {
                                DispEvent = it.Value;

                                Mu2e_Ch[] cha = DispEvent.ChanData.ToArray();
                                double[] y = new double[cha[0].data.Count() - 1];

                                for (int i = 0; i < 4; i++)
                                {
                                    PP.myRun.max_adc[i] += cha[i].data.Max();
                                    Console.WriteLine(PP.myRun.max_adc[i].ToString());
                                }

                            }

                            PP.myRun.Spills.Last().IsDisplayed = true;

                            //DispSpill = PP.myRun.Spills.Last();
                            btnDisplaySpill.Text = "DONE";

                            for (int i = 0; i < DispSpill.ExpectNumCh; i++)
                            {
                                this.chkChEnable[i].Visible = true;
                            }
                            lblEventCount.Text = "Spill " + DispSpill.SpillCounter + "," + DispSpill.SpillEvents.Count + " e";
                            txtEvent.Text = Convert.ToString(1);
                            DispEvent = DispSpill.SpillEvents.First();
                            txtEvent.Text = DispEvent.TrigCounter.ToString();
                            UpdateEventDisplay();

                        }
                    }
                }
            }
            else
            {
                btnDisplaySpill.Text = "DISPLAY";
                foreach (SpillData aSpill in PP.myRun.Spills)
                {
                    if (aSpill.IsDisplayed) { aSpill.IsDisplayed = false; }
                }
                for (int i = 0; i < 64; i++)
                {
                    this.chkChEnable[i].Visible = false;
                }
            }
        }

        private void BtnNextDisp_Click(object sender, EventArgs e)
        {
            try
            {
                LinkedListNode<Mu2e_Event> node = DispSpill.SpillEvents.Find(DispEvent).Next;
                DispEvent = node.Value;
                txtEvent.Text = DispEvent.TrigCounter.ToString();
                UpdateEventDisplay();
            }
            catch { }
        }

        private void BtnPrevDisp_Click(object sender, EventArgs e)
        {
            try
            {
                LinkedListNode<Mu2e_Event> node = DispSpill.SpillEvents.Find(DispEvent).Previous;
                DispEvent = node.Value;
                txtEvent.Text = DispEvent.TrigCounter.ToString();
                UpdateEventDisplay();
            }
            catch { }

        }

        private void UpdateEventDisplay()
        {//zg1
            Color[] this_color = new Color[12];
            Mu2e_Ch[] cha = DispEvent.ChanData.ToArray();
            zg1.GraphPane.Legend.IsVisible = false;
            if ((!chkPersist.Checked) || (zg1.GraphPane.CurveList.Count > 500))
            { zg1.GraphPane.CurveList.Clear(); }
            try
            {
                if (cha.Count() > 0)
                {
                    double[] x = new double[cha[0].data.Count() - 1];
                    double[] y = new double[cha[0].data.Count() - 1];
                    zg1.GraphPane.YAxis.Scale.Max = Convert.ToDouble(ud_VertMax.Value);
                    zg1.GraphPane.YAxis.Scale.Min = Convert.ToDouble(ud_VertMin.Value);
                    zg1.GraphPane.XAxis.Scale.Max = Convert.ToDouble(x.Count());
                    zg1.GraphPane.XAxis.Scale.Min = Convert.ToDouble(1);
                    zg1.GraphPane.YAxis.Scale.MajorStep = 0.2 * (Convert.ToDouble(ud_VertMax.Value) - Convert.ToDouble(ud_VertMin.Value));
                    zg1.GraphPane.XAxis.Scale.MajorStep = 10;
                    zg1.GraphPane.YAxis.Scale.MinorStep = 0.2 * zg1.GraphPane.YAxis.Scale.MajorStep;
                    zg1.GraphPane.XAxis.Scale.MinorStep = 0.2 * zg1.GraphPane.XAxis.Scale.MajorStep;

                    for (int i = 0; i < x.Count(); i++)
                    { x[i] = i + 1; }

                    for (int i = 0; i < 32; i++)
                    {
                        if (chkChEnable[i].Checked)
                        {
                            for (int j = 0; j < x.Count(); j++)
                            {
                                y[j] = cha[i].data[j + 1];
                            }
                            zg1.GraphPane.AddCurve(cha[i].data[0].ToString(), x, y, Color.DarkRed, SymbolType.None);
                        }
                    }
                    zg1.Invalidate(true);

                }
            }
            catch
            { }
            Application.DoEvents();
        }

        private void BtnDebugLogging_Click(object sender, EventArgs e)
        {
            string hName = "";
            if (btnDebugLogging.Text.Contains("START LOG"))
            {
                btnDebugLogging.Text = "STOP LOG";
                hName = "FEB" + _ActiveFEB + "_commands_";
                hName += "_" + DateTime.Now.Year.ToString("0000");
                hName += DateTime.Now.Month.ToString("00");
                hName += DateTime.Now.Day.ToString("00");
                hName += "_" + DateTime.Now.Hour.ToString("00");
                hName += DateTime.Now.Minute.ToString("00");
                hName += DateTime.Now.Second.ToString("00");
            }
        }

        private void BtnTimerFix_Click(object sender, EventArgs e)
        {
            timer1.Enabled = false;

            //PP.FEB1.client.Close();
            //Application.DoEvents();
            //PP.FEB1.Open();
            //Application.DoEvents();
            Mu2e_Register csr = new Mu2e_Register();
            Mu2e_Register.FindName("CONTROL_STATUS", ref PP.FEB1.arrReg, out csr);
            for (int i = 0; i < 4; i++)
            {
                csr.fpga_index = (UInt16)(i);
                Mu2e_Register.WriteReg(0xfc, ref csr, ref PP.FEB1.client);
                System.Threading.Thread.Sleep(10);
            }
            if (timer1.Enabled) { }
            else { timer1.Enabled = true; }
        }

        private void ChkLast_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void FrmMain_Load(object sender, EventArgs e)
        {

        }

        private void Label10_Click(object sender, EventArgs e)
        {

        }

        private void GroupBoxEvDisplay_Enter(object sender, EventArgs e)
        {

        }

        private void Label15_Click(object sender, EventArgs e)
        {

        }

        private void NumLabel_Click(object sender, EventArgs e)
        {

        }

        private void NumButton_Click(object sender, EventArgs e)
        {
            numButton.Enabled = false;  //prevents multiple clicks on the button
            autoThreshBtn.Enabled = false;
            lightCheckResetThresh.Enabled = false;
            lightCheckBtn.Enabled = false;
            RadioButton[] statusButtons = { but0, but1, but2, but3, but4, but5, but6, but7 }; //holds the 8 buttons
            string[] chanOuts = new string[8];
            using (StreamWriter writer = File.AppendText("C:\\data\\ScanningData.txt"))
            {
                string dicountNumber = numTextBox.Text;
                writer.Write("{0}\t", dicountNumber);
                DateTime today = DateTime.Now;
                writer.Write("{0}\t", today.ToString("g"));

                //string name = tabControl.SelectedTab.Text;
                //if (name.Contains("QA"))
                //{
                switch (_ActiveFEB)
                {
                    case 1:
                        PP.FEB1.SetV(Convert.ToDouble(qaBias.Text), (int)udFPGA.Value);
                        txtI.Text = PP.FEB1.ReadA0((int)udFPGA.Value, (int)udChan.Value).ToString("0.0000");
                        break;
                    case 2:
                        PP.FEB2.SetV(Convert.ToDouble(qaBias.Text));
                        txtI.Text = PP.FEB2.ReadA0((int)udFPGA.Value, (int)udChan.Value).ToString("0.0000");
                        break;

                    default:
                        break;
                }

                //initially set to green
                for (int i = 0; i < 8; i++)
                {
                    statusButtons[i].BackColor = Color.Green;
                    statusButtons[i].Update();
                }
                int lastChan = 8; //by default set lastChan = 8 (number of channels to read when doing counter QA)
                if (oneReadout.Checked)
                    lastChan = 4; //If single readout is checked, then only read the first 4 channels
                else
                    lastChan = 8;

                for (int i = 0; i < 8; i++)
                {
                    double average = 0;
                    for (int measurementNumber = 0; measurementNumber < Convert.ToInt32(numAvg.Value) && i < lastChan; measurementNumber++)
                        average += Convert.ToDouble(PP.FEB1.ReadA0((int)udFPGA.Value, i));
                    average /= Convert.ToDouble(numAvg.Value);
                    //Console.WriteLine("" + average);
                    if (average < Convert.ToDouble(iWarningThresh.Text)) //if the current was less than warning thresh && we still have current, update the color of the lamp
                        statusButtons[i].BackColor = Color.Red;
                    if (average < 0.025) //if there was no current then
                        statusButtons[i].BackColor = Color.Blue; //set lamp color to blue "cold-no-current"
                    //statusButtons[i].Text = "" + Math.Round(average,2);
                    statusButtons[i].Update();

                    chanOuts[i] = average.ToString("0.0000");
                    //Console.WriteLine("Channel {0}: {1}", i, chanOuts[i]);
                    writer.Write("{0}\t", chanOuts[i]);
                    autoDataProgress.Increment(1);
                }
                double[] cmb_temp = new double[4];
                PP.FEB1.ReadTemp(out cmb_temp[0], out cmb_temp[1], out cmb_temp[2], out cmb_temp[3], (int)udFPGA.Value);
                writer.Write("{0} C", cmb_temp[0].ToString("0.000"));
                writer.WriteLine();

                switch (_ActiveFEB)
                {
                    case 1:
                        PP.FEB1.SetV(0.0, (int)udFPGA.Value);
                        txtI.Text = PP.FEB1.ReadA0((int)udFPGA.Value, (int)udChan.Value).ToString("0.0000");
                        break;
                    case 2:
                        PP.FEB2.SetV(0.0);
                        txtI.Text = PP.FEB2.ReadA0((int)udFPGA.Value, (int)udChan.Value).ToString("0.0000");
                        break;

                    default:
                        break;
                }
                autoDataProgress.Increment(-7);

                //}


            }
            numButton.Enabled = true;
            autoThreshBtn.Enabled = true;
            lightCheckResetThresh.Enabled = true;
            lightCheckBtn.Enabled = true;
        }


        private void AutoThreshBtn_Click(object sender, EventArgs e)
        {
            autoThreshBtn.Enabled = false;  //prevents multiple clicks on the button
            numButton.Enabled = false;
            lightCheckResetThresh.Enabled = false;
            lightCheckBtn.Enabled = false;
            RadioButton[] statusButtons = { lightBut0, lightBut1, lightBut2, lightBut3,
                                            lightBut4, lightBut5, lightBut6, lightBut7,
                                            lightBut8, lightBut9, lightBut10, lightBut11,
                                            lightBut12, lightBut13, lightBut14, lightBut15,
                                            lightBut16, lightBut17, lightBut18, lightBut19,
                                            lightBut20, lightBut21, lightBut22, lightBut23,
                                            lightBut24, lightBut25, lightBut26, lightBut27,
                                            lightBut28, lightBut29, lightBut30, lightBut31 }; //holds the 32 indicators
            switch (_ActiveFEB)
            {
                case 1:
                    PP.FEB1.SetV(Convert.ToDouble(qaBias.Text), (int)udFPGA.Value);
                    txtI.Text = PP.FEB1.ReadA0((int)udFPGA.Value, (int)udChan.Value).ToString("0.0000");
                    break;
                case 2:
                    PP.FEB2.SetV(Convert.ToDouble(qaBias.Text));
                    txtI.Text = PP.FEB2.ReadA0((int)udFPGA.Value, (int)udChan.Value).ToString("0.0000");
                    break;

                default:
                    break;
            }

            //initially set to green
            for (int i = 0; i < 31; i++)
            {
                if (!statusButtons[i].Checked)
                {
                    statusButtons[i].BackColor = Color.Green;
                    statusButtons[i].Update();
                }
            }

            for (int i = 0; i < 31; i++)
            {
                if (!statusButtons[i].Checked)
                {
                    double average = PP.FEB1.ReadA0((int)udFPGA.Value, i);
                    Console.WriteLine("" + Math.Round(average, 2));

                    if (average < 0.025) //If no current update the color of the lamp
                    {
                        statusButtons[i].BackColor = Color.Blue; //set lamp color to blue "cold-no-current"
                        statusButtons[i].Text = "" + Math.Round(average, 1);
                        statusButtons[i].Update();
                    }
                    else
                    {
                        PP.lightCheckChanThreshs[i] = average * 1.10; //set the threshold to 10% higher than dark-current
                    }
                }
                lightAutoThreshProgress.Increment(1);
            }

            switch (_ActiveFEB)
            {
                case 1:
                    PP.FEB1.SetV(0.0, (int)udFPGA.Value);
                    txtI.Text = PP.FEB1.ReadA0((int)udFPGA.Value, (int)udChan.Value).ToString("0.0000");
                    break;
                case 2:
                    PP.FEB2.SetV(0.0);
                    txtI.Text = PP.FEB2.ReadA0((int)udFPGA.Value, (int)udChan.Value).ToString("0.0000");
                    break;

                default:
                    break;
            }
            lightAutoThreshProgress.Increment(-32);

            uint selection = Convert.ToUInt16(lightCheckChanSelec.Value);
            lightCheckChanThresh.Text = PP.lightCheckChanThreshs[selection].ToString("0.0000"); //update the current channel to new thresh


            autoThreshBtn.Enabled = true;
            numButton.Enabled = true;
            lightCheckResetThresh.Enabled = true;
            lightCheckBtn.Enabled = true;
        }

        private void LightCheckResetThresh_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < 32; i++)
                PP.lightCheckChanThreshs[i] = 0.25;
            lightGlobalThresh.Text = "0.25";
        }

        private void LightCheckChanSelec_ValueChanged(object sender, EventArgs e)
        {
            uint selection = Convert.ToUInt16(lightCheckChanSelec.Value);
            lightCheckChanThresh.Text = PP.lightCheckChanThreshs[selection].ToString("0.0000");
        }

        private void LightCheckChanThreshBtn_Click(object sender, EventArgs e)
        {
            uint selection = Convert.ToUInt16(lightCheckChanSelec.Value);
            PP.lightCheckChanThreshs[selection] = Convert.ToDouble(lightCheckChanThresh.Text);
        }

        private void LightCheckBtn_Click(object sender, EventArgs e)
        {
            lightCheckBtn.Enabled = false;  //prevents multiple clicks on the button
            autoThreshBtn.Enabled = false;
            numButton.Enabled = false;
            lightCheckResetThresh.Enabled = false;
            RadioButton[] statusButtons = { lightBut0, lightBut1, lightBut2, lightBut3,
                                            lightBut4, lightBut5, lightBut6, lightBut7,
                                            lightBut8, lightBut9, lightBut10, lightBut11,
                                            lightBut12, lightBut13, lightBut14, lightBut15,
                                            lightBut16, lightBut17, lightBut18, lightBut19,
                                            lightBut20, lightBut21, lightBut22, lightBut23,
                                            lightBut24, lightBut25, lightBut26, lightBut27,
                                            lightBut28, lightBut29, lightBut30, lightBut31 }; //holds the 32 indicators
            switch (_ActiveFEB)
            {
                case 1:
                    PP.FEB1.SetV(Convert.ToDouble(qaBias.Text), (int)udFPGA.Value);
                    txtI.Text = PP.FEB1.ReadA0((int)udFPGA.Value, (int)udChan.Value).ToString("0.0000");
                    break;
                case 2:
                    PP.FEB2.SetV(Convert.ToDouble(qaBias.Text));
                    txtI.Text = PP.FEB2.ReadA0((int)udFPGA.Value, (int)udChan.Value).ToString("0.0000");
                    break;

                default:
                    break;
            }

            //initially set to green
            for (int i = 0; i < 31; i++)
            {
                if (!statusButtons[i].Checked)
                {
                    statusButtons[i].BackColor = Color.Green;
                    statusButtons[i].Text = "" + i;
                    statusButtons[i].Update();
                }
            }
            int numChecked = 0;
            foreach (RadioButton r in statusButtons)
            {
                if (!r.Checked) numChecked++;
            }
            int numAverages = numChecked;
            //double[] averages = new double[numAverages];

            //for (int i = 0; i < numAverages; i++)
            //{
            //    averages[i] = 0;
            //}
            int numTimesToCheck = Convert.ToInt32(numTimesToCheckTextBox.Text);
            for (int j = 0; j < numTimesToCheck; j++)
            {

                double sum = 0;
                for (int i = 0; i < 32; i++)
                {
                    if (!statusButtons[i].Checked)
                    {
                        double average = PP.FEB1.ReadA0((int)udFPGA.Value, i);
                        //averages[i] = average;
                        if (average > 0)
                        {
                            sum += average / numAverages;
                        }
                        Console.WriteLine("" + average);
                        if (!globalThreshChkBox.Checked) // if the global thresh box is not checked, then use individual channels thresh
                        {
                            if (average > PP.lightCheckChanThreshs[i]) //If the current is above the thresh, light leak!
                            {
                                statusButtons[i].BackColor = Color.Red; //flag the button
                            }
                            else if (average < 0.025)
                            {
                                statusButtons[i].BackColor = Color.Blue; //flag the button
                            }

                            statusButtons[i].Text = "" + Math.Round(average, 2);
                            statusButtons[i].Update();

                        }
                        else
                        {
                            if (average > Convert.ToDouble(lightGlobalThresh.Text)) //If the current is above the thresh, light leak!
                            {
                                statusButtons[i].BackColor = Color.Red; //flag the button
                            }
                            else if (average < 0.025)
                            {
                                statusButtons[i].BackColor = Color.Blue; //flag the button
                            }

                            statusButtons[i].Text = "" + Math.Round(average, 2);
                            statusButtons[i].Update();

                        }
                    }
                    else
                    {
                        //averages[i] = 0;
                    }
                }
                //foreach (double d in averages)
                //{
                //    sum += d / (double)numAverages;
                //}
                Console.WriteLine("Sum: " + sum);
                if (sum >= 0.1)
                {
                    int signal = 0;
                    if (sum / .1 < 1)
                    {
                        signal = 1;
                    } else if (sum / .1 > 20)
                    {
                        signal = 20;
                    } else
                    {
                        signal = (int)(sum / .1);
                    }
                    Console.WriteLine("Signal: " + signal);
                    //System.Windows.Media.MediaPlayer[] players = new System.Windows.Media.MediaPlayer[signal];
                    for (int m = 0; m < signal; m++)
                    {
                        new System.Threading.Thread(() =>
                        {
                            var c = new System.Windows.Media.MediaPlayer();
                            c.Open(new System.Uri(@"C:\Users\FEB-Laptop-1\Desktop\beep-07.wav"));
                            c.Play();
                        }).Start();
                        //players[m] = new System.Windows.Media.MediaPlayer();
                        //players[m].Open(new System.Uri(@"C:\Users\FEB-Laptop-1\Desktop\chimes.wav"));
                        //players[m].Play();
                        System.Threading.Thread.Sleep(100);
                        //                        System.Media.SystemSounds.Beep.Play();

                    }
                }
                lightAutoThreshProgress.Increment(1);
            }

            switch (_ActiveFEB)
            {
                case 1:
                    PP.FEB1.SetV(0.0, (int)udFPGA.Value);
                    txtI.Text = PP.FEB1.ReadA0((int)udFPGA.Value, (int)udChan.Value).ToString("0.0000");
                    break;
                case 2:
                    PP.FEB2.SetV(0.0);
                    txtI.Text = PP.FEB2.ReadA0((int)udFPGA.Value, (int)udChan.Value).ToString("0.0000");
                    break;

                default:
                    break;
            }
            lightAutoThreshProgress.Increment(-32);
            lightCheckBtn.Enabled = true;
            autoThreshBtn.Enabled = true;
            numButton.Enabled = true;
            lightCheckResetThresh.Enabled = true;
        }

        private void QaBias_TextChanged(object sender, EventArgs e)
        {
            qaBias.BackColor = Color.White;
        }

        private void LightBut0_Click(object sender, EventArgs e)
        {
            if (!lightBut0.Checked)
            {
                lightBut0.BackColor = Color.DimGray;
                lightBut0.Update();
                lightBut0.Checked = true;
            }
            else
            {
                lightBut0.BackColor = Color.Green;
                lightBut0.Update();
                lightBut0.Checked = false;
            }
        }

        private void LightBut1_Click(object sender, EventArgs e)
        {
            if (!lightBut1.Checked)
            {
                lightBut1.BackColor = Color.DimGray;
                lightBut1.Update();
                lightBut1.Checked = true;
            }
            else
            {
                lightBut1.BackColor = Color.Green;
                lightBut1.Update();
                lightBut1.Checked = false;
            }
        }

        private void LightBut2_Click(object sender, EventArgs e)
        {
            if (!lightBut2.Checked)
            {
                lightBut2.BackColor = Color.DimGray;
                lightBut2.Update();
                lightBut2.Checked = true;
            }
            else
            {
                lightBut2.BackColor = Color.Green;
                lightBut2.Update();
                lightBut2.Checked = false;
            }
        }

        private void LightBut3_Click(object sender, EventArgs e)
        {
            if (!lightBut3.Checked)
            {
                lightBut3.BackColor = Color.DimGray;
                lightBut3.Update();
                lightBut3.Checked = true;
            }
            else
            {
                lightBut3.BackColor = Color.Green;
                lightBut3.Update();
                lightBut3.Checked = false;
            }
        }

        private void LightBut4_Click(object sender, EventArgs e)
        {
            if (!lightBut4.Checked)
            {
                lightBut4.BackColor = Color.DimGray;
                lightBut4.Update();
                lightBut4.Checked = true;
            }
            else
            {
                lightBut4.BackColor = Color.Green;
                lightBut4.Update();
                lightBut4.Checked = false;
            }
        }

        private void LightBut5_Click(object sender, EventArgs e)
        {
            if (!lightBut5.Checked)
            {
                lightBut5.BackColor = Color.DimGray;
                lightBut5.Update();
                lightBut5.Checked = true;
            }
            else
            {
                lightBut5.BackColor = Color.Green;
                lightBut5.Update();
                lightBut5.Checked = false;
            }
        }

        private void LightBut6_Click(object sender, EventArgs e)
        {
            if (!lightBut6.Checked)
            {
                lightBut6.BackColor = Color.DimGray;
                lightBut6.Update();
                lightBut6.Checked = true;
            }
            else
            {
                lightBut6.BackColor = Color.Green;
                lightBut6.Update();
                lightBut6.Checked = false;
            }
        }

        private void LightBut7_Click(object sender, EventArgs e)
        {
            if (!lightBut7.Checked)
            {
                lightBut7.BackColor = Color.DimGray;
                lightBut7.Update();
                lightBut7.Checked = true;
            }
            else
            {
                lightBut7.BackColor = Color.Green;
                lightBut7.Update();
                lightBut7.Checked = false;
            }
        }

        private void LightBut8_Click(object sender, EventArgs e)
        {
            if (!lightBut8.Checked)
            {
                lightBut8.BackColor = Color.DimGray;
                lightBut8.Update();
                lightBut8.Checked = true;
            }
            else
            {
                lightBut8.BackColor = Color.Green;
                lightBut8.Update();
                lightBut8.Checked = false;
            }
        }

        private void LightBut9_Click(object sender, EventArgs e)
        {
            if (!lightBut9.Checked)
            {
                lightBut9.BackColor = Color.DimGray;
                lightBut9.Update();
                lightBut9.Checked = true;
            }
            else
            {
                lightBut9.BackColor = Color.Green;
                lightBut9.Update();
                lightBut9.Checked = false;
            }
        }

        private void LightBut10_Click(object sender, EventArgs e)
        {
            if (!lightBut10.Checked)
            {
                lightBut10.BackColor = Color.DimGray;
                lightBut10.Update();
                lightBut10.Checked = true;
            }
            else
            {
                lightBut10.BackColor = Color.Green;
                lightBut10.Update();
                lightBut10.Checked = false;
            }
        }

        private void LightBut11_Click(object sender, EventArgs e)
        {
            if (!lightBut11.Checked)
            {
                lightBut11.BackColor = Color.DimGray;
                lightBut11.Update();
                lightBut11.Checked = true;
            }
            else
            {
                lightBut11.BackColor = Color.Green;
                lightBut11.Update();
                lightBut11.Checked = false;
            }
        }

        private void LightBut12_Click(object sender, EventArgs e)
        {
            if (!lightBut12.Checked)
            {
                lightBut12.BackColor = Color.DimGray;
                lightBut12.Update();
                lightBut12.Checked = true;
            }
            else
            {
                lightBut12.BackColor = Color.Green;
                lightBut12.Update();
                lightBut12.Checked = false;
            }
        }

        private void LightBut13_Click(object sender, EventArgs e)
        {
            if (!lightBut13.Checked)
            {
                lightBut13.BackColor = Color.DimGray;
                lightBut13.Update();
                lightBut13.Checked = true;
            }
            else
            {
                lightBut13.BackColor = Color.Green;
                lightBut13.Update();
                lightBut13.Checked = false;
            }
        }

        private void LightBut14_Click(object sender, EventArgs e)
        {
            if (!lightBut14.Checked)
            {
                lightBut14.BackColor = Color.DimGray;
                lightBut14.Update();
                lightBut14.Checked = true;
            }
            else
            {
                lightBut14.BackColor = Color.Green;
                lightBut14.Update();
                lightBut14.Checked = false;
            }
        }

        private void LightBut15_Click(object sender, EventArgs e)
        {
            if (!lightBut15.Checked)
            {
                lightBut15.BackColor = Color.DimGray;
                lightBut15.Update();
                lightBut15.Checked = true;
            }
            else
            {
                lightBut15.BackColor = Color.Green;
                lightBut15.Update();
                lightBut15.Checked = false;
            }
        }

        private void LightBut16_Click(object sender, EventArgs e)
        {
            if (!lightBut16.Checked)
            {
                lightBut16.BackColor = Color.DimGray;
                lightBut16.Update();
                lightBut16.Checked = true;
            }
            else
            {
                lightBut16.BackColor = Color.Green;
                lightBut16.Update();
                lightBut16.Checked = false;
            }
        }

        private void LightBut17_Click(object sender, EventArgs e)
        {
            if (!lightBut17.Checked)
            {
                lightBut17.BackColor = Color.DimGray;
                lightBut17.Update();
                lightBut17.Checked = true;
            }
            else
            {
                lightBut17.BackColor = Color.Green;
                lightBut17.Update();
                lightBut17.Checked = false;
            }
        }

        private void LightBut18_Click(object sender, EventArgs e)
        {
            if (!lightBut18.Checked)
            {
                lightBut18.BackColor = Color.DimGray;
                lightBut18.Update();
                lightBut18.Checked = true;
            }
            else
            {
                lightBut18.BackColor = Color.Green;
                lightBut18.Update();
                lightBut18.Checked = false;
            }
        }

        private void LightBut19_Click(object sender, EventArgs e)
        {
            if (!lightBut19.Checked)
            {
                lightBut19.BackColor = Color.DimGray;
                lightBut19.Update();
                lightBut19.Checked = true;
            }
            else
            {
                lightBut19.BackColor = Color.Green;
                lightBut19.Update();
                lightBut19.Checked = false;
            }
        }

        private void LightBut20_Click(object sender, EventArgs e)
        {
            if (!lightBut20.Checked)
            {
                lightBut20.BackColor = Color.DimGray;
                lightBut20.Update();
                lightBut20.Checked = true;
            }
            else
            {
                lightBut20.BackColor = Color.Green;
                lightBut20.Update();
                lightBut20.Checked = false;
            }
        }

        private void LightBut21_Click(object sender, EventArgs e)
        {
            if (!lightBut21.Checked)
            {
                lightBut21.BackColor = Color.DimGray;
                lightBut21.Update();
                lightBut21.Checked = true;
            }
            else
            {
                lightBut21.BackColor = Color.Green;
                lightBut21.Update();
                lightBut21.Checked = false;
            }
        }

        private void LightBut22_Click(object sender, EventArgs e)
        {
            if (!lightBut22.Checked)
            {
                lightBut22.BackColor = Color.DimGray;
                lightBut22.Update();
                lightBut22.Checked = true;
            }
            else
            {
                lightBut22.BackColor = Color.Green;
                lightBut22.Update();
                lightBut22.Checked = false;
            }
        }

        private void LightBut23_Click(object sender, EventArgs e)
        {
            if (!lightBut23.Checked)
            {
                lightBut23.BackColor = Color.DimGray;
                lightBut23.Update();
                lightBut23.Checked = true;
            }
            else
            {
                lightBut23.BackColor = Color.Green;
                lightBut23.Update();
                lightBut23.Checked = false;
            }
        }

        private void LightBut24_Click(object sender, EventArgs e)
        {
            if (!lightBut24.Checked)
            {
                lightBut24.BackColor = Color.DimGray;
                lightBut24.Update();
                lightBut24.Checked = true;
            }
            else
            {
                lightBut24.BackColor = Color.Green;
                lightBut24.Update();
                lightBut24.Checked = false;
            }
        }

        private void LightBut25_Click(object sender, EventArgs e)
        {
            if (!lightBut25.Checked)
            {
                lightBut25.BackColor = Color.DimGray;
                lightBut25.Update();
                lightBut25.Checked = true;
            }
            else
            {
                lightBut25.BackColor = Color.Green;
                lightBut25.Update();
                lightBut25.Checked = false;
            }
        }

        private void LightBut26_Click(object sender, EventArgs e)
        {
            if (!lightBut26.Checked)
            {
                lightBut26.BackColor = Color.DimGray;
                lightBut26.Update();
                lightBut26.Checked = true;
            }
            else
            {
                lightBut26.BackColor = Color.Green;
                lightBut26.Update();
                lightBut26.Checked = false;
            }
        }

        private void LightBut27_Click(object sender, EventArgs e)
        {
            if (!lightBut27.Checked)
            {
                lightBut27.BackColor = Color.DimGray;
                lightBut27.Update();
                lightBut27.Checked = true;
            }
            else
            {
                lightBut27.BackColor = Color.Green;
                lightBut27.Update();
                lightBut27.Checked = false;
            }
        }

        private void LightBut28_Click(object sender, EventArgs e)
        {
            if (!lightBut28.Checked)
            {
                lightBut28.BackColor = Color.DimGray;
                lightBut28.Update();
                lightBut28.Checked = true;
            }
            else
            {
                lightBut28.BackColor = Color.Green;
                lightBut28.Update();
                lightBut28.Checked = false;
            }
        }

        private void LightBut29_Click(object sender, EventArgs e)
        {
            if (!lightBut29.Checked)
            {
                lightBut29.BackColor = Color.DimGray;
                lightBut29.Update();
                lightBut29.Checked = true;
            }
            else
            {
                lightBut29.BackColor = Color.Green;
                lightBut29.Update();
                lightBut29.Checked = false;
            }
        }

        private void LightBut30_Click(object sender, EventArgs e)
        {
            if (!lightBut30.Checked)
            {
                lightBut30.BackColor = Color.DimGray;
                lightBut30.Update();
                lightBut30.Checked = true;
            }
            else
            {
                lightBut30.BackColor = Color.Green;
                lightBut30.Update();
                lightBut30.Checked = false;
            }
        }

        private void LightBut31_Click(object sender, EventArgs e)
        {
            if (!lightBut31.Checked)
            {
                lightBut31.BackColor = Color.DimGray;
                lightBut31.Update();
                lightBut31.Checked = true;
            }
            else
            {
                lightBut31.BackColor = Color.Green;
                lightBut31.Update();
                lightBut31.Checked = false;
            }
        }

        private void J11_Click(object sender, EventArgs e)
        {
            LightBut0_Click(sender, e);
            LightBut1_Click(sender, e);
            LightBut2_Click(sender, e);
            LightBut3_Click(sender, e);
        }

        private void J12_Click(object sender, EventArgs e)
        {
            LightBut4_Click(sender, e);
            LightBut5_Click(sender, e);
            LightBut6_Click(sender, e);
            LightBut7_Click(sender, e);
        }

        private void J13_Click(object sender, EventArgs e)
        {
            LightBut8_Click(sender, e);
            LightBut9_Click(sender, e);
            LightBut10_Click(sender, e);
            LightBut11_Click(sender, e);
        }

        private void J14_Click(object sender, EventArgs e)
        {
            LightBut12_Click(sender, e);
            LightBut13_Click(sender, e);
            LightBut14_Click(sender, e);
            LightBut15_Click(sender, e);
        }

        private void J15_Click(object sender, EventArgs e)
        {
            LightBut16_Click(sender, e);
            LightBut17_Click(sender, e);
            LightBut18_Click(sender, e);
            LightBut19_Click(sender, e);
        }

        private void J16_Click(object sender, EventArgs e)
        {
            LightBut20_Click(sender, e);
            LightBut21_Click(sender, e);
            LightBut22_Click(sender, e);
            LightBut23_Click(sender, e);
        }

        private void J17_Click(object sender, EventArgs e)
        {
            LightBut24_Click(sender, e);
            LightBut25_Click(sender, e);
            LightBut26_Click(sender, e);
            LightBut27_Click(sender, e);
        }

        private void J18_Click(object sender, EventArgs e)
        {
            LightBut28_Click(sender, e);
            LightBut29_Click(sender, e);
            LightBut30_Click(sender, e);
            LightBut31_Click(sender, e);
        }



        private void CmbTestBtn_Click(object sender, EventArgs e)
        {
            //Check that an FEB client exists, otherwise, don't bother setting up the pulser or trying to get data
            if (PP.FEB1.client != null)
            {
                //byte[] incomingData = new byte[5]; //Declare new array of bytes for incoming data
                System.Net.Sockets.Socket febSocket = PP.FEB1.TNETSocket_prop; //Declare and define FEB socket variable
                febSocket.ReceiveTimeout = 500; //Set timeout on FEB socket to 500
                byte[] sendRDB = Encoding.ASCII.GetBytes("rdb\r\n"); //PP.GetBytes("rdb\r\n"); //Lazy way for converting RDB command into packet to send to FEB
                if (febSocket.Available > 0) //Pick up and discard whatever the board may currently be trying to send until we are ready for "fresh data"
                {
                    byte[] junk = new byte[febSocket.Available];
                    febSocket.Receive(junk);
                }

                //Open up the file used to store/read channel responses
                //Stores in single line per channel format, channel# AverageADCresponse
                #region ReadFileAvgs
                String cmbAvgFileName = "C:\\data\\cmb_tester_data\\cmb_channel_averages.dat";
                String[] channelAvgString = new String[64];
                double[] channelAvgVal = new double[64];
                if (File.Exists(cmbAvgFileName))
                {
                    channelAvgString = File.ReadAllLines(cmbAvgFileName); //read all the lines from the file and store each line as a different entry in the channelAvgString array
                    for (int channel = 0; channel < 64; channel++)
                    {
                        channelAvgVal[channel] = Convert.ToDouble(channelAvgString[channel].Substring(2)); //convert the average recorded value for each channel into a usable number
                    }
                }
                else //If the channel averages file does not exist, then print out an error
                {
                    System.Console.WriteLine("ERR: Could not find " + cmbAvgFileName + "!");
                    System.Console.WriteLine("Does the file exist and is it accessible?");
                }
                #endregion ReadFileAvgs

                //These registers are required for setting up the onboard histogrammers and acquiring the data sent by them
                Mu2e_Register.FindAddr(0x10, ref PP.FEB1.arrReg, out Mu2e_Register histo_control); //Histogram control register
                Mu2e_Register.FindAddr(0x11, ref PP.FEB1.arrReg, out Mu2e_Register histo_accum_inter); //Histogram accumulation interval
                Mu2e_Register.FindAddr(0x14, ref PP.FEB1.arrReg, out Mu2e_Register afe0_memRdPtr0); //AFE 0 histogram memory read pointer
                Mu2e_Register.FindAddr(0x15, ref PP.FEB1.arrReg, out Mu2e_Register afe1_memRdPtr1); //AFE 1 histogram memory read pointer
                Mu2e_Register.FindAddr(0x16, ref PP.FEB1.arrReg, out Mu2e_Register afe0_dataPort); //AFE 0 data port
                Mu2e_Register.FindAddr(0x17, ref PP.FEB1.arrReg, out Mu2e_Register afe1_dataPort); //AFE 1 data port
                var histCntrl_Dark_Base = 0x60; //this leaves the bin size at 1 ADC/bin and starts the histogrammer - the base is necessary since the channel numbers will be added before it starts histogramming

                //In this loop we will go over each channel and histogram it
                for (uint channel = 0; channel < 8; channel++)
                {
                    Mu2e_Register.WriteReg((uint) histCntrl_Dark_Base + channel, ref histo_control, ref PP.FEB1.client);
                }

                //These registers are required for setting up the FEB to record triggered events and the flashing of the LED.
                //The FEB is responsible for controlling the pulser via the FEB's GPO port into the pulser's external clock port
                Mu2e_Register.FindAddr(0x00, ref PP.FEB1.arrReg, out Mu2e_Register controlStatusReg); //Control status register
                Mu2e_Register.FindAddr(0x303, ref PP.FEB1.arrReg, out Mu2e_Register trigControlReg); //Trigger control register
                Mu2e_Register.FindAddr(0x308, ref PP.FEB1.arrReg, out Mu2e_Register spillDurReg); //Spill duration register
                Mu2e_Register.FindAddr(0x309, ref PP.FEB1.arrReg, out Mu2e_Register interSpillDurReg); //Interspill duration register
                Mu2e_Register.FindAddr(0x305, ref PP.FEB1.arrReg, out Mu2e_Register sampleLengthReg); //Sample length for each event/trigger
                Mu2e_Register.FindAddr(0x307, ref PP.FEB1.arrReg, out Mu2e_Register testPulseFreqReg); //Onboard test pulser frequency
                Mu2e_Register.WriteReg(0x0, ref testPulseFreqReg, ref PP.FEB1.client); //Set test pulser frequency to zero, this allows external triggering from LEMO
                Mu2e_Register.WriteReg(0x20, ref controlStatusReg, ref PP.FEB1.client); //Issue a reset of the trigger/spill counters on the board
                Mu2e_Register.WriteReg(0xA, ref interSpillDurReg, ref PP.FEB1.client); //Set the interspill duration for 10 seconds

                

                #region Pedestal/Calibration
                for (uint fpga = 0; fpga < 4; fpga++) //Turn on bias for SiPMs
                    PP.FEB1.SetV(Convert.ToDouble(cmbBias.Text), (int)fpga);
                #region BiasWait
                cmbBiasTicker.Text= "Waiting for bias";
                for (int tick = 0; tick < 5; tick++){ cmbBiasTicker.Text += "."; cmbBiasTicker.Update(); System.Threading.Thread.Sleep(1000);}
                cmbBiasTicker.Text += "done"; cmbBiasTicker.Update(); System.Threading.Thread.Sleep(250);
                cmbBiasTicker.Text = "                               "; cmbBiasTicker.Update();
                #endregion BiasWait
                


                //Code that was here can be found in the following file: C:\FLASH\..\Redacted.txt
                bool passed_calibration = false;
                double[] pedestals = new double[64]; //pedestal for each channel
                double[] gains = new double[64]; //gain for each channel (adc/pe)                
                ROOTNET.NTH1F[] peHistos = new ROOTNET.NTH1F[64]; //Histograms used to determine gains
                var histo_file = ROOTNET.NTFile.Open("D:/Calibrations.root", "RECREATE");
                if (histo_file == null)
                {
                    throw new AccessViolationException("Cannot open/modify D:/Calibrations.root");
                    return; //if the file can't be opened, don't waste any time doing anything else
                }

                //Use histogramming in FEB to establish gains
                //

                histo_file.Write();
                foreach (var histo in peHistos)
                    if(histo != null)
                        histo.Delete();
                histo_file.Close();

                #region Move this
                febSocket.Send(sendRDB);
                while (febSocket.Available == 0) //Wait to receive data from the FEB
                    System.Threading.Thread.Sleep(2);

                int old_available = 0;
                while (febSocket.Available > old_available) //Wait until the FEB has all the data to send
                {
                    old_available = febSocket.Available;
                    System.Threading.Thread.Sleep(250);
                }
                byte[] rec_buf = new byte[febSocket.Available];
                int lret = febSocket.Receive(rec_buf, rec_buf.Length, System.Net.Sockets.SocketFlags.None);
                for (int iByte = 0; iByte < lret - 1; iByte++)
                    rec_buf[iByte] = rec_buf[iByte + 1]; //ignore 0x3e at the beginning of data
                #endregion Move this

                #endregion Pedestal/Calibration


                #region LED Response Evaluation

                uint spill_status = 0;
                uint spill_num = 0;
                uint trig_num = 0;
                numTrigsDisp.Text = trig_num.ToString();
                numTrigsDisp.Update();

                if (passed_calibration)
                {
                    //MessageBox.Show("CMB Evaluation\nPlease connect the LED");
                    //Mu2e_Register.WriteReg(0x0C, ref controlStatusReg, ref PP.FEB1.client); //Issues a reset of the AFE deserializers on the FPGA and the MIG DDR interface
                    Mu2e_Register.WriteReg(0xA, ref spillDurReg, ref PP.FEB1.client); //Set the spill duration for 10 seconds
                    Mu2e_Register.WriteReg(0x64, ref sampleLengthReg, ref PP.FEB1.client); //Set the number of ADC samples to record per trigger
                    for (uint fpga = 0; fpga < 4; fpga++) //Turn on bias for SiPMs
                        PP.FEB1.SetV(Convert.ToDouble(cmbBias.Text), (int)fpga);
                    Mu2e_Register.WriteReg(0x300, ref trigControlReg, ref PP.FEB1.client); //Open the spill gate: Set trig-control register to enable board to record data for 1 spill
                    while (spill_status != 2) //trig_num < Convert.ToInt16(requestNumTrigs.Text))
                    {
                        System.Threading.Thread.Sleep(250); //Slow down the polling of the FEB for triggers/status
                        PP.FEB1.CheckStatus(out spill_status, out spill_num, out trig_num); //Keep polling the board about how many triggers it has seen
                        numTrigsDisp.Text = trig_num.ToString();
                        numTrigsDisp.Update();
                    }
                    //Mu2e_Register.WriteReg(0x42, ref trigControlReg, ref PP.FEB1.client); //Stops triggering
                    Mu2e_Register.WriteReg(0x0, ref trigControlReg, ref PP.FEB1.client);

                    //receive data and unpack in memory
                    //Write average response to a file on disk, compare response of each CMB channel to running average (since SiPMs do not change)
                    SpillData testerData = new SpillData(); //create new SpillData object to hold incoming data from FEB

                    //Send RDB to FEB
                    febSocket.Send(sendRDB);
                    while (febSocket.Available == 0) //Wait to receive data from the FEB
                        System.Threading.Thread.Sleep(10);

                    old_available = 0;
                    while (febSocket.Available > old_available) //Wait until the FEB has all the data to send
                    {
                        old_available = febSocket.Available;
                        System.Threading.Thread.Sleep(250);
                    }
                    rec_buf = new byte[febSocket.Available];
                    lret = febSocket.Receive(rec_buf, rec_buf.Length, System.Net.Sockets.SocketFlags.None);

                    if (testerData.ParseInput(rec_buf))
                    {
                        double[] avgResponse = new double[64]; //hold the average response for each channel

                        foreach (var tEvent in testerData.SpillEvents)
                        {
                            Mu2e_Ch[] cha = tEvent.ChanData.ToArray();
                            for (int chan = 0; chan < 4/*tEvent.ChNum*/; chan++) // ! ! ! INITIAL TESTING ! ! !
                            {
                                avgResponse[chan] += cha[chan].data.Max() - pedestals[chan];

                                System.Console.Write("Chan {0}: ", chan);

                                for (uint i = 0; i < tEvent.ChanData[chan].data.Length; i++)
                                {
                                    //In here is where the evaluation of the "CMB" will be performed
                                    //For now, simply looking at the max ADC for each channel and comparing
                                    //it to a running average for that channel should tell us if there are
                                    //any extraneous issues with that particular CMB
                                    if (tEvent.ChanData[chan].data[i] - pedestals[chan] > 0)
                                        System.Console.Write("{0} ", (int)(tEvent.ChanData[chan].data[i] - pedestals[chan]));
                                    else
                                        System.Console.Write(".");



                                    /*
                                     Compare the current response to the average response in the histogram
                                        -> Check for deviations from the mean, if > XX%, reject this CMB
                                     If CMB appears normal, add the measurements to the histogram and resave the file
                                        float recMean = ;
                                        if(Math.Abs(chanAvg - recMean)/(0.5*(chanAvg + recMean)) < 0.1)
                                        {
                                            //If there is less than a 10% difference, must be okay... (???)
                                            //Update the record-average with this 'good' response (avoids contamination (?))
                                        }
                                    */
                                }
                                System.Console.WriteLine();
                            }

                        }

                    }
                }
                else
                {
                    MessageBox.Show("Calibration Failed\n\nAborting");
                }
                #endregion LED Response Evaluation

                //Turn off bias for SiPMs
                for (uint fpga = 0; fpga < 4; fpga++)
                    PP.FEB1.SetV(0.0, (int)fpga);

            }
        }

        private void CmbBiasOverride_CheckedChanged(object sender, EventArgs e)
        {
            if (cmbBiasOverride.Checked)
                cmbBias.Enabled = true;
            else
            {
                cmbBias.Enabled = false;
                cmbBias.Text = "56.0";
            }
        }

        private void CmbBias_TextChanged(object sender, EventArgs e)
        {
            cmbBias.BackColor = Color.White;
            bool parsed = double.TryParse(cmbBias.Text, out var isNumber);
            //Check for invalid input
            if (string.IsNullOrWhiteSpace(cmbBias.Text))
                cmbBias.BackColor = Color.Red; 
            else if (!parsed || isNumber < 0 || isNumber > 80)
                cmbBias.Text = "56.0";
        }

        private void RequestNumTrigs_TextChanged(object sender, EventArgs e)
        {
            requestNumTrigs.BackColor = Color.White;
            bool parsed = uint.TryParse(requestNumTrigs.Text, out var isNumber);
            //Check for invalid input
            if (string.IsNullOrWhiteSpace(requestNumTrigs.Text))
                requestNumTrigs.BackColor = Color.Red;
            else if (!parsed || isNumber < 1 || isNumber > 65535)
                requestNumTrigs.Text = "100";
        }

    }



    public class ConnectAttemptEventArgs : EventArgs
    {
        public int ConnectAttempt { get; set; }
    }



}
