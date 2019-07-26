using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
//using System.Threading.Tasks;
using System.Windows.Forms;
using ZedGraph;

namespace TB_mu2e
{

    public partial class frmMain : Form
    {
        private int _ActiveFEB = 0;
        private Mu2e_FEB_client activeFEB;
        private uConsole console;

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
        private int[] spill_trig_num;
        private uint spill_num;

        private const int num_chans = 16;
        private System.Windows.Forms.Label[] BDVoltLabels = new System.Windows.Forms.Label[num_chans];

        private const double NO_CURRENT_THRESH = 0.025;
        private const double AUTO_THRESH_MULTIPLIER = 1.10; //10% higher
        private bool auto_thresh_enabled = false;

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
        private double spill_record_delay = 0.1;

        public void AddConsoleMessage(string msg)
        {
            console_Disp.Text = console.Add_messg(msg);
            Application.DoEvents();
        }

        public frmMain()
        {
            InitializeComponent();

            console = new uConsole();

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

        private void ClientChangeBtn_Click(object sender, EventArgs e)
        {
            ClientSelectForm clientAddressForm = new ClientSelectForm();
            if (PP.FEB_client_addresses != null)
                clientAddressForm.PopulateCurrentAddresses(PP.FEB_client_addresses);
            clientAddressForm.ShowDialog();
            PP.FEB_client_addresses = clientAddressForm.GetClientAddresses();
            PP.Num_FEB_clients = PP.FEB_client_addresses.Length;
        }

        private void BtnConnectAll_Click(object sender, EventArgs e)
        {
            if (PP.FEB_clients != null) //if we were already connected, disconnect all clients so we can reconnect
            {
                foreach (Mu2e_FEB_client client in PP.FEB_clients)
                    client.Close();
                PP.FEB_clients = null;
                while (FEBSelectPanel.Controls.Count > 0)
                    foreach (Control febBtn in FEBSelectPanel.Controls)
                        FEBSelectPanel.Controls.Remove(febBtn);
            }
            
            if(PP.Num_FEB_clients == 0)
                ClientChangeBtn_Click(this, e);
            if (PP.Num_FEB_clients > 0)
            {
                spill_trig_num = new int[PP.Num_FEB_clients];
                PP.FEB_clients = new Mu2e_FEB_client[PP.Num_FEB_clients];
                for (int feb_client_num = 0; feb_client_num < PP.Num_FEB_clients; feb_client_num++)
                {
                    PP.FEB_clients[feb_client_num] = new Mu2e_FEB_client() { name = "FEB" + Convert.ToString(feb_client_num), host_name_prop = PP.FEB_client_addresses[feb_client_num], clientNum = feb_client_num };
                    PP.FEB_clients[feb_client_num].Open();
                    if (PP.FEB_clients[feb_client_num].ClientOpen)
                    {
                        FEBSelectPanel.Controls.Add(new System.Windows.Forms.Button()
                        {
                            Enabled = true,
                            Name = "dbgFEB" + feb_client_num,
                            Dock = System.Windows.Forms.DockStyle.Fill,
                            Text = PP.FEB_clients[feb_client_num].name
                        });
                        FEBSelectPanel.Controls[feb_client_num].Click += new System.EventHandler(this.FEB_Click);
                    }
                    else
                    {
                        FEBSelectPanel.Controls.Add(new System.Windows.Forms.Button()
                        {
                            Enabled = false,
                            BackColor = Color.DarkRed,
                            Name = "dbgFEB" + feb_client_num,
                            Dock = System.Windows.Forms.DockStyle.Fill,
                            Text = PP.FEB_clients[feb_client_num].name
                        });
                    }
                }
                FEBSelectPanel.Update();
                if(FEBSelectPanel.Controls.Count > 0)
                    FEB_Click(FEBSelectPanel.Controls[0], e); //Select the first client
            }
        }

        private void ConsoleBox_TextChanged(object sender, EventArgs e)
        {
            if (activeFEB != null)
            {
                if (activeFEB.ClientOpen)
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
                                while (activeFEB.TNETSocket.Available > 0)
                                {
                                    byte[] rbuf = new byte[activeFEB.TNETSocket.Available];
                                    activeFEB.TNETSocket.Receive(rbuf);
                                }
                                if (!this_is_a_write)
                                {
                                    activeFEB.TNETSocket.Send(buf);
                                    System.Threading.Thread.Sleep(100);
                                    if (sent_string.ToLower().Contains("a0 "))
                                    {
                                        int delay = Convert.ToInt16(sent_string.Split().Skip(1).First().ToString());
                                        System.Threading.Thread.Sleep(delay * 100);
                                    }
                                    byte[] rec_buf = new byte[activeFEB.TNETSocket.Available];
                                    int ret_len = activeFEB.TNETSocket.Receive(rec_buf, rec_buf.Length, System.Net.Sockets.SocketFlags.None);
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
                                                activeFEB.TNETSocket.Send(buf);
                                                System.Threading.Thread.Sleep(10);
                                            }
                                            buf = PP.GetBytes("rdi " + write_elements[1] + " " + write_elements[3]); //command of the form rdi [start register] [num_reads = num_writes]
                                            activeFEB.TNETSocket.Send(buf);
                                            System.Threading.Thread.Sleep(100);
                                            byte[] rec_buf = new byte[activeFEB.TNETSocket.Available];
                                            int ret_len = activeFEB.TNETSocket.Receive(rec_buf);
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
                                        activeFEB.TNETSocket.Send(buf);
                                        l = sent_string;
                                        string read_string = string.Join(" ", sent_string.Split().Skip(1).Take(1));
                                        //read_string = read_string.Substring(read_string.ToLower().IndexOf("wr") + 2, read_string.Length - read_string.ToLower().IndexOf("wr") + 2);
                                        read_string = "rd " + read_string + "\r\n";
                                        buf = PP.GetBytes(read_string);
                                        activeFEB.TNETSocket.Send(buf);
                                        System.Threading.Thread.Sleep(100);
                                        byte[] rec_buf = new byte[activeFEB.TNETSocket.Available];
                                        int ret_len = activeFEB.TNETSocket.Receive(rec_buf);
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
            }
        }

        private void BtnRegREAD_Click(object sender, EventArgs e)
        {
            if (PP.FEB_clients[_ActiveFEB] != null)
            {
                Mu2e_FEB_client selectedFEB = PP.FEB_clients[_ActiveFEB];
                if (selectedFEB.ClientOpen)
                {
                    //int i = this.tabControl.SelectedIndex;
                    //if (tabControl.SelectedTab.Text.Contains("FEB"))
                    //{
                    //    if (_ActiveFEB == 1)
                    //    { myFEB = PP.FEB1; }
                    //    else if (_ActiveFEB == 2)
                    //    { myFEB = PP.FEB2; }
                    //    else
                    //    { MessageBox.Show("No FEB active"); return; }

                    //    myFEB.CheckFEB_connection();

                    ushort fpga_num = Convert.ToUInt16(udFPGA.Value);
                    for (int j = 0; j < num_reg; j++)
                    {
                        Mu2e_Register.FindName(rnames[j], fpga_num, ref selectedFEB.arrReg, out Mu2e_Register r1);
                        //r1.fpga_index = fpga_num;
                        Mu2e_Register.ReadReg(ref r1, ref selectedFEB.client);
                        if (!r1.pref_hex)
                        { txtREGISTERS[j].Text = r1.val.ToString(); }
                        else
                        { txtREGISTERS[j].Text = "0x" + Convert.ToString(r1.val, 16); }
                    }
                }
            }
            //}
        }

        private void BtnRegWRITE_Click(object sender, EventArgs e)
        {
            if (PP.FEB_clients[_ActiveFEB] != null)
            {
                Mu2e_FEB_client selectedFEB = PP.FEB_clients[_ActiveFEB];
                //int i = this.tabControl.SelectedIndex;
                //if (tabControl.SelectedTab.Text.Contains("FEB"))
                //{
                //    if (_ActiveFEB == 1)
                //    { myFEB = PP.FEB1; }
                //    if (_ActiveFEB == 2)
                //    { myFEB = PP.FEB2; }
                if (selectedFEB.ClientOpen)
                {
                    ushort fpga_num = Convert.ToUInt16(udFPGA.Value);
                    for (int j = 0; j < num_reg; j++)
                    {
                        Mu2e_Register.FindName(rnames[j], fpga_num, ref selectedFEB.arrReg, out Mu2e_Register r1);
                        //r1.fpga_index = fpga_num;
                        if (txtREGISTERS[j].Text.Contains("x"))
                        {
                            try
                            {
                                UInt32 v = Convert.ToUInt32(txtREGISTERS[j].Text, 16);
                                Mu2e_Register.WriteReg(v, ref r1, ref selectedFEB.client);
                            }
                            catch
                            { txtREGISTERS[j].Text = "?"; }
                        }
                        else
                        {
                            try
                            {
                                UInt32 v = Convert.ToUInt32(txtREGISTERS[j].Text);
                                Mu2e_Register.WriteReg(v, ref r1, ref selectedFEB.client);
                            }
                            catch
                            { txtREGISTERS[j].Text = "?"; }
                        }
                    }
                }
            }
            //}
        }

        private void FEB_Click(object sender, EventArgs e)
        {
            Button sndrBtn = (Button)sender;
            if(tabControl.SelectedTab.Text.Contains("Console"))
                console_Disp.Text = console.Add_messg("-- " + sndrBtn.Text + " --\r\n");
            int feb_client_num = -1;
            try
            {
                feb_client_num = Convert.ToUInt16(sndrBtn.Name.Substring(6));
                _ActiveFEB = feb_client_num;
                if (PP.FEB_clients != null)
                {
                    activeFEB = PP.FEB_clients[feb_client_num];
                    foreach (Button febBtn in FEBSelectPanel.Controls)
                        if(febBtn.BackColor != Color.DarkRed)
                            febBtn.BackColor = Color.Gray;
                    if (activeFEB.ClientOpen)
                        sndrBtn.BackColor = Color.Green;
                    else
                    {
                        sndrBtn.BackColor = Color.DarkRed;
                        sndrBtn.Enabled = false;
                    }
                }
            } catch { System.Console.WriteLine("Something went wrong with seting the client."); }


            //string myName = mySender.Text;
            //if (myName.Contains("FEB1"))
            //{
            //    Connect_Click((object)btnFEB1, e);
            //    console_Disp.Text = console.Add_messg("---- FEB1 ----\r\n");
            //}
            //if (myName.Contains("FEB2"))
            //{
            //    Connect_Click((object)btnFEB2, e);
            //    console_Disp.Text = console.Add_messg("---- FEB2 ----\r\n");
            //}
            //if (myName.Contains("WC"))
            //{
            //    Connect_Click((object)btnWC, e);
            //    console_Disp.Text = console.Add_messg("----  WC  ----\r\n");
            //}
            //if (myName.Contains("FECC")) { }
        }

        private void BtnBiasREAD_Click(object sender, EventArgs e)
        {
            double[] cmb_temp = new double[4];
            int fpga = (int)udFPGA.Value;
            if (activeFEB != null)
            {
                if (activeFEB.ClientOpen)
                {
                    txtV.Text = activeFEB.ReadV(fpga).ToString("0.000");
                    txtI.Text = activeFEB.ReadA0(fpga, (int)udChan.Value).ToString("0.0000");
                    cmb_temp = activeFEB.ReadTempFPGA(fpga);
                }

                txtCMB_Temp1.Text = cmb_temp[0].ToString("0.0");
                txtCMB_Temp2.Text = cmb_temp[1].ToString("0.0");
                txtCMB_Temp3.Text = cmb_temp[2].ToString("0.0");
                txtCMB_Temp4.Text = cmb_temp[3].ToString("0.0");
            }


            //string name = tabControl.SelectedTab.Text;
            //if (name.Contains("FEB"))
            //{
            //    //Console.WriteLine("NEW READING");
            //    //for (int t = 0; t < 300; t++)
            //    //{
            //    //    for (int i = 0; i < 8; i++)
            //    //    {
            //    //        double I1 = 0;
            //    //        int nGoodReads = 0;
            //    //        for (int k = 0; k < nreads.Value; k++)
            //    //        {
            //    //            double I1now = PP.FEB1.ReadA0((int)udFPGA.Value, i);
            //    //            if (I1now > 0)
            //    //            {
            //    //                I1 = I1 + I1now;
            //    //                nGoodReads++;
            //    //            }
            //    //        }
            //    //        Console.Write(" " + I1/nGoodReads);
            //    //    }
            //    //    Console.WriteLine("");
            //    //    System.Threading.Thread.Sleep(60000);
            //    //}

            //    switch (_ActiveFEB)
            //    {
            //        case 1:
            //            txtV.Text = PP.FEB1.ReadV((int)udFPGA.Value).ToString("0.000");
            //            txtI.Text = PP.FEB1.ReadA0((int)udFPGA.Value, (int)udChan.Value).ToString("0.0000");

            //            //
            //            // Who knows why this junk is here...
            //            //
            //            //double I1 = 0;
            //            //int nGoodReads = 0;
            //            //for (int i = 0; i < nreads.Value; i++)
            //            //{
            //            //    double I1now = PP.FEB1.ReadA0((int)udFPGA.Value, (int)udChan.Value);
            //            //    if (I1now > 0)
            //            //    {
            //            //        I1 = I1 + I1now;
            //            //        nGoodReads++;
            //            //    }
            //            //    //                            Console.WriteLine(PP.FEB1.ReadA0((int)udFPGA.Value, (int)udChan.Value));
            //            //}
            //            //txtI.Text = (I1 / (double)(nGoodReads)).ToString("0.0000");

            //            cmb_temp = PP.FEB1.ReadTempFPGA((int)udFPGA.Value);
            //            break;
            //        case 2:
            //            txtV.Text = PP.FEB2.ReadV((int)udFPGA.Value).ToString("0.000");
            //            txtI.Text = PP.FEB2.ReadA0((int)udFPGA.Value, (int)udChan.Value).ToString("0.0000");
            //            cmb_temp = PP.FEB2.ReadTempFPGA((int)udFPGA.Value);
            //            break;

            //        default:
            //            break;
            //    }
            //}
        }

        private void BtnBiasWRITE_Click(object sender, EventArgs e)
        {
            if (activeFEB != null)
            {
                if (activeFEB.ClientOpen)
                {
                    activeFEB.SetV(Convert.ToDouble(txtV.Text), (int)udFPGA.Value);
                    txtI.Text = activeFEB.ReadA0((int)udFPGA.Value, (int)udChan.Value).ToString("0.0000");
                }
            }


            //string name = tabControl.SelectedTab.Text;
            //if (name.Contains("FEB"))
            //{
            //    switch (_ActiveFEB)
            //    {
            //        case 1:
            //            PP.FEB1.SetV(Convert.ToDouble(txtV.Text), (int)udFPGA.Value);
            //            txtI.Text = PP.FEB1.ReadA0((int)udFPGA.Value, (int)udChan.Value).ToString("0.0000");
            //            break;
            //        case 2:
            //            PP.FEB2.SetV(Convert.ToDouble(txtV.Text));
            //            txtI.Text = PP.FEB2.ReadA0((int)udFPGA.Value, (int)udChan.Value).ToString("0.0000");
            //            break;

            //        default:
            //            break;
            //    }
            //}
        }

        private void BtnBiasWRITEALL_Click(object sender, EventArgs e)
        {
            if (activeFEB != null)
            {
                if (activeFEB.ClientOpen)
                {
                    activeFEB.SetVAll(Convert.ToDouble(txtV.Text));
                    txtI.Text = activeFEB.ReadA0((int)udFPGA.Value, (int)udChan.Value).ToString("0.0000");
                }
            }
        }

        private void BtnScan_Click(object sender, EventArgs e)
        {

            if (activeFEB != null)
            {
                //zedFEB1.GraphPane.CurveList.Clear();
                //switch (_ActiveFEB)
                //{
                //    case 1:
                //        FEB = PP.FEB1;
                //        break;
                //    case 2:
                //        FEB = PP.FEB2;
                //        break;
                //}

                //if (btnScan.Text != "SCAN")
                //{ flgBreak = true; return; }

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
                Application.DoEvents();
            }
        }

        private void UpdateDisplay_IV(IV_curve myIV)
        {
            //zedFEB1.GraphPane.CurveList.Clear();
            //if (chkLogY.Checked)
            //{
            //    if (Math.Round((double)(Math.Log10(myIV.min_I))) < -2)
            //    { zedFEB1.GraphPane.YAxis.Scale.Min = -2; }
            //    else
            //    { zedFEB1.GraphPane.YAxis.Scale.Min = Math.Round((double)(Math.Log10(myIV.min_I))) - .1; }
            //    zedFEB1.GraphPane.YAxis.Scale.Max = Math.Round((double)(Math.Log10(myIV.max_I * 1000)), 0);
            //    if (zedFEB1.GraphPane.YAxis.Scale.Max < 0.1) { zedFEB1.GraphPane.YAxis.Scale.Max = 0.1; }
            //    zedFEB1.GraphPane.AddCurve(myIV.chan.ToString(), myIV.loglist, Color.DarkRed, SymbolType.None);
            //    zedFEB1.GraphPane.YAxis.Scale.MajorStep = .1 * (double)(Math.Log10(myIV.max_I * 1000));
            //}
            //else
            //{
            //    zedFEB1.GraphPane.YAxis.Scale.Min = 0.0;
            //    zedFEB1.GraphPane.YAxis.Scale.Max = Math.Round((double)(myIV.max_I + 0.1 * (myIV.max_I - myIV.min_I)), 0);
            //    zedFEB1.GraphPane.AddCurve(myIV.chan.ToString(), myIV.list, Color.DarkRed, SymbolType.None);
            //}
            //double s = Math.Round((double)(myIV.max_v - myIV.min_v) / 10.0, 0);
            //if (zedFEB1.GraphPane.XAxis.Scale.MajorStep < s) { zedFEB1.GraphPane.XAxis.Scale.MajorStep = s; }
            //zedFEB1.GraphPane.XAxis.Scale.MinorStep = zedFEB1.GraphPane.XAxis.Scale.MajorStep / 4;
            //zedFEB1.GraphPane.XAxis.Scale.Min = (double)udStart.Value - .2;
            //zedFEB1.GraphPane.XAxis.Scale.Max = (double)udStop.Value + .2;

            //s = Math.Round((myIV.max_I - myIV.min_I) / 10.0, 0);
            //zedFEB1.GraphPane.YAxis.Scale.MinorStep = zedFEB1.GraphPane.YAxis.Scale.MajorStep / 4;

            //zedFEB1.Invalidate(true);
            //Application.DoEvents();
        }

        private void UpdateDisplay()
        {
            //Color[] this_color = new Color[12];
            //Histo_helper.InitColorList(ref this_color);

            //List<HISTO_curve> myHistoList = null;
            //zedFEB1.GraphPane.CurveList.Clear();

            //if (_ActiveFEB == 1) { myHistoList = PP.FEB1Histo; }
            //if (_ActiveFEB == 2) { myHistoList = PP.FEB2Histo; }

            //foreach (HISTO_curve h1 in myHistoList)
            //{
            //    if (chkLogY.Checked)
            //    {
            //        zedFEB1.GraphPane.YAxis.Scale.Max = Math.Round((double)(Math.Log10(h1.max_count + 0.1 * (h1.max_count - h1.min_count))), 0);
            //        zedFEB1.GraphPane.AddCurve(h1.chan.ToString(), h1.loglist, Color.DarkRed, SymbolType.None);
            //    }
            //    else
            //    {
            //        zedFEB1.GraphPane.YAxis.Scale.Max = Math.Round((double)(h1.max_count + 0.1 * (h1.max_count - h1.min_count)), 0);
            //        zedFEB1.GraphPane.AddCurve(h1.chan.ToString(), h1.list, this_color[h1.chan % 16], SymbolType.None);
            //    }
            //    double s = 0;
            //    s = Math.Round((double)(h1.max_thresh - h1.min_thresh) / 10.0, 0);
            //    if (zedFEB1.GraphPane.XAxis.Scale.MajorStep < s) { zedFEB1.GraphPane.XAxis.Scale.MajorStep = s; }
            //    zedFEB1.GraphPane.XAxis.Scale.MinorStep = zedFEB1.GraphPane.XAxis.Scale.MajorStep / 4;

            //    s = Math.Round((h1.max_count - h1.min_count) / 10.0, 0);
            //    if (zedFEB1.GraphPane.YAxis.Scale.MajorStep < s) { zedFEB1.GraphPane.YAxis.Scale.MajorStep = s; }
            //    zedFEB1.GraphPane.YAxis.Scale.MinorStep = zedFEB1.GraphPane.YAxis.Scale.MajorStep / 4;
            //}
            //zedFEB1.Invalidate(true);
            //Application.DoEvents();
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
                if (activeFEB != null)
                {
                    Mu2e_Register.FindName(rnames[i], Convert.ToUInt16(udFPGA.Value), ref activeFEB.arrReg, out Mu2e_Register reg);
                    lblREG[i].Text = rnames[i] + "(x" + Convert.ToString(reg.addr, 16) + ")";
                }
            }

            BtnRegREAD_Click(null, null);
        }

