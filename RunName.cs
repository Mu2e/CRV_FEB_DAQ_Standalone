using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TB_mu2e
{
    public partial class RunName : Form
    {
        public RunName()
        {
            InitializeComponent();

            this.GainTextBox.Text = "";
            Mu2e_Register reg;

            if (PP.FEB_clients != null)
            {
                if (PP.FEB_clients.Any(x => x.ClientOpen))
                {
                    foreach (Mu2e_FEB_client feb in PP.FEB_clients)
                    {
                        if (feb.ClientOpen)
                        {
                            Mu2e_Register.FindName("AFE_VGA0", 0, ref feb.arrReg, out reg);
                            Mu2e_Register.ReadReg(ref reg, ref feb.client);
                            GainTextBox.Text += reg.val.ToString();
                            Mu2e_Register.FindName("AFE_VGA1", 0, ref feb.arrReg, out reg);
                            Mu2e_Register.ReadReg(ref reg, ref feb.client);
                            GainTextBox.Text += "-" + reg.val.ToString() + "-";
                        }
                    }
                }
            }

            //if (PP.FEB1.client != null)
            //{
            //    Mu2e_FEB_client feb1 = PP.FEB1;
            //    Mu2e_Register r1;

            //    Mu2e_Register.FindName("AFE_VGA0", 0, ref feb1.arrReg, out r1);
            //    //r1.fpga_index = 0;
            //    Mu2e_Register.ReadReg(ref r1, ref feb1.client);
            //    this.GainTextBox.Text += r1.val.ToString();
            //    Mu2e_Register.FindName("AFE_VGA1", 0, ref feb1.arrReg, out r1);
            //    //r1.fpga_index = 0;
            //    Mu2e_Register.ReadReg(ref r1, ref feb1.client);
            //    this.GainTextBox.Text += "-" + r1.val.ToString();
            //}

            //if (PP.FEB2.client != null)
            //{
            //    Mu2e_FEB_client feb2 = PP.FEB2;
            //    Mu2e_Register r1;

            //    Mu2e_Register.FindName("AFE_VGA0", 0, ref feb2.arrReg, out r1);
            //    //r1.fpga_index = 0;
            //    Mu2e_Register.ReadReg(ref r1, ref feb2.client);
            //    this.GainTextBox.Text += "-" + r1.val.ToString();
            //    Mu2e_Register.FindName("AFE_VGA1", 0, ref feb2.arrReg, out r1);
            //    //r1.fpga_index = 0;
            //    Mu2e_Register.ReadReg(ref r1, ref feb2.client);
            //    this.GainTextBox.Text += "-" + r1.val.ToString();
            //}

        }

        private void InitValButton_Click(object sender, EventArgs e)
        {

            //(this.num.ToString() + " " + rn.textEbeam.Text + " " + rn.textIbeam.Text + " " + rn.BIASVtextBox.Text + " " + rn.GainTextBox.Text + " " + 
            //rn.comboPID.Text + " " + rn.textAngle.Text + " " + rn.textXpos.Text + " " + rn.textZpos.Text + " " + rn.textPressure.Text);
            this.txtRunNum.Text = PP.myRun.num.ToString();
            if (PP.myRun.RunParams.Count() > 2)
            {
                textEbeam.Text = PP.myRun.RunParams[1];
                textIbeam.Text = PP.myRun.RunParams[2];
                BIASVtextBox.Text = PP.myRun.RunParams[3];
                GainTextBox.Text = PP.myRun.RunParams[4];
                comboPID.Text = PP.myRun.RunParams[5];
                textAngle.Text = PP.myRun.RunParams[6];
                textXpos.Text = PP.myRun.RunParams[7];
                textZpos.Text = PP.myRun.RunParams[8];
                textTemp.Text = PP.myRun.RunParams[9];
            }

        }

        private void Button1_Click(object sender, EventArgs e)
        {
            //PP.myRun.run_name = this.textBox1.Text;


            DateTime n = System.DateTime.Now;

            //PP.myRun.run_name = "RUN_FEB_" + PP.myRun.num.ToString() + "_TB" + n.Year.ToString() + n.Month.ToString("00") + n.Day.ToString("00") + n.Hour.ToString("00") + n.Minute.ToString("00") + "_";
            PP.myRun.run_name = "raw.mu2e.CRV_wideband_cosmics.crvaging-" + textEbeam.Text + "." + PP.myRun.num.ToString("000000");

            //PP.myRun.run_name += textEbeam.Text;
            //PP.myRun.run_name += "GeV_";
            //PP.myRun.run_name += textIbeam.Text;
            //PP.myRun.run_name += "Kcnt_";
            //PP.myRun.run_name += BIASVtextBox.Text;
            //PP.myRun.run_name += "BV_";
            //PP.myRun.run_name += GainTextBox.Text;
            //PP.myRun.run_name += "Gain_";
            //PP.myRun.run_name += comboPID.Text;
            //PP.myRun.run_name += "_";
            //PP.myRun.run_name += textAngle.Text;
            //PP.myRun.run_name += "deg_x";
            //PP.myRun.run_name += textXpos.Text;
            //PP.myRun.run_name += "_z";
            //PP.myRun.run_name += textZpos.Text;
            //PP.myRun.run_name += "_";
            //PP.myRun.run_name += textTemp.Text;
            //PP.myRun.run_name += "degF";

            textBox1.Text = PP.myRun.run_name;

            Close();
        }
    }
}
