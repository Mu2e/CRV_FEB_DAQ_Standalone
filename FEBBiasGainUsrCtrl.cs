using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TB_mu2e
{
    public partial class FEBBiasGainUsrCtrl : UserControl
    {
        
        public int FEBNumber { get; set; }
        public FEBSettingsBiasGain MySettings { get; set; }
        public string LoadFileName { get; set; }
        public bool ReadSuccess { get; set; }
        public bool LoadSuccess { get; set; }

        public bool IsValidDoubleString(string input)
        {
            // Use a regular expression to validate the input
            // The pattern allows for optional sign, digits, and an optional decimal point
            // It does not allow multiple decimal points
            string pattern = @"^[+-]?(\d+(\.\d*)?|\.\d+)$";

            return Regex.IsMatch(input, pattern);
        }
        
        public FEBBiasGainUsrCtrl()
        {
            InitializeComponent();
        }

        public void AddTextToLoadLogBox(string text)
        {
            loadLogBox.AppendText(text);
        }

        private void LoadLog_TextChanged(object sender, EventArgs e)
        {
            try
            {
                //Autoscroll to the end of the text box
                loadLogBox.SelectionStart = loadLogBox.TextLength;
                loadLogBox.SelectionLength = 0;
                loadLogBox.ScrollToCaret();
            }
            catch { }
        }

        private void RdSettingsBtn_Click(object sender, EventArgs e)
        {
            DynamicVoverSettingsForm parentForm = this.Parent.Parent as DynamicVoverSettingsForm;
            loadLogBox.Text = string.Empty;

            if (IsValidDoubleString(txtRefCMBTemp.Text) && IsValidDoubleString(txtRefFEBTemp.Text) && IsValidDoubleString(txtCMBRate.Text) && IsValidDoubleString(txtFEBRate.Text))
            {
                double myCMBTemp = double.Parse(txtRefCMBTemp.Text);
                double myFEBTemp = double.Parse(txtRefFEBTemp.Text);
                bool loadSettingsToFEB = false;
                if (chkBoxLoadSettings.Checked) { loadSettingsToFEB = true; }

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
                            this.LoadFileName = loadCmdsDialog.SafeFileName;
                            using (StreamReader cmdFile = new StreamReader(loadCmdsDialog.OpenFile()))
                            {
                                this.MySettings = new FEBSettingsBiasGain(cmdFile, this.FEBNumber, myCMBTemp, myFEBTemp);
                                if (this.MySettings.CheckValid())
                                {
                                    this.ReadSuccess = true;
                                }
                                else
                                {
                                    this.ReadSuccess = false;
                                }
                            }
                        }
                        catch
                        {
                        }
                        // load the file if needed to
                        if (this.ReadSuccess && loadSettingsToFEB)
                        {
                            this.LoadSuccess = false;
                            try
                            {
                                Mu2e_FEB_client selectedFEB = PP.FEB_clients[this.FEBNumber];
                                if (selectedFEB != null && selectedFEB.ClientOpen)
                                {
                                    using (StreamReader cmdFile = new StreamReader(loadCmdsDialog.OpenFile()))
                                    {
                                        while (!cmdFile.EndOfStream)
                                        {
                                            string cmd = cmdFile.ReadLine();
                                            char commentChk = cmd.First();
                                            if (commentChk != '$' || commentChk != '#' || commentChk != '/')
                                                selectedFEB.SendStr(cmd);
                                        }
                                    }
                                }
                                this.LoadSuccess = true;
                            }
                            catch
                            {
                            }
                        }
                    }
                }));

                loadCmdsFileDialogThread.SetApartmentState(ApartmentState.STA);
                loadCmdsFileDialogThread.Start();
                loadCmdsFileDialogThread.Join();

                loadLogBox.AppendText($"Attempted loading from {this.LoadFileName}\r\n");
                if (this.ReadSuccess)
                { 
                    List<bool> newValidList = parentForm.FEBSettingsValidList;
                    newValidList[this.FEBNumber] = true;
                    parentForm.FEBSettingsValidList = newValidList;
                    if (parentForm.FEBSettingsValidList.All(b => b))
                    {
                        parentForm.EnableBtnLoadAllSettings();
                    }

                    loadLogBox.AppendText(this.MySettings.DumpToText(true));
                    if (loadSettingsToFEB)
                    {
                        if (this.LoadSuccess) 
                        {
                            loadLogBox.AppendText($"Settings file {this.LoadFileName} loaded to FEB successfully!\r\n");
                        }
                        else
                        {
                            loadLogBox.AppendText("Settings file failed to load to FEB.\r\n");
                        }

                    }
                }
                else
                {
                    loadLogBox.AppendText("ERROR: Selected file does not contain a full set of bias and gain settings! \r\n");
                    if (loadSettingsToFEB)
                    {
                        loadLogBox.AppendText("       Settings file was not loaded. \r\n");
                    }
                }
            }
            else
            {
                loadLogBox.AppendText($"ERROR: Specify the above values correctly first!\r\n");
            }
            return;
        }
    }
}
