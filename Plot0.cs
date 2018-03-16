using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using ZedGraph;

namespace TB_mu2e
{
    public partial class Plot0 : Form
    {
        public CheckBox[] chk_chan = new CheckBox[100];
        public Color[] chk_color = new Color[100];
        public int max_chk = 0;
        public Button btn_all_off_A = new Button();
        public Button btn_all_on_A = new Button();
        public Event plot_e = new Event();
        public LinkedListNode<Event> enode;
        public bool flgAuto;

        //        public string[] cmb_Start0_text = new string[8];
        //        public ComboBox[] cmb_Trace_arr = new ComboBox[4];

        public Plot0()
        {
            if (PP.myRun.Events == null)
            { return; }
            InitializeComponent();
            enode = PP.myRun.Events.First;
            if (enode != null)
            {
                plot_e = PP.myRun.Events.First.Value;
                txtEvt.Text = plot_e.EvNum.ToString();
                int i = 0;
                if (plot_e != null)
                {
                    for (i = 0; i < 32; i++)
                    {
                        this.chk_chan[i] = new CheckBox();
                        this.chk_chan[i].AutoSize = true;
                        this.chk_chan[i].Enabled = true;
                        this.chk_chan[i].Location = new System.Drawing.Point(5 + (60 * (i & 0x18) >> 3), 425 + (20 * (i & 0x7)));
                        this.chk_chan[i].Name = "checkBox" + i.ToString();
                        this.chk_chan[i].Size = new System.Drawing.Size(51, 17);
                        this.chk_chan[i].UseVisualStyleBackColor = true;
                        this.chk_chan[i].Visible = true;
                        this.chk_chan[i].ForeColor = Color.Black;
                        this.chk_chan[i].Tag = i.ToString();
                        this.chk_chan[i].CheckedChanged += new System.EventHandler(chk_changed);
                        chk_chan[i].Text = "Ch" + i.ToString();
                        this.Controls.Add(chk_chan[i]);
                        chk_color[i] = Color.Black;
                    }
                    max_chk = i - 1;
                    btn_all_off_A = new Button();
                    btn_all_off_A.Enabled = true;
                    btn_all_off_A.Text = "All off";
                    btn_all_off_A.Size = new System.Drawing.Size(75, 20);
                    btn_all_off_A.Location = new System.Drawing.Point(5, 425 + 160);
                    btn_all_off_A.Click += new EventHandler(btn_all_off_A_Click);
                    this.Controls.Add(btn_all_off_A);
                    btn_all_on_A = new Button();
                    btn_all_on_A.Enabled = true;
                    btn_all_on_A.Text = "All on";
                    btn_all_on_A.Size = new System.Drawing.Size(75, 20);
                    btn_all_on_A.Location = new System.Drawing.Point(105, 425 + 160);
                    btn_all_on_A.Click += new EventHandler(btn_all_on_A_Click);
                    this.Controls.Add(btn_all_on_A);
                }
            }
        }

        void chk_changed(object sender, EventArgs e)
        {
            CheckBox this_box = (CheckBox)sender;
            int ind = Convert.ToInt32(this_box.Tag);
            if (this_box.Checked)
            {
                //if (this_box.ForeColor.ToArgb() == Color.Black.ToArgb())
                //{
                //    if (colorDialog1.ShowDialog() == DialogResult.OK)
                //    { this_box.ForeColor = colorDialog1.Color; }
                //}
                this_box.ForeColor = Color.Red;
                Plot0_display();
            }

        }

