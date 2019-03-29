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

            if (PP.FEB1.client != null)
            {
                Mu2e_FEB_client feb1 = PP.FEB1;
                Mu2e_Register r1;

                Mu2e_Register.FindName("AFE_VGA0", 0, ref feb1.arrReg, out r1);
                //r1.fpga_index = 0;
                Mu2e_Register.ReadReg(ref r1, ref feb1.client);
                this.GainTextBox.Text += r1.val.ToString();
                Mu2e_Register.FindName("AFE_VGA1", 0, ref feb1.arrReg, out r1);
                //r1.fpga_index = 0;
                Mu2e_Register.ReadReg(ref r1, ref feb1.client);
                this.GainTextBox.Text += "-" + r1.val.ToString();
            }

            if (PP.FEB2.client != null)
            {
                Mu2e_FEB_client feb2 = PP.FEB2;
                Mu2e_Register r1;

                Mu2e_Register.FindName("AFE_VGA0", 0, ref feb2.arrReg, out r1);
                //r1.fpga_index = 0;
                Mu2e_Register.ReadReg(ref r1, ref feb2.client);
                this.GainTextBox.Text += "-" + r1.val.ToString();
                Mu2e_Register.FindName("AFE_VGA1", 0, ref feb2.arrReg, out r1);
                //r1.fpga_index = 0;
                Mu2e_Register.ReadReg(ref r1, ref feb2.client);
                this.GainTextBox.Text += "-" + r1.val.ToString();
            }

        }

        private void InitValButton_Click(object sender, EventArgs e)
        {

            //(this.num.ToString() + " " + rn.textEbeam.Text + " " + rn.textIbeam.Text + " " + rn.BIASVtextBox.Text + " " + rn.GainTextBox.Text + " " + 
            //rn.comboPID.Text + " " + rn.textAngle.Text + " " + rn.textXpos.Text + " " + rn.textZpos.Text + " " + rn.textPressure.Text);
            this.txtRunNum.Text = PP.myRun.num.ToString();
            if (PP.myRun.RunParams.Count() > 2)
            {
                this.textEbeam.Text = PP.myRun.RunParams[1];
                this.textIbeam.Text = PP.myRun.RunParams[2];
                this.BIASVtextBox.Text = PP.myRun.RunParams[3];
                this.GainTextBox.Text = PP.myRun.RunParams[4];
                this.comboPID.Text = PP.myRun.RunParams[5];
                this.textAngle.Text = PP.myRun.RunParams[6];
                this.textXpos.Text = PP.myRun.RunParams[7];
                this.textZpos.Text = PP.myRun.RunParams[8];
                this.textTemp.Text = PP.myRun.RunParams[9];
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            //PP.myRun.run_name = this.textBox1.Text;


            DateTime n = System.DateTime.Now;

            PP.myRun.run_name = "RUN_FEB_" + PP.myRun.num.ToString() + "_TB" + n.Year.ToString() + n.Month.ToString("00") + n.Day.ToString("00") + n.Hour.ToString("00") + n.Minute.ToString("00") + "_";
            PP.myRun.run_name += textEbeam.Text;
            PP.myRun.run_name += "GeV_";
            PP.myRun.run_name += textIbeam.Text;
            PP.myRun.run_name += "Kcnt_";
            PP.myRun.run_name += BIASVtextBox.Text;
            PP.myRun.run_name += "BV_";
            PP.myRun.run_name += GainTextBox.Text;
            PP.myRun.run_name += "Gain_";
            PP.myRun.run_name += comboPID.Text;
            PP.myRun.run_name += "_";
            PP.myRun.run_name += textAngle.Text;
            PP.myRun.run_name += "deg_x";
            PP.myRun.run_name += textXpos.Text;
            PP.myRun.run_name += "_z";
            PP.myRun.run_name += textZpos.Text;
            PP.myRun.run_name += "_";
            PP.myRun.run_name += textTemp.Text;
            PP.myRun.run_name += "degF";

            this.textBox1.Text = PP.myRun.run_name;

            this.Close();
        }
    }
}
