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
                        console_Disp.Text = l;
                        Application.DoEvents();
                    }
                }
                catch
                {
                    string m = "Exception caught. Do you have a module selected?";
                    console_Disp.Text = m;
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

                myFEB.CheckFEB_connection();

                ushort fpga_num = Convert.ToUInt16(udFPGA.Value);
                for (int j = 0; j < num_reg; j++)
                {
                    Mu2e_Register.FindName(rnames[j], fpga_num, ref myFEB.arrReg, out Mu2e_Register r1);
                    //r1.fpga_index = fpga_num;
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
                    Mu2e_Register.FindName(rnames[j], fpga_num, ref myFEB.arrReg, out Mu2e_Register r1);
                    //r1.fpga_index = fpga_num;
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
                console_Disp.Text = console_label.add_messg("---- FEB1 ----\r\n");
            }
            if (myName.Contains("FEB2"))
            {
                Button1_Click((object)btnFEB2, e);
                console_Disp.Text = console_label.add_messg("---- FEB2 ----\r\n");
            }
            if (myName.Contains("WC"))
            {
                Button1_Click((object)btnWC, e);
                console_Disp.Text = console_label.add_messg("----  WC  ----\r\n");
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
                    Mu2e_Register.FindName(rnames[j], fpga_num, ref myFEB.arrReg, out Mu2e_Register r1);
                    //r1.fpga_index = fpga_num;
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
                    Mu2e_Register.FindName(rnames[j], fpga_num, ref myFEB.arrReg, out Mu2e_Register r1);
                    //r1.fpga_index = fpga_num;
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
            double[] cmb_temp = new double[4];
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

                        cmb_temp = PP.FEB1.ReadTemp((int)udFPGA.Value);
                        break;
                    case 2:
                        txtV.Text = PP.FEB2.ReadV((int)udFPGA.Value).ToString("0.000");
                        txtI.Text = PP.FEB2.ReadA0((int)udFPGA.Value, (int)udChan.Value).ToString("0.0000");
                        cmb_temp = PP.FEB2.ReadTemp((int)udFPGA.Value);
                        break;

                    default:
                        break;
                }
            }
            txtCMB_Temp1.Text = cmb_temp[0].ToString("0.0");
            txtCMB_Temp2.Text = cmb_temp[1].ToString("0.0");
            txtCMB_Temp3.Text = cmb_temp[2].ToString("0.0");
            txtCMB_Temp4.Text = cmb_temp[3].ToString("0.0");
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
            s[5, 5] = "ONE_WIRE_READ2";
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

                Mu2e_Register.FindName(rnames[i], Convert.ToUInt16(udFPGA.Value), ref PP.FEB1.arrReg, out Mu2e_Register r1);
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


            Mu2e_Register.FindName("TRIG_CONTROL", Convert.ToUInt16(udFPGA.Value), ref PP.FEB1.arrReg, out Mu2e_Register r2);
            // Mu2e_Register.WriteReg(0x0350, ref r2, ref PP.FEB1.client);
            Mu2e_Register.FindName("SPILL_STATE", Convert.ToUInt16(udFPGA.Value), ref PP.FEB1.arrReg, out Mu2e_Register r3);
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
            uint FPGA_index = (uint)udFPGA.Value;
            uint chan = 0;
            Mu2e_Register mux_reg = new Mu2e_Register();
            Mu2e_FEB_client myFEB = new Mu2e_FEB_client();
            if (_ActiveFEB == 1) { myFEB = PP.FEB1; }
            if (_ActiveFEB == 2) { myFEB = PP.FEB2; }
            chan = (uint)udChan.Value;
            //mux_reg.fpga_index = (ushort)FPGA_index;
            if (myFEB != null)
            {
                Mu2e_Register.FindAddrFPGA(0x020, FPGA_index, ref myFEB.arrReg, out mux_reg);
                if (chan < 16)
                {
                    uint v = (uint)(0x10 + chan);
                    Mu2e_Register.WriteReg(v, ref mux_reg, ref myFEB.client);
                }
            }
            BtnCHANGE_Click(null, null);
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
                        Mu2e_Register.FindName("TRIG_CONTROL", Convert.ToUInt16(udFPGA.Value), ref PP.FEB1.arrReg, out Mu2e_Register r_spill);
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
                    runLog/*lblRunLog*/.Text = "";
                    string[] all_m = PP.myRun.RunStatus.ToArray<string>();
                    int l = all_m.Length;
                    if (PP.myRun.RunStatus.Count > 13)
                    {
                        for (int i = l - 13; i < l; i++)
                        { runLog/*lblRunLog*/.Text += all_m[i] + "\r\n"; }
                    }
                    else
                    {
                        for (int i = 0; i < all_m.Length; i++)
                        { runLog/*lblRunLog*/.Text += all_m[i] + "\r\n"; }
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
            Mu2e_Register.FindName("TRIG_CONTROL", Convert.ToUInt16(udFPGA.Value), ref PP.FEB1.arrReg, out Mu2e_Register r_spill);
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
            Mu2e_Register.FindName("CONTROL_STATUS", Convert.ToUInt16(udFPGA.Value), ref PP.FEB1.arrReg, out csr);
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
            CreateButtonArrays();
        }

        private void CreateButtonArrays()
        {
            //To-do: put these buttons into a TableLayoutPanel for easy organization as well as adding/subtracting buttons
            #region DiCounterQAButtons
            qaDiButtons = new System.Windows.Forms.RadioButton[8]; //Create an array of new buttons
            for(int btni = 0; btni < qaDiButtons.Length; btni++)
            {
                qaDiButtons[btni] = new System.Windows.Forms.RadioButton
                {
                    Appearance = System.Windows.Forms.Appearance.Button,
                    AutoCheck = false,
                    BackColor = System.Drawing.Color.Green,
                    Location = new System.Drawing.Point(385 + ((btni % 4) * 30), 25 + ((btni / 4) * 30)),
                    Name = "qaDiButton" + btni,
                    Size = new System.Drawing.Size(25, 25),
                    TabIndex = 109+btni,
                    Text = btni.ToString(),
                    TextAlign = System.Drawing.ContentAlignment.MiddleCenter,
                    UseVisualStyleBackColor = false
                }; //Create a new button in the array and set some properties
                qaDiButtons[btni].Click += new System.EventHandler(QaDiButton_Click); //Assign the button a handler for when it is clicked
                dicounterQAGroup.Controls.Add(qaDiButtons[btni]); //Add the button to the group box
            }
            #endregion DiCounterQAButtons


            #region LightCheckButtons
            lightButtons = new System.Windows.Forms.RadioButton[64]; //Create an array of new buttons
            lightCheckGroupFPGAs = new System.Windows.Forms.GroupBox[4]; //Create an array of new group boxes
            lightCMBlabels = new System.Windows.Forms.Label[16]; //Create an array of new labels
            for(int cmb = 0; cmb < 16; cmb++)
            {
                lightCMBlabels[cmb] = new System.Windows.Forms.Label
                {
                    AutoSize = false,
                    BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle,
                    Name = "lightCMB" + cmb,
                    Size = new System.Drawing.Size(30, 20),
                    TabIndex = 182+cmb,
                    Text = cmb.ToString(),
                    TextAlign = System.Drawing.ContentAlignment.MiddleCenter
                }; //Create a label for each CMB
                lightCMBlabels[cmb].Click += new System.EventHandler(LightCMBLabel_Click); //Assign the label a handler for when it is clicked
            } //Create labels for each CMB
            qaFPGALabels = new System.Windows.Forms.Label[4]; //Create an array of new labels
            int fpga = 0; //Define a variable called FPGA for drawing and filling through group boxes
            for (int btni = 0; btni < lightButtons.Length; btni++)
            {
                if (btni > 0 && btni % 16 == 0) //increment fpga #
                    fpga++;
                lightButtons[btni] = new System.Windows.Forms.RadioButton
                {
                    Appearance = System.Windows.Forms.Appearance.Button,
                    AutoCheck = false,
                    BackColor = System.Drawing.Color.Green,
                    Cursor = System.Windows.Forms.Cursors.Cross,
                    Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0))),
                    Name = "lightButton" + btni,
                    Size = new System.Drawing.Size(60, 30),
                    TabIndex = 134 + btni,
                    Text = btni.ToString(),
                    TextAlign = System.Drawing.ContentAlignment.MiddleCenter,
                    UseVisualStyleBackColor = false
                }; //Create a new button and set some properties
                lightButtons[btni].Click += new System.EventHandler(LightButton_Click); //Assign the button a handler for when it is clicked

                if (lightCheckGroupFPGAs[fpga] == null) //If the group box for that particular FPGA has not been created yet, create it now
                {
                    qaFPGALabels[fpga] = new System.Windows.Forms.Label
                    {
                        AutoSize = false,
                        BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle,
                        Name = "fpga" + fpga + "Label",
                        Size = new System.Drawing.Size(60, 20),
                        TabIndex = 240+fpga,
                        Text = "FPGA " + fpga,
                        TextAlign = System.Drawing.ContentAlignment.MiddleCenter
                    }; //Create a label for the FPGA
                    qaFPGALabels[fpga].Click += new System.EventHandler(LightFPGALabel_Click); //Assign the label a handler for when it is clicked

                    lightCheckGroupFPGAs[fpga] = new System.Windows.Forms.GroupBox
                    {
                        Location = new System.Drawing.Point(300 + ((fpga % 2) * 410), 85 + ((fpga / 2) * 200)),
                        Name = "lightCheckGroup",
                        Size = new System.Drawing.Size(400, 180),
                        TabIndex = 130 + fpga,
                        TabStop = false,
                        Text = "FPGA " + fpga
                    }; //Create a new group box and set some properties
                    var localPanel = new TableLayoutPanel
                    {
                        Dock = System.Windows.Forms.DockStyle.Fill,
                        Size = new System.Drawing.Size(380, 160),
                        Name = "lightCheckGroupFPGApanel" + fpga
                    }; //Create a new TableLayoutPanel which will live in the group box and arrange the buttons
                    lightCheckGroupFPGAs[fpga].Controls.Add(localPanel); //Add the panel to the group box
                    lightCheckFPGApanel.Controls.Add(lightCheckGroupFPGAs[fpga]); //Add the FPGA group box to the light check group box
                }
                TableLayoutPanel fpga_panel = lightCheckGroupFPGAs[fpga].Controls.OfType<TableLayoutPanel>().First() ; //Get the panel from the group box (use First() to get actual panel, since there is only one here)
                fpga_panel.Controls.Add(lightButtons[btni], btni % 4, (btni%16)/ 4); //Add the button to the panel inside the FPGA's group box
                if(((btni+1) % 4) == 0)//At the end of filling each row with buttons, add a label
                    fpga_panel.Controls.Add(lightCMBlabels[btni / 4], 5, (btni%16) / 4);
                if (((btni+1) % 16) == 0)//At the end of filling the panel with buttons and cmb labels, add the FPGA 
                    fpga_panel.Controls.Add(qaFPGALabels[fpga], 6, 1);//Add the FPGA label to the layout table
            }
            #endregion LightCheckButtons
        }

        private void GroupBoxEvDisplay_Enter(object sender, EventArgs e)
        {

        }

        private void QaStartButton_Click(object sender, EventArgs e)
        {
            if (PP.FEB1.client != null) //If the FEB is connected, then proceed.
            {
                qaStartButton.Enabled = false;  //prevents multiple clicks of the buttons
                autoThreshBtn.Enabled = false;
                lightCheckResetThresh.Enabled = false;
                lightCheckBtn.Enabled = false;
                string[] chanOuts = new string[8];

                //Data are written to the Google Drive, CRV Fabrication Documents folder ScanningData, subfolder DicounterQA
                //'using' will ensure the writer is closed/destroyed if the scope of the structure is left due to code-completion or a thrown exception
                using (StreamWriter writer = File.AppendText("C:\\Users\\FEB-Laptop-1\\Google Drive\\CRV Fabrication Documents\\QA_Safety\\QA\\Dicounter Source Testing\\ScanningData_" + qaOutputFileName.Text + ".txt")) //The output file
                {
                    writer.Write("{0}\t", numTextBox.Text); //Write dicounter number to file

                    //Write temp to file
                    double[] temp = { 0, 0, 0, 0 };
                    for (int numTries = 0; numTries < 10; numTries++) //Try and read the temperature 10 times
                        temp = PP.FEB1.ReadTemp();                    //read the temperatures on FPGA 0
                    writer.Write("{0}\t", temp[0].ToString("0.00"));  //Write the temperature, in degrees C, as measured by the CMB

                    //Write date to file
                    writer.Write("{0}\t", DateTime.Now.ToString("MM/dd/yy HH:mm\t")); //Changed to 24 hour time format to match database storage format

                    PP.FEB1.SetV(Convert.ToDouble(qaBias.Text)); //Turn on bias for the first FPGA (since this is the only one used for dicounter QA

                    foreach(var btn in qaDiButtons) { if (!btn.Checked) { btn.BackColor = Color.Green; btn.Update(); } } //Reset all active channel indicators to green

                    foreach (var btn in qaDiButtons)
                    {
                        double averageCurrent = 0;
                        int channel = Convert.ToInt16(btn.Text);
                        if (!btn.Checked)
                        {
                            for (int measI = 0; measI < Convert.ToInt16(qaDiNumAvg.Value); measI++)
                                averageCurrent += Convert.ToDouble(PP.FEB1.ReadA0(0, channel)); //read the current for the specified channel on FPGA 0
                            averageCurrent /= Convert.ToDouble(qaDiNumAvg.Value);
                            if (averageCurrent < Convert.ToDouble(qaDiIWarningThresh.Text)) //if the current was less than warning thresh && we still have current, update the color of the lamp
                                qaDiButtons[channel].BackColor = Color.Red;
                            if (averageCurrent < 0.025) //if there was no current then
                                qaDiButtons[channel].BackColor = Color.Blue; //set indicator color to blue: "cold-no-current"
                            qaDiButtons[channel].Update();
                        }

                        chanOuts[channel] = averageCurrent.ToString("0.0000");
                        Console.WriteLine("Channel {0}: {1}", channel, chanOuts[channel]);
                        writer.Write("{0}\t", chanOuts[channel]); //write out the current for the channel
                        autoDataProgress.Increment(1);
                    }

                    writer.WriteLine(); //write a 'return' to file

                    PP.FEB1.SetV(0.0); //Turn off the bias
                    autoDataProgress.Value = 0; //Set the progress bar back to 0
                    autoDataProgress.Update();
                }

                qaStartButton.Enabled = true;
                autoThreshBtn.Enabled = true;
                lightCheckResetThresh.Enabled = true;
                lightCheckBtn.Enabled = true;
            }
        }

        private void AutoThreshBtn_Click(object sender, EventArgs e)
        {
            if (PP.FEB1.client != null) //If the FEB is connected, then proceed
            {
                autoThreshBtn.Enabled = false;  //prevents multiple clicks of the buttons
                qaStartButton.Enabled = false;
                lightCheckResetThresh.Enabled = false;
                lightCheckBtn.Enabled = false;

                PP.FEB1.SetVAll(Convert.ToDouble(qaBias.Text)); //Turn on the bias for ALL FPGAs

                foreach (var btn in lightButtons) 
                {
                    if (!btn.Checked)
                    {
                        btn.BackColor = Color.Green;
                        btn.Update();
                    }
                } //Reset all active channel indicators to green

                foreach (var btn in lightButtons)
                {
                    if (!btn.Checked)
                    {
                        int channel = Convert.ToInt16(btn.Text);
                        double average = PP.FEB1.ReadA0(channel / 16, channel % 16); //read the current for the specified channel on the correct FPGA

                        if (average < 0.025) //If no current update the color of the lamp
                        {
                            btn.BackColor = Color.Blue; //set lamp color to blue "cold-no-current"
                            btn.Update();
                        }
                        else
                        {
                            PP.lightCheckChanThreshs[channel] = average * 1.10; //set the threshold to 10% higher than dark-current
                        }
                    }
                    lightAutoThreshProgress.Increment(1);
                }

                PP.FEB1.SetVAll(0.0);//Turn off the bias
                lightAutoThreshProgress.Value = 0;
                lightAutoThreshProgress.Update();

                lightCheckChanThresh.Text = PP.lightCheckChanThreshs[Convert.ToUInt16(lightCheckChanSelec.Value)].ToString("0.0000"); //update the current channel to display the new thresh
                
                autoThreshBtn.Enabled = true;
                qaStartButton.Enabled = true;
                lightCheckResetThresh.Enabled = true;
                lightCheckBtn.Enabled = true;
            }
        }

        private void LightCheckResetThresh_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < PP.lightCheckChanThreshs.Length; i++)
                PP.lightCheckChanThreshs[i] = 0.25;
            lightGlobalThresh.Text = "0.25";
        }

        private void LightCheckChanSelec_ValueChanged(object sender, EventArgs e)
        {
            lightCheckChanThresh.Text = PP.lightCheckChanThreshs[Convert.ToUInt16(lightCheckChanSelec.Value)].ToString("0.0000");
        }

        private void LightCheckChanThreshBtn_Click(object sender, EventArgs e)
        {
            PP.lightCheckChanThreshs[Convert.ToUInt16(lightCheckChanSelec.Value)] = Convert.ToDouble(lightCheckChanThresh.Text);
        }

        private void LightCheckBtn_Click(object sender, EventArgs e)
        {
            if (PP.FEB1.client != null)
            {
                lightCheckBtn.Enabled = false;  //prevents multiple clicks on the button
                autoThreshBtn.Enabled = false;
                qaStartButton.Enabled = false;
                lightCheckResetThresh.Enabled = false;

                //Writes the file to the CRV Fabrication Documents, ScanningData folder on the Google Drive, subfolder ModuleLightCheck
                //'using' will ensure the writer is closed/destroyed if the scope of the structure is left due to code-completion or a thrown exception
                using (StreamWriter writer = File.AppendText("C:\\Users\\FEB-Laptop-1\\Google Drive\\CRV Fabrication Documents\\QA_Safety\\QA\\Module Light Leak Testing\\LightCheck.txt")) //Path for file output of lightcheck
                {
                    if (lightWriteToFileBox.Checked)
                    {
                        writer.Write("{0}\t", lightModuleLabel.Text); //write out the module name
                        writer.Write("{0}\t", lightModuleSide.Text); //write out the module side
                        writer.Write("{0}\t", lightModuleLayer.Value.ToString()); //write out the module layer
                        writer.Write("{0}", DateTime.Now.ToString("g")); // write out the timestamp
                    }

                    PP.FEB1.SetVAll(Convert.ToDouble(qaBias.Text)); //Turn on the bias

                    //initially set to green
                    for(int btn = 0; btn < lightButtons.Length; btn++)
                    {
                        if (!lightButtons[btn].Checked)
                        {
                            lightButtons[btn].BackColor = Color.Green;
                            lightButtons[btn].Text = btn.ToString();
                            lightButtons[btn].Update();
                        }

                    }
                    int numChecked = 0;
                    foreach (RadioButton r in lightButtons)
                    {
                        if (!r.Checked) numChecked++;
                    }
                    int numAverages = numChecked;
                    //double[] averages = new double[numAverages];

                    //for (int i = 0; i < numAverages; i++)
                    //{
                    //    averages[i] = 0;
                    //}
                    int numTimesToCheck = 1;
                    if (!lightWriteToFileBox.Checked)
                        numTimesToCheck = (int)lightNumChecks.Value;
                    for (int j = 0; j < numTimesToCheck; j++)
                    {
                        double total_avg_I = 0;
                        for (int chan = 0; chan < lightButtons.Length; chan++)
                        {
                            if (!lightButtons[chan].Checked)
                            {
                                double average = PP.FEB1.ReadA0(chan / 16, chan % 16);
                                //averages[i] = average;
                                if (average > 0)
                                    total_avg_I += average / numAverages;

                                if (!globalThreshChkBox.Checked) // if the global thresh box is not checked, then use individual channels thresh
                                {
                                    if (average > PP.lightCheckChanThreshs[chan]) //If the current is above the thresh, light leak!
                                        lightButtons[chan].BackColor = Color.Red; //flag the channel
                                    else if (average < 0.025) //if it died or bias is lost, set "cold-no-current"
                                        lightButtons[chan].BackColor = Color.Blue;

                                    lightButtons[chan].Text = "" + Math.Round(average, 2);
                                    lightButtons[chan].Update();
                                    if (j == 0 && lightWriteToFileBox.Checked && lightWriteToFileBox.Enabled)
                                    {
                                        writer.WriteLine("");
                                        writer.Write("\t{0}\t", chan.ToString());
                                        writer.Write("{0}\t", ((double)PP.lightCheckChanThreshs[chan] / 1.1).ToString("0.0000"));
                                        writer.Write("{0}", average.ToString("0.0000"));
                                    }
                                }
                                else
                                {
                                    if (average > Convert.ToDouble(lightGlobalThresh.Text)) //If the current is above the thresh, light leak!
                                        lightButtons[chan].BackColor = Color.Red; //flag the channel
                                    else if (average < 0.025) //if it died or bias is lost, set "cold-no-current"
                                        lightButtons[chan].BackColor = Color.Blue;

                                    lightButtons[chan].Text = Math.Round(average, 2).ToString(); //Update each channel with the current reading
                                    lightButtons[chan].Update();

                                    if (j == 0 && lightWriteToFileBox.Checked && lightWriteToFileBox.Enabled)
                                    {
                                        writer.WriteLine("");
                                        writer.Write("\t{0}\t", chan.ToString());
                                        writer.Write("{0}\t", lightGlobalThresh.Text);
                                        writer.Write("{0}", average.ToString("0.0000"));
                                    }
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
                        Console.WriteLine("Sum: " + total_avg_I);
                        if (total_avg_I >= 0.1)
                        {
                            int signal = 0;
                            if (total_avg_I / .1 < 1)
                            {
                                signal = 1;
                            }
                            else if (total_avg_I / .1 > 20)
                            {
                                signal = 20;
                            }
                            else
                            {
                                signal = (int)(total_avg_I / .1);
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
                            //PP.FEB1.SetV(0.0, (int)udFPGA.Value);
                            PP.FEB1.SetV(0.0, 0);
                            PP.FEB1.SetV(0.0, 1);
                            PP.FEB1.SetV(0.0, 2);
                            PP.FEB1.SetV(0.0, 3);

                            txtI.Text = PP.FEB1.ReadA0((int)udFPGA.Value, (int)udChan.Value).ToString("0.0000");
                            break;
                        case 2:
                            PP.FEB2.SetV(0.0);
                            txtI.Text = PP.FEB2.ReadA0((int)udFPGA.Value, (int)udChan.Value).ToString("0.0000");
                            break;

                        default:
                            break;
                    }
                    if (lightWriteToFileBox.Checked)
                        writer.WriteLine();
                }
                lightAutoThreshProgress.Increment(-64);
                lightCheckBtn.Enabled = true;
                autoThreshBtn.Enabled = true;
                qaStartButton.Enabled = true;
                lightCheckResetThresh.Enabled = true;
            }
        }

        private void QaBias_TextChanged(object sender, EventArgs e)
        {
            qaBias.BackColor = Color.Orange;
            bool parsed = double.TryParse(qaBias.Text, out var isNumber);
            //Check for invalid input
            if (string.IsNullOrWhiteSpace(qaBias.Text))
            {
                qaBias.BackColor = Color.Red;
                qaStartButton.Enabled = false;
                lightCheckBtn.Enabled = false;
                autoThreshBtn.Enabled = false;
            }
            else if (!parsed || isNumber < 0 || isNumber > 80)
            {
                qaBias.Text = "57.0";
                qaBias.BackColor = Color.LightGray;
                qaStartButton.Enabled = true;
                lightCheckBtn.Enabled = true;
                autoThreshBtn.Enabled = true;
            }
            else
            {
                qaStartButton.Enabled = true;
                lightCheckBtn.Enabled = true;
                autoThreshBtn.Enabled = true;
            }
        }

        private void QaDiButton_Click(object sender, EventArgs e)
        {
            RadioButton btn = (RadioButton)sender;
            if (!btn.Checked)
            {
                btn.BackColor = Color.DimGray;
                btn.Update();
                btn.Checked = true;
            }
            else
            {
                btn.BackColor = Color.Green;
                btn.Update();
                btn.Checked = false;
            }
        }

        private void LightButton_Click(object sender, EventArgs e)
        {
            RadioButton btn = (RadioButton) sender;
            if (!btn.Checked)
            {
                btn.BackColor = Color.DimGray;
                btn.Update();
                btn.Checked = true;
            }
            else
            {
                btn.BackColor = Color.Green;
                btn.Update();
                btn.Checked = false;
            }
        }

        private void LightCMBLabel_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.Label lbl = (System.Windows.Forms.Label)sender; //Get the label that was clicked
            int cmb = Convert.ToInt16(lbl.Text); //Convert to CMB number index starting at 0
            for(int btn = (cmb*4); btn < (cmb*4)+4; btn++) //Set loop bounds and 'click' all of the buttons
            {
                sender = lightButtons[btn];
                LightButton_Click(sender, e);
            }
        }

        private void LightFPGALabel_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.Label lbl = (System.Windows.Forms.Label)sender;
            int fpga_num = Convert.ToInt16(lbl.Text.Substring(lbl.Text.Length-1, 1));
            for(int btni = fpga_num*16; btni < (fpga_num*16)+16; btni++)
            {
                sender = lightButtons[btni];
                LightButton_Click(sender, e);
            }
        }

        private void CmbTestBtn_Click(object sender, EventArgs e)
        {
            //Check that an FEB client exists, otherwise, don't bother setting up the pulser or trying to get data
            if (PP.FEB1.client != null)
            {
                //These registers are required for setting up the FEB to record triggered events and the flashing of the LED.
                //The FEB is responsible for controlling the pulser via the FEB's GPO port into the pulser's external clock port
                Mu2e_Register.FindAddr(0x300, ref PP.FEB1.arrReg, out Mu2e_Register flashGateControlReg); //Flash gate control register
                Mu2e_Register.FindAddr(0x303, ref PP.FEB1.arrReg, out Mu2e_Register trigControlReg); //Trigger control register
                Mu2e_Register.FindAddr(0x304, ref PP.FEB1.arrReg, out Mu2e_Register hitPipelineDelayReg); //Hit Pipeline Delay register
                Mu2e_Register.FindAddr(0x305, ref PP.FEB1.arrReg, out Mu2e_Register sampleLengthReg); //Sample length for each event/trigger
                Mu2e_Register.FindAddr(0x307, ref PP.FEB1.arrReg, out Mu2e_Register testPulseFreqReg); //Onboard test pulser frequency
                Mu2e_Register.FindAddr(0x308, ref PP.FEB1.arrReg, out Mu2e_Register spillDurReg); //Spill duration register
                Mu2e_Register.FindAddr(0x309, ref PP.FEB1.arrReg, out Mu2e_Register interSpillDurReg); //Interspill duration register
                Mu2e_Register.WriteReg(0x0, ref testPulseFreqReg, ref PP.FEB1.client); //Set test pulser frequency to zero, this allows external triggering from LEMO
                Mu2e_Register.WriteReg(0xA, ref interSpillDurReg, ref PP.FEB1.client); //Set the interspill duration for 10 seconds

                Mu2e_Register[] controlStatusReg = Mu2e_Register.FindAllAddr(0x00, ref PP.FEB1.arrReg);
                Mu2e_Register.WriteAllReg(0x20, ref controlStatusReg, ref PP.FEB1.client); //issue a general reset for each FPGA
                Mu2e_Register[][] gainControlReg = Mu2e_Register.FindAllAddrRange(0x46, 0x47, ref PP.FEB1.arrReg);
                Mu2e_Register.WriteAllRegRange(0x250, ref gainControlReg, ref PP.FEB1.client); //Set the gain for all AFE chips on all FPGAs to the same value

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
                if (updateFilesChkBox.Checked)
                {
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
                }
                #endregion ReadFileAvgs

                #region Pedestal/Calibration

                #region BiasWait
                cmbInfoBox.Text = "Waiting for bias";
                PP.FEB1.SetVAll(Convert.ToDouble(cmbBias.Text));
                #endregion BiasWait

                //Code that was here can be found in the following file: C:\FLASH\..\Redacted.txt
                bool passed_calibration = false;
                double[] pedestals = new double[64]; //pedestal for each channel
                double[] gains = new double[64]; //gain for each channel (adc/pe)      

                ROOTNET.NTH1I[] peHistos = new ROOTNET.NTH1I[64]; //Histograms used to determine gains
                ROOTNET.NTSpectrum[] peakFinders = new ROOTNET.NTSpectrum[2];
                HistoHelper hist_helper = new HistoHelper(ref PP.FEB1, 0xFFE);//0x400);
                ROOTNET.NTH1I[] histos = new ROOTNET.NTH1I[2];
                ROOTNET.NTF1[] peakFits;
                cmbInfoBox.Text = "Calibrating"; cmbInfoBox.Update();
                for (uint channel = 0; channel < 4; channel++)
                {
                    if (peHistos[channel] == null) //skip the channels that have already been histogrammed (due to the two channel histograms return from the FEB)
                    {
                        histos = hist_helper.GetHistogram(channel, 1);
                        uint[] channels = { channel, Convert.ToUInt32(histos[1].GetTitle()) }; //Lazily grab the other channel's label from the histogram title...
                        System.Console.WriteLine("Histo Chans: " + channels[0] + ", " + channels[1]);

                        for (int hist = 0; hist < 2; hist++)//Loop over each of the two histograms
                        {
                            peHistos[channels[hist]] = histos[hist];
                            peakFinders[hist] = new ROOTNET.NTSpectrum(3); //Only try and compute the gain from pedestal, 1st, and possibly 2nd PE
                            int peaksFound = peakFinders[hist].Search(peHistos[channels[hist]], 1.5, "nobackground", 0.00001); //Don't try and estimate background, and set the threshold to only include pedestal, 1st, and 2nd PE
                            if (peaksFound < 2) { System.Console.WriteLine("Cannot find 1+ PE for Chan {0}", channels[hist]); continue; } //Need this beacuse we need to know the gain, which is impossible if we can't see first PE
                            var peakPositions = peakFinders[hist].GetPositionX();
                            peakFits = new ROOTNET.NTF1[peaksFound];
                            for (uint peak = 0; peak < peaksFound; peak++)
                            {
                                float x_loc = peakPositions[(int)peak];
                                peakFits[peak] = new ROOTNET.NTF1("Ch" + channels[hist].ToString() + "peak" + peak.ToString(), "gaus", x_loc - 2.5, x_loc + 2.5);
                                peakFits[peak].SetLineColor(Convert.ToInt16(peak + 2));
                                peHistos[channels[hist]].Fit(peakFits[peak], "R+");// R option forces the function to fit only over its specified range, + option tells it to add fit to histogram list without deleting previous fits
                            }
                            pedestals[channels[hist]] = peakFits[0].GetParameter(1);
                            gains[channels[hist]] = peakFits[1].GetParameter(1) - pedestals[channels[hist]];
                            if (peakFits.Length > 2)
                                gains[channels[hist]] = (gains[channels[hist]] + peakFits[2].GetParameter(1) - peakFits[1].GetParameter(1)) / 2.0;

                        }
                    }
                }

                peHistos[0].Draw();
                
                var histo_file = ROOTNET.NTFile.Open("D:/Calibrations.root", "RECREATE");
                if (histo_file == null)
                {
                    histo_file = ROOTNET.NTFile.Open("D:/Calib_BACKUP_" + System.DateTime.Now.ToFileTime().ToString() + ".root", "RECREATE");
                    System.Console.WriteLine("Cannot open/modify D:/Calibrations.root. A backup has been created.");
                }
                
                histo_file.Write();
                foreach (var histo in peHistos)
                    if (histo != null)
                    {
                        histo.Write();
                        histo.Delete();
                    }
                histo_file.Close();

                passed_calibration = false; //find a new home for this guy

                //return;
                #endregion Pedestal/Calibration
                

                #region LED Response Evaluation
                uint spill_status = 0;
                uint spill_num = 0;
                uint trig_num = 0;
                numTrigsDisp.Text = trig_num.ToString();
                numTrigsDisp.Update();

                if (passed_calibration)
                {
                    //This is for evaluating response to CMB LEDs
                    //Need to get the LED Flasher DACs
                    //Need to get the flash gate control register
                    //Need to use the trigControlReg to set the GPO select
                    //Need to use the testPulseFreqReg to set the on-board pulser frequency, which will send pulses out the GPO port
                    //Need to use the Flash Gate Control to set the routing on each FPGA
                    //Histogram SiPM Response to LEDs or Use pin-diode to measure current

                    //SiPM response to external LED flash
                    //[X] Need to set the flash gate control register to flash gate and not CMB LEDs (set 0x300 to 2)
                    //[X] Need to set the trigControlReg to set the GPO select
                    //[X] Need to set the test pulser frequency
                    //[?] Need to set the delay between internal pulser and external LED (hit-pipeline delay if it affects the raw SiPM data for histogramming, or adjust on pulser)
                    //[X] Get histograms
                    //[ ] Fit data
                    //[ ] Report evaluation

                    Mu2e_Register.WriteReg(0x2, ref flashGateControlReg, ref PP.FEB1.client); //Set the CMB Pulse routing to the Flash Gate (to LED flasher will create interference on CMB)
                    Mu2e_Register.WriteReg(0x100, ref trigControlReg, ref PP.FEB1.client); //Enable the on-board test pulser, output of this signal will be delivered to external pulser to flash LED
                    Mu2e_Register.WriteReg(0x5E5E5E, ref testPulseFreqReg, ref PP.FEB1.client); //Set the on-board test pulser's frequency to ~230kHz
                    Mu2e_Register.WriteReg(0x1, ref hitPipelineDelayReg, ref PP.FEB1.client); //Set the hit pipeline delay to minimum value (12.56ns)

                    //ROOTNET.NTSpectrum[] peakFinders = new ROOTNET.NTSpectrum[2];
                    //HistoHelper hist_helper = new HistoHelper(ref PP.FEB1, 0x400);
                    //ROOTNET.NTH1I[] histos = new ROOTNET.NTH1I[2];
                    //ROOTNET.NTF1[] peakFits;
                    ROOTNET.NTH1I[] ledHistos = new ROOTNET.NTH1I[64]; //Histograms from LED flash
                    cmbInfoBox.Text = "Flashing LED"; cmbInfoBox.Update();
                    for (uint channel = 0; channel < 4; channel++)
                    {
                        if (ledHistos[channel] == null) //skip the channels that have already been histogrammed (due to the two channel histograms return from the FEB)
                        {
                            hist_helper.SetAccumulation_interval(0xFFE); //Set a long accumulation interval
                            histos = hist_helper.GetHistogram(channel, 1);
                            uint[] channels = { channel, Convert.ToUInt32(histos[1].GetTitle()) }; //Lazily grab the other channel's label from the histogram title...
                            System.Console.WriteLine("Histo Chans: " + channels[0] + ", " + channels[1]);

                            for (int hist = 0; hist < 2; hist++) //Loop over each of the two histograms
                            {
                                ledHistos[channels[hist]] = histos[hist];
                                peakFinders[hist] = new ROOTNET.NTSpectrum(10); //Maybe try and get all the PEs
                                int peaksFound = peakFinders[hist].Search(ledHistos[channels[hist]], 1.5, "nobackground", 0.001); //Don't try and estimate background, and set the threshold to only include pedestal, 1st, and 2nd PE
                                //if (peaksFound < 2) { System.Console.WriteLine("Cannot find 1+ PE for Chan {0}", channels[hist]); continue; } //Need this beacuse we need to know the gain, which is impossible if we can't see first PE
                                //var peakPositions = peakFinders[hist].GetPositionX();
                                //peakFits = new ROOTNET.NTF1[peaksFound];
                                //for (uint peak = 0; peak < peaksFound; peak++)
                                //{
                                //    float x_loc = peakPositions[(int)peak];
                                //    peakFits[peak] = new ROOTNET.NTF1("Ch" + channels[hist].ToString() + "peak" + peak.ToString(), "gaus", x_loc - 2.5, x_loc + 2.5);
                                //    peakFits[peak].SetLineColor(Convert.ToInt16(peak + 2));
                                //    ledHistos[channels[hist]].Fit(peakFits[peak], "R+");// R option forces the function to fit only over its specified range, + option tells it to add fit to histogram list without deleting previous fits
                                //}
                                //pedestals[channels[hist]] = peakFits[0].GetParameter(1);
                                //gains[channels[hist]] = peakFits[1].GetParameter(1) - pedestals[channels[hist]];
                                //if (peakFits.Length > 2)
                                //gains[channels[hist]] = (gains[channels[hist]] + peakFits[2].GetParameter(1) - peakFits[1].GetParameter(1)) / 2.0;
                            }
                        }
                    }

                    histo_file = ROOTNET.NTFile.Open("D:/Responses.root", "RECREATE");
                    if (histo_file == null)
                    {
                        histo_file = ROOTNET.NTFile.Open("D:/Resp_BACKUP_" + System.DateTime.Now.ToFileTime().ToString() + ".root", "RECREATE");
                        System.Console.WriteLine("Cannot open/modify D:/Responses.root. A backup has been created.");
                    }

                    foreach (var histo in ledHistos)
                        if (histo != null)
                        {
                            histo.Write();
                            histo.Delete();
                        }
                    histo_file.Close();
                }

                Mu2e_Register.WriteReg(0x0, ref trigControlReg, ref PP.FEB1.client); //Disable the on-board test pulser
                Mu2e_Register.WriteReg(0x0, ref testPulseFreqReg, ref PP.FEB1.client); //Set the on-board test pulser's frequency to 0


                //Turn off bias for SiPMs
                for (uint fpga = 0; fpga < 4; fpga++)
                    PP.FEB1.SetV(0.0, (int)fpga);

                cmbInfoBox.Text = ""; cmbInfoBox.Update();

                return; //FULL STOP, DONT DO ANYTHING MORE, SHIT BELOW IS ABSOLUTELY CRAY CRAY

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
                cmbBias.Text = "57.0";
            }
        }

        private void CmbBias_TextChanged(object sender, EventArgs e)
        {
            cmbBias.BackColor = Color.White;
            bool parsed = double.TryParse(cmbBias.Text, out var isNumber);
            //Check for invalid input
            if (string.IsNullOrWhiteSpace(cmbBias.Text))
            {
                cmbBias.BackColor = Color.Red;
                cmbTestBtn.Enabled = false;
            }
            else if (!parsed || isNumber < 0 || isNumber > 80)
            {
                cmbBias.Text = "57.0";
                cmbTestBtn.Enabled = true;
            }
            else
                cmbTestBtn.Enabled = true;
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

        private void OneReadout_CheckedChanged(object sender, EventArgs e)
        {
            if (oneReadout.Checked == true)
            {
                foreach (var btn in qaDiButtons.Take(4).ToArray()) //If any of the first 4 buttons have been checked, uncheck them
                    if (btn.Checked)
                        QaDiButton_Click(btn, e);

                foreach (var btn in qaDiButtons.Skip(4).ToArray()) //Check the remaining buttons, if not checked already
                    if (!btn.Checked) //if it isn't checked, then check it
                        QaDiButton_Click(btn, e);
            }
            else
            {
                foreach (var btn in qaDiButtons) //If any button is checked, uncheck it
                    if (btn.Checked)
                        QaDiButton_Click(btn, e);
            }
        }

        private void LightModuleLabel_TextChanged(object sender, EventArgs e)
        {
            lightModuleLabel.BackColor = Color.White;
            if (string.IsNullOrWhiteSpace(lightModuleSide.Text))
                lightModuleSide.SelectedIndex = 0;
            if (!string.IsNullOrWhiteSpace(lightModuleLabel.Text))
            {
                lightWriteToFileBox.Enabled = true;
                lightModuleSide.Enabled = true;
                lightModuleLayer.Enabled = true;
            }
            else
            {
                lightWriteToFileBox.Checked = false;
                lightWriteToFileBox.Enabled = false;
                lightModuleLabel.BackColor = Color.Yellow;
                lightModuleSide.Enabled = false;
                lightModuleLayer.Enabled = false;
            }
            lightModuleLabel.Update();
        }

        private void LightWriteToFileBox_CheckedChanged(object sender, EventArgs e)
        {
            if (lightWriteToFileBox.Checked)
            {
                lightNumChecks.Value = 1;
                lightNumChecks.Enabled = false;
            }
            else
                lightNumChecks.Enabled = true;
        }

        private void LightGlobalThresh_TextChanged(object sender, EventArgs e)
        {
            lightGlobalThresh.BackColor = Color.White;
            bool parsed = double.TryParse(lightGlobalThresh.Text, out var isNumber);
            //Check for invalid input
            if (string.IsNullOrWhiteSpace(lightGlobalThresh.Text))
            {
                lightGlobalThresh.BackColor = Color.Red;
                globalThreshChkBox.Enabled = false;
            }
            else if (!parsed || isNumber < 0)
            {
                lightGlobalThresh.Text = "0.25";
                globalThreshChkBox.Enabled = true;
            }
            else
                globalThreshChkBox.Enabled = true;
        }

        private void Console_Disp_TextChanged(object sender, EventArgs e)
        {
            //Autoscroll to the end of the text box
            console_Disp.SelectionStart = console_Disp.Text.Length;
            console_Disp.ScrollToCaret();
        }

        private void RunLog_TextChanged(object sender, EventArgs e)
        {
            //Autoscroll to the end of the text box
            runLog.SelectionStart = console_Disp.Text.Length;
            runLog.ScrollToCaret();
        }

        private void qaDiIWarningThresh_TextChanged(object sender, EventArgs e)
        {
            qaDiIWarningThresh.BackColor = Color.White;
            bool parsed = double.TryParse(qaDiIWarningThresh.Text, out var isNumber);
            //Check for invalid input
            if (string.IsNullOrWhiteSpace(qaDiIWarningThresh.Text))
            {
                qaDiIWarningThresh.BackColor = Color.Red;
                qaStartButton.Enabled = false;
            }
            else if (!parsed || isNumber < 0)
            {
                qaDiIWarningThresh.Text = "0.1";
                qaStartButton.Enabled = true;
            }
            else
                qaStartButton.Enabled = true;
        }
    }


    public class ConnectAttemptEventArgs : EventArgs
    {
        public int ConnectAttempt { get; set; }
    }

}
