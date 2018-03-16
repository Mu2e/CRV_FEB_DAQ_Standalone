using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
//using System.Threading.Tasks;
using System.Windows.Forms;

namespace TB_mu2e
{
    public partial class ClientNameChange : Form
    {
        public ClientNameChange()
        {
            InitializeComponent();
        }

        private void ClientNameChange_Load(object sender, EventArgs e)
        {
            textBox1.Text = PP.FEB1.host_name_prop;
            textBox2.Text = PP.FEB2.host_name_prop;
            textBox3.Text = PP.WC.host_name_prop;
        }

        private void btnSetFEB1_Click(object sender, EventArgs e)
        {
            PP.FEB1.host_name_prop = textBox1.Text;
        }

        private void btnSetFEB2_Click(object sender, EventArgs e)
        {
            PP.FEB2.host_name_prop = textBox2.Text;
        }

        private void btnSetWC_Click(object sender, EventArgs e)
        {
            PP.WC.host_name_prop = textBox3.Text;
        }

        private void btnDone_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