        void btn_all_off_A_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < chk_chan.Length; i++)
            {
                if (chk_chan[i] != null)
                { chk_chan[i].Checked = false; chk_chan[i].ForeColor = Color.Black; }
                else { break; }
            }
            Application.DoEvents();
        }

        void btn_all_on_A_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < chk_chan.Length; i++)
            {
                if (chk_chan[i] != null)
                { chk_chan[i].Checked = true; chk_chan[i].ForeColor = Color.Black; }
                else { break; }
            }
            Application.DoEvents();
        }


        public void Plot0_display()
        {

            double min_val = -98;
            double max_val = 1000;
            GraphPane myPane = zg1.GraphPane;

            if (chk_persist.Checked) { }
            //{ myPane.Legend.IsVisible = false; }
            else
            {
                myPane.CurveList.Clear();
                myPane.Legend.IsVisible = false;
            }
            double min_x = (double)ud_MinX.Value;
            double max_x = (double)ud_MaxX.Value;
            max_val = (double)ud_MaxY.Value;
            min_val = (double)ud_MinY.Value;

            if (plot_e != null)
            {
                //for (int j = 0; j < max_chk; j++)
                //{
                //    if (chk_chan[j].Checked)
                //    {
                //        for (int i = 0; i < 32; i++)
                //        {
                //            if ((string)chk_chan[j].Tag == i.ToString())
                //            {
                                double[] x = new double[256];
                                double[] y = new double[256];
                                for (int ii = 0; ii < 256; ii++)
                                {
                                    x[ii] = (double)ii;
                                    y[ii] = (double)plot_e.Channels[0, ii];
                                    //y[ii] = (double)plot_e.RawBytes[0, ii];
                                }
                                PointPairList list = new PointPairList(x, y);
                                SymbolType s;
                                s = SymbolType.None;
                                //myPane.AddCurve(i.ToString(), list, chk_chan[j].ForeColor, s);
                                myPane.AddCurve("ch3", list, Color.Black, s);
                    //        }
                    //    }
                    //}
                //}
            }

            // Set the titles and axis labels
            myPane.Title.Text = "ADC data";
            myPane.XAxis.Title.Text = "point";
            //PointPairList list = new PointPairList(x,y0);

            //max_val = max_val +(max_val-min_val)*0.1;
            //min_val = min_val - (max_val - min_val) * 0.1;


            // Generate a red curve with diamond symbols, and "Alpha" in the legend
            //            if (data_assigned[0]) { LineItem myCurve = myPane.AddCurve("Ch0", list0, Color.Red, SymbolType.None); }

            // Show the x axis grid
            myPane.XAxis.MajorGrid.IsVisible = true;
            // Make the Y axis scale red
            myPane.YAxis.Scale.FontSpec.FontColor = Color.Red;
            myPane.YAxis.Title.FontSpec.FontColor = Color.Red;
            // turn off the opposite tics so the Y tics don't show up on the Y2 axis
            myPane.YAxis.MajorTic.IsOpposite = false;
            myPane.YAxis.MinorTic.IsOpposite = false;
            // Don't display the Y zero line
            myPane.YAxis.MajorGrid.IsZeroLine = false;
            // Align the Y axis labels so they are flush to the axis
            myPane.YAxis.Scale.Align = AlignP.Inside;
            // Manually set the axis range
            myPane.YAxis.Scale.Min = min_val;
            myPane.YAxis.Scale.Max = max_val;
            myPane.XAxis.Scale.Min = min_x;
            myPane.XAxis.Scale.Max = max_x;

            //// Add a text box with instructions
            //TextObj text = new TextObj(
            //  "Zoom: left mouse & drag\nPan: middle mouse & drag\nContext Menu: right mouse",
            //  0.05f, 0.95f, CoordType.ChartFraction, AlignH.Left, AlignV.Bottom);
            //text.FontSpec.StringAlignment = StringAlignment.Near;
            //myPane.GraphObjList.Add(text);

            // Enable scrollbars if needed
            zg1.IsShowHScrollBar = true;
            zg1.IsShowVScrollBar = true;
            zg1.IsAutoScrollRange = true;
            //zg1.IsScrollY2 = true;

            //// OPTIONAL: Show tooltips when the mouse hovers over a point
            //zg1.IsShowPointValues = true;
            //zg1.PointValueEvent += new ZedGraphControl.PointValueHandler(MyPointValueHandler);

            //// OPTIONAL: Add a custom context menu item
            //zg1.ContextMenuBuilder += new ZedGraphControl.ContextMenuBuilderEventHandler(
            //        MyContextMenuBuilder);

            // OPTIONAL: Handle the Zoom Event
            //zg1.ZoomEvent += new ZedGraphControl.ZoomEventHandler(MyZoomEvent);

            // Size the control to fit the window
            //SetSize();

            // Tell ZedGraph to calculate the axis ranges
            // Note that you MUST call this after enabling IsAutoScrollRange, since AxisChange() sets
            // up the proper scrolling parameters
            zg1.AxisChange();
            //if (TB4.myRun.flg_slope_pos)
            //{ myPane.AddCurve("trig thresh", trig_x, trig_y, Color.Black, SymbolType.Triangle); }
            //else
            //{ myPane.AddCurve("trig thresh", trig_x, trig_y, Color.Black, SymbolType.TriangleDown); }

            //Double[] noise_lvl_x = new Double[3];
            //Double[] noise_lvl_y = new Double[3];
            //    noise_lvl_x[0] = myPane.XAxis.Scale.Min;
            //    noise_lvl_x[1] = 0.5 * (myPane.XAxis.Scale.Max - myPane.XAxis.Scale.Min);
            //    noise_lvl_x[2] = myPane.XAxis.Scale.Max;
            //    noise_lvl_y[0] = TB4.myRun.glbNoise_Level;
            //    noise_lvl_y[1] = noise_lvl_y[0];
            //    noise_lvl_y[2] = noise_lvl_y[0];
            //    if (TB4.myRun.flg_slope_pos)
            //    { myPane.AddCurve("noise_lvl thresh", noise_lvl_x, noise_lvl_y, Color.OrangeRed, SymbolType.TriangleDown); }
            //    else
            //    { myPane.AddCurve("noise_lvl thresh", noise_lvl_x, noise_lvl_y, Color.OrangeRed, SymbolType.Triangle); }

            // Make sure the Graph gets redrawn
            zg1.Invalidate();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            PP.myRun.Events.Clear();
        }

        private void btnPrevE_Click(object sender, EventArgs e)
        {
            try
            {
                enode = PP.myRun.Events.Find(plot_e).Previous;
                if (enode != null)
                {
                    plot_e = enode.Value;
                    txtEvt.Text = plot_e.EvNum.ToString();
                    Plot0_display();
                    lblBooardID.Visible = true;
                    lblBooardID.Text = "BoardID= " + plot_e.BoardId.ToString();
                }
            }
            catch { }
        }

        private void btnNextE_Click(object sender, EventArgs e)
        {
            try
            {

                if (plot_e != null)
                {
                    enode = PP.myRun.Events.Find(plot_e).Next;
                    if (enode != null)
                    {
                        plot_e = enode.Value;
                        txtEvt.Text = plot_e.EvNum.ToString();
                        Plot0_display();
                        lblBooardID.Visible = true;
                        lblBooardID.Text = "BoardID= " + plot_e.BoardId.ToString();
                    }
                }

                else
                { plot_e = PP.myRun.Events.First.Value; }
            }
            catch { }
        }

        private void btnAutoScale_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            GraphPane myPane = zg1.GraphPane;
            YAxis myY = myPane.YAxis;
            if (btn.Text == "Autoscale")
            {
                myY.Scale.MaxAuto = true;
                btnAutoScale.Text = "Stop Auto";
            }
            else
            {
                myY.Scale.MaxAuto = false;
                btnAutoScale.Text = "Autoscale";
            }
        }

        private void ud_MaxX_ValueChanged(object sender, EventArgs e)
        {

        }

        private void btnSoftTrig_Click(object sender, EventArgs e)
        {
            if (RC_client.ClientOpen)
            {
                RC_client.SendStr("wr 2e 1");
                Thread.Sleep(5);
                RC_client.SendStr("rd 2f");
                string t = "";
                int rt;
                RC_client.ReadStr(out t, out rt);
                char[] sep = new char[2];
                sep[0] = ' ';
                sep[1] = '\r';
                string[] tok = t.Split(sep);
                if (tok.Length > 3)
                { label5.Text = tok[3]; }
                else
                { label5.Text = "blank line"; }
                label5.Visible = true;
                Application.DoEvents();
            }
        }

        private void btnRead_Click(object sender, EventArgs e)
        {
            //btnSoftTrig_Click(null, null);
            //Thread.Sleep(10);
            RC_client.SendStr("rdm 13 101");
            string t = "";
            int rt;
            RC_client.ReadStr(out t, out rt);
            RC_client.Disarm();
            char[] sep = new char[2];
            sep[0] = ' ';
            //sep[1] = '\r';
            string[] tok = t.Split(sep, StringSplitOptions.RemoveEmptyEntries);
            Event e13 = new Event();
            int j = 0; int k = 0;
            for (int i = 0; i < tok.Length; i++)
            {
                if (j == 8) { j = 0; k++; }
                else
                {
                    try
                    {
                        e13.RawBytes[0, j+8*k] = Convert.ToInt16(tok[i], 16);
                        if (e13.RawBytes[0, j + 8 * k] > 0x7ff) 
                        { 
                            e13.RawBytes[0, j + 8 * k] = (Int16)(e13.RawBytes[0, j + 8 * k] - 0xfff); 
                        }
                    }
                    catch
                    {
                        e13.RawBytes[0, j + 8 * k] = -99;
                    }
                    
                    e13.Channels[0, j + 8 * k] = e13.RawBytes[0, j + 8 * k] + 100;
                    j++;
                }
            }
            e13.EvNum = PP.myRun.Events.Count;
            PP.myRun.Events.AddLast(e13);
            plot_e = PP.myRun.Events.Last.Value;
            txtEvt.Text = PP.myRun.Events.Count.ToString();
            Plot0_display();
            Application.DoEvents();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (btnCountZeros.Text.Contains("COUNT ZEROs"))
            {
                
                flgAuto = true;
                btnCountZeros.Text = "PRESS TO STOP";
                while (flgAuto)
                {
                    btnRead_Click(null, null);
                    int max_evt = Convert.ToInt32(udNumEvt_for_ZF.Value);
                    double z = PP.myRun.ZeroFrac(111, 113, 90, 115, max_evt);

                    label6.Visible = true;
                    double err = z * (1 - z) / max_evt;
                    label6.Text = z.ToString("F4")+"  "+(System.Math.Sqrt(err).ToString("F4"));
                    label7.Visible = true;
                    if (z > 0)
                    { label7.Text = System.Math.Log(1 / z).ToString("F2"); }
                    Application.DoEvents();
                }
            }
            else
            {
                btnCountZeros.Text = "COUNT ZEROs";
                flgAuto = false;
                Thread.Sleep(10);
            }
        }

        private void btnSetV_Click(object sender, EventArgs e)
        {
            label9.Visible = true;
            label9.Text = "measuring...";
            Application.DoEvents();
            RC_client.SetV((double)udV.Value);
            double v = RC_client.ReadA0();
            if (v < 1)
            {
                v = v * 1000;
                label9.Text = v.ToString("F2") + "nA";
            }
            else if (v > 1000)
            {
                v = v / 1000;
                label9.Text = v.ToString("F3") + "mA";
            }
            else
            {
                label9.Text = v.ToString("F3") + "uA";
            }
            RC_client.SendStr("wr 20 00");
            Application.DoEvents();
        }


    }
}