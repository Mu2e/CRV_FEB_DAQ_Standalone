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
        private uConsole console;
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
        private uint spill_num;

        private const int num_chans = 16;
        private System.Windows.Forms.Label[] BDVoltLabels = new System.Windows.Forms.Label[num_chans];

        private SerialPort comPort;
        private string[] portList;
        private bool zerod;
        private bool darkCurrent;
        private int currentChannel;
        private int currentDicounter;
        private bool stepperCheckForOK;
        private bool stepperReceivedOK;
        private DateTime runStart;
        private bool first_spill_taken;
        private bool waiting_for_data;
        private double spill_record_delay = 5;

        public void AddConsoleMessage(string msg)
        {
            console_Disp.Text = console.Add_messg(msg);
            Application.DoEvents();
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

            console = new uConsole();


            spill_trig_num = new int[3];
            spill_num = 0;
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
                    { DbgFEB_Click((object)dbgWC, null); ConsoleBox.Text = ""; return; }
                    if (dbgFEB1.BackColor == Color.Green)
                    { DbgFEB_Click((object)dbgFEB2, null); ConsoleBox.Text = ""; return; }
                    if (dbgWC.BackColor == Color.Green)
                    { DbgFEB_Click((object)dbgFEB1, null); ConsoleBox.Text = ""; return; }
                }
                if (e.KeyChar.ToString() == "1")
                { DbgFEB_Click((object)dbgWC, null); }
                if (e.KeyChar.ToString() == "2")
                { DbgFEB_Click((object)dbgWC, null); }
            }
        }

        private void ConsoleBox_TextChanged(object sender, EventArgs e)
        {
            string l = "";
            bool this_is_a_write = false;
            bool multi_write = false;
            string sent_string = "";

            if (ConsoleBox.Text.Contains("\n"))
            {
                try
                {
                    if (ConsoleBox.Text.Contains("$")) //this is a comment
                    {
                        AddConsoleMessage(ConsoleBox.Text);
                        ConsoleBox.Text = "";
                    }
                    else
                    {
                        byte[] buf = PP.GetBytes(ConsoleBox.Text);
                        sent_string = ConsoleBox.Text;
                        if (ConsoleBox.Text.ToLower().Contains("wr")) { this_is_a_write = true; if (ConsoleBox.Text.ToLower().Contains("wrm")) { multi_write = true; } }
                        ConsoleBox.Text = "";
                        while (PP.active_Socket.Available > 0)
                        {
                            byte[] rbuf = new byte[PP.active_Socket.Available];
                            PP.active_Socket.Receive(rbuf);
                        }
                        if (!this_is_a_write)
                        {
                            PP.active_Socket.Send(buf);
                            System.Threading.Thread.Sleep(100);
                            if (sent_string.ToLower().Contains("a0 "))
                            {
                                int delay = Convert.ToInt16(sent_string.Split().Skip(1).First().ToString());
                                System.Threading.Thread.Sleep(delay * 100);
                            }
                            byte[] rec_buf = new byte[PP.active_Socket.Available];
                            int ret_len = PP.active_Socket.Receive(rec_buf, rec_buf.Length, System.Net.Sockets.SocketFlags.None);
                            string t = string.Join("", PP.GetString(rec_buf, ret_len).Split('>'));
                            t = sent_string + t;
                            AddConsoleMessage(t);
                        }
                        else if (this_is_a_write)
                        {
                            if (multi_write)
                            {
                                string[] write_elements = sent_string.Split(' '); //split up the command into wri, start-register, value, number of writes
                                ushort start_reg = Convert.ToUInt16(write_elements[1], 16);
                                ushort num_writes = Convert.ToUInt16(write_elements[3]);
                                if (write_elements.Length == 4)
                                {
                                    string base_cmd = "wr ";
                                    for (ushort register = start_reg; register < start_reg + num_writes; register++)
                                    {
                                        buf = PP.GetBytes(base_cmd + register.ToString("X2") + " " + write_elements[2] + "\r\n");
                                        PP.active_Socket.Send(buf);
                                        System.Threading.Thread.Sleep(10);
                                    }
                                    buf = PP.GetBytes("rdi " + write_elements[1] + " " + write_elements[3]); //command of the form rdi [start register] [num_reads = num_writes]
                                    PP.active_Socket.Send(buf);
                                    System.Threading.Thread.Sleep(100);
                                    byte[] rec_buf = new byte[PP.active_Socket.Available];
                                    int ret_len = PP.active_Socket.Receive(rec_buf);
                                    string t = string.Join("", PP.GetString(rec_buf, ret_len).Split('>'));
                                    t = sent_string + t;
                                    AddConsoleMessage(t);

                                }
                                else
                                {
                                    AddConsoleMessage("Writing to multiple registers must be of the form: wri [start_reg] [value] [num_writes] \r\n Your command was: " + sent_string);
                                }

                            }
                            else
                            {
                                PP.active_Socket.Send(buf);
                                l = sent_string;
                                string read_string = string.Join(" ", sent_string.Split().Skip(1).Take(1));
                                //read_string = read_string.Substring(read_string.ToLower().IndexOf("wr") + 2, read_string.Length - read_string.ToLower().IndexOf("wr") + 2);
                                read_string = "rd " + read_string + "\r\n";
                                buf = PP.GetBytes(read_string);
                                PP.active_Socket.Send(buf);
                                System.Threading.Thread.Sleep(100);
                                byte[] rec_buf = new byte[PP.active_Socket.Available];
                                int ret_len = PP.active_Socket.Receive(rec_buf);
                                string t = string.Join("", PP.GetString(rec_buf, ret_len).Split('>'));
                                t = sent_string + t;
                                AddConsoleMessage(t);
                            }
                        }
                    }
                }
                catch (Exception dispExcep)
                {
                    AddConsoleMessage(dispExcep.ToString());
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
                console_Disp.Text = console.Add_messg("---- FEB1 ----\r\n");
            }
            if (myName.Contains("FEB2"))
            {
                Button1_Click((object)btnFEB2, e);
                console_Disp.Text = console.Add_messg("---- FEB2 ----\r\n");
            }
            if (myName.Contains("WC"))
            {
                Button1_Click((object)btnWC, e);
                console_Disp.Text = console.Add_messg("----  WC  ----\r\n");
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

                        cmb_temp = PP.FEB1.ReadTempFPGA((int)udFPGA.Value);
                        break;
                    case 2:
                        txtV.Text = PP.FEB2.ReadV((int)udFPGA.Value).ToString("0.000");
                        txtI.Text = PP.FEB2.ReadA0((int)udFPGA.Value, (int)udChan.Value).ToString("0.0000");
                        cmb_temp = PP.FEB2.ReadTempFPGA((int)udFPGA.Value);
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
            BtnRegREAD_Click(null, null);
            BtnBiasREAD_Click(null, null);
        }

        private void UdFPGA_ValueChanged(object sender, EventArgs e)
        {
            BtnRegREAD_Click(null, null);
        }

        private void SpillTimer_Tick(object sender, EventArgs e)
        {
            if (PP.myRun != null)
            {
                if (PP.myRun.ACTIVE) //If we are actively looking for spills
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

                        WC_client.check_status(out bool in_spill, out string num_trig, out string mytime);
                        lblSpillWC.Text = in_spill.ToString();
                        try { spill_trig_num[2] = Convert.ToInt32(num_trig); } catch { spill_trig_num[2] = 0; }
                        lblWCTrigNum.Text = spill_trig_num[2].ToString("0");

                        if (in_spill)
                        {
                            if (first_spill_taken == false)
                            {
                                first_spill_taken = true;
                                PP.myRun.UpdateStatus("First Spill Synchronization");
                            }
                            PP.myRun.timeLastSpill = DateTime.Now;
                            PP.myRun.UpdateStatus("Detected spill. Run file is " + PP.myRun.OutFileName);
                            PP.myRun.spill_complete = false;
                            waiting_for_data = true;
                        }
                        else
                        {
                            PP.myRun.spill_complete = true;
                        }
                    }

                    if (PP.myRun.spill_complete) //If we are no longer in a spill
                    {
                        if (first_spill_taken) //If we took the first spill, we can now see if we are a few seconds after the spill to record data
                        {
                            double time_past_spill = (DateTime.Now - PP.myRun.timeLastSpill).TotalSeconds;
                            if ((time_past_spill > spill_record_delay) && waiting_for_data) //If we have waited a sufficient amount of time and we are expecting data, save the data
                            {
                                waiting_for_data = false;
                                PP.myRun.RecordSpill();
                                spill_num++;
                                //Update the total number of triggers
                                lblFEB1_TotTrig.Text = (Convert.ToUInt64(lblFEB1_TotTrig.Text) + (ulong)spill_trig_num[0]).ToString("0");
                                lblFEB2_TotTrig.Text = (Convert.ToUInt64(lblFEB2_TotTrig.Text) + (ulong)spill_trig_num[1]).ToString("0");
                                lblWC_TotTrig.Text = (Convert.ToUInt64(lblWC_TotTrig.Text) + (ulong)spill_trig_num[2]).ToString("0");
                            }
                        }
                    }
                }

                lblRunName.Text = "Run_" + PP.myRun.num.ToString(); //Keep the run name updated
                while (PP.myRun.RunStatus.Count > 0) //If there are any status messages in the queue, print them to the run console
                    runLog.AppendText(PP.myRun.RunStatus.Dequeue() + "\r\n");
                lblRunTime.Text = (DateTime.Now - runStart).TotalSeconds.ToString("0"); //Keep the time we have been running updated
                lblFEB1Spill.Text = spill_num.ToString("0");
                if (first_spill_taken) //If we already took our first spill, we can now keep the spill timer updated
                    lblSpillTime.Text = (DateTime.Now - PP.myRun.timeLastSpill).TotalSeconds.ToString("0"); //update spill timer

            }
            else
            {
                runLog.AppendText("Cannot take data, prepare to run first!\r\n");
                SpillTimer.Enabled = false;
            }


            // --------------------------------------
            // ~         Old Code Below             ~
            // --------------------------------------
            #region OldCode
            //bool in_spill;
            //if (PP.glbDebug) { Console.WriteLine("tick"); }
            //if (tabControl.SelectedIndex == 0)
            //{
            //    //if (PP.glbDebug){ Console.WriteLine("timer");}
            //    try
            //    {
            //        TimeSpan since_last_spill = DateTime.Now.Subtract(PP.myRun.timeLastSpill);
            //        lblSpillTime.Text = since_last_spill.TotalSeconds.ToString("0");
            //        if ((since_last_spill.TotalSeconds > 2) && (PP.myRun.spill_complete))
            //        {
            //            Mu2e_Register.FindName("TRIG_CONTROL", Convert.ToUInt16(udFPGA.Value), ref PP.FEB1.arrReg, out Mu2e_Register r_spill);
            //            if (!PP.myRun.OneSpill)
            //            { }// Mu2e_Register.WriteReg(0x02, ref r_spill, ref PP.FEB1.client); }
            //        }
            //        if (PP.myRun != null)
            //        {
            //            lblFEB1_TotTrig.Text = PP.myRun.total_trig[0].ToString();
            //            lblFEB2_TotTrig.Text = PP.myRun.total_trig[1].ToString();
            //            lblWC_TotTrig.Text = PP.myRun.total_trig[2].ToString();
            //            if (PP.myRun.ACTIVE)
            //            {
            //                lblRunTime.Text = DateTime.Now.Subtract(PP.myRun.created).Seconds.ToString("0");
            //                if ((since_last_spill.Seconds > 2) && (since_last_spill.Seconds < 20))
            //                {
            //                    if (PP.myRun.spill_complete == false)
            //                    {

            //                        PP.myRun.RecordSpill();
            //                        if (PP.myRun.OneSpill)
            //                        {
            //                            //display it
            //                            PP.myRun.DeactivateRun();
            //                        }
            //                        else
            //                        {


            //                        }
            //                        PP.myRun.total_trig[0] += spill_trig_num[0];
            //                        PP.myRun.total_trig[1] += spill_trig_num[1];
            //                        PP.myRun.total_trig[2] += spill_trig_num[2];


            //                        DispSpill = PP.myRun.Spills.Last();
            //                        for (LinkedListNode<Mu2e_Event> it = DispSpill.SpillEvents.First; it != null; it = it.Next)
            //                        {
            //                            DispEvent = it.Value;

            //                            Mu2e_Ch[] cha = DispEvent.ChanData.ToArray();
            //                            double[] y = new double[cha[0].data.Count() - 1];
            //                            for (int i = 0; i < 4; i++)
            //                            {
            //                                double ped = cha[i].data.Skip(1).Take(50).Average();
            //                                double maxADC = cha[i].data.Max() - ped;
            //                                PP.myRun.max_adc[i] += maxADC;
            //                            }

            //                        }

            //                        for (int i = 0; i < 4; i++)
            //                        {
            //                            double meanADC = PP.myRun.max_adc[i] / PP.myRun.total_trig[0];
            //                            //Console.WriteLine(i.ToString() + " " + meanADC.ToString() + " " + PP.myRun.total_trig[0]);
            //                        }
            //                        string _lblmaxadc0 = string.Format("{0:N2}", PP.myRun.max_adc[0] / PP.myRun.total_trig[0]);
            //                        lblMaxADC0.Text = _lblmaxadc0;

            //                        string _lblmaxadc1 = string.Format("{0:N2}", PP.myRun.max_adc[1] / PP.myRun.total_trig[0]);
            //                        lblMaxADC1.Text = _lblmaxadc1;

            //                        string _lblmaxadc2 = string.Format("{0:N2}", PP.myRun.max_adc[2] / PP.myRun.total_trig[0]);
            //                        lblMaxADC2.Text = _lblmaxadc2;

            //                        string _lblmaxadc3 = string.Format("{0:N2}", PP.myRun.max_adc[3] / PP.myRun.total_trig[0]);
            //                        lblMaxADC3.Text = _lblmaxadc3;

            //                    }
            //                    else
            //                    {
            //                        if ((btnDisplaySpill.Text == "DONE") && (since_last_spill.Seconds > 8))
            //                        {
            //                            SpillTimer.Enabled = false;
            //                            this.BtnDisplaySpill_Click(null, null);
            //                            for (int i = 0; i < 10; i++)
            //                            {
            //                                Application.DoEvents();
            //                                System.Threading.Thread.Sleep(50);
            //                            }
            //                            this.BtnDisplaySpill_Click(null, null);
            //                            SpillTimer.Enabled = true;
            //                        }
            //                    }
            //                }

            //            }
            //        }
            //    }
            //    catch (Exception ex) { SpillTimer.Enabled = false; if (PP.glbDebug) { Console.WriteLine("timer off " + ex.Message); } }

            //    if (PP.myRun != null) //update log
            //    {
            //        lblRunName.Text = "Run_" + PP.myRun.num.ToString();
            //        while(PP.myRun.RunStatus.Count > 0)
            //            runLog.AppendText(PP.myRun.RunStatus.Dequeue() + "\r\n");
            //        //runLog/*lblRunLog*/.Text = "";
            //        //string[] all_m = PP.myRun.RunStatus.ToArray<string>();
            //        //int l = all_m.Length;
            //        //if (PP.myRun.RunStatus.Count > 13)
            //        //{
            //        //    for (int i = l - 13; i < l; i++)
            //        //    { runLog/*lblRunLog*/.Text += all_m[i] + "\r\n"; }
            //        //}
            //        //else
            //        //{
            //        //    for (int i = 0; i < all_m.Length; i++)
            //        //    { runLog/*lblRunLog*/.Text += all_m[i] + "\r\n"; }
            //        //}


            //        if (PP.myRun.fake)
            //        {
            //            if ((PP.FEB1.ClientOpen) && (chkFEB1.Checked))
            //            {

            //                PP.FEB1.CheckStatus(out uint spill_status, out uint spill_num, out uint trig_num);
            //                if (spill_status > 2) { in_spill = true; } else { in_spill = false; }
            //                lblSpillFEB1.Text = spill_status.ToString();
            //                lblFEB1TrigNum.Text = trig_num.ToString();
            //                spill_trig_num[0] = (int)trig_num;
            //                if (in_spill)
            //                {
            //                    if (PP.myRun != null)
            //                    {
            //                        PP.myRun.timeLastSpill = DateTime.Now;
            //                        PP.myRun.UpdateStatus("Detected spill. Run file is " + PP.myRun.OutFileName);
            //                        PP.myRun.spill_complete = false;
            //                    }
            //                }

            //            }
            //        }

            //        else //if (!PP.myRun.fake)
            //        {
            //            if (PP.FEB1.ClientOpen)
            //            {
            //                PP.FEB1.CheckStatus(out uint spill_status, out uint spill_num, out uint trig_num);
            //                lblSpillFEB1.Text = spill_status.ToString();
            //                lblFEB1TrigNum.Text = trig_num.ToString();
            //                spill_trig_num[0] = (int)trig_num;
            //            }

            //            if (PP.FEB2.ClientOpen)
            //            {
            //                PP.FEB2.CheckStatus(out uint spill_status, out uint spill_num, out uint trig_num);
            //                lblSpillFEB2.Text = spill_status.ToString();
            //                lblFEB2TrigNum.Text = trig_num.ToString();
            //                spill_trig_num[1] = (int)trig_num;
            //            }

            //            if (PP.WC.ClientOpen)
            //            {

            //                WC_client.check_status(out in_spill, out string num_trig, out string mytime);
            //                lblSpillWC.Text = in_spill.ToString();
            //                //spill_trig_num[2] = Convert.ToInt32(num_trig);

            //                if (in_spill)
            //                {
            //                    if (PP.myRun != null)
            //                    {
            //                        PP.myRun.timeLastSpill = DateTime.Now;
            //                        PP.myRun.UpdateStatus("Detected spill. Run file is " + PP.myRun.OutFileName);
            //                        PP.myRun.spill_complete = false;
            //                    }
            //                }


            //                lblWCTrigNum.Text = num_trig;
            //            }
            //        }
            //    }
            //}
            #endregion OldCode
        }

        private void BtnPrepare_Click(object sender, EventArgs e)
        {
            if (SpillTimer.Enabled) //Stop the timer if it was already running
                SpillTimer.Enabled = false;

            if (PP.myRun != null) //If a run already exists, orphan it so it gets garbage collected
                PP.myRun = null;

            if ((PP.FEB1.ClientOpen && chkFEB1.Checked) && (PP.FEB2.ClientOpen && chkFEB2.Checked) && (PP.WC.ClientOpen && chkWC.Checked))
            {
                WC_client.check_status(out bool inspill, out string num_trig, out string time);
                while (inspill) //in case we started prep while we are in a spill
                {
                    System.Threading.Thread.Sleep(250);
                    WC_client.check_status(out inspill, out num_trig, out time);
                    Application.DoEvents();
                }

                PP.myRun = new Run();

                waiting_for_data = false;
                first_spill_taken = false;
                spill_num = 0;
                for(int i = 0; i < 3; i++)
                    spill_trig_num[i] = 0;
                lblFEB1Spill.Text = "0";
                lblFEB1_TotTrig.Text = "0";
                lblFEB2_TotTrig.Text = "0";
                lblWC_TotTrig.Text = "0";
                lblFEB1TrigNum.Text = "0";
                lblFEB2TrigNum.Text = "0";
                lblWCTrigNum.Text = "0";
                lblRunTime.Text = "0";
                lblSpillTime.Text = "0";
                lblRunName.Text = PP.myRun.run_name;
                PP.FEB1.GetReady(); //Prep the FEB
                PP.FEB2.GetReady(); //Prep the FEB

                SpillTimer.Enabled = true;
            }
            else
            { SpillTimer.Enabled = false; PP.myRun = null; MessageBox.Show("Are all clients open & checked?");  }


            // --------------------------------------
            // ~         Old Code Below             ~
            // --------------------------------------
            #region OldCode
            //timer1.Enabled = false;
            //PP.myRun = new Run();
            //if (chkFakeIt.Checked) { PP.myRun.fake = true; }
            //else { PP.myRun.fake = false; }

            //if (PP.myRun.fake == false)
            //#region notfake
            //{
            //    WC_client.DisableTrig();
            //    PP.myRun.UpdateStatus("waiting for spill to disable WC");
            //    if (!PP.WC.in_spill) { WC_client.FakeSpill(); }
            //    int spill_timeout = 0;
            //    int big_count = 0;
            //    bool inspill = false;
            //    string X = "";
            //    string Y = "";
            //    lblRunTime.Text = "not running";
            //    PP.myRun.ACTIVE = false;
            //    while (!inspill)
            //    {
            //        if (PP.glbDebug) { Console.WriteLine("waiting for spill"); }
            //        System.Threading.Thread.Sleep(200);
            //        Application.DoEvents();
            //        WC_client.check_status(out inspill, out X, out Y);
            //        spill_timeout++;
            //        if (spill_timeout > 500) { WC_client.FakeSpill(); spill_timeout = 0; big_count++; }
            //        if (big_count > 3) { MessageBox.Show("can't get a spill..."); return; }
            //    }
            //    PP.myRun.UpdateStatus("in spill....");
            //    while (PP.WC.in_spill)
            //    {
            //        if (PP.glbDebug) { Console.WriteLine("waiting for spill to end"); }
            //        System.Threading.Thread.Sleep(200);
            //        WC_client.check_status(out inspill, out X, out Y);
            //        Application.DoEvents();
            //    }
            //    PP.myRun.UpdateStatus("Prepairing FEB1 and FEB2");
            //    //            PP.FEB1.GetReady();
            //    //            PP.FEB2.GetReady();
            //    PP.myRun.UpdateStatus("Arming WC");
            //    if (!PP.WC.in_spill) { WC_client.EnableTrig(); }
            //    timer1.Enabled = true;
            //}
            //#endregion notfake
            //else
            //{
            //    PP.myRun.UpdateStatus("Fake Run- sending spills to FEB1");
            //    lblRunName.Text = PP.myRun.run_name;
            //    timer1.Enabled = true;
            //}
            #endregion OldCode       
        }

        private void BtnStartRun_Click(object sender, EventArgs e)
        {
            if (PP.myRun != null)
            {
                waiting_for_data = false;
                first_spill_taken = false;
                runStart = DateTime.Now;
                PP.myRun.SaveAscii = saveAsciiBox.Checked;
                PP.myRun.UpdateStatus("RUN STARTING");
                PP.myRun.ActivateRun();
                try
                {
                    using (StreamWriter stabStream = new StreamWriter(PP.myRun.OutFileName, true))
                    {
                        byte[] buff;
                        byte[] stab = PP.GetBytes("stab 2\r\n"); //Get the info for the first two FPGAs
                        System.Net.Sockets.Socket[] sockets = { PP.FEB1.TNETSocket, PP.FEB2.TNETSocket };

                        foreach (System.Net.Sockets.Socket socket in sockets)
                        {
                            if (socket.Available > 0)
                            {
                                buff = new byte[socket.Available];
                                socket.Receive(buff);
                            }
                            socket.Send(stab);
                            System.Threading.Thread.Sleep(10);
                            buff = new byte[socket.Available];
                            socket.Receive(buff);
                            stabStream.WriteLine(Encoding.ASCII.GetString(buff));
                            
                            //foreach (byte b in buff)
                            //{
                            //    stabStream.Write(b.ToString());
                            //    stabStream.Write(" ");
                            //}
                            //stabStream.WriteLine();
                        }
                    }
                }
                catch { PP.myRun.UpdateStatus("Trouble saving FEB settings to file!"); }

                //if (saveAsciiBox.Checked)
                //    PP.myRun.SaveAscii = true;
                //else
                //    PP.myRun.SaveAscii = false;
                //PP.myRun.ActivateRun(); 
                //PP.myRun.OneSpill = false;
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
                waiting_for_data = false;
                first_spill_taken = false;
                PP.myRun.UpdateStatus("RUN STOPPING");
                PP.myRun.DeactivateRun();
                //timer1.Enabled = false;
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
            string hName = "c:\\data\\";
            if (btnDebugLogging.Text.Contains("START LOG"))
            {
                //hName += "FEB" + _ActiveFEB + "_commands_";
                hName += "FEB_commands_";
                hName += "_" + DateTime.Now.Year.ToString("0000");
                hName += DateTime.Now.Month.ToString("00");
                hName += DateTime.Now.Day.ToString("00");
                hName += "_" + DateTime.Now.Hour.ToString("00");
                hName += DateTime.Now.Minute.ToString("00");
                hName += DateTime.Now.Second.ToString("00");
                hName += ".txt";
                if (console.SetLogFile(hName))
                {
                    btnDebugLogging.Text = "STOP LOG";
                    console.LogSave = true;
                }

            }
            else if (btnDebugLogging.Text.Contains("STOP LOG"))
            {
                btnDebugLogging.Text = "START LOG";
                console.LogSave = false;
            }
        }

        private void BtnTimerFix_Click(object sender, EventArgs e)
        {
            //SpillTimer.Enabled = false;

            ////PP.FEB1.client.Close();
            ////Application.DoEvents();
            ////PP.FEB1.Open();
            ////Application.DoEvents();
            //Mu2e_Register csr = new Mu2e_Register();
            //Mu2e_Register.FindName("CONTROL_STATUS", Convert.ToUInt16(udFPGA.Value), ref PP.FEB1.arrReg, out csr);
            //for (int i = 0; i < 4; i++)
            //{
            //    csr.fpga_index = (UInt16)(i);
            //    Mu2e_Register.WriteReg(0xfc, ref csr, ref PP.FEB1.client);
            //    System.Threading.Thread.Sleep(10);
            //}
            //if (SpillTimer.Enabled) { }
            //else { SpillTimer.Enabled = true; }
        }

        private void ChkLast_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void FrmMain_Load(object sender, EventArgs e)
        {
            CreateButtonArrays();
            InitializeModuleQATab();
        }

        private void CreateButtonArrays()
        {
            //To-do: put these buttons into a TableLayoutPanel for easy organization as well as adding/subtracting buttons
            #region DiCounterQAButtons
            qaDiButtons = new System.Windows.Forms.RadioButton[12]; //Create an array of new buttons, originally 8, but extra 4 for crystals,
            for (int btni = 0; btni < qaDiButtons.Length; btni++)
            {
                qaDiButtons[btni] = new System.Windows.Forms.RadioButton
                {
                    Appearance = System.Windows.Forms.Appearance.Button,
                    AutoCheck = false,
                    BackColor = System.Drawing.Color.Green,
                    Location = new System.Drawing.Point(385 + ((btni % 4) * 35), 17 + ((btni / 4) * 25)),
                    Name = "qaDiButton" + btni,
                    Size = new System.Drawing.Size(35, 25),
                    TabIndex = 109 + btni,
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
            for (int cmb = 0; cmb < 16; cmb++)
            {
                lightCMBlabels[cmb] = new System.Windows.Forms.Label
                {
                    AutoSize = false,
                    BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle,
                    Name = "lightCMB" + cmb,
                    Size = new System.Drawing.Size(30, 20),
                    TabIndex = 182 + cmb,
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
                        TabIndex = 240 + fpga,
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
                TableLayoutPanel fpga_panel = lightCheckGroupFPGAs[fpga].Controls.OfType<TableLayoutPanel>().First(); //Get the panel from the group box (use First() to get actual panel, since there is only one here)
                fpga_panel.Controls.Add(lightButtons[btni], btni % 4, (btni % 16) / 4); //Add the button to the panel inside the FPGA's group box
                if (((btni + 1) % 4) == 0)//At the end of filling each row with buttons, add a label
                    fpga_panel.Controls.Add(lightCMBlabels[btni / 4], 5, (btni % 16) / 4);
                if (((btni + 1) % 16) == 0)//At the end of filling the panel with buttons and cmb labels, add the FPGA 
                    fpga_panel.Controls.Add(qaFPGALabels[fpga], 6, 1);//Add the FPGA label to the layout table
            }
            #endregion LightCheckButtons
        }

        private void InitializeModuleQATab()
        {
            //Create labels for the Module QA Tab
            ModuleQALabels = new System.Windows.Forms.Label[2][]; //One collection of 64 labels for each FEB
            for (uint feb = 0; feb < 2; feb++)
            {
                ModuleQALabels[feb] = new System.Windows.Forms.Label[64]; //64 channels on an FEB
                for (uint channel = 0; channel < 64; channel++)
                {
                    ModuleQALabels[feb][channel] = new System.Windows.Forms.Label
                    {
                        Name = "moduleQAChannelLbl_FEB" + (feb).ToString() + "_ch" + (channel).ToString(),
                        Text = (channel).ToString() + ":\n",
                        Margin = new System.Windows.Forms.Padding(0, 3, 0, 3),
                        TextAlign = System.Drawing.ContentAlignment.MiddleLeft,
                        Dock = System.Windows.Forms.DockStyle.Fill
                    };
                    switch (feb) //Put the info into the table
                    {
                        case 0:
                            ModuleQATableFEB1.Controls.Add(ModuleQALabels[feb][channel]);
                            break;
                        case 1:
                            ModuleQATableFEB2.Controls.Add(ModuleQALabels[feb][channel]);
                            break;
                        default:
                            break;
                    };
                }
            }

            portList = SerialPort.GetPortNames();
            comPortBox.DataSource = portList;
        }

        private void UpdateModuleLabel(int feb, int channel, double value)
        {
            ModuleQALabels[feb][channel].Text = (channel).ToString() + ":\n";
            ModuleQALabels[feb][channel].Text += (value).ToString("F4");
            ModuleQALabels[feb][channel].Refresh();
        }

        private void GroupBoxEvDisplay_Enter(object sender, EventArgs e)
        {

        }

        private void QaStartButton_Click(object sender, EventArgs e)
        {
            if (PP.FEB1.client != null && qaDiCounterMeasurementTimer.Enabled == false) //If the FEB is connected and we aren't currently taking measurements, then proceed.
            {
                //qaStartButton.Enabled = false;  //prevents multiple clicks of the buttons
                autoThreshBtn.Enabled = false;
                lightCheckResetThresh.Enabled = false;
                lightCheckBtn.Enabled = false;
                string[] chanOuts = new string[qaDiButtons.Length];
                autoDataProgress.Maximum = qaDiButtons.Length; //set the max of the progress bar

                if (PP.qaDicounterMeasurements == null)
                    PP.qaDicounterMeasurements = new CurrentMeasurements(PP.FEB1, "C:\\Users\\Boi\\Desktop\\DiCounterQA_Test.txt");
                else
                    PP.qaDicounterMeasurements.Purge();

                foreach (var btn in qaDiButtons) { if (!btn.Checked) { btn.BackColor = Color.Green; btn.Update(); } } //Reset all active channel indicators to green

                PP.FEB1.SetV(Convert.ToDouble(qaBias.Text)); //Turn on the bias

                currentChannel = 0; //Set the current channel being measured to 0
                dicounterNumberTextBox.Enabled = false;
                qaDiCounterMeasurementTimer.Enabled = true;

                ////Data are written to the Google Drive, CRV Fabrication Documents folder ScanningData, subfolder DicounterQA
                ////'using' will ensure the writer is closed/destroyed if the scope of the structure is left due to code-completion or a thrown exception
                //using (StreamWriter writer = File.AppendText("C:\\Users\\Boi\\Desktop\\ScanningData_test.txt"))//"C:\\Users\\FEB-Laptop-1\\Google Drive\\CRV Fabrication Documents\\Data\\QA\\Dicounter Source Testing\\ScanningData_" + qaOutputFileName.Text + ".txt")) //The output file
                //{
                //    writer.Write("{0}\t", numTextBox.Text); //Write dicounter number to file

                //    //Write temp to file
                //    double[] temp = { 0, 0, 0, 0 };
                //    for (int numTries = 0; numTries < 10; numTries++) //Try and read the temperature 10 times
                //        temp = PP.FEB1.ReadTempFPGA();                    //read the temperatures on FPGA 0
                //    writer.Write("{0}\t", temp[0].ToString("0.00"));  //Write the temperature, in degrees C, as measured by the first CMB

                //    //Write date to file
                //    writer.Write("{0}\t", DateTime.Now.ToString("MM/dd/yy HH:mm\t")); //Changed to 24 hour time format to match database storage format

                //    PP.FEB1.SetV(Convert.ToDouble(qaBias.Text)); //Turn on bias for the first FPGA (since this is the only one used for dicounter QA)

                //    foreach (var btn in qaDiButtons) { if (!btn.Checked) { btn.BackColor = Color.Green; btn.Update(); } } //Reset all active channel indicators to green

                //    foreach (var btn in qaDiButtons)
                //    {
                //        double averageCurrent = 0;
                //        int channel = Convert.ToInt16(btn.Name.Substring(10)); //Gets the channel number from the name "qaDiButton##"
                //        if (!btn.Checked)
                //        {
                //            for (int measI = 0; measI < Convert.ToInt16(qaDiNumAvg.Value); measI++)
                //                averageCurrent += Convert.ToDouble(PP.FEB1.ReadA0(0, channel)); //read the current for the specified channel on FPGA 0
                //            averageCurrent /= Convert.ToDouble(qaDiNumAvg.Value);
                //            if (averageCurrent < Convert.ToDouble(qaDiIWarningThresh.Text)) //if the current was less than warning thresh && we still have current, update the color of the lamp
                //                qaDiButtons[channel].BackColor = Color.Red;
                //            if (averageCurrent < 0.025) //if there was no current then
                //                qaDiButtons[channel].BackColor = Color.Blue; //set indicator color to blue: "cold-no-current"
                //            qaDiButtons[channel].Update();
                //        }

                //        chanOuts[channel] = averageCurrent.ToString("0.0000");
                //        Console.WriteLine("Channel {0}: {1}", channel, chanOuts[channel]);
                //        writer.Write("{0}\t", chanOuts[channel]); //write out the current for the channel
                //        autoDataProgress.Increment(1);
                //    }
                //    writer.WriteLine(); //write a 'return' to file

                //    PP.FEB1.SetV(0.0); //Turn off the bias
                //    autoDataProgress.Value = 0; //Set the progress bar back to 0
                //    autoDataProgress.Update();
                //}

                //qaStartButton.Enabled = true;
                //autoThreshBtn.Enabled = true;
                //lightCheckResetThresh.Enabled = true;
                //lightCheckBtn.Enabled = true;
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
                using (StreamWriter writer = File.AppendText("C:\\Users\\Boi\\Desktop\\Module.txt"))// "C:\\Users\\FEB-Laptop-1\\Google Drive\\CRV Fabrication Documents\\Data\\QA\\Module Light Leak Testing\\LightCheck.txt")) //Path for file output of lightcheck
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
                    for (int btn = 0; btn < lightButtons.Length; btn++)
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
                        //Console.WriteLine("Sum: " + total_avg_I);
                        //if (total_avg_I >= 0.1)
                        //{
                        //    int signal = 0;
                        //    if (total_avg_I / .1 < 1)
                        //    {
                        //        signal = 1;
                        //    }
                        //    else if (total_avg_I / .1 > 20)
                        //    {
                        //        signal = 20;
                        //    }
                        //    else
                        //    {
                        //        signal = (int)(total_avg_I / .1);
                        //    }
                        //    Console.WriteLine("Signal: " + signal);
                        //    //System.Windows.Media.MediaPlayer[] players = new System.Windows.Media.MediaPlayer[signal];
                        //    for (int m = 0; m < signal; m++)
                        //    {
                        //        new System.Threading.Thread(() =>
                        //        {
                        //            var c = new System.Windows.Media.MediaPlayer();
                        //            c.Open(new System.Uri(@"C:\Users\FEB-Laptop-1\Desktop\beep-07.wav"));
                        //            c.Play();
                        //        }).Start();
                        //        //players[m] = new System.Windows.Media.MediaPlayer();
                        //        //players[m].Open(new System.Uri(@"C:\Users\FEB-Laptop-1\Desktop\chimes.wav"));
                        //        //players[m].Play();
                        //        System.Threading.Thread.Sleep(100);
                        //        //                        System.Media.SystemSounds.Beep.Play();

                        //    }
                        //}
                        lightAutoThreshProgress.Increment(1);
                    }


                    PP.FEB1.SetVAll(0.0);
                    txtI.Text = PP.FEB1.ReadA0((int)udFPGA.Value, (int)udChan.Value).ToString("0.0000");


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

        private void LightCMBLabel_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.Label lbl = (System.Windows.Forms.Label)sender; //Get the label that was clicked
            int cmb = Convert.ToInt16(lbl.Text); //Convert to CMB number index starting at 0
            for (int btn = (cmb * 4); btn < (cmb * 4) + 4; btn++) //Set loop bounds and 'click' all of the buttons
            {
                sender = lightButtons[btn];
                LightButton_Click(sender, e);
            }
        }

        private void LightFPGALabel_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.Label lbl = (System.Windows.Forms.Label)sender;
            int fpga_num = Convert.ToInt16(lbl.Text.Substring(lbl.Text.Length - 1, 1));
            for (int btni = fpga_num * 16; btni < (fpga_num * 16) + 16; btni++)
            {
                sender = lightButtons[btni];
                LightButton_Click(sender, e);
            }
        }

        private void CmbTestBtn_Click(object sender, EventArgs e)
        {
            double fitThresh = 7.5;
            double flashGateDifferenceThresh = 20;
            double ledDifferenceThresh = 50; //Initial testing value
            uint ledFlasherIntensity = 0x400; //3.5V
            double maxUndershootThresh = 30; //Value is in ADC
            //Check that an FEB client exists, otherwise, don't bother setting up the pulser or trying to get data
            if (PP.FEB1.client != null)
            {
                cmbInfoBox.Text = ""; cmbInfoBox.Refresh();

                #region Registers
                //These registers are required for setting up the FEB to record triggered events and the flashing of the LED.
                //The FEB is responsible for controlling the pulser via the FEB's GPO port into the pulser's external clock port
                Mu2e_Register.FindAddr(0x303, ref PP.FEB1.arrReg, out Mu2e_Register trigControlReg); //Trigger control register
                Mu2e_Register.FindAddr(0x304, ref PP.FEB1.arrReg, out Mu2e_Register hitPipelineDelayReg); //Hit Pipeline Delay register
                Mu2e_Register.FindAddr(0x305, ref PP.FEB1.arrReg, out Mu2e_Register sampleLengthReg); //Sample length for each event/trigger
                Mu2e_Register.FindAddr(0x307, ref PP.FEB1.arrReg, out Mu2e_Register testPulseFreqReg); //Onboard test pulser frequency
                Mu2e_Register.FindAddr(0x308, ref PP.FEB1.arrReg, out Mu2e_Register spillDurReg); //Spill duration register
                Mu2e_Register.FindAddr(0x309, ref PP.FEB1.arrReg, out Mu2e_Register interSpillDurReg); //Interspill duration register
                Mu2e_Register.WriteReg(0x0, ref testPulseFreqReg, ref PP.FEB1.client); //Set test pulser frequency to zero, this allows external triggering from LEMO
                Mu2e_Register.WriteReg(0xA, ref interSpillDurReg, ref PP.FEB1.client); //Set the interspill duration for 10 seconds

                Mu2e_Register[] muxReg = Mu2e_Register.FindAllAddr(0x20, ref PP.FEB1.arrReg);//Get the mux register so it can be set to 0 so as to not interfere with histogramming
                Mu2e_Register.WriteAllReg(0x0, ref muxReg, ref PP.FEB1.client); //Set all the mux to 0 so as to not interfere with histogramming
                Mu2e_Register[] flashGateControlReg = Mu2e_Register.FindAllAddr(0x300, ref PP.FEB1.arrReg); //Flash Gate Control registers
                Mu2e_Register.WriteAllReg(0x2, ref flashGateControlReg, ref PP.FEB1.client); //Set the CMB Pulse routing to the Flash Gate (to LED flasher will create interference on CMB)
                Mu2e_Register[] controlStatusReg = Mu2e_Register.FindAllAddr(0x00, ref PP.FEB1.arrReg);
                Mu2e_Register.WriteAllReg(0x20, ref controlStatusReg, ref PP.FEB1.client); //issue a general reset for each FPGA
                Mu2e_Register[][] gainControlReg = Mu2e_Register.FindAllAddrRange(0x46, 0x47, ref PP.FEB1.arrReg);
                Mu2e_Register.WriteAllRegRange(0x300, ref gainControlReg, ref PP.FEB1.client); //Set the gain of all AFE chips on all FPGAs to the same value
                #endregion Registers

                System.Net.Sockets.Socket febSocket = PP.FEB1.TNETSocket_prop; //Declare and define FEB socket variable
                febSocket.ReceiveTimeout = 500; //Set timeout on FEB socket to 500ms
                byte[] sendRDB = Encoding.ASCII.GetBytes("rdb\r\n"); //PP.GetBytes("rdb\r\n"); //Lazy way for converting RDB command into packet to send to FEB
                if (febSocket.Available > 0) //Pick up and discard whatever the board may currently be trying to send until we are ready for "fresh data"
                {
                    byte[] junk = new byte[febSocket.Available];
                    febSocket.Receive(junk);
                }

                #region Read CMB Temps and IDs
                CMB[] cmbs = new CMB[16]; //make a list of CMBs
                if (cmbInfoLabels == null) //If this is the first time the labels will be displayed, create an array of labels
                    cmbInfoLabels = new System.Windows.Forms.Label[16][];
                PP.FEB1.SendStr("cmb"); //Ask the FEB about the CMBs
                System.Threading.Thread.Sleep(100); //Wait for the FEB to get the message and respond
                PP.FEB1.ReadStr(out string cmbMsg, out int r);
                if (cmbMsg.Length > 400)//If it got 'all' the info
                {
                    try
                    {
                        string[] tok = cmbMsg.Split(new string[] { " ", Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

                        if (String.Equals(tok[1], "DegC")) //Preproduction FEB 'cmb' format
                        {
                            for (int cmb = 0; cmb < 16; cmb++)
                            {
                                cmbs[cmb].num = cmb;
                                cmbs[cmb].temp = Convert.ToDouble(tok[(cmb * 3) + 5]); //Starting at index 5, every 3rd string is the cmb temperature
                                cmbs[cmb].rom_id = tok[(cmb * 3) + 6]; //Starting at index 6, every 3rd string is the cmb ROM_ID
                                if (cmbs[cmb].temp == 0 || cmbs[cmb].rom_id == "0")
                                {
                                    cmbs[cmb].flagged = true; //Flag the CMB if the temp or ROM_ID couldn't be read
                                    cmbs[cmb].failureType = (int)CMB.Failure.TempRom;
                                }
                                else
                                    cmbs[cmb].flagged = false;
                            }
                        }
                        else if (String.Equals(tok[1], "Cnts_TEMP_DegC")) //Prototype FEB 'cmb' format
                        {
                            for (int cmb = 0; cmb < 16; cmb++)
                            {
                                cmbs[cmb].num = cmb;
                                cmbs[cmb].temp = Convert.ToDouble(tok[(cmb * 4) + 5]); //Starting at index 5, every 4th string is the cmb temperature
                                cmbs[cmb].rom_id = tok[(cmb * 4) + 6]; //Starting at index 6, every 4th string is the cmb ROM_ID
                                if (cmbs[cmb].temp == 0 || cmbs[cmb].rom_id == "0")
                                {
                                    cmbs[cmb].flagged = true; //Flag the CMB if the temp or ROM_ID couldn't be read
                                    cmbs[cmb].failureType = (int)CMB.Failure.TempRom;
                                }
                                else
                                    cmbs[cmb].flagged = false;
                            }
                        }
                        else
                        {
                            for (int cmb = 0; cmb < 16; cmb++)
                            {
                                cmbs[cmb].flagged = true;
                            }

                        } //unknown format
                    }
                    catch //Catch errors
                    {
                        for (int cmb = 0; cmb < 16; cmb++)
                            cmbs[cmb].flagged = true;
                    }
                }

                foreach (CMB cmb in cmbs)
                {
                    if (cmbInfoLabels[cmb.num] == null) //If this is the first time the labels will be displayed, make new labels
                    {
                        cmbInfoLabels[cmb.num] = new System.Windows.Forms.Label[12];
                        cmbInfoLabels[cmb.num][0] = new System.Windows.Forms.Label
                        {
                            Name = "cmbnum" + cmb.num,
                            Text = (cmb.num + 1).ToString(),
                            Margin = new System.Windows.Forms.Padding(0, 3, 0, 3),
                            TextAlign = System.Drawing.ContentAlignment.MiddleCenter,
                            Dock = System.Windows.Forms.DockStyle.Fill
                        };
                        cmbInfoLabels[cmb.num][1] = new System.Windows.Forms.Label
                        {
                            Name = "cmbtemp" + cmb.num,
                            Text = cmb.temp.ToString(),
                            Margin = new System.Windows.Forms.Padding(0, 3, 0, 3),
                            TextAlign = System.Drawing.ContentAlignment.MiddleCenter,
                            Dock = System.Windows.Forms.DockStyle.Fill
                        };
                        cmbInfoLabels[cmb.num][2] = new System.Windows.Forms.Label
                        {
                            Name = "cmbromid" + cmb.num,
                            Text = cmb.rom_id,
                            Margin = new System.Windows.Forms.Padding(0, 3, 0, 3),
                            TextAlign = System.Drawing.ContentAlignment.MiddleCenter,
                            Dock = System.Windows.Forms.DockStyle.Fill
                        };
                        for (int i = 3; i < 12; i++)//initialize the remaining labels
                        {
                            cmbInfoLabels[cmb.num][i] = new System.Windows.Forms.Label
                            {
                                Name = "cmbinfo" + i + "_" + cmb.num,
                                Text = "",
                                Margin = new System.Windows.Forms.Padding(0, 3, 0, 3),
                                TextAlign = System.Drawing.ContentAlignment.MiddleCenter,
                                Dock = System.Windows.Forms.DockStyle.Fill
                            };
                        }
                        for (int i = 0; i < 12; i++)
                            cmbDataTable.Controls.Add(cmbInfoLabels[cmb.num][i], i, cmb.num); //Put the info into the table

                    }
                    else //Just update the labels
                    {
                        ClearCMBInfoLabel(cmb.num);
                        cmbInfoLabels[cmb.num][0].Text = (cmb.num + 1).ToString();
                        cmbInfoLabels[cmb.num][1].Text = cmb.temp.ToString();
                        cmbInfoLabels[cmb.num][2].Text = cmb.rom_id;
                    }

                    if (cmb.flagged) //Flag the CMB if the temp or ROM_ID couldn't be read
                    {
                        cmbInfoLabels[cmb.num][11].Text = cmb.FailType();
                        SetRowColor(cmb.num, Color.MistyRose);
                    }
                    else
                        SetRowColor(cmb.num, SystemColors.Control);

                    UpdateCMBInfoLabel(cmb.num);
                }

                #endregion Read CMB Temps and IDs

                //Open up the file used to store/read channel responses
                //Stores in single line per channel format, channel# AverageADCresponse
                #region ReadFileAvgs
                String cmbAvgFileName = "D:\\data\\cmb_tester_data\\cmb_channel_averages.root";
                ROOTNET.NTH1I[] channelAvgHist = new ROOTNET.NTH1I[64];
                ROOTNET.NTFile cmbAvgsFile;
                double[] avgResp = new double[64];

                if (File.Exists(cmbAvgFileName))
                {
                    String fileOptn = "";
                    if (updateFilesChkBox.Checked) //If the file is to be updated, change the open option to "UPDATE" instead of just "READ" (preserves existing data)
                        fileOptn = "UPDATE";
                    else
                        fileOptn = "READ";
                    cmbAvgsFile = new ROOTNET.NTFile(cmbAvgFileName, fileOptn); //update file prevents destorying an already existing file
                    for (int channel = 0; channel < 64; channel++)
                    {
                        channelAvgHist[channel] = (ROOTNET.NTH1I)cmbAvgsFile.Get("Chan" + channel.ToString()); //convert the average recorded value for each channel into a usable number
                        if (channelAvgHist[channel] != null)
                        {
                            avgResp[channel] = channelAvgHist[channel].GetMean();
                            channelAvgHist[channel].Fill(450);
                        }
                    }
                }
                else //If the channel averages file does not exist, then print out an error and return, we need the average response to make comparisons
                {
                    System.Console.WriteLine("ERR: Could not find " + cmbAvgFileName + "!");
                    System.Console.WriteLine("Does the file exist and is it accessible?");
                    return;
                }
                #endregion ReadFileAvgs

                #region Output histograms
                var histo_file = ROOTNET.NTFile.Open("D:/Resp_Calib_BACKUP_" + System.DateTime.Now.ToFileTime().ToString() + ".root", "RECREATE");
                bool outputOpened = false;
                if (histo_file != null)
                    outputOpened = true;
                #endregion Output histograms


                #region LED Response Evaluation and Gain/Pedestal Computation

                #region BiasWait
                cmbInfoBox.Text = "Waiting for bias"; cmbInfoBox.Refresh();
                PP.FEB1.SetVAll(Convert.ToDouble(cmbBias.Text));
                #endregion BiasWait

                double[] pedestals = new double[64]; //pedestal for each channel
                double[] gains = new double[64]; //gain for each channel (adc/pe)      

                ROOTNET.NTH1I[] peHistos = new ROOTNET.NTH1I[64]; //Histograms used to determine response to LED and gains
                ROOTNET.NTGraph[] peCalibs = new ROOTNET.NTGraph[64]; //2D Plots used to compute gains/pedestal
                ROOTNET.NTSpectrum peakFinder = new ROOTNET.NTSpectrum(20, 2); //Peak finder, set it to find a maximum of 10 peaks (pedestal + 9 peaks)
                HistoHelper hist_helper = new HistoHelper(ref PP.FEB1, 0xFFE);//0x400); //Tune later
                ROOTNET.NTH1I[] histos_temp = new ROOTNET.NTH1I[2]; //temporary spot to store the incoming histograms from the histohelper
                ROOTNET.NTF1 gainFit = new ROOTNET.NTF1("gainfit", "pol1"); //linear fit for computing the gain
                ROOTNET.NTF1 bulkRespFit = new ROOTNET.NTF1("respfit", "gaus"); //Gaussian fit for bulk of LED Response
                cmbInfoBox.Text = "LED/Calibrating"; cmbInfoBox.Refresh();

                Mu2e_Register.WriteReg(0x100, ref trigControlReg, ref PP.FEB1.client); //Enable the on-board test pulser, output of this signal will be delivered to external pulser to flash LED
                Mu2e_Register.WriteReg(0x5E5E5E/*0x5E5E5E*/, ref testPulseFreqReg, ref PP.FEB1.client); //Set the on-board test pulser's frequency to ~230kHz
                Mu2e_Register.WriteReg(0x1, ref hitPipelineDelayReg, ref PP.FEB1.client); //Set the hit pipeline delay to minimum value (12.56ns)

                for (uint channel = 0; channel < 64; channel++)
                {
                    int cmbNum = (int)channel / 4; //spans from 0-15
                    if (peHistos[channel] == null && !(cmbs[cmbNum].flagged)) //skip the channels that have already been histogrammed (due to the two channel histograms return from the FEB), and skip any channels on flagged cmbs
                    {
                        histos_temp = hist_helper.GetHistogram(channel, 1);
                        uint[] channels = { channel, Convert.ToUInt32(histos_temp[1].GetTitle()) }; //Lazily grab the other channel's label from the histogram title...
                        System.Console.WriteLine("Histo Chans: " + channels[0] + ", " + channels[1]);

                        for (int hist = 0; hist < 2; hist++)//Loop over each of the two histograms
                        {
                            if (cmbs[channels[hist] / 4].flagged) //Skip if one of the two received channels was flagged
                                continue;
                            peHistos[channels[hist]] = histos_temp[hist];
                            peCalibs[channels[hist]] = new ROOTNET.NTGraph();
                            int peaksFound = peakFinder.Search(peHistos[channels[hist]], 1.5, "nobackground", 0.00001); //Don't try and estimate background, and set the threshold to only include pedestal, 1st, and 2nd PE
                            if (peaksFound < 2)
                            {
                                System.Console.WriteLine("Cannot find 1+ PE for Chan {0}", channels[hist]);
                                cmbs[cmbNum].flagged = true;
                                cmbs[cmbNum].failureType = (int)CMB.Failure.SiPMResp;
                                cmbInfoLabels[cmbNum][11].Text = "PeakFinder";
                                SetRowColor(cmbNum, Color.MistyRose);
                                UpdateCMBInfoLabel(cmbNum);
                                continue;
                            } //Need this beacuse we need to know the gain, which is impossible if we can't see first PE
                            List<float> peakPositions = peakFinder.GetPositionX().as_array(peaksFound).ToList();
                            for (int p = 1; p < peakPositions.Count; p++)
                                if (peakPositions[p] < peakPositions[0])//if there are any peaks less than pedestal, remove them
                                    peakPositions.RemoveAt(p);
                            peakPositions.Sort(); //Should sort in ascending order by default
                            pedestals[channel] = peakPositions[0]; //first entry should be pedestal
                            List<int> fittingRanges = new List<int>();
                            if (peakPositions.Count < 2)
                            {
                                cmbs[cmbNum].flagged = true;
                                cmbs[cmbNum].failureType = (int) CMB.Failure.SiPMResp;
                                cmbInfoLabels[cmbNum][11].Text = "GainFail";
                                SetRowColor(cmbNum, Color.MistyRose);
                                UpdateCMBInfoLabel(cmbNum);
                                continue;
                            }
                            float gain_estimate_thresh = (peakPositions[1] - peakPositions[0]) * 2; //Set threshold at ~2pe difference
                            fittingRanges.Add(0);
                            for (int p = 1; p < peakPositions.Count; p++)
                            {
                                if (peakPositions[p] - peakPositions[p - 1] > gain_estimate_thresh)
                                {
                                    fittingRanges.Add(p - 1);
                                    fittingRanges.Add(p);
                                }
                            }
                            fittingRanges.Add(peakPositions.Count - 1);
                            for (int peak = 0; peak < peakPositions.Count; peak++) //This is under the assumption that peak 0 = pedestal, peak 1 = 1st PE, ...
                                peCalibs[channels[hist]].SetPoint(peak, peak, peakPositions[peak]);
                            for (int fitRange = 0; fitRange < fittingRanges.Count; fitRange += 2)
                            {
                                peCalibs[channels[hist]].Fit(gainFit, "CRQ+", "", fittingRanges[fitRange], fittingRanges[fitRange + 1]); //Fit quietly please
                                gains[channels[hist]] += gainFit.GetParameter(1); //get the slope of the line, which is the gain
                            }
                            gains[channels[hist]] /= (fittingRanges.Count / 2); //Average the gain fits
                            //peCalibs[channels[hist]].Fit(gainFit, "CRQ+", "", );
                            peCalibs[channels[hist]].SetTitle(channels[hist].ToString()); //Set some info
                            peCalibs[channels[hist]].SetName(channels[hist].ToString());
                            peCalibs[channels[hist]].GetXaxis().SetTitle("PE");
                            peCalibs[channels[hist]].GetYaxis().SetTitle("ADC");
                            peHistos[channels[hist]].Fit(bulkRespFit, "CRQ+", "", gains[channels[hist]] * fitThresh, 512); //Fit from 7.5 PE (in ADC) up to max of histogram (will need to adjust later)

                            //Display info for gain and pedestal
                            cmbInfoLabels[cmbNum][(channel % 4) * 2 + 3].Text = Math.Floor(pedestals[channel]).ToString();
                            cmbInfoLabels[cmbNum][(channel % 4) * 2 + 4].Text = Math.Floor(gains[channel]).ToString();
                            UpdateCMBInfoLabel(cmbNum);

                            //compare fit response to 'lookup value'
                            //For gaus fit: p0 is amplitude, p1 is mean, p2 is sigma
                            if (PercentDifference(bulkRespFit.GetParameter(1), avgResp[channel] /*table value*/) > 200) //if the differnce is greater than 20%
                            {
                                cmbs[cmbNum].flagged = true;
                                cmbs[cmbNum].failureType = (int)CMB.Failure.SiPMResp;
                                cmbInfoLabels[cmbNum][11].Text = cmbs[cmbNum].FailType();
                                SetRowColor(cmbNum, Color.MistyRose);
                                UpdateCMBInfoLabel(cmbNum);
                            }
                            else if (updateFilesChkBox.Checked) //Update file here
                                channelAvgHist[channel].Fill(bulkRespFit.GetParameter(1)); //Add mean response to histogram
                        }
                    }
                }

                //Write the updated response histograms
                if(updateFilesChkBox.Checked)
                {
                    cmbAvgsFile.cd();
                    for (uint channel = 0; channel < 64; channel++)
                        channelAvgHist[channel].Write("", 2); //2 = kOverwrite
                    cmbAvgsFile.Close();
                }

                histo_file.cd();
                if (outputOpened)
                {
                    foreach (var histo in peHistos)
                        if (histo != null)
                        {
                            histo.Write();
                            histo.Delete();
                        }
                    foreach (var plot in peCalibs)
                        if (plot != null)
                        {
                            plot.Write();
                            plot.Delete();
                        }
                }


                #region FlashGate
                cmbInfoBox.Text = "Testing flashgate"; cmbInfoBox.Refresh();

                ROOTNET.NTH1I[] flashHistos = new ROOTNET.NTH1I[64]; //Histograms used to determine response to LED (flashgate)

                //Set registers for flashgate (0x3 enables flash gate and sets the routing to the flash gate (not LED)
                Mu2e_Register.WriteAllReg(0x3, ref flashGateControlReg, ref PP.FEB1.client);

                for (uint channel = 0; channel < 64; channel++) //Loop over the channels, re-histogram response to LED flashing, check that response is low
                {
                    int cmbNum = (int)channel / 4; //spans from 0-15
                    if (flashHistos[channel] == null && !(cmbs[cmbNum].flagged)) //skip the channels that have already been histogrammed (due to the two channel histograms return from the FEB), and skip any channels on flagged cmbs
                    {
                        histos_temp = hist_helper.GetHistogram(channel, 1);
                        uint[] channels = { channel, Convert.ToUInt32(histos_temp[1].GetTitle()) }; //Lazily grab the other channel's label from the histogram title...
                        System.Console.WriteLine("Histo Chans: " + channels[0] + ", " + channels[1]);

                        for (int hist = 0; hist < 2; hist++)//Loop over each of the two histograms
                        {
                            if (cmbs[channels[hist] / 4].flagged) //Skip if one of the two received channels was flagged
                                continue;
                            flashHistos[channels[hist]] = histos_temp[hist];
                            //flashHistos[channels[hist]].Fit(bulkRespFit, "CRQ+", "", gains[channels[hist]] * 7.5, 512); //Fit from 7.5 PE (in ADC) up to max of histogram (will need to adjust later)
                            int lowerIntegralBin = (int) (pedestals[channels[hist]] + gains[channels[hist]] * fitThresh); //truncate the value of 7.5PE for the bin # (since all histograms start at 0 and have 512 bins)
                            double flashIntegral = flashHistos[channels[hist]].Integral(lowerIntegralBin, 512); //512 upper bound because all histograms have 512 bins
                            double ledIntegral = peHistos[channels[hist]].Integral(lowerIntegralBin, 512); //Also compute integral for led histogram, so we can see if the response to LED has diminished

                            if (PercentDifference(flashIntegral, ledIntegral) < flashGateDifferenceThresh) //if the differnce is less than flashGateDifferenceThresh, flashgate must not be working...
                            {
                                cmbs[cmbNum].flagged = true;
                                cmbs[cmbNum].failureType = (int)CMB.Failure.Flashgate;
                                cmbInfoLabels[cmbNum][11].Text = cmbs[cmbNum].FailType();
                                SetRowColor(cmbNum, Color.MistyRose);
                                UpdateCMBInfoLabel(cmbNum);
                            }
                        }
                    }
                }

                Mu2e_Register.WriteAllReg(0x2, ref flashGateControlReg, ref PP.FEB1.client); //Turn off flash gate and keep routing to the Flash Gate (to LED flasher will create interference on CMB)

                if (outputOpened)
                {
                    foreach (var histo in flashHistos)
                        if (histo != null)
                        {
                            histo.Write();
                            histo.Delete();
                        }
                }

                #endregion FlashGate

                //return;
                #endregion LED Response Evaluation and Gain/Pedestal Computation


                #region CMB LED Flashers

                cmbInfoBox.Text = "LED Flasher Eval"; cmbInfoBox.Refresh();

                //
                // TO-DO:
                //
                // [X] Set Flash gate on individual FPGAs so we can read with adjacent FPGAs
                // [X] Write logic to flash/read 0->1, 1->0, 2->3, 3->2
                // [X] Evaluate flashers (need parameter or threshold, or is determining if working/not good enough?
                // [ ] Tune LED flasher intensity to match closely to external LED flash -> could make evaluation easier (?)
                //

                Mu2e_Register[][] ledFlasherIntensityRegs = Mu2e_Register.FindAllAddrRange(0x40, 0x43, ref PP.FEB1.arrReg); //Get the led flasher intensity registers, for all FPGAs
                Mu2e_Register.WriteReg(0x500, ref trigControlReg, ref PP.FEB1.client); //Keep the on-board pulser enabled, but set the GPO to only output a single pulse at the beginning and end of the spill gate (which should not turn on while recording histograms AFAIK)

                System.Threading.Thread.Sleep(250); //wait for the pulse to go by...

                ROOTNET.NTH1I[] ledHistos = new ROOTNET.NTH1I[64]; //histograms for cmb-led flashers 

                for(uint fpga = 0; fpga < 4; fpga++)
                {
                    Mu2e_Register.WriteAllRegRange(0x0, ref ledFlasherIntensityRegs, ref PP.FEB1.client); //Set all cmb flasher intensities to 0
                    Mu2e_Register.WriteAllReg(0x2, ref flashGateControlReg, ref PP.FEB1.client); //Set all CMBs to flash gate routing

                    for (uint cmb = 0; cmb < 4; cmb++)
                    {
                        //We will run as follows: 0 reads 1, 1 reads 0, 2 reads 3, 3 reads 2
                        if (fpga == 1 || fpga == 3) //1 reads 0, 3 reads 2
                        {
                            Mu2e_Register.WriteReg(0x1, ref flashGateControlReg[fpga - 1], ref PP.FEB1.client); //Set LED pulse routing for single CMB
                            Mu2e_Register.WriteReg(ledFlasherIntensity, ref ledFlasherIntensityRegs[fpga - 1][cmb], ref PP.FEB1.client); //Set single CMB flasher intensity to ledFlasherIntensity
                        }
                        else //0 reads 1, 2 reads 3
                        {
                            Mu2e_Register.WriteReg(0x1, ref flashGateControlReg[fpga + 1], ref PP.FEB1.client); //Set LED pulse routing for single CMB
                            Mu2e_Register.WriteReg(ledFlasherIntensity, ref ledFlasherIntensityRegs[fpga+1][cmb], ref PP.FEB1.client); //Set single CMB flasher intensity to ledFlasherIntensity
                        }

                        //For only the outer channels on each CMB (ch. 0 and ch. 3), histogram the flashing of 
                        for (uint ch = 0; ch < 4; ch+=3)
                        {
                            uint cmbNum = (fpga * 4) + cmb;
                            uint channel = (fpga * 16) + (cmb * 4) + ch;
                            // [X] Get histograms
                            // [X] Evaluate flashers

                            histos_temp = hist_helper.GetHistogram(channel, 1);
                            ledHistos[channel] = histos_temp[0]; //"Discard" the second histogram, becuase it doesn't help us atm...
                            int lowerIntegralBin = (int)(pedestals[channel] + (gains[channel] * fitThresh)); //truncate the value of 7.5PE for the bin # (since all histograms start at 0 and have 512 bins)
                            double cmbLedIntegral = ledHistos[channel].Integral(lowerIntegralBin, 512); //512 upper bound because all histograms have 512 bins
                            double ledIntegral = peHistos[channel].Integral(lowerIntegralBin, 512); //Also compute integral for led histogram, so we can see if the response to LED has diminished
                            
                            if (PercentDifference(cmbLedIntegral, ledIntegral) > ledDifferenceThresh) //if the differnce is greater than flashGateDifferenceThresh, flashgate must not be working...
                            {
                                cmbs[cmbNum].flagged = true;
                                cmbs[cmbNum].failureType = (int)CMB.Failure.LED;
                                cmbInfoLabels[cmbNum][11].Text = cmbs[cmbNum].FailType();
                                SetRowColor((int)cmbNum, Color.MistyRose);
                                UpdateCMBInfoLabel((int)cmbNum);
                            }

                        }
                    }

                    Mu2e_Register.WriteAllRegRange(0x0, ref ledFlasherIntensityRegs, ref PP.FEB1.client); //Set all cmb flasher intensities to 0
                    Mu2e_Register.WriteAllReg(0x2, ref flashGateControlReg, ref PP.FEB1.client); //Set all CMBs to flash gate routing

                }

                if (outputOpened)
                {
                    foreach (var histo in ledHistos)
                        if (histo != null)
                        {
                            histo.Write();
                            histo.Delete();
                        }
                    histo_file.Close();
                }

                cmbInfoBox.Text = ""; cmbInfoBox.Refresh();

                #endregion CMB LED Flashers

                //This is used later for gathering trace data for tail-cancellation evaluation
                uint spill_status = 0;
                uint spill_num = 0;
                uint trig_num = 0;
                numTrigsDisp.Text = trig_num.ToString();
                numTrigsDisp.Refresh();


                Mu2e_Register.WriteReg(0x0, ref trigControlReg, ref PP.FEB1.client); //Disable the on-board test pulser
                Mu2e_Register.WriteReg(0x0, ref testPulseFreqReg, ref PP.FEB1.client); //Set the on-board test pulser's frequency to 0


                //Turn off bias for SiPMs
                //for (uint fpga = 0; fpga < 4; fpga++)
                //    PP.FEB1.SetV(0.0, (int)fpga);

                #region Undershoot Evaluation
                cmbInfoBox.Text = "Undershoot Evaluation"; cmbInfoBox.Refresh();

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

                //MessageBox.Show("CMB Evaluation\nPlease connect the LED");
                //Mu2e_Register.WriteReg(0x0C, ref controlStatusReg, ref PP.FEB1.client); //Issues a reset of the AFE deserializers on the FPGA and the MIG DDR interface
                Mu2e_Register.WriteReg(0x2, ref spillDurReg, ref PP.FEB1.client); //Set the spill duration for 2 seconds
                Mu2e_Register.WriteReg(0x64, ref sampleLengthReg, ref PP.FEB1.client); //Set the number of ADC samples to record per trigger
                for (uint fpga = 0; fpga < 4; fpga++) //Turn on bias for SiPMs
                    PP.FEB1.SetV(Convert.ToDouble(cmbBias.Text), (int)fpga);
                Mu2e_Register.WriteReg(0x300, ref trigControlReg, ref PP.FEB1.client); //Open the spill gate: Set trig-control register to enable board to record data for 1 spill, LED flashes during this time
                while (spill_status != 2) //trig_num < Convert.ToInt16(requestNumTrigs.Text))
                {
                    System.Threading.Thread.Sleep(250); //Slow down the polling of the FEB for triggers/status
                    PP.FEB1.CheckStatus(out spill_status, out spill_num, out trig_num); //Keep polling the board about how many triggers it has seen
                    numTrigsDisp.Text = trig_num.ToString();
                    numTrigsDisp.Refresh();
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
                    double[] averageUndershoot = new double[64]; //hold the average response for each channel

                    foreach (var tEvent in testerData.SpillEvents)
                    {
                        Mu2e_Ch[] cha = tEvent.ChanData.ToArray();
                        for (int chan = 0; chan < tEvent.ChNum; chan++)
                            averageUndershoot[chan] = 0.5 * (averageUndershoot[chan] + (pedestals[chan] - cha[chan].data.Min())); //Compute the average undershoot for each channel from all the traces
                    }

                    for(uint channel = 0; channel < 64; channel++)
                    {
                        uint cmbNum = channel / 4;
                        if (averageUndershoot[channel] > maxUndershootThresh)
                        {
                            cmbs[cmbNum].flagged = true;
                            cmbs[cmbNum].failureType = (int)CMB.Failure.LED;
                            cmbInfoLabels[cmbNum][11].Text = cmbs[cmbNum].FailType();
                            SetRowColor((int)cmbNum, Color.MistyRose);
                            UpdateCMBInfoLabel((int)cmbNum);
                        }
                    }
                }

                #endregion Undershoot Evaluation

                cmbInfoBox.Text = ""; cmbInfoBox.Refresh();

                //Turn off bias for SiPMs
                for (uint fpga = 0; fpga < 4; fpga++)
                    PP.FEB1.SetV(0.0, (int)fpga);

            }
            else
            {
                cmbInfoBox.Text = "Connect to FEB";
                cmbInfoBox.Refresh();
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
            runLog.SelectionStart = runLog.Text.Length;
            runLog.ScrollToCaret();
        }

        private void QaDiIWarningThresh_TextChanged(object sender, EventArgs e)
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

        private void SetRowColor(int cmbRow, Color col)
        {
            for (int i = 0; i < 12; i++)
            {
                cmbInfoLabels[cmbRow][i].BackColor = col;
                cmbInfoLabels[cmbRow][i].Update();
            }
        }

        private void UpdateCMBInfoLabels()
        {
            foreach (var lblRow in cmbInfoLabels)
                foreach (var lbl in lblRow)
                    lbl.Refresh();
        }

        private void UpdateCMBInfoLabel(int cmb)
        {
            foreach (var lbl in cmbInfoLabels[cmb])
                lbl.Refresh();
        }

        private void ClearCMBInfoLabel(int cmb)
        {
            foreach (var lbl in cmbInfoLabels[cmb])
            {
                lbl.Text = "";
                lbl.BackColor = SystemColors.Control;
                lbl.Update();
            }
        }

        private double PercentDifference(double first, double second)
        {
            return 100*Math.Abs((first - second) / (0.5 * (first + second)));
        }

        private void LostCMBavgsBtn_Click(object sender, EventArgs e)
        {
            String cmbAvgFileName = "D:\\data\\cmb_tester_data\\cmb_channel_averages.root";
            ROOTNET.NTFile cmbAvgsFile;
            if (!(File.Exists(cmbAvgFileName)))
            {
                cmbAvgsFile = new ROOTNET.NTFile(cmbAvgFileName, "CREATE");
                ROOTNET.NTH1I[] channelAvgHist = new ROOTNET.NTH1I[64];
                for (int channel = 0; channel < 64; channel++)
                {
                    channelAvgHist[channel] = new ROOTNET.NTH1I("Chan" + channel.ToString(), channel.ToString(), 1024, 0, 512); //convert the average recorded value for each channel into a usable number
                    channelAvgHist[channel].Write();
                }
                cmbAvgsFile.Close();
            }
        }

        private void ModuleQADarkCurrentBtn_Click(object sender, EventArgs e)
        {
            if (true)//PP.FEB1.client != null && PP.FEB2.client != null)
            {
                if (PP.moduleQACurrentMeasurements == null) //if we didn't make a measurement object yet, do so, else purge the existing one of information
                    PP.moduleQACurrentMeasurements = new ModuleQACurrentMeasurements(PP.FEB1);//, PP.FEB2);
                else
                    PP.moduleQACurrentMeasurements.Purge();


                ModuleQADarkCurrentBtn.Enabled = false; //disabled the button to prevent repeated clicks
                //Loop
                // take current measurements
                // move source
                //repeat until entire module is scanned
                //Move source back to position 0
                currentChannel = 0; //Set back to 0, controlled by measurment timer
                currentDicounter = 0; //Set back to 0, controlled by measurement timer
                PP.moduleQACurrentMeasurements.SetSide(ModuleQASide.Text);
                PP.FEB1.SetVAll(Convert.ToDouble(qaBias.Text)); //Turn on the bias for the FEBs
                //PP.FEB2.SetVAll(Convert.ToDouble(qaBias.Text));
                darkCurrent = true;
                moduleQAMeasurementTimer.Enabled = true;
            }
            else
            {
                if (PP.FEB1.client == null && PP.FEB2.client == null)
                {
                    MessageBox.Show("Check FEB 1 and FEB 2 connection.", "Connect to the FEBs, you dummy", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else if (PP.FEB1.client == null)
                {
                    MessageBox.Show("Check FEB 1 connection.", "Connect to FEB 1, you dummy", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else if (PP.FEB2.client == null)
                {
                    MessageBox.Show("Check FEB 2 connection.", "Connect to FEB 2, you dummy", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

        }

        private void ModuleQABtn_Click(object sender, EventArgs e)
        {
            //Check that both FEBs are connected
            if(true && moduleQAMeasurementTimer.Enabled == false)//PP.FEB1.client != null && PP.FEB2.client != null)
            {
                if (PP.moduleQACurrentMeasurements == null) //if we didn't make a measurement object yet, do so, else purge the existing one of information
                    PP.moduleQACurrentMeasurements = new ModuleQACurrentMeasurements(PP.FEB1);//, PP.FEB2);
                else
                    PP.moduleQACurrentMeasurements.Purge();


                //ModuleQABtn.Enabled = false; //disabled the button to prevent repeated clicks
                ModuleQADarkCurrentBtn.Enabled = false;
                ModuleQAHomeResetBtn.Enabled = false;

                //Loop
                // take current measurements
                // move source
                //repeat until entire module is scanned
                //Move source back to position 0
                currentChannel = 0; //Set back to 0, controlled by measurment timer
                currentDicounter = 0; //Set back to 0, controlled by measurement timer
                PP.moduleQACurrentMeasurements.SetSide(ModuleQASide.Text);
                PP.FEB1.SetVAll(Convert.ToDouble(qaBias.Text)); //Turn on the bias for the FEBs
                comPort.WriteLine(PP.moduleQACurrentMeasurements.GetgCodeDicounterPosition(currentDicounter, 1200)); //tell it to go to the 0th dicounter position
                ModuleQAStepTimer.Enabled = true;
                //PP.FEB2.SetVAll(Convert.ToDouble(qaBias.Text));
                moduleQAMeasurementTimer.Enabled = true;
            }
            else
            {
                if (PP.FEB1.client == null && PP.FEB2.client == null)
                {
                    MessageBox.Show("Check FEB 1 and FEB 2 connection.", "Connect to the FEBs, you dummy", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else if (PP.FEB1.client == null)
                {
                    MessageBox.Show("Check FEB 1 connection.", "Connect to FEB 1, you dummy", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else if(PP.FEB2.client == null)
                {
                    MessageBox.Show("Check FEB 2 connection.", "Connect to FEB 2, you dummy", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void ComPortConnectBtn_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(comPortBox.Text))
            {
                ComPortStatusBox.Text = "Connecting";

                if (comPort == null)
                    comPort = new SerialPort()
                    {
                        PortName = comPortBox.Text,
                        BaudRate = 115200,
                        Parity = Parity.None,
                        DataBits = 8,
                        StopBits = StopBits.One,
                        WriteTimeout = 1000,
                        ReadTimeout = 2000
                    };

                try
                {
                    comPort.Open();
                    comPort.WriteLine("");                    
                    if (comPort.ReadLine().Contains("Smoothie")) //Success, we are talking to the controller
                    {
                        StepperCheckForMessages();
                        zerod = false;
                        stepperCheckForOK = false;
                        stepperReceivedOK = false;
                        moduleQAHomingTimer.Enabled = true; //start the thread which moves the stepper home
                    }
                    //check status
                    ////if connected, enable the source test button
                    ////if not connected, keep the source test button disabled
                }
                catch(TimeoutException)
                {
                    MessageBox.Show("Reached timeout while communicating with controller.", "Oh shit, something went wrong", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    comPort.Close();
                    ModuleQABtn.Enabled = false; //disable the QA button if we aren't connected to the stepper controller
                }
                catch
                {
                    MessageBox.Show("Trouble communicating with controller.", "Oh shit, something went wrong", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    //comPort.Close();
                    //ModuleQABtn.Enabled = false; //disable the QA button if we aren't connected to the stepper controller
                }
            }
        }

        private void ComPortDisconnectBtn_Click(object sender, EventArgs e)
        {
            if (comPort != null && moduleQAMeasurementTimer.Enabled == false)
            {
                try
                {
                    comPort.WriteLine("reset");
                    comPort.Close();
                }
                catch (TimeoutException)
                { comPort.Close(); } //if we can't issue a reset, then just close the comPort  
                catch //Catch comport IO exception or anything else
                {  }
            }
            ModuleQABtn.Enabled = false; //disable the QA button if we aren't connected to the stepper controller
            zerod = false;
            ComPortStatusBox.Text = "Disconnected";
        }

        private void ComPortRefresh_Click(object sender, EventArgs e)
        {
            portList = SerialPort.GetPortNames();
            comPortBox.Refresh();
        }

        private void ModuleQAHomingTimer_Tick(object sender, EventArgs e)
        {
            ComPortStatusBox.Text = "Homing";
            try
            {
                if (!zerod)
                {
                    if (!stepperCheckForOK && !stepperReceivedOK) //if its not zeroed, and doesn't need to look for an OK, then we can issue a home command
                    {
                        comPort.WriteLine("G28 X0"); //tell the controller to find the home position
                        stepperCheckForOK = true;
                        stepperReceivedOK = false;
                    }
                    else if (stepperCheckForOK && !stepperReceivedOK) //if it hasn't been zeroed yet and needs to check for the OK, look for it
                    {
                        if (comPort.BytesToRead > 0)
                            if (comPort.ReadLine().Contains("ok")) //if it gets the OK, then it received the OK (yeah)
                                stepperReceivedOK = true;
                    }
                    else if (stepperCheckForOK && stepperReceivedOK) //if it hasn't been zeroed yet and received the OK
                    {
                        comPort.WriteLine("G92 X0"); //Set origin to home position
                        comPort.ReadLine(); //grab the 'ok'
                        zerod = true;
                        stepperCheckForOK = false;
                        stepperReceivedOK = false;
                        ComPortStatusBox.Text = "Zero'd";
                        moduleQAHomingTimer.Enabled = false;
                    }
                }
            }
            catch (TimeoutException)
            {
                moduleQAHomingTimer.Enabled = false;
                MessageBox.Show("Reached timeout while communicating with controller.", "Oh shit, something went wrong", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                comPort.Close();
                ModuleQABtn.Enabled = false; //disable the QA button if we aren't connected to the stepper controller
                ComPortStatusBox.Text = "ERR";
            }
            catch
            {
                moduleQAHomingTimer.Enabled = false;
                MessageBox.Show("Trouble communicating with controller.", "Oh shit, something went wrong", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                comPort.Close();
                ModuleQABtn.Enabled = false; //disable the QA button if we aren't connected to the stepper controller
                ComPortStatusBox.Text = "ERR";
            }
        }

        private void ModuleQAMeasurementTimer_Tick(object sender, EventArgs e)
        {
            System.Threading.Thread.Sleep(0); //yield
            if (ModuleQAStepTimer.Enabled == false) //Block the contents of this timer if the stepper is moving
            {
                if (!darkCurrent)
                {
                    if (currentDicounter < 100)//5)//8)
                    {
                        if (currentChannel < 20)//64)
                        {
                            //take measurments
                            ComPortStatusBox.Text = "Measuring " + currentDicounter + " " + currentChannel;
                            PP.moduleQACurrentMeasurements.TakeMeasurement(currentChannel);
                            currentChannel++; //increment the channel
                        }
                        else //must have reached the end of 64 channels
                        {
                            PP.moduleQACurrentMeasurements.WriteMeasurements("C:\\Users\\Boi\\Desktop\\ScanningData_" + ModuleQAFilenameBox.Text + ".txt", 0, currentDicounter);
                            PP.moduleQACurrentMeasurements.Purge();
                            currentDicounter++; //increment the dicounter
                            if (currentDicounter < 100)//5)//8)
                                comPort.WriteLine(PP.moduleQACurrentMeasurements.GetgCodeDicounterPosition(currentDicounter, 1200)); //tell it the position to go to
                            currentChannel = 0;
                            ModuleQAStepTimer.Enabled = true;
                        }
                    }
                    else //must have reached the end of scanning across the module
                    {
                        comPort.WriteLine(PP.moduleQACurrentMeasurements.GetgCodeDicounterPosition(0, 2800)); //go back to the home position quickly
                        ModuleQAStepTimer.Enabled = true;
                        ModuleQADarkCurrentBtn.Enabled = true;
                        moduleQAMeasurementTimer.Enabled = false;
                        PP.moduleQACurrentMeasurements.TurnOffBias(); //turn off the bias
                    }
                }
                else if (darkCurrent) //if it is a dark current measurement, just record all 64 channels
                {
                    if (currentChannel < 20)//64)
                    {
                        //take measurments
                        Console.WriteLine("Measuring " + currentChannel);
                        PP.moduleQACurrentMeasurements.TakeMeasurement(currentChannel);
                        //System.Threading.Thread.Sleep(10);
                        currentChannel++;
                    }
                    else //must have reached the end of 64 channels, write out the dark currents
                    {
                        PP.moduleQACurrentMeasurements.WriteMeasurements("C:\\Users\\Boi\\Desktop\\ScanningData_" + ModuleQAFilenameBox.Text + ".txt", 0, -1); //-1 will denote dark current measurement
                        PP.moduleQACurrentMeasurements.Purge();
                        darkCurrent = false; //done with darkcurrent
                        ModuleQADarkCurrentBtn.Enabled = true;
                        ModuleQABtn.Enabled = true; //since we took the dark current measurements, now we can QA a module
                        moduleQAMeasurementTimer.Enabled = false;
                    }
                }
            }
        }

        private void ModuleQAHaltBtn_Click(object sender, EventArgs e)
        {
            if (moduleQAHomingTimer.Enabled == false) //cannot issue a halt while it is homing, a property of the controller...
            {
                moduleQAMeasurementTimer.Enabled = false;
                ModuleQAStepTimer.Enabled = false;
                moduleQAHomingTimer.Enabled = false;
                ModuleQABtn.Enabled = false;
                ModuleQAHomeResetBtn.Enabled = true;
                zerod = false;

                try
                {
                    StepperCheckForMessages();
                    comPort.WriteLine("M112"); //Issue HALT to stepper controller
                    StepperCheckForMessages();
                }
                catch (TimeoutException)
                {
                    moduleQAHomingTimer.Enabled = false;
                    MessageBox.Show("Reached timeout while communicating with controller.", "Oh shit, something went wrong", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    comPort.Close();
                    ModuleQABtn.Enabled = false; //disable the QA button if we aren't connected to the stepper controller
                    zerod = false;
                }
                catch
                {
                    moduleQAHomingTimer.Enabled = false;
                    MessageBox.Show("Trouble communicating with controller.", "Oh shit, something went wrong", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    comPort.Close();
                    ModuleQABtn.Enabled = false; //disable the QA button if we aren't connected to the stepper controller
                    zerod = false;
                }

                ComPortStatusBox.Text = "HALT, RESET REQUIRED";
            }
        }

        private void ModuleQAHomeResetBtn_Click(object sender, EventArgs e)
        {
            try
            {
                if (zerod)
                {
                    comPort.WriteLine("G1 X0"); //Go back to zero if zero'd
                    ModuleQAStepTimer.Enabled = true;
                }
                else
                { //Else issue a reset, since a halt must have been issued
                    zerod = false;
                    stepperCheckForOK = false;
                    stepperReceivedOK = false;
                    comPort.WriteLine("M999"); //Reset
                    StepperCheckForMessages();
                    moduleQAHomingTimer.Enabled = true; //re-home the device
                }
            }
            catch { ComPortStatusBox.Text = "It's dead, Jim.";  }
        }

        private void ModuleQAStepTimer_Tick(object sender, EventArgs e)
        {
            System.Threading.Thread.Sleep(0); //yield
            ComPortStatusBox.Text = "Moving";
            try
            {
                StepperCheckForMessages();

                comPort.WriteLine("get pos");
                System.Threading.Thread.Sleep(100); //wait for it to reply
                List<string> positionInfo = new List<string>();
                while (comPort.BytesToRead > 0)
                {
                    string s = comPort.ReadLine(); //grab the positions
                    if (s.Contains("last") || s.Contains("realtime")) //if it contains last or realtime, this holds the information we are interested in
                        positionInfo.Add(s);
                }
                if (positionInfo.Count == 2) //Double check that we only have the strings for current and destination positions
                {
                    double destination_xPos = Convert.ToDouble(positionInfo[0].Split(new char[] { ' ', ':' })[4]); //split the string and extract the destination X position
                    double current_xPos = Convert.ToDouble(positionInfo[1].Split(new char[] { ' ', ':' })[4]); //split the string and extract the current X position

                    if (Math.Abs(destination_xPos - current_xPos) < 0.01) //arbitrary threshold, if the current position is at the destination, we are done moving
                    {
                        ModuleQAStepTimer.Enabled = false;
                        ComPortStatusBox.Text = "";
                    }
                }
            }
            catch (TimeoutException)
            {
                moduleQAHomingTimer.Enabled = false;
                MessageBox.Show("Reached timeout while communicating with controller.", "Oh shit, something went wrong", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                comPort.Close();
                ModuleQABtn.Enabled = false; //disable the QA button if we aren't connected to the stepper controller
            }
            catch(Exception exc)
            {
                Console.WriteLine(exc);
                moduleQAHomingTimer.Enabled = false;
                var answer = MessageBox.Show("Trouble communicating with controller. Close connection?", "Oh shit, something went wrong", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
                if(answer.Equals(DialogResult.Yes))
                    comPort.Close();
                ModuleQABtn.Enabled = false; //disable the QA button if we aren't connected to the stepper controller
            }

        }

        private void StepperCheckForMessages()
        {
            if (comPort != null)
            {
                try
                {
                    if (comPort.BytesToRead > 0)
                    {
                        while (comPort.BytesToRead > 0) //clear any messages that were being sent
                            if(comPort.ReadLine().Contains("!"))
                            {
                                MessageBox.Show("The controller is alarmed, did you say something mean to it?", "Oh shit, something went wrong", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
                                comPort.Close();
                                ModuleQABtn.Enabled = false; //disable the QA button if we aren't connected to the stepper controller

                            }
                    }
                }
                catch (TimeoutException)
                {
                    moduleQAHomingTimer.Enabled = false;
                    MessageBox.Show("Reached timeout while communicating with controller.", "Oh shit, something went wrong", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    comPort.Close();
                    ModuleQABtn.Enabled = false; //disable the QA button if we aren't connected to the stepper controller
                }
            }
        }

        private void QaDiCounterMeasurementTimer_Tick(object sender, EventArgs e)
        {
            System.Threading.Thread.Sleep(0); //yield
            if(currentChannel < qaDiButtons.Length)
            {
                if (!qaDiButtons[currentChannel].Checked)
                {
                    double measurement = PP.qaDicounterMeasurements.TakeMeasurement(currentChannel);
                    if (measurement < Convert.ToDouble(qaDiIWarningThresh.Text)) //Low current
                        qaDiButtons[currentChannel].BackColor = Color.Red;
                    if (measurement < 0.025) //No Current
                        qaDiButtons[currentChannel].BackColor = Color.Blue;
                    qaDiButtons[currentChannel].Update();
                }
                currentChannel++;
                autoDataProgress.Increment(1);
            }
            else
            {
                PP.qaDicounterMeasurements.WriteMeasurements(dicounterNumberTextBox.Text, PP.FEB1.ReadTemp(0));
                PP.qaDicounterMeasurements.TurnOffBias();
                currentChannel = 0;
                autoDataProgress.Value = 0;
                autoDataProgress.Update();
                qaDiCounterMeasurementTimer.Enabled = false;
            }
        }
    }



    public class ConnectAttemptEventArgs : EventArgs
    {
        public int ConnectAttempt { get; set; }
    }

}