        private void BtnErase_Click(object sender, EventArgs e)
        {
            //zedFEB1.GraphPane.CurveList.Clear();
            //zedFEB1.Invalidate(true);
            //List<HISTO_curve> myHistoList = null;
            ////List<HISTO_curve> EraseHistoList = null;
            //if (_ActiveFEB == 1) { myHistoList = PP.FEB1Histo; }
            //if (_ActiveFEB == 2) { myHistoList = PP.FEB2Histo; }


            ////foreach (HISTO_curve h1 in myHistoList)
            ////{
            ////    if (h1.saved) { myHistoList.Remove(h1); }
            ////}
            //myHistoList.Clear();
        }

        private void ChkLogY_CheckedChanged(object sender, EventArgs e)
        {
            //if (ShowIV.Visible)
            //{ UpdateDisplay(); }
            //if (ShowSpect.Visible)
            //{
            //    if (_ActiveFEB == 1)
            //    {
            //        if (PP.FEB1IVs.Count > 0) { UpdateDisplay_IV(PP.FEB1IVs.Last()); }
            //    }
            //    if (_ActiveFEB == 2)
            //    {
            //        if (PP.FEB2IVs.Count > 0) { UpdateDisplay_IV(PP.FEB2IVs.Last()); }
            //    }
            //}
        }

        private void ChkIntegral_CheckedChanged(object sender, EventArgs e)
        {
            if (chkIntegral.Checked) { _IntegralScan = true; }
            else { _IntegralScan = false; }
            UpdateDisplay();
        }

        private void BtnSaveHistos_Click(object sender, EventArgs e)
        {
            //if (ShowIV.Visible)
            //{
            //    List<HISTO_curve> myHistoList = null;
            //    if (_ActiveFEB == 1) { myHistoList = PP.FEB1Histo; }
            //    if (_ActiveFEB == 2) { myHistoList = PP.FEB2Histo; }

            //    foreach (HISTO_curve h1 in myHistoList)
            //    {
            //        if (h1.saved) { }
            //        else { h1.Save(); }
            //    }
            //}
        }

