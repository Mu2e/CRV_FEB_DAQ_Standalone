using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TB_mu2e
{
    public partial class DynamicVoverSettingsForm : Form
    {
        private List<FEBBiasGainUsrCtrl> FEBBiasGainUsrCtrlList = new List<FEBBiasGainUsrCtrl>();
        private List<bool> febSettingsValidList = new List<bool>();
        private readonly frmMain mainForm;

        public List<bool> FEBSettingsValidList
        {
            get { return this.febSettingsValidList; }
            set { this.febSettingsValidList = value; }
        }

        public DynamicVoverSettingsForm(frmMain mainForm)
        {
            InitializeComponent();
            InitializeFEBControls();
            this.mainForm = mainForm;
        }

        private void DynamicVoverSettingsForm_Load(object sender, EventArgs e)
        {

        }

        private void BtnUseDefault_Click(object sender, EventArgs e)
        {
            useCMBTempBox.Checked = true;
            useFEBTempBox.Checked = true;
            txtCMBRate.Text = "-55.4";
            txtFEBRate.Text = "4.09";
            txtRefCMBTemp.Text = $"{PP.defaultCMBTemp:F1}";
            txtRefFEBTemp.Text = $"{PP.defaultFEBTemp:F1}";
        }

        private void BtnApplyToAll_Click(object sender, EventArgs e)
        {
            foreach (FEBBiasGainUsrCtrl febBiasGainUsrCtrl in FEBBiasGainUsrCtrlList)
            {
                if (useCMBTempBox.Checked)
                {
                    febBiasGainUsrCtrl.txtCMBRate.Text = txtCMBRate.Text;
                    febBiasGainUsrCtrl.txtRefCMBTemp.Text = txtRefCMBTemp.Text;
                }
                else
                {
                    febBiasGainUsrCtrl.txtCMBRate.Text = "0.0";
                    febBiasGainUsrCtrl.txtRefCMBTemp.Text = txtRefCMBTemp.Text;
                }
                if (useFEBTempBox.Checked)
                {
                    febBiasGainUsrCtrl.txtFEBRate.Text = txtFEBRate.Text;
                    febBiasGainUsrCtrl.txtRefFEBTemp.Text = txtRefFEBTemp.Text;
                }
                else
                {
                    febBiasGainUsrCtrl.txtFEBRate.Text = "0.0";
                    febBiasGainUsrCtrl.txtRefFEBTemp.Text = txtRefFEBTemp.Text;
                }
            }
        }

        public void EnableBtnLoadAllSettings()
        {
            btnApplyAllSettings.Enabled = true;
        }

        public bool IsValidDoubleString(string input)
        {
            // Use a regular expression to validate the input
            // The pattern allows for optional sign, digits, and an optional decimal point
            // It does not allow multiple decimal points
            string pattern = @"^[+-]?(\d+(\.\d*)?|\.\d+)$";

            return Regex.IsMatch(input, pattern);
        }

        private void BtnApplyAllSettings_Click(object sender, EventArgs e)
        {
            bool all_coefficient_valid = true;
            List<int> index_failed_coeff = new List<int>();
            for (int i = 0; i < FEBBiasGainUsrCtrlList.Count; i++)
            {
                bool thisfailed = false;
                if (!IsValidDoubleString(FEBBiasGainUsrCtrlList[i].txtFEBRate.Text))
                {
                    all_coefficient_valid = false;
                    thisfailed = true;
                }
                if (!IsValidDoubleString(FEBBiasGainUsrCtrlList[i].txtCMBRate.Text))
                {
                    all_coefficient_valid = false;
                    thisfailed = true;
                }
                if (thisfailed) index_failed_coeff.Add(i);
            }
            if (all_coefficient_valid)
            {
                PP.referenceFEBSettings = new FEBSettingsBiasGain[FEBBiasGainUsrCtrlList.Count];
                PP.FEBcoef = new double[FEBBiasGainUsrCtrlList.Count];
                PP.CMBcoef = new double[FEBBiasGainUsrCtrlList.Count];
                string output = "";
                for (int i = 0; i < PP.referenceFEBSettings.Length; i++)
                {
                    PP.referenceFEBSettings[i] = FEBBiasGainUsrCtrlList[i].MySettings;
                    output += PP.referenceFEBSettings[i].DumpToText();
                    PP.FEBcoef[i] = Convert.ToDouble(FEBBiasGainUsrCtrlList[i].txtFEBRate.Text);
                    PP.CMBcoef[i] = Convert.ToDouble(FEBBiasGainUsrCtrlList[i].txtCMBRate.Text);
                }
                mainForm.EnableControlChkBox();
                this.Close();
                // debugging dump for initial settings
                //mainForm.AddToRunLog(output);
                return;
            }
            else
            {
                foreach (int i in index_failed_coeff)
                {
                    FEBBiasGainUsrCtrlList[i].AddTextToLoadLogBox("\r\nERROR: Unable to update the setup! Check the coefficients above!\r\n");
                }
                return;
            }
        }
    }
}
