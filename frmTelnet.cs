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
    public partial class frmTelnet : Form
    {
        public frmTelnet()
        {
            InitializeComponent();
        }

        private void frmTelnet_KeyUp(object sender, KeyEventArgs e)
        {
            string r;
            if (e.KeyValue == 13)
            {
                if (RC_client.ClientOpen)
                {
                    string l = txtNet.Lines[txtNet.Lines.Length - 2];
                    r=RC_client.SendRead(l);
                    txtNet.Text += r;
                    txtNet.Text += "\n";
                    
                }
                else
                {
                    txtNet.Text+="Doh! The the telnet client is not open \n";
                }
            }
        }

        private void frmTelnet_Load(object sender, EventArgs e)
        {

        }

        private void frmTelnet_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }



    }
}