        private void Disconnect_Click(object sender, EventArgs e)
        {
            if (PP.FEB_clients != null) //Disconnect all clients
            {
                foreach (Mu2e_FEB_client client in PP.FEB_clients)
                    client.Close();
                PP.FEB_clients = null;
                while (FEBSelectPanel.Controls.Count > 0)
                    foreach (Control febBtn in FEBSelectPanel.Controls)
                        FEBSelectPanel.Controls.Remove(febBtn);
            }
            //if (PP.FEB1.ClientOpen) { PP.FEB1.Close(); dbgFEB1.BackColor = Color.LightGray; btnFEB1.BackColor = Color.LightGray; }
            //if (PP.FEB2.ClientOpen) { PP.FEB2.Close(); dbgFEB2.BackColor = Color.LightGray; btnFEB2.BackColor = Color.LightGray; }
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

        private void UdChan_ValueChanged(object sender, EventArgs e)
        {
            uint FPGA_index = (uint)udFPGA.Value;
            uint chan = 0;
            if (activeFEB != null)
            {
                if (activeFEB.ClientOpen)
                {
                    chan = (uint)udChan.Value;
                    if (activeFEB != null)
                    {
                        Mu2e_Register.FindAddrFPGA(0x020, FPGA_index, ref activeFEB.arrReg, out Mu2e_Register mux_reg);
                        if (chan < 16)
                        {
                            uint v = (uint)(0x10 + chan);
                            Mu2e_Register.WriteReg(v, ref mux_reg, ref activeFEB.client);
                        }
                    }
                    BtnRegREAD_Click(null, null);
                    BtnBiasREAD_Click(null, null);
                }
            }
        }

        private void UdFPGA_ValueChanged(object sender, EventArgs e)
        {
            BtnRegREAD_Click(null, null);
        }

        private void SpillTimer_Tick(object sender, EventArgs e)
        {
            Thread.Sleep(1);
            if (PP.myRun != null)
            {
                if (PP.myRun.ACTIVE && !PP.myRun.READING) //If we are actively looking for spills
                {
                    bool in_spill_any_feb = false;

                    foreach(Mu2e_FEB_client feb in PP.FEB_clients)
                    {
                        if(feb.ClientOpen)
                        {
                            feb.CheckStatus(out uint spill_status, out uint spill_num, out uint trig_num);
                            if (spill_status == 4)
                                in_spill_any_feb = true;
                            SpillStatusTable.GetControlFromPosition(1, feb.clientNum+1).Text = spill_status.ToString();
                            SpillStatusTable.GetControlFromPosition(2, feb.clientNum+1).Text = trig_num.ToString();
                            spill_trig_num[feb.clientNum] = (int)trig_num;                            
                        }
                    }
                    //if (PP.FEB1.ClientOpen)
                    //{
                    //    PP.FEB1.CheckStatus(out uint spill_status, out uint spill_num, out uint trig_num);
                    //    if (spill_status == 4)
                    //        in_spill_febs[0] = true;
                    //    lblSpillFEB1.Text = spill_status.ToString();
                    //    lblFEB1TrigNum.Text = trig_num.ToString();
                    //    spill_trig_num[0] = (int)trig_num;
                    //}

                    //if (PP.FEB2.ClientOpen)
                    //{
                    //    PP.FEB2.CheckStatus(out uint spill_status, out uint spill_num, out uint trig_num);
                    //    if (spill_status == 4)
                    //        in_spill_febs[1] = true;
                    //    lblSpillFEB2.Text = spill_status.ToString();
                    //    lblFEB2TrigNum.Text = trig_num.ToString();
                    //    spill_trig_num[1] = (int)trig_num;
                    //}

                    if (in_spill_any_feb)
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


                    //if (false)//(PP.WC.ClientOpen)
                    //{

                    //    WC_client.check_status(out bool in_spill, out string num_trig, out string mytime);
                    //    lblSpillWC.Text = in_spill.ToString();
                    //    try { spill_trig_num[2] = Convert.ToInt32(num_trig); } catch { spill_trig_num[2] = 0; }
                    //    lblWCTrigNum.Text = spill_trig_num[2].ToString("0");

                    //    if (in_spill)
                    //    {
                    //        if (first_spill_taken == false)
                    //        {
                    //            first_spill_taken = true;
                    //            PP.myRun.UpdateStatus("First Spill Synchronization");
                    //        }
                    //        PP.myRun.timeLastSpill = DateTime.Now;
                    //        PP.myRun.UpdateStatus("Detected spill. Run file is " + PP.myRun.OutFileName);
                    //        PP.myRun.spill_complete = false;
                    //        waiting_for_data = true;
                    //    }
                    //    else
                    //    {
                    //        PP.myRun.spill_complete = true;
                    //    }
                    //}

                    if (PP.myRun.spill_complete) //If we are no longer in a spill
                    {
                        if (first_spill_taken) //If we took the first spill, we can now see if we are a few seconds after the spill to record data
                        {
                            double time_past_spill = (DateTime.Now - PP.myRun.timeLastSpill).TotalSeconds;
                            if ((time_past_spill > spill_record_delay) && waiting_for_data) //If we have waited a sufficient amount of time and we are expecting data, save the data
                            {
                                waiting_for_data = false;
                                Thread recorder = new Thread(()=>PP.myRun.RecordSpill());
                                recorder.Start();
                                spill_num++;
                                //Update the total number of triggers
                                foreach(Mu2e_FEB_client feb in PP.FEB_clients)
                                {
                                    System.Windows.Forms.Label trigLbl = (System.Windows.Forms.Label) SpillStatusTable.GetControlFromPosition(3, feb.clientNum+1);
                                    trigLbl.Text = (Convert.ToUInt64(trigLbl.Text) + (ulong)spill_trig_num[feb.clientNum]).ToString("0");
                                }
                            }
                        }
                    }
                }

                lblTxtRunName.Text = "Run_" + PP.myRun.num.ToString(); //Keep the run name updated
                lock (PP.myRun)
                    while (PP.myRun.RunStatus.Count > 0) //If there are any status messages in the queue, print them to the run console
                        runLog.AppendText(PP.myRun.RunStatus.Dequeue() + "\r\n");

                lblTimeInRun.Text = (DateTime.Now - runStart).TotalSeconds.ToString("0"); //Keep the time we have been running updated
                lblSpillsNum.Text = spill_num.ToString("0");
                if (first_spill_taken) //If we already took our first spill, we can now keep the spill timer updated
                    lblTimeInSpill.Text = (DateTime.Now - PP.myRun.timeLastSpill).TotalSeconds.ToString("0"); //update spill timer

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
            if (PP.FEB_clients != null)
            {
                if (SpillTimer.Enabled) //Stop the timer if it was already running
                    SpillTimer.Enabled = false;

                if (PP.myRun != null) //If a run already exists, orphan it so it gets garbage collected
                    PP.myRun = null;

                //if ((PP.FEB1.ClientOpen && chkFEB1.Checked) || (PP.FEB2.ClientOpen && chkFEB2.Checked))// && (PP.WC.ClientOpen && chkWC.Checked))
                int num_active_clients = PP.FEB_clients.Count(x => x.ClientOpen);
                if (num_active_clients > 0) //Check if any client is open
                {
                    //btnPrepare.Enabled = false;

                    //WC_client.check_status(out bool inspill, out string num_trig, out string time);
                    //while (inspill) //in case we started prep while we are in a spill
                    //{
                    //    System.Threading.Thread.Sleep(250);
                    //    WC_client.check_status(out inspill, out num_trig, out time);
                    //    Application.DoEvents();
                    //}

                    PP.myRun = new Run(num_active_clients);
                    validateParseChkBox.Enabled = true;

                    waiting_for_data = false;
                    first_spill_taken = false;
                    spill_num = 0;
                    while (SpillStatusTable.Controls.Count > 4) //dump anything that was in the table down to just the 4 headers
                        SpillStatusTable.Controls.RemoveAt(SpillStatusTable.Controls.Count-1);
                    //SpillStatusTable.RowCount = PP.Num_FEB_clients;
                    for (int feb = 0; feb < PP.Num_FEB_clients; feb++)
                    {
                        spill_trig_num[feb] = 0;
                        SpillStatusTable.Controls.Add(new System.Windows.Forms.Label() { Name = "lblFebCol_FEB" + feb, Text = "FEB" + feb, Dock = DockStyle.Fill, TextAlign = ContentAlignment.TopCenter });
                        SpillStatusTable.Controls.Add(new System.Windows.Forms.Label() { Name = "lblSpillStatusFEB" + feb, Text = "", Dock = DockStyle.Fill, TextAlign = ContentAlignment.TopCenter });
                        SpillStatusTable.Controls.Add(new System.Windows.Forms.Label() { Name = "lblLastSpillTrigsFEB" + feb, Text = "0", Dock = DockStyle.Fill, TextAlign = ContentAlignment.TopCenter });
                        SpillStatusTable.Controls.Add(new System.Windows.Forms.Label() { Name = "lblTotalTrigsFEB" + feb, Text = "0", Dock = DockStyle.Fill, TextAlign = ContentAlignment.TopCenter });
                    }


                    //lblFEB1Spill.Text = "0";
                    //lblFEB1_TotTrig.Text = "0";
                    //lblFEB2_TotTrig.Text = "0";
                    //lblWC_TotTrig.Text = "0";
                    //lblFEB1TrigNum.Text = "0";
                    //lblFEB2TrigNum.Text = "0";
                    //lblWCTrigNum.Text = "0";
                    lblTimeInRun.Text = "0";
                    lblTimeInSpill.Text = "0";
                    lblTxtRunName.Text = PP.myRun.run_name;
                    foreach (Mu2e_FEB_client feb in PP.FEB_clients)
                        feb.GetReady(); //Prep the FEB

                    SpillTimer.Enabled = true;

                    btnStartRun.Enabled = true;
                }
                else
                { SpillTimer.Enabled = false; PP.myRun = null; MessageBox.Show("Are there any active and open clients?"); }

            }
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
                btnStartRun.Enabled = false;
                btnStopRun.Enabled = true;
                btnPrepare.Enabled = false;
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
                        List<System.Net.Sockets.Socket> sockets = new List<System.Net.Sockets.Socket>();// = { PP.FEB1.TNETSocket, PP.FEB2.TNETSocket };
                        foreach(Mu2e_FEB_client feb in PP.FEB_clients)
                            sockets.Add(feb.TNETSocket);

                        foreach (System.Net.Sockets.Socket socket in sockets)
                        {
                            if (socket != null)
                            {
                                if (socket.Available > 0) //discard any junk before asking it about the current settings
                                {
                                    buff = new byte[socket.Available];
                                    socket.Receive(buff);
                                }
                                socket.Send(stab); //ask it about the current settings for the first two FPGAs
                                System.Threading.Thread.Sleep(10);
                                buff = new byte[socket.Available];
                                socket.Receive(buff);
                                stabStream.WriteLine(Encoding.ASCII.GetString(buff));
                            }                            
                        }
                    }
                }
                catch { PP.myRun.UpdateStatus("Trouble saving FEB settings to file!"); }
            }
        }

