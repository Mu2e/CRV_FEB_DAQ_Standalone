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
    public partial class PlotIV : Form
    {
        public class IV_curve
        {
            public int num_points;
            public double[] V;
            public double[] I;
            public double[] gain;
            public double[] calib_slope;
            public double[] calib_offset;
            public double[] logY;
        }

        public PlotIV()
        {
            InitializeComponent();
        }
        /// <summary>
        /// Convert A0 reading to current
        /// </summary>
        /// <param name="V">the value from the ADC (A0 reading)</param>
        /// <param name="g_exp">gain =2^g_exp</param>
        /// <returns>current in uA</returns>
        private double ADC_to_uA(double V, byte g_exp, out double correct_V, out int gain_adj)
        {
            double res = -1000;
            if (V > 4.096) { V = V - 8.192; }
            if (System.Math.Abs(V) < 1) { gain_adj = 1; }
            else if (System.Math.Abs(V) > 4) { gain_adj = -1; }
            else { gain_adj = 0; }

            V = V * (2 ^ g_exp);
            res = V / 0.004; //res in uA
            correct_V = V - res / 250;
            return res;
        }

        private void btn_IVscan_Click(object sender, EventArgs e)
        {
            //set mux to correct chan
            //check current
            //set bias to min
        }
    }
}