        private void BtnStopRun_Click(object sender, EventArgs e)
        {
            if (PP.myRun != null)
            {
                btnStopRun.Enabled = false;
                btnPrepare.Enabled = true;
                waiting_for_data = false;
                first_spill_taken = false;
                PP.myRun.UpdateStatus("RUN STOPPING");
                PP.myRun.DeactivateRun();
                //timer1.Enabled = false;
            }
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
            if(ModuleQALabels == null)             //Create labels for the Module QA Tab
                ModuleQALabels = new System.Windows.Forms.Label[2][]; //One collection of 64 labels for each FEB
            for (uint feb = 0; feb < 2; feb++)
            {
                if(ModuleQALabels[feb] == null)
                    ModuleQALabels[feb] = new System.Windows.Forms.Label[64]; //64 channels on an FEB
                for (uint channel = 0; channel < 64; channel++)
                {
                    if (ModuleQALabels[feb][channel] == null)
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
                    else //reset the label text
                    {
                        ModuleQALabels[feb][channel].Text = (channel).ToString() + ":\n";
                        ModuleQALabels[feb][channel].Refresh();
                    }
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
            if (activeFEB != null)
            {
                if (activeFEB.ClientOpen)
                {
                    if (qaDiCounterMeasurementTimer.Enabled)
                    {
                        qaDiCounterMeasurementTimer.Enabled = false;
                        PP.qaDicounterMeasurements.TurnOffBias();
                        PP.qaDicounterMeasurements.Purge();
                        qaStartButton.Text = "Auto Data";
                        qaStartButton.BackColor = SystemColors.Control;
                        qaStartButton.Update();
                        lightCheckGroup.Enabled = true;
                        FEBSelectPanel.Enabled = true;
                        dicounterNumberTextBox.Enabled = true;
                    }
                    else
                    {
                        //qaStartButton.Enabled = false;  //prevents multiple clicks of the buttons
                        qaStartButton.Text = "STOP";
                        qaStartButton.BackColor = Color.Red;
                        qaStartButton.Update();
                        using (System.Media.SoundPlayer soundPlayer = new System.Media.SoundPlayer(Environment.GetFolderPath(Environment.SpecialFolder.Windows) + "\\media\\chord.wav"))// "C:\\Windows\\media\\Windows Proximity Notification.wav"))
                        {
                            soundPlayer.Play();
                        }
                        FEBSelectPanel.Enabled = false;
                        autoThreshBtn.Enabled = false;
                        lightCheckResetThresh.Enabled = false;
                        lightCheckBtn.Enabled = false;
                        //string[] chanOuts = new string[qaDiButtons.Length];
                        autoDataProgress.Maximum = qaDiButtons.Length; //set the max of the progress bar

                        if (PP.qaDicounterMeasurements == null)
                            PP.qaDicounterMeasurements = new CurrentMeasurements(activeFEB, Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\Google Drive\\CRV Fabrication Documents\\Data\\QA\\Dicounter Source Testing\\ScanningData_" + qaOutputFileName.Text + ".txt");
                        else
                        {
                            PP.qaDicounterMeasurements.Purge();
                            PP.qaDicounterMeasurements.ChangeClient(activeFEB);
                        }

                        foreach (var btn in qaDiButtons) { if (!btn.Checked) { btn.BackColor = Color.Green; btn.Update(); } } //Reset all active channel indicators to green


                        currentChannel = 0; //Set the current channel being measured to 0
                        dicounterNumberTextBox.Enabled = false;
                        lightCheckGroup.Enabled = false;

                        activeFEB.SetV(Convert.ToDouble(qaBias.Text)); //Turn on the bias

                        qaDiCounterMeasurementTimer.Enabled = true;
                    }

                    ////Data are written to the Google Drive, CRV Fabrication Documents folder ScanningData, subfolder DicounterQA
                    ////'using' will ensure the writer is closed/destroyed if the scope of the structure is left due to code-completion or a thrown exception
                    //using (StreamWriter writer = File.AppendText("C:\\Users\\Boi\\Desktop\\ScanningData_test.txt"))//"C:\\Users\\FEB-Laptop-1\\Google Drive\\CRV Fabrication Documents\\Data\\QA\\Dicounter Source Testing\\ScanningData_" + qaOutputFileName.Text + ".txt")) //The output file
                }
            }
        }

        private void AutoThreshBtn_Click(object sender, EventArgs e)
        {
            if (activeFEB != null)
            {
                if (activeFEB.ClientOpen)
                {

                    if (LightCheckMeasurementTimer.Enabled)
                    {
                        LightCheckMeasurementTimer.Enabled = false;
                        //PP.lightCheckMeasurements.TurnOffBias();
                        PP.lightCheckMeasurements.Purge();
                        autoThreshBtn.Text = "Auto Thresh";
                        autoThreshBtn.BackColor = SystemColors.Control;
                        autoThreshBtn.Update();
                        auto_thresh_enabled = false;
                        lightModuleLabel.Enabled = true;
                        lightModuleLayer.Enabled = true;
                        lightModuleSide.Enabled = true;
                        dicounterQAGroup.Enabled = true;
                        lightCheckResetThresh.Enabled = true;
                        lightCheckBtn.Enabled = true;
                        FEBSelectPanel.Enabled = true;

                    }
                    else
                    {
                        //Allow the button to switch to being a stop for the threshold finding
                        autoThreshBtn.Text = "STOP";
                        autoThreshBtn.BackColor = Color.Red;
                        autoThreshBtn.Update();
                        FEBSelectPanel.Enabled = false;

                        if (PP.lightCheckMeasurements == null) //If the measurement object is not yet created, create it now
                            PP.lightCheckMeasurements = new CurrentMeasurements(activeFEB, Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\Module.txt"); //"C:\\Users\\Boi\\Desktop\\Module.txt");
                        else //Otherwise, purge any data that was present
                        {
                            PP.lightCheckMeasurements.Purge();
                            PP.lightCheckMeasurements.ChangeClient(activeFEB);
                        }

                        foreach (var btn in lightButtons) { if (!btn.Checked) { btn.BackColor = Color.Green; btn.Update(); } } //Reset all active channels

                        currentChannel = 0;
                        lightModuleLabel.Enabled = false;
                        lightModuleLayer.Enabled = false;
                        lightModuleSide.Enabled = false;
                        dicounterQAGroup.Enabled = false;
                        lightCheckResetThresh.Enabled = false;
                        lightCheckBtn.Enabled = false;
                        auto_thresh_enabled = true;

                        //activeFEB.SetVAll(Convert.ToDouble(qaBias.Text));

                        LightCheckMeasurementTimer.Enabled = true;
                    }
                }
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
            if (activeFEB != null)
            {
                if (activeFEB.ClientOpen)
                {
                    if (LightCheckMeasurementTimer.Enabled)
                    {
                        LightCheckMeasurementTimer.Enabled = false;
                        //PP.lightCheckMeasurements.TurnOffBias();
                        PP.lightCheckMeasurements.Purge();
                        lightCheckBtn.Text = "Light Check";
                        lightCheckBtn.BackColor = SystemColors.Control;
                        lightCheckBtn.Update();
                        lightModuleLabel.Enabled = true;
                        lightModuleLayer.Enabled = true;
                        lightModuleSide.Enabled = true;
                        dicounterQAGroup.Enabled = true;
                        lightCheckResetThresh.Enabled = true;
                        autoThreshBtn.Enabled = true;
                        lightWriteToFileBox.Enabled = true;
                        FEBSelectPanel.Enabled = true;

                    }
                    else
                    {
                        //Allow the button to switch to being a stop for the light tight checking
                        lightCheckBtn.Text = "STOP";
                        lightCheckBtn.BackColor = Color.Red;
                        lightCheckBtn.Update();

                        FEBSelectPanel.Enabled = false;

                        if (PP.lightCheckMeasurements == null) //If the measurement object is not yet created, create it now
                            PP.lightCheckMeasurements = new CurrentMeasurements(activeFEB, Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\Module.txt");// "C:\\Users\\Boi\\Desktop\\Module.txt");
                        else //Otherwise, purge any data that was present
                        {
                            PP.lightCheckMeasurements.Purge();
                            PP.lightCheckMeasurements.ChangeClient(activeFEB);
                        }

                        foreach (var btn in lightButtons) { if (!btn.Checked) { btn.BackColor = Color.Green; btn.Update(); } } //Reset all active channels

                        currentChannel = 0;
                        lightModuleLabel.Enabled = false;
                        lightModuleLayer.Enabled = false;
                        lightModuleSide.Enabled = false;
                        dicounterQAGroup.Enabled = false;
                        lightCheckResetThresh.Enabled = false;
                        autoThreshBtn.Enabled = false;
                        lightWriteToFileBox.Enabled = false;

                        //activeFEB.SetVAll(Convert.ToDouble(qaBias.Text));

                        LightCheckMeasurementTimer.Enabled = true;
                    }
                }
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
            double fitThresh = 8.5;//7.5;
            double integralThresh = 2E5;
            double flashgateThresh = 8E6;
            double flashGateDifferenceThresh = 20;
            double ledDifferenceThresh = 2000;// 50; //Initial testing value
            double cmbLedIntThresh = 1000;
            uint ledFlasherIntensity = 0xFFF; //14V //0x400; //3.5V
            double maxUndershootThresh = 30; //Value is in ADC
            int upperBinInt = 1000;
            //Check that an FEB client exists, otherwise, don't bother setting up the pulser or trying to get data
            if (activeFEB != null)
            {
                if (activeFEB.ClientOpen)
                {
                    FEBSelectPanel.Enabled = false;
                    cmbInfoBox.Text = ""; cmbInfoBox.Refresh();

                    #region Registers
                    //These registers are required for setting up the FEB to record triggered events and the flashing of the LED.
                    //The FEB is responsible for controlling the pulser via the FEB's GPO port into the pulser's external clock port
                    Mu2e_Register.FindAddr(0x300, ref activeFEB.arrReg, out Mu2e_Register flashGateControlAllReg);
                    Mu2e_Register.FindAddr(0x301, ref activeFEB.arrReg, out Mu2e_Register flashGateTurnOnReg);
                    Mu2e_Register.FindAddr(0x302, ref activeFEB.arrReg, out Mu2e_Register flashGateTurnOffReg);
                    Mu2e_Register.FindAddr(0x303, ref activeFEB.arrReg, out Mu2e_Register trigControlReg); //Trigger control register
                    Mu2e_Register.FindAddr(0x304, ref activeFEB.arrReg, out Mu2e_Register hitPipelineDelayReg); //Hit Pipeline Delay register
                    Mu2e_Register.FindAddr(0x305, ref activeFEB.arrReg, out Mu2e_Register sampleLengthReg); //Sample length for each event/trigger
                    Mu2e_Register.FindAddr(0x307, ref activeFEB.arrReg, out Mu2e_Register testPulseFreqReg); //Onboard test pulser frequency
                    Mu2e_Register.FindAddr(0x308, ref activeFEB.arrReg, out Mu2e_Register spillDurReg); //Spill duration register
                    Mu2e_Register.FindAddr(0x309, ref activeFEB.arrReg, out Mu2e_Register interSpillDurReg); //Interspill duration register
                    Mu2e_Register.WriteReg(0x2, ref flashGateControlAllReg, ref activeFEB.client); //Set the CMB pulse routing to the flash gate (LED flasher will create interference on CMB)
                    Mu2e_Register.WriteReg(0x4, ref flashGateTurnOnReg, ref activeFEB.client); //Set the turn on time to 4 counts
                    Mu2e_Register.WriteReg(0x50, ref flashGateTurnOffReg, ref activeFEB.client); //Set the turn off time to 80 counts
                    Mu2e_Register.WriteReg(0x80, ref sampleLengthReg, ref activeFEB.client); //Set the sample length to 128 samples
                    Mu2e_Register.WriteReg(0x5, ref spillDurReg, ref activeFEB.client); //Set the spill duration to 5 seconds
                    Mu2e_Register.WriteReg(0x0, ref testPulseFreqReg, ref activeFEB.client); //Set test pulser frequency to zero, this allows external triggering from LEMO
                    Mu2e_Register.WriteReg(0xA, ref interSpillDurReg, ref activeFEB.client); //Set the interspill duration for 10 seconds

                    Mu2e_Register[] muxReg = Mu2e_Register.FindAllAddr(0x20, ref activeFEB.arrReg);//Get the mux register so it can be set to 0 so as to not interfere with histogramming
                    Mu2e_Register.WriteAllReg(0x0, ref muxReg, ref activeFEB.client); //Set all the mux to 0 so as to not interfere with histogramming
                    Mu2e_Register[] flashGateControlReg = Mu2e_Register.FindAllAddr(0x18, ref activeFEB.arrReg); //Flash Gate Control registers
                    //Mu2e_Register.WriteAllReg(0x2, ref flashGateControlReg, ref activeFEB.client); //Set the CMB Pulse routing to the Flash Gate (to LED flasher will create interference on CMB)
                    Mu2e_Register[] controlStatusReg = Mu2e_Register.FindAllAddr(0x00, ref activeFEB.arrReg);
                    Mu2e_Register.WriteAllReg(0x20, ref controlStatusReg, ref activeFEB.client); //issue a general reset for each FPGA
                    Mu2e_Register[][] gainControlReg = Mu2e_Register.FindAllAddrRange(0x46, 0x47, ref activeFEB.arrReg);
                    Mu2e_Register.WriteAllRegRange(0x300/*0x300*/, ref gainControlReg, ref activeFEB.client); //Set the gain of all AFE chips on all FPGAs to the same value
                    #endregion Registers

                    System.Net.Sockets.Socket febSocket = activeFEB.TNETSocket_prop; //Declare and define FEB socket variable
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
                    activeFEB.SendStr("cmb"); //Ask the FEB about the CMBs
                    System.Threading.Thread.Sleep(100); //Wait for the FEB to get the message and respond
                    activeFEB.ReadStr(out string cmbMsg, out int r);
                    if (cmbMsg.Length > 32) //If it got 'all' the info (32 is just the header: "Ch    DegC     ROM_ID  (Errs=0)")
                    {
                        try
                        {
                            string[] tok = cmbMsg.Split(new string[] { " ", Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

                            if (String.Equals(tok[1], "DegC")) //Preproduction FEB 'cmb' format
                            {
                                for (int i = 4; i + 2 < tok.Length; i += 3)
                                {
                                    int cmb;
                                    try { cmb = Convert.ToInt16(tok[i]) - 1; } //start numbering at 0 instead of 1 
                                    catch { continue; } //in case there is an 'invalid' cmb number, skip.

                                    cmbs[cmb].num = cmb;

                                    try
                                    {
                                        cmbs[cmb].temp = Convert.ToDouble(tok[i + 1]);
                                        cmbs[cmb].rom_id = tok[i + 2]; //TODO: Determine what the format of the rom_id should be so it can be checked for validity
                                        cmbs[cmb].flagged = false;
                                    }
                                    catch (FormatException)
                                    {
                                        cmbs[cmb].temp = -1;
                                        cmbs[cmb].flagged = true;
                                        cmbs[cmb].failureType = (int)CMB.Failure.TempRom;
                                    }
                                }

                                //Loop over all the CMBs in the list and flag all the ones that weren't shown by the FEB
                                for (int cmb = 0; cmb < cmbs.Length; cmb++)
                                {
                                    if (String.IsNullOrEmpty(cmbs[cmb].rom_id))
                                    {
                                        cmbs[cmb].num = cmb;
                                        cmbs[cmb].flagged = true;
                                    }
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
                    activeFEB.SetVAll(Convert.ToDouble(cmbBias.Text));
                    #endregion BiasWait

                    //return;
                    double[] pedestals = new double[64]; //pedestal for each channel
                    double[] gains = new double[64]; //gain for each channel (adc/pe)      

                    ROOTNET.NTH1I[] peHistos = new ROOTNET.NTH1I[64]; //Histograms used to determine response to LED and gains
                    ROOTNET.NTGraph[] peCalibs = new ROOTNET.NTGraph[64]; //2D Plots used to compute gains/pedestal
                    ROOTNET.NTSpectrum peakFinder = new ROOTNET.NTSpectrum(20, 2); //Peak finder, set it to find a maximum of 10 peaks (pedestal + 9 peaks)
                    HistoHelper hist_helper = new HistoHelper(ref activeFEB, 0xFFE);//0x400); //Tune later
                    ROOTNET.NTH1I[] histos_temp = new ROOTNET.NTH1I[8]; //temporary spot to store the incoming histograms from the histohelper
                    ROOTNET.NTF1 gainFit = new ROOTNET.NTF1("gainfit", "pol1"); //linear fit for computing the gain
                    ROOTNET.NTF1 bulkRespFit = new ROOTNET.NTF1("respfit", "gaus"); //Gaussian fit for bulk of LED Response
                    cmbInfoBox.Text = "LED/Calibrating"; cmbInfoBox.Refresh();

                    Mu2e_Register.WriteReg(0x100, ref trigControlReg, ref activeFEB.client); //Enable the on-board test pulser, output of this signal will be delivered to external pulser to flash LED
                    Mu2e_Register.WriteReg(0x270000 /*~ 95.25 KHz , was 0x5E5E5E (~230khz) TOO HIGH*/, ref testPulseFreqReg, ref activeFEB.client); //Set the on-board test pulser's frequency to ~230kHz
                    Mu2e_Register.WriteReg(0x4, ref hitPipelineDelayReg, ref activeFEB.client); //Set the hit pipeline delay to a low value (4 x 12.56ns)

                    for (uint channel = 0; channel < 64; channel++)
                    {
                        int cmbNum = (int)channel / 4; //spans from 0-15
                        if (peHistos[channel] == null && !(cmbs[cmbNum].flagged)) //skip the channels that have already been histogrammed (due to the two channel histograms return from the FEB), and skip any channels on flagged cmbs
                        {
                            histos_temp = hist_helper.GetHistogram(channel, 1, "extLED");
                            //uint[] channels = { channel, Convert.ToUInt32(histos_temp[1].GetTitle()) }; //Lazily grab the other channel's label from the histogram title...
                            uint[] channels = new uint[8]; //Could get the channel from the histogram title, or compute it here. Not really sure which is safer...
                            for (int ch = 0; ch < channels.Length; ch++)
                                channels[ch] = Convert.ToUInt16(histos_temp[ch].GetTitle());
                                //channels[ch] = (uint)((ch * 8) + (channel%16));
                            System.Console.WriteLine("Histo Chans: {0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}", channels[0], channels[1], channels[2], channels[3], channels[4], channels[5], channels[6], channels[7]);

                            for (int hist = 0; hist < 8; hist++)//Loop over each of the 8 histograms
                            {
                                cmbNum = (int)channels[hist] / 4; //Recompute the cmb number since we will get histograms from other cmbs along with our requested channel
                                if (cmbs[cmbNum].flagged) //Skip if flagged
                                    continue;

                                histos_temp[hist].Title += "_CMB_" + cmbNum.ToString();
                                histos_temp[hist].Name += "_" + cmbs[cmbNum].rom_id;

                                peHistos[channels[hist]] = histos_temp[hist];
                                peCalibs[channels[hist]] = new ROOTNET.NTGraph();
                                int peaksFound = peakFinder.Search(peHistos[channels[hist]], 1.4, "nobackground", 0.00001); //Don't try and estimate background, and set the threshold to only include pedestal, 1st, and 2nd PE
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
                                pedestals[channels[hist]] = peakPositions[0]; //first entry should be pedestal
                                List<int> fittingRanges = new List<int>();
                                if (peakPositions.Count < 2)
                                {
                                    cmbs[cmbNum].flagged = true;
                                    cmbs[cmbNum].failureType = (int)CMB.Failure.SiPMResp;
                                    cmbInfoLabels[cmbNum][11].Text = "GainFail";
                                    SetRowColor(cmbNum, Color.MistyRose);
                                    UpdateCMBInfoLabel(cmbNum);
                                    continue;
                                }
                                float gain_estimate_thresh = (peakPositions[1] - peakPositions[0]) * 1.5f; //Set threshold at ~1.5pe difference
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
                                peCalibs[channels[hist]].SetName("gain_est_Ch" + channels[hist].ToString() + "_" + cmbs[cmbNum].rom_id);
                                peCalibs[channels[hist]].GetXaxis().SetTitle("PE");
                                peCalibs[channels[hist]].GetYaxis().SetTitle("ADC");
                                peHistos[channels[hist]].Fit(bulkRespFit, "CRQ+", "", gains[channels[hist]] * fitThresh, upperBinInt); //Fit from 7.5 PE (in ADC) up to (max-2) of histogram (will need to adjust later)

                                //Display info for gain and pedestal
                                cmbInfoLabels[cmbNum][(channels[hist] % 4) * 2 + 3].Text = Math.Floor(pedestals[channels[hist]]).ToString();
                                cmbInfoLabels[cmbNum][(channels[hist] % 4) * 2 + 4].Text = Math.Floor(gains[channels[hist]]).ToString();
                                UpdateCMBInfoLabel(cmbNum);

                                //compare fit response to 'lookup value'
                                //For gaus fit: p0 is amplitude, p1 is mean, p2 is sigma
                                //if (PercentDifference(bulkRespFit.GetParameter(1), avgResp[channels[hist]] /*table value*/) > ledDifferenceThresh) //if the differnce is greater than 20%

                                int lowerIntegralBin = (int)(pedestals[channels[hist]] + gains[channels[hist]] * fitThresh); //truncate the value of 7.5PE for the bin # (since all histograms start at 0 and have 512 bins)
                                double ledIntegral = peHistos[channels[hist]].Integral(lowerIntegralBin, 1000); //512 upper bound because all histograms have 512 bins
                                if (ledIntegral < integralThresh)
                                {
                                    cmbs[cmbNum].flagged = true;
                                    cmbs[cmbNum].failureType = (int)CMB.Failure.SiPMResp;
                                    cmbInfoLabels[cmbNum][11].Text = cmbs[cmbNum].FailType();
                                    SetRowColor(cmbNum, Color.MistyRose);
                                    UpdateCMBInfoLabel(cmbNum);
                                }
                                else if (updateFilesChkBox.Checked) //Update file here
                                    channelAvgHist[channels[hist]].Fill(bulkRespFit.GetParameter(1)); //Add mean response to histogram
                            }
                        }
                    }

                    //Write the updated response histograms
                    if (updateFilesChkBox.Checked)
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
                                //histo.Delete();
                            }
                        foreach (var plot in peCalibs)
                            if (plot != null)
                            {
                                plot.Write();
                                plot.Delete();
                            }
                        //histo_file.Close(); ///REMOVE
                    }

                    //activeFEB.SetVAll(0);
                    //FEBSelectPanel.Enabled = true;
                    //return;

                    #region FlashGate
                    cmbInfoBox.Text = "Testing flashgate"; cmbInfoBox.Refresh();

                    ROOTNET.NTH1I[] flashHistos = new ROOTNET.NTH1I[64]; //Histograms used to determine response to LED (flashgate)

                    //Set register for flashgate (0x3 enables flash gate and sets the routing to the flash gate (not LED)
                    Mu2e_Register.WriteReg(0x3, ref flashGateControlAllReg, ref activeFEB.client);

                    for (uint channel = 0; channel < 64; channel++) //Loop over the channels, re-histogram response to LED flashing, check that response is low
                    {
                        int cmbNum = (int)channel / 4; //spans from 0-15
                        if (flashHistos[channel] == null && !(cmbs[cmbNum].flagged)) //skip the channels that have already been histogrammed (due to the two channel histograms return from the FEB), and skip any channels on flagged cmbs
                        {
                            histos_temp = hist_helper.GetHistogram(channel, 1, "FlashGate");
                            uint[] channels = new uint[8]; //Could get the channel from the histogram title, or compute it here. Not really sure which is safer...
                            for (int ch = 0; ch < channels.Length; ch++)
                                channels[ch] = Convert.ToUInt16(histos_temp[ch].GetTitle());
                            //channels[ch] = (uint)((ch * 8) + (channel % 16));
                            System.Console.WriteLine("Histo Chans: {0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}", channels[0], channels[1], channels[2], channels[3], channels[4], channels[5], channels[6], channels[7]);

                            for (int hist = 0; hist < 8; hist++)//Loop over each of the 8 histograms
                            {
                                cmbNum = (int)channels[hist] / 4; //Recompute the cmb number since we will get histograms from other cmbs along with our requested channel
                                if (cmbs[cmbNum].flagged) //Skip if flagged
                                    continue;

                                histos_temp[hist].Title += "_CMB_" + cmbNum.ToString();
                                histos_temp[hist].Name += "_" + cmbs[cmbNum].rom_id;

                                flashHistos[channels[hist]] = histos_temp[hist];
                                //flashHistos[channels[hist]].Fit(bulkRespFit, "CRQ+", "", gains[channels[hist]] * 7.5, 512); //Fit from 7.5 PE (in ADC) up to max of histogram (will need to adjust later)
                                //int lowerIntegralBin = (int)(pedestals[channels[hist]] + gains[channels[hist]] * fitThresh); //truncate the value of 7.5PE for the bin # (since all histograms start at 0 and have 512 bins)
                                //double flashIntegral = flashHistos[channels[hist]].Integral(lowerIntegralBin, 512); //512 upper bound because all histograms have 512 bins
                                //double ledIntegral = peHistos[channels[hist]].Integral(lowerIntegralBin, 512); //Also compute integral for led histogram, so we can see if the response to LED has diminished

                                if (flashHistos[channels[hist]].GetBinContent(1024) + flashHistos[channels[hist]].GetBinContent(1)  < flashgateThresh)//PercentDifference(flashIntegral, ledIntegral) < flashGateDifferenceThresh) //if the differnce is less than flashGateDifferenceThresh, flashgate must not be working...
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

                    Mu2e_Register.WriteReg(0x2, ref flashGateControlAllReg, ref activeFEB.client); //Turn off flash gate and keep routing to the Flash Gate (to LED flasher will create interference on CMB)

                    if (outputOpened)
                    {
                        foreach (var histo in flashHistos)
                            if (histo != null)
                            {
                                histo.Write();
                                histo.Delete();
                            }
                        //histo_file.Close();
                    }

                    #endregion FlashGate
                    //activeFEB.SetVAll(0);
                    //FEBSelectPanel.Enabled = true;
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
                    // [ NO ] Tune LED flasher intensity to match closely to external LED flash -> could make evaluation easier (?)
                    //

                    Mu2e_Register[][] ledFlasherIntensityRegs = Mu2e_Register.FindAllAddrRange(0x40, 0x43, ref activeFEB.arrReg); //Get the led flasher intensity registers, for all FPGAs
                    Mu2e_Register.WriteReg(0x500, ref trigControlReg, ref activeFEB.client); //Keep the on-board pulser enabled, but set the GPO to only output a single pulse at the beginning and end of the spill gate (which should not turn on while recording histograms AFAIK)
                    Mu2e_Register.WriteAllRegRange(0x0, ref ledFlasherIntensityRegs, ref activeFEB.client); //Set all cmb flasher intensities to 0
                    Mu2e_Register.WriteAllReg(0x2, ref flashGateControlReg, ref activeFEB.client); //Set all CMBs to flash gate routing

                    System.Threading.Thread.Sleep(250); //wait for the pulse to go by...

                    ROOTNET.NTH1I[] ledHistos = new ROOTNET.NTH1I[64]; //histograms for cmb-led flashers 

                    for (uint flash_cmb = 0; flash_cmb < 8; flash_cmb++) //flash_cmb is only used to denote cmbs on HALF the box
                    {
                        //We will run as follows: 0 reads 1, 1 reads 0, 2 reads 3, 3 reads 2
                        //Two FPGAs will flash simultaneously and be read by the other two FPGAs, for maximum efficiency
                        uint[] flashing_cmb = { flash_cmb, flash_cmb + 8 }; //flashing_cmb is used to denote the actual cmb numbers of the cmbs that are flashing

                        uint fpga = flash_cmb / 4; //compute this once to make it easier for writing to the appropriate registers
                        Mu2e_Register.WriteReg(0x1, ref flashGateControlReg[fpga], ref activeFEB.client); //Set LED to flash for the requested CMB
                        Mu2e_Register.WriteReg(0x1, ref flashGateControlReg[fpga + 2], ref activeFEB.client); //Set the LED to flash for the CMB on the other side of the box too
                        Mu2e_Register.WriteReg(ledFlasherIntensity, ref ledFlasherIntensityRegs[fpga][flashing_cmb[0]%4], ref activeFEB.client); //Set the CMB on the requested FPGA to flash at ledFlasherIntensity
                        Mu2e_Register.WriteReg(ledFlasherIntensity, ref ledFlasherIntensityRegs[fpga + 2][flashing_cmb[1]%4], ref activeFEB.client); //Set the CMB on the other FPGA to flash at ledFlasherIntensity

                        uint[] readcmb = { flashing_cmb[0], flashing_cmb[1] }; //used to remember which CMBs are reading the flashing CMBs

                        if (flash_cmb > 3)
                        {
                            readcmb[0] %= 4;
                            readcmb[1] = (readcmb[1] % 4) + 8;
                        }
                        else
                        {
                            readcmb[0] += 4;
                            readcmb[1] += 4;
                        }


                        //For only the outer channels on each CMB (ch. 0 and ch. 3), histogram the flashing of 
                        for (uint ch = 0; ch < 4; ch += 3)
                        {
                            uint[] readchannel = { (readcmb[0] * 4) + ch, (readcmb[1] * 4) + ch }; //the channels expected to read the flashing cmbs
                            uint[] readchannel_backup = { readchannel[0], readchannel[1] }; //backup channels in case the other channels were on flagged cmbs
                            if ((readchannel[0] % 16) >= 8) //if we are over halfway through the reading FPGA, we need to set the appropriate offset for the backup channel
                            {
                                readchannel_backup[0] -= 8;
                                readchannel_backup[1] -= 8;
                            }
                            else
                            {
                                readchannel_backup[0] += 8;
                                readchannel_backup[1] += 8;
                            }

                            //If the flashing cmb is not flagged AND either the reading cmb or the backup is not flagged, OR the flashing cmb on the other side of the box is not flagged AND either the reading cmb or the backup is not flagged, continue
                            if ((!(cmbs[flashing_cmb[0]].flagged) && (!(cmbs[readchannel[0] / 4].flagged) || !(cmbs[readchannel_backup[0] / 4].flagged))) || (!(cmbs[flashing_cmb[1]].flagged) && (!(cmbs[readchannel[1] / 4].flagged) || !(cmbs[readchannel_backup[1] / 4].flagged))))
                            { 
                                histos_temp = hist_helper.GetHistogram(readchannel[0], 1, "intLEDforCMB_");

                                for (uint fcmb = 0; fcmb < 2; fcmb++)
                                {
                                    uint thiscmb = flashing_cmb[fcmb];
                                    if (cmbs[thiscmb].flagged) //skip it if it was already flagged
                                        continue;

                                    if ((!cmbs[readchannel[fcmb] / 4].flagged) || (!cmbs[readchannel_backup[fcmb] / 4].flagged)) //should we not be able to read the flashing cmb with the expected or backup channel, flag it as untested
                                    {
                                        uint channel_select = readchannel[fcmb];
                                        uint fch = (thiscmb * 4) + ch;

                                        if (!cmbs[readchannel[fcmb] / 4].flagged) //try and read the flashing LED with the expected channel
                                        {
                                            ledHistos[fch] = histos_temp[readchannel[fcmb] / 8];
                                        }
                                        else if (!cmbs[readchannel_backup[fcmb] / 4].flagged) //should there be a problem with the expected channel, we can try and read using a backup
                                        {
                                            channel_select = readchannel_backup[fcmb];
                                            ledHistos[fch] = histos_temp[readchannel_backup[fcmb] / 8];
                                        }

                                        ledHistos[fch].Name += Convert.ToString(thiscmb) + "_" + cmbs[thiscmb].rom_id;

                                        int lowerIntegralBin = (int)(pedestals[channel_select] + (gains[channel_select] * fitThresh)); //truncate the value of 7.5PE for the bin # (since all histograms start at 0 and have 512 bins)
                                        double cmbLedIntegral = ledHistos[fch].Integral(lowerIntegralBin, upperBinInt); //Upper bound set
                                        if (cmbLedIntegral < cmbLedIntThresh)
                                        {
                                            cmbs[thiscmb].flagged = false; //not really a disqualifying failure (i.e. doesn't stop us from using the CMB to look at other CMBs), but not good
                                            cmbs[thiscmb].failureType = (int)CMB.Failure.LED;
                                            cmbInfoLabels[thiscmb][11].Text = cmbs[thiscmb].FailType();
                                            SetRowColor((int)thiscmb, Color.MistyRose);
                                            UpdateCMBInfoLabel((int)thiscmb);
                                        }
                                    }
                                    else
                                    {
                                        cmbs[thiscmb].flagged = false; //Also not a real failure, just needs to be tested again or manually tested
                                        cmbs[thiscmb].failureType = (int)CMB.Failure.LEDUntested;
                                        cmbInfoLabels[thiscmb][11].Text = cmbs[thiscmb].FailType();
                                        SetRowColor((int)thiscmb, Color.LightGoldenrodYellow);
                                        UpdateCMBInfoLabel((int)thiscmb);
                                    }
                                }
                            }
                            else
                            {
                                for (int i = 0; i < 2; i++)
                                {
                                    if (!cmbs[flashing_cmb[i]].flagged)
                                    {
                                        cmbs[flashing_cmb[i]].flagged = false; //Also not a real failure, just needs to be tested again or manually tested
                                        cmbs[flashing_cmb[i]].failureType = (int)CMB.Failure.LEDUntested;
                                        cmbInfoLabels[flashing_cmb[i]][11].Text = cmbs[flashing_cmb[i]].FailType();
                                        SetRowColor((int)flashing_cmb[i], Color.LightGoldenrodYellow);
                                        UpdateCMBInfoLabel((int)flashing_cmb[i]);
                                    }
                                }
                            }
                        }

                        Mu2e_Register.WriteAllRegRange(0x0, ref ledFlasherIntensityRegs, ref activeFEB.client); //Set all cmb flasher intensities to 0
                        Mu2e_Register.WriteAllReg(0x2, ref flashGateControlReg, ref activeFEB.client); //Set all CMBs to flash gate routing (but do not enable flashgate)
                    }


                    #region hide
                    //for (uint fpga = 0; fpga < 4; fpga++)
                    //{
                    //    for (uint cmb = 0; cmb < 4; cmb++)
                    //    {
                    //        int flashing_cmb = -1; //used to remember which CMB's LEDs we are looking at
                    //                               //We will run as follows: 0 reads 1, 1 reads 0, 2 reads 3, 3 reads 2
                    //        if (fpga == 1 || fpga == 3) //1 reads 0, 3 reads 2
                    //        {
                    //            Mu2e_Register.WriteReg(0x1, ref flashGateControlReg[fpga - 1], ref activeFEB.client); //Set LED pulse routing for single CMB
                    //            Mu2e_Register.WriteReg(ledFlasherIntensity, ref ledFlasherIntensityRegs[fpga - 1][cmb], ref activeFEB.client); //Set single CMB flasher intensity to ledFlasherIntensity
                    //            flashing_cmb = (((int)fpga - 1) * 4) + (int)cmb;
                    //        }
                    //        else //0 reads 1, 2 reads 3
                    //        {
                    //            Mu2e_Register.WriteReg(0x1, ref flashGateControlReg[fpga + 1], ref activeFEB.client); //Set LED pulse routing for single CMB
                    //            Mu2e_Register.WriteReg(ledFlasherIntensity, ref ledFlasherIntensityRegs[fpga + 1][cmb], ref activeFEB.client); //Set single CMB flasher intensity to ledFlasherIntensity
                    //            flashing_cmb = (((int)fpga + 1) * 4) + (int)cmb;
                    //        }

                    //        //For only the outer channels on each CMB (ch. 0 and ch. 3), histogram the flashing of 
                    //        for (uint ch = 0; ch < 4; ch += 3)
                    //        {
                    //            uint cmbNum = (fpga * 4) + cmb;
                    //            uint channel = (fpga * 16) + (cmb * 4) + ch;

                    //            // [X] Get histograms
                    //            // [X] Evaluate flashers

                    //            if (ledHistos[channel] == null && !(cmbs[cmbNum].flagged)) //skip flagged cmbs
                    //            {

                    //                histos_temp = hist_helper.GetHistogram(channel, 1, "intLEDforCMB_" + flashing_cmb.ToString() + "_");
                    //                ledHistos[channel] = histos_temp[0]; //"Discard" the second histogram, becuase it doesn't help us atm...
                    //                ledHistos[channel].Name += cmbs[flashing_cmb].rom_id;
                    //                for(int i = 1; i < histos_temp.Length; i++)
                    //                    histos_temp[i].Delete();
                    //                int lowerIntegralBin = (int)(pedestals[channel] + (gains[channel] * fitThresh)); //truncate the value of 7.5PE for the bin # (since all histograms start at 0 and have 512 bins)
                    //                double cmbLedIntegral = ledHistos[channel].Integral(lowerIntegralBin, upperBinInt); //Upper bound set
                    //                //double ledIntegral = peHistos[channel].Integral(lowerIntegralBin, upperBinInt); //Also compute integral for led histogram, so we can see if the response to LED has diminished

                    //                //if (PercentDifference(cmbLedIntegral, ledIntegral) > ledDifferenceThresh) //if the differnce is greater than flashGateDifferenceThresh, flashgate must not be working...
                    //                //{
                    //                //    cmbs[cmbNum].flagged = true;
                    //                //    cmbs[cmbNum].failureType = (int)CMB.Failure.LED;
                    //                //    cmbInfoLabels[cmbNum][11].Text = cmbs[cmbNum].FailType();
                    //                //    SetRowColor((int)cmbNum, Color.MistyRose);
                    //                //    UpdateCMBInfoLabel((int)cmbNum);
                    //                //}
                    //                if (cmbLedIntegral < cmbLedIntThresh)
                    //                {
                    //                    //cmbs[cmbNum].flagged = true;
                    //                    //cmbs[cmbNum].failureType = (int)CMB.Failure.LED;
                    //                    //cmbInfoLabels[cmbNum][11].Text = cmbs[cmbNum].FailType();
                    //                    //SetRowColor((int)cmbNum, Color.MistyRose);
                    //                    //UpdateCMBInfoLabel((int)cmbNum);
                    //                    cmbs[flashing_cmb].flagged = true;
                    //                    cmbs[flashing_cmb].failureType = (int)CMB.Failure.LED;
                    //                    cmbInfoLabels[flashing_cmb][11].Text = cmbs[flashing_cmb].FailType();
                    //                    SetRowColor(flashing_cmb, Color.MistyRose);
                    //                    UpdateCMBInfoLabel(flashing_cmb);
                    //                }
                    //            }
                    //        }

                    //        Mu2e_Register.WriteAllRegRange(0x0, ref ledFlasherIntensityRegs, ref activeFEB.client); //Set all cmb flasher intensities to 0
                    //        Mu2e_Register.WriteAllReg(0x2, ref flashGateControlReg, ref activeFEB.client); //Set all CMBs to flash gate routing (but do not enable flashgate)
                    //    }
                    //}
                    #endregion hide

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
                    activeFEB.SetVAll(0);
                    FEBSelectPanel.Enabled = true;
                    return;

                    #endregion CMB LED Flashers


                    //This is used later for gathering trace data for tail-cancellation evaluation
                    //uint spill_status = 0;
                    //uint spill_num = 0;
                    //uint trig_num = 0;
                    //numTrigsDisp.Text = trig_num.ToString();
                    //numTrigsDisp.Refresh();


                    Mu2e_Register.WriteReg(0x0, ref trigControlReg, ref activeFEB.client); //Disable the on-board test pulser
                    //Mu2e_Register.WriteReg(0x0, ref testPulseFreqReg, ref activeFEB.client); //Set the on-board test pulser's frequency to 0


                    //Turn off bias for SiPMs
                    //for (uint fpga = 0; fpga < 4; fpga++)
                    //    activeFEB.SetV(0.0, (int)fpga);
                    //activeFEB.SetVAll(0);
                    //return;

                    #region Undershoot Evaluation
                    //cmbInfoBox.Text = "Undershoot Evaluation"; cmbInfoBox.Refresh();

                    //febSocket.Send(sendRDB);
                    //int old_available = 0;
                    //while (febSocket.Available > old_available) //Wait until the FEB has all the data to send
                    //{
                    //    old_available = febSocket.Available;
                    //    System.Threading.Thread.Sleep(10);
                    //}
                    //byte[] rec_buf = new byte[febSocket.Available];
                    //int lret = febSocket.Receive(rec_buf, rec_buf.Length, System.Net.Sockets.SocketFlags.None);
                    ////for (int iByte = 0; iByte < lret - 1; iByte++)
                    ////    rec_buf[iByte] = rec_buf[iByte + 1]; //ignore 0x3e at the beginning of data

                    ////MessageBox.Show("CMB Evaluation\nPlease connect the LED");
                    ////Mu2e_Register.WriteReg(0x0C, ref controlStatusReg, ref activeFEB.client); //Issues a reset of the AFE deserializers on the FPGA and the MIG DDR interface
                    //Mu2e_Register.WriteReg(0x2, ref spillDurReg, ref activeFEB.client); //Set the spill duration for 2 seconds
                    //Mu2e_Register.WriteReg(0x64, ref sampleLengthReg, ref activeFEB.client); //Set the number of ADC samples to record per trigger
                    //                                                                       //for (uint fpga = 0; fpga < 4; fpga++) //Turn on bias for SiPMs
                    //                                                                       //    activeFEB.SetV(Convert.ToDouble(cmbBias.Text), (int)fpga);
                    //Mu2e_Register.WriteReg(0x300, ref trigControlReg, ref activeFEB.client); //Open the spill gate: Set trig-control register to enable board to record data for 1 spill, LED flashes during this time
                    //while (spill_status != 2) //trig_num < Convert.ToInt16(requestNumTrigs.Text))
                    //{
                    //    System.Threading.Thread.Sleep(250); //Slow down the polling of the FEB for triggers/status
                    //    activeFEB.CheckStatus(out spill_status, out spill_num, out trig_num); //Keep polling the board about how many triggers it has seen
                    //    numTrigsDisp.Text = trig_num.ToString();
                    //    numTrigsDisp.Refresh();
                    //}
                    ////Mu2e_Register.WriteReg(0x42, ref trigControlReg, ref activeFEB.client); //Stops triggering
                    //Mu2e_Register.WriteReg(0x0, ref trigControlReg, ref activeFEB.client);

                    ////receive data and unpack in memory
                    ////Write average response to a file on disk, compare response of each CMB channel to running average (since SiPMs do not change)
                    //SpillData testerData = new SpillData(); //create new SpillData object to hold incoming data from FEB

                    ////Send RDB to FEB
                    ////febSocket.Send(sendRDB);
                    ////old_available = 0;
                    ////while (febSocket.Available > old_available) //Wait until the FEB has all the data to send
                    ////{
                    ////    old_available = febSocket.Available;
                    ////    System.Threading.Thread.Sleep(250);
                    ////}
                    ////rec_buf = new byte[febSocket.Available];
                    ////lret = febSocket.Receive(rec_buf, rec_buf.Length, System.Net.Sockets.SocketFlags.None);

                    //if (TCP_receiver.ReadFeb(ref activeFEB, ref testerData, out long num_bytes))
                    //{
                    //    double[] averageUndershoot = new double[64]; //hold the average response for each channel

                    //    foreach (var tEvent in testerData.SpillEvents)
                    //    {
                    //        Mu2e_Ch[] cha = tEvent.ChanData.ToArray();
                    //        for (int chan = 0; chan < tEvent.ChNum; chan++)
                    //            averageUndershoot[chan] = 0.5 * (averageUndershoot[chan] + (pedestals[chan] - cha[chan].data.Min())); //Compute the average undershoot for each channel from all the traces
                    //    }

                    //    for (uint channel = 0; channel < 64; channel++)
                    //    {
                    //        uint cmbNum = channel / 4;
                    //        if (averageUndershoot[channel] > maxUndershootThresh)
                    //        {
                    //            cmbs[cmbNum].flagged = true;
                    //            cmbs[cmbNum].failureType = (int)CMB.Failure.LED;
                    //            cmbInfoLabels[cmbNum][11].Text = cmbs[cmbNum].FailType();
                    //            SetRowColor((int)cmbNum, Color.MistyRose);
                    //            UpdateCMBInfoLabel((int)cmbNum);
                    //        }
                    //    }
                    //}
                    //else
                    //{
                    //    MessageBox.Show("Couldn't get trace data to evaluate undershoot!");
                    //}

                    #endregion Undershoot Evaluation

                    //cmbInfoBox.Text = ""; cmbInfoBox.Refresh();

                    //Turn off bias for SiPMs
                    //activeFEB.SetVAll(0);

                    FEBSelectPanel.Enabled = true;
                }
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
            try
            {
                //Autoscroll to the end of the text box
                runLog.SelectionStart = runLog.TextLength;
                runLog.SelectionLength = 0;
                runLog.ScrollToCaret();
            }
            catch { }
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
            if (PP.FEB_clients != null)
            {
                if ((PP.FEB_clients.Count() > 0) && PP.FEB_clients.Any(x => x.ClientOpen)) //Check if any client is open
                {
                    if (moduleQAMeasurementTimer.Enabled == false)
                    {
                        InitializeModuleQATab(); //reset the labels in the table

                        if (PP.moduleQACurrentMeasurements == null) //if we didn't make a measurement object yet, do so, else purge the existing one of information
                            PP.moduleQACurrentMeasurements = new ModuleQACurrentMeasurements(PP.FEB_clients);
                        else
                        {
                            PP.moduleQACurrentMeasurements.Purge();
                            PP.moduleQACurrentMeasurements.ChangeClients(PP.FEB_clients);
                        }


                        ModuleQADarkCurrentBtn.Enabled = false; //disabled the button to prevent repeated clicks
                                                                //Loop
                                                                // take current measurements
                                                                // move source
                                                                //repeat until entire module is scanned
                                                                //Move source back to position 0
                        currentChannel = 0; //Set back to 0, controlled by measurment timer
                        currentDicounter = 0; //Set back to 0, controlled by measurement timer
                        PP.moduleQACurrentMeasurements.SetName(ModuleQAModuleNameBox.Text);
                        PP.moduleQACurrentMeasurements.SetSide(ModuleQASide.Text);
                        PP.moduleQACurrentMeasurements.SetFlip(ModuleQA_flipped_Chkbox.Checked);
                        //PP.moduleQACurrentMeasurements.TurnOnBias(Convert.ToDouble(qaBias.Text));
                        //PP.FEB1.SetVAll(Convert.ToDouble(qaBias.Text)); //Turn on the bias for the FEBs
                        //PP.FEB2.SetVAll(Convert.ToDouble(qaBias.Text));
                        darkCurrent = true;
                        moduleQAMeasurementTimer.Enabled = true;
                    }
                }
                else
                {
                    MessageBox.Show("Check that FEBs are connected.", "Connect to the FEBs, you dummy", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Did you forget to populate the FEB client list?", "Add and connect to the FEBs, you dummy", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    
        private void ModuleQABtn_Click(object sender, EventArgs e)
        {
            if (PP.FEB_clients != null)
            {
                if (PP.FEB_clients.Count(x => x.ClientOpen) >= 2) //Check if any client is open
                {
                    //Check that both FEBs are connected
                    if (moduleQAMeasurementTimer.Enabled == false)
                    {
                        InitializeModuleQATab(); //Reset the labels in the table

                        if (PP.moduleQACurrentMeasurements == null) //if we didn't make a measurement object yet, do so, else purge the existing one of information
                            PP.moduleQACurrentMeasurements = new ModuleQACurrentMeasurements(PP.FEB_clients);
                        else
                        {
                            PP.moduleQACurrentMeasurements.Purge();
                            PP.moduleQACurrentMeasurements.ChangeClients(PP.FEB_clients);
                        }


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
                        PP.moduleQACurrentMeasurements.SetName(ModuleQAModuleNameBox.Text);
                        PP.moduleQACurrentMeasurements.SetSide(ModuleQASide.Text);
                        PP.moduleQACurrentMeasurements.SetFlip(ModuleQA_flipped_Chkbox.Checked);
                        //PP.moduleQACurrentMeasurements.TurnOnBias(Convert.ToDouble(qaBias.Text));
                        //PP.FEB1.SetVAll(Convert.ToDouble(qaBias.Text)); //Turn on the bias for the FEBs
                        //PP.FEB2.SetVAll(Convert.ToDouble(qaBias.Text));
                        comPort.WriteLine(PP.moduleQACurrentMeasurements.GetgCodeDicounterPosition(currentDicounter, 1200, (int)ModuleQA_Offset.Value)); //tell it to go to the 0th dicounter position
                        ModuleQAStepTimer.Enabled = true;
                        moduleQAMeasurementTimer.Enabled = true;
                    }
                }
                else
                {
                    MessageBox.Show("Check that FEBs are connected.", "Connect to the FEBs, you dummy", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Did you forget to populate the FEB client list?", "Add and connect to the FEBs, you dummy", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            comPortBox.SelectedText = "";
            portList = SerialPort.GetPortNames();
            comPortBox.DataSource = portList;
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
            System.Threading.Thread.Sleep(1); //yield
            if (ModuleQAStepTimer.Enabled == false) //Block the contents of this timer if the stepper is moving
            {
                if (!darkCurrent)
                {
                    if (currentDicounter < 9)//100)//5)//8)
                    {
                        if (currentChannel < 64)//32)//20)//64)
                        {
                            //take measurments
                            ComPortStatusBox.Text = "Measuring " + currentDicounter + " " + currentChannel;
                            double[] currents = PP.moduleQACurrentMeasurements.TakeMeasurement(currentChannel);
                            for(int feb = 0; feb < 2; feb++)
                            {
                                ModuleQALabels[feb][currentChannel].Text += currents[feb].ToString("0.0000");
                            }
                            currentChannel++; //increment the channel
                        }
                        else //must have reached the end of 64 channels
                        {
                            PP.moduleQACurrentMeasurements.WriteMeasurements("C:\\Users\\Boi\\Desktop\\ScanningData_" + ModuleQAFilenameBox.Text + ".txt", currentDicounter);
                            PP.moduleQACurrentMeasurements.Purge();
                            currentDicounter++; //increment the dicounter
                            if (currentDicounter < 9)//100)//5)//8)
                                comPort.WriteLine(PP.moduleQACurrentMeasurements.GetgCodeDicounterPosition(currentDicounter, 1200, (int)ModuleQA_Offset.Value)); //tell it the position to go to
                            currentChannel = 0;
                            InitializeModuleQATab();
                            ModuleQAStepTimer.Enabled = true;
                        }
                    }
                    else //must have reached the end of scanning across the module
                    {
                        comPort.WriteLine(PP.moduleQACurrentMeasurements.GetgCodeDicounterPosition(0, 2800, (int)ModuleQA_Offset.Value)); //go back to the home position quickly
                        ModuleQAStepTimer.Enabled = true;
                        ModuleQADarkCurrentBtn.Enabled = true;
                        moduleQAMeasurementTimer.Enabled = false;
                        //PP.moduleQACurrentMeasurements.TurnOffBias(); //turn off the bias
                    }
                }
                else if (darkCurrent) //if it is a dark current measurement, just record all 64 channels
                {
                    if (currentChannel < 64)//32)//20)//64)
                    {
                        //take measurments
                        Console.WriteLine("Measuring " + currentChannel);
                        //PP.moduleQACurrentMeasurements.TakeMeasurement(currentChannel);
                        double[] currents = PP.moduleQACurrentMeasurements.TakeMeasurement(currentChannel);
                        for (int feb = 0; feb < 2; feb++)
                        {
                            ModuleQALabels[feb][currentChannel].Text += currents[feb].ToString("0.0000");
                        }
                        //System.Threading.Thread.Sleep(10);
                        currentChannel++;
                    }
                    else //must have reached the end of 64 channels, write out the dark currents
                    {
                        PP.moduleQACurrentMeasurements.WriteMeasurements("C:\\Users\\Boi\\Desktop\\ScanningData_" + ModuleQAFilenameBox.Text + ".txt", -1); //-1 will denote dark current measurement
                        PP.moduleQACurrentMeasurements.Purge();
                        //PP.moduleQACurrentMeasurements.TurnOffBias();
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
            //PP.moduleQACurrentMeasurements.TurnOffBias();
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
            //PP.moduleQACurrentMeasurements.TurnOffBias();

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
            System.Threading.Thread.Sleep(1); //yield
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
                    while (comPort.BytesToRead > 0) //clear any messages that were being sent
                        if(comPort.ReadLine().Contains("!"))
                        {
                            MessageBox.Show("The controller is alarmed, did you say something mean to it?", "Oh shit, something went wrong", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
                            comPort.Close();
                            ModuleQABtn.Enabled = false; //disable the QA button if we aren't connected to the stepper controller

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
            System.Threading.Thread.Sleep(1); //yield
            if (currentChannel < qaDiButtons.Length)
            {
                if (!qaDiButtons[currentChannel].Checked)
                {
                    double measurement = PP.qaDicounterMeasurements.TakeMeasurement(currentChannel);
                    if (measurement < Convert.ToDouble(qaDiIWarningThresh.Text)) //Low current
                        qaDiButtons[currentChannel].BackColor = Color.Red;
                    if (measurement < NO_CURRENT_THRESH) //No Current
                        qaDiButtons[currentChannel].BackColor = Color.Blue;
                    qaDiButtons[currentChannel].Update();
                }
                currentChannel++;
                autoDataProgress.Increment(1);
            }
            else
            {
                qaDiCounterMeasurementTimer.Enabled = false;
                PP.qaDicounterMeasurements.WriteMeasurements(dicounterNumberTextBox.Text);//PP.FEB1.ReadTemp(0));
                PP.qaDicounterMeasurements.TurnOffBias();
                currentChannel = 0;
                autoDataProgress.Value = 0;
                autoDataProgress.Update();
                using (System.Media.SoundPlayer soundPlayer = new System.Media.SoundPlayer(Environment.GetFolderPath(Environment.SpecialFolder.Windows) + "\\media\\chord.wav"))// "C:\\Windows\\media\\Windows Proximity Notification.wav"))
                {
                    soundPlayer.Play();
                }
                FEBSelectPanel.Enabled = true;
                autoThreshBtn.Enabled = true;
                lightCheckResetThresh.Enabled = true;
                lightCheckBtn.Enabled = true;
                dicounterNumberTextBox.Enabled = true;
                lightCheckGroup.Enabled = true;
                qaStartButton.Text = "Auto Data";
                qaStartButton.BackColor = SystemColors.Control;
                qaStartButton.Update();
            }
        }

        private void ValidateParseChkBox_CheckedChanged(object sender, EventArgs e)
        {
            if (!validateParseChkBox.Checked)
                PP.myRun.validateParse = true;
            else if (validateParseChkBox.Checked)
                PP.myRun.validateParse = false;
        }

        private void LightCheckMeasurementTimer_Tick(object sender, EventArgs e)
        {
            System.Threading.Thread.Sleep(1); //yield
            if(currentChannel < lightButtons.Length)
            {
                if(!lightButtons[currentChannel].Checked)
                {
                    double measurement = PP.lightCheckMeasurements.TakeMeasurement(currentChannel);
                    if (measurement < NO_CURRENT_THRESH)
                        lightButtons[currentChannel].BackColor = Color.Blue;
                    else
                    {
                        if (auto_thresh_enabled) //if we are setting thresholds automatically
                        {
                            PP.lightCheckChanThreshs[currentChannel] = measurement * AUTO_THRESH_MULTIPLIER; //set the threshold to 10% higher than dark-current
                            lightCheckChanThresh.Text = PP.lightCheckChanThreshs[Convert.ToUInt16(lightCheckChanSelec.Value)].ToString("0.0000"); //update the current channel to display the new thresh
                            lightCheckChanThresh.Update();
                        }
                        else //else we must be doing actual measurements
                        {
                            if (globalThreshChkBox.Checked) //if we should use the global threshold
                            {
                                if (measurement > Convert.ToDouble(lightGlobalThresh.Text)) //check the current against global threshold
                                    lightButtons[currentChannel].BackColor = Color.Red;
                            }
                            else if (measurement > PP.lightCheckChanThreshs[currentChannel]) //else if we are using set thresholds, check against channel thresholds
                                lightButtons[currentChannel].BackColor = Color.Red;
                        }                        
                    }
                    lightButtons[currentChannel].Text = measurement.ToString("0.0000");
                    lightButtons[currentChannel].Update();
                }
                currentChannel++;
                lightCheckProgress.Increment(1);
            }
            else
            {
                if (lightWriteToFileBox.Checked && !auto_thresh_enabled)
                    PP.lightCheckMeasurements.WriteMeasurements(lightModuleLabel.Text);//, PP.FEB1.ReadTemp(0));
                //PP.lightCheckMeasurements.TurnOffBias();
                if (auto_thresh_enabled)
                    auto_thresh_enabled = false;
                currentChannel = 0;
                lightCheckProgress.Value = 0;
                lightCheckProgress.Update();
                using (System.Media.SoundPlayer soundPlayer = new System.Media.SoundPlayer(Environment.GetFolderPath(Environment.SpecialFolder.Windows) + "\\media\\chord.wav"))// "C:\\Windows\\media\\Windows Proximity Notification.wav"))
                {
                    soundPlayer.Play();
                }
                lightCheckBtn.Text = "Light Check";
                lightCheckBtn.BackColor = SystemColors.Control;
                lightCheckBtn.Update();
                autoThreshBtn.Text = "Auto Thresh";
                autoThreshBtn.BackColor = SystemColors.Control;
                autoThreshBtn.Update();
                lightModuleLabel.Enabled = true;
                lightModuleLayer.Enabled = true;
                lightModuleSide.Enabled = true;
                lightCheckResetThresh.Enabled = true;
                lightCheckBtn.Enabled = true;
                lightWriteToFileBox.Enabled = true;
                dicounterQAGroup.Enabled = true;
                autoThreshBtn.Enabled = true;
                FEBSelectPanel.Enabled = true;
                LightCheckMeasurementTimer.Enabled = false;
            }
        }

        private void LoadCmdsBtn_Click(object sender, EventArgs e)
        {
            if (activeFEB != null && activeFEB.ClientOpen)
            {

                Thread loadCmdsFileDialogThread = new Thread((ThreadStart)(() =>
                {
                    OpenFileDialog loadCmdsDialog = new OpenFileDialog
                    {
                        DefaultExt = "txt",
                        CheckFileExists = true,
                        CheckPathExists = true,
                        Filter = "Text file (*.txt)|*.txt|Commands file (*.cmds)|*.cmds",
                        InitialDirectory = Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop)
                    };

                    if (loadCmdsDialog.ShowDialog() == DialogResult.Cancel)
                        return;
                    else
                    {
                        try
                        {
                            using (StreamReader cmdFile = new StreamReader(loadCmdsDialog.OpenFile()))
                            {
                                while (!cmdFile.EndOfStream)
                                {
                                    string cmd = cmdFile.ReadLine();
                                    char commentChk = cmd.First();
                                    if (commentChk != '$' || commentChk != '#' || commentChk != '/')
                                        activeFEB.SendStr(cmd);
                                }
                            }
                        }
                        catch
                        {
                        }
                    }
                }));

                loadCmdsFileDialogThread.SetApartmentState(ApartmentState.STA);
                loadCmdsFileDialogThread.Start();
                loadCmdsFileDialogThread.Join();
            }
        }
    }



    public class ConnectAttemptEventArgs : EventArgs
    {
        public int ConnectAttempt { get; set; }
    }

}
