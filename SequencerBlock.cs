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
using TB_mu2e;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Button;

namespace TB_mu2e
{
    public partial class SequencerBlock : UserControl
    {
        private System.Windows.Forms.CheckBox[] checkBoxes;
        public bool[] FEBSelection { get; set; }
        public int SequencerOrder { get; set; }
        public bool BlockValidated { get; set; }
        public bool InProgress { get; set; }
        public int TimerCounter { get; set; }

        private frmMain parentForm;

        public void InvalidateBlock()
        {
            BlockValidated = false;
        }
        private void ChkBoxFEB_CheckChanged(object sender, EventArgs e)
        {
            InvalidateBlock();
        }

        public SequencerBlock(int numFEB, int orderNum, string setvalues = "")
        {
            InitializeComponent();

            checkBoxes = new System.Windows.Forms.CheckBox[numFEB];
            FEBSelection = new bool[numFEB];
            SequencerOrder = orderNum;
            InProgress = false;
            TimerCounter = 0;

            for (int i = 0; i < numFEB; i++)
            {
                FEBSelection[i] = false;
                checkBoxes[i] = new System.Windows.Forms.CheckBox();
                checkBoxes[i].Name = $"chkBoxFEB{i}";
                checkBoxes[i].Checked = false;
                checkBoxes[i].Text = $"FEB {i}";
                checkBoxes[i].Location = new System.Drawing.Point(3, 1 + 15 * i);
                checkBoxes[i].Size = new System.Drawing.Size(50, 17);
                checkBoxes[i].CheckedChanged += ChkBoxFEB_CheckChanged;
                panelFEB.Controls.Add(checkBoxes[i]);
            }

            if (!string.IsNullOrEmpty(setvalues))
            {
                string[] fields = setvalues.Split(new string[] { "\n", "\r\n" }, StringSplitOptions.None);
                if (comboBoxAction.Items.Contains(fields[0]))
                {
                    comboBoxAction.SelectedItem = fields[0];
                    comboBoxAction_SelectedIndexChanged_Core();

                    string selectedOption = fields[0];
                    if (selectedOption == "Idle")
                    {
                        txtParameter.Text = fields[1];
                    }
                    else if (selectedOption == "Load")
                    {
                        int a = (new List<int> { checkBoxes.Length, fields[1].Length }).Min();
                        for (int i = 0; i < a; i++)
                        {
                            if (fields[1][i] == '1') checkBoxes[i].Checked = true;
                            else checkBoxes[i].Checked = false;
                        }
                        txtLoadFile.Text = fields[2];
                    }
                    else if (selectedOption == "Set Pipeline Delay" || 
                             selectedOption == "Set All Bias By Bulk" || 
                             selectedOption == "Increase Bulk By Counts" || 
                             selectedOption == "Increase Trim By Counts" || 
                             selectedOption == "Set Gain To")
                    {
                        txtParameter.Text = fields[1];
                        int a = (new List<int> { checkBoxes.Length, fields[2].Length }).Min();
                        for (int i = 0; i < a; i++)
                        {
                            if (fields[2][i] == '1') checkBoxes[i].Checked = true;
                            else checkBoxes[i].Checked = false;
                        }
                    }
                    else if (selectedOption == "Take Data Run")
                    {
                        txtParameter.Text = fields[1];
                        txtDataTag.Text = fields[2];
                        if (fields[3][0] == '1') this.DynamicVbiasChkBox.Checked = true;
                        else this.DynamicVbiasChkBox.Checked = false;
                        if (fields[3][1] == '1') this.LogDynamicBiasChkBox.Checked = true;
                        else this.LogDynamicBiasChkBox.Checked = false;
                        DynamicVChkBoxChange();
                    }
                }
                else
                {
                    parentForm.sequencerLog.AppendText($"Step {orderNum + 1}: Unable to parse the sequencer block.\r\n");
                }
            }

            InvalidateBlock();
        }

        private void ResetFields()
        {
            txtParameter.Text = string.Empty;
            foreach (System.Windows.Forms.CheckBox checkBox in checkBoxes)
            {
                checkBox.Checked = false;
            }
            txtDataTag.Text = string.Empty;
            this.DynamicVbiasChkBox.Checked = false;
            DynamicVChkBoxChange();
            txtLoadFile.Text = string.Empty;
            InvalidateBlock();
        }

        public void SetRoot()
        {
            Control currentControl = this;
            while (currentControl != null && !(currentControl is frmMain)) { currentControl = currentControl.Parent; }
            parentForm = (frmMain)currentControl;
        }

        private void comboBoxAction_SelectedIndexChanged_Core()
        {
            ResetFields();
            string selectedOption = comboBoxAction.SelectedItem.ToString();

            groupBoxFEB.Enabled = true;
            if (selectedOption == "Idle")
            {
                groupBoxParameter.Visible = true;
                groupBoxParameter.Text = "Duration";
                lblParameter.Text = "Time [s]";
                groupBoxFEB.Visible = false;
                groupBoxRun.Visible = false;
                groupBoxLoad.Visible = false;
            }
            else if (selectedOption == "Set Pipeline Delay")
            {
                groupBoxParameter.Visible = true;
                groupBoxParameter.Text = "Delay";
                lblParameter.Text = "Value (HEX)";
                groupBoxFEB.Visible = true;
                groupBoxRun.Visible = false;
                groupBoxLoad.Visible = false;
            }
            else if (selectedOption == "Load")
            {
                groupBoxParameter.Visible = false;
                groupBoxFEB.Visible = true;
                groupBoxRun.Visible = false;
                groupBoxLoad.Visible = true;
                txtLoadFile.Enabled = false;
            }
            else if (selectedOption == "Set All Bias By Bulk")
            {
                groupBoxParameter.Visible = true;
                groupBoxParameter.Text = "Set Bulk";
                lblParameter.Text = "Voltages [V]";
                groupBoxFEB.Visible = true;
                groupBoxRun.Visible = false;
                groupBoxLoad.Visible = false;
            }
            else if (selectedOption == "Increase Bulk By Counts")
            {
                groupBoxParameter.Visible = true;
                groupBoxParameter.Text = "Increase By";
                lblParameter.Text = $"{PP.mVPerBulkUnit:F0} mV/Count";
                groupBoxFEB.Visible = true;
                groupBoxRun.Visible = false;
                groupBoxLoad.Visible = false;
            }
            else if (selectedOption == "Increase Trim By Counts")
            {
                groupBoxParameter.Visible = true;
                groupBoxParameter.Text = "Increase By";
                lblParameter.Text = $"{PP.mVPerTrimUnit:F0} mV/Count";
                groupBoxFEB.Visible = true;
                groupBoxRun.Visible = false;
                groupBoxLoad.Visible = false;
            }
            else if (selectedOption == "Set Gain To")
            {
                groupBoxParameter.Visible = true;
                groupBoxParameter.Text = "Set Gain";
                lblParameter.Text = "Gain (HEX)";
                groupBoxFEB.Visible = true;
                groupBoxRun.Visible = false;
                groupBoxLoad.Visible = false;
            }
            else if (selectedOption == "Take Data Run")
            {
                groupBoxParameter.Visible = true;
                groupBoxParameter.Text = "Length";
                lblParameter.Text = "Spill Count";
                groupBoxFEB.Visible = true;
                groupBoxFEB.Enabled = false;
                foreach (System.Windows.Forms.CheckBox checkBox in checkBoxes)
                {
                    checkBox.Checked = true;
                }
                groupBoxRun.Visible = true;
                this.DynamicVbiasChkBox.Enabled = true;
                this.DynamicVbiasChkBox.Checked = false;
                this.LogDynamicBiasChkBox.Enabled = false;
                this.LogDynamicBiasChkBox.Checked = false;
                groupBoxLoad.Visible = false;
            }
        }

        private void comboBoxAction_SelectedIndexChanged(object sender, EventArgs e)
        {
            comboBoxAction_SelectedIndexChanged_Core();
        }

        public void SelectFEB()
        {
            for (int i = 0; i < checkBoxes.Length; i++)
            {
                if (checkBoxes[i].Checked) 
                { 
                    FEBSelection[i] = true;
                }
                else 
                { 
                    FEBSelection[i] = false;
                }
            }
        }

        private void btnAllFEB_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < checkBoxes.Length; i++)
            {
                checkBoxes[i].Checked = true;
            }
            InvalidateBlock();
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < checkBoxes.Length; i++)
            {
                checkBoxes[i].Checked = false;
            }
            InvalidateBlock();
        }

        public void DynamicVChkBoxChange()
        {
            if (!this.DynamicVbiasChkBox.Checked)
            {
                this.LogDynamicBiasChkBox.Enabled = false;
                this.LogDynamicBiasChkBox.Checked = false;
            }
            else
            {
                this.LogDynamicBiasChkBox.Enabled = true;
            }
        }

        private void DynamicVbiasChkBox_CheckedChanged(object sender, EventArgs e)
        {
            DynamicVChkBoxChange();
            InvalidateBlock();
        }

        private void btnSelectFile_Click(object sender, EventArgs e)
        {
            string buffer = "";
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
                    buffer = loadCmdsDialog.FileName;
                    InvalidateBlock();
                }
            }));
            loadCmdsFileDialogThread.SetApartmentState(ApartmentState.STA);
            loadCmdsFileDialogThread.Start();
            loadCmdsFileDialogThread.Join();

            txtLoadFile.Text = buffer;
        }

        private void SequencerBlock_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                parentForm.SequencerPtr = this.SequencerOrder + 1;
            }
            parentForm.HighlightSequencerBlock();
        }

        public string AssembleFEBNames()
        {
            string FEBSelected = "[";
            for (int j = 0; j < FEBSelection.Length; j++)
            {
                if (FEBSelection[j])
                {
                    FEBSelected += $"{j}, ";
                }
            }
            FEBSelected = FEBSelected.Remove(FEBSelected.Length - 2);
            FEBSelected += "]";
            return FEBSelected;
        }

        public bool CheckSequencerBlockValid(int iStep)
        {
            bool isValid = false;
            string selectedOption = comboBoxAction.SelectedItem.ToString();
            if (selectedOption == "Idle")
            {
                bool containsOnlyDigits = Regex.IsMatch(txtParameter.Text, @"^\d+$");
                if (containsOnlyDigits)
                {
                    isValid = true;
                    parentForm.sequencerLog.AppendText($"Step {iStep + 1}: Idle for {txtParameter.Text} seconds.\r\n");
                }
                else
                {
                    parentForm.sequencerLog.AppendText($"Invalid input at Step {iStep + 1}: Expect an integer value input.\r\n");
                }
            }
            else if (selectedOption == "Set Pipeline Delay")
            {
                bool isGoodHex = Regex.IsMatch(txtParameter.Text, "^[0-9A-Fa-f]+$");

                bool selected = false;
                SelectFEB();
                foreach (bool b in FEBSelection)
                {
                    if (b)
                    {
                        selected = true;
                        break;
                    }
                }

                if (isGoodHex && selected)
                {
                    isValid = true;
                    parentForm.sequencerLog.AppendText($"Step {iStep + 1}: Set all pipeline delays to 0x{txtParameter.Text} for FEB {AssembleFEBNames()}.\r\n");
                }
                else
                {
                    if (!isGoodHex) { parentForm.sequencerLog.AppendText($"Invalid input at Step {iStep + 1}: Expect a hex value (no 0x prefix).\r\n"); }
                    if (!selected) { parentForm.sequencerLog.AppendText($"Invalid input at Step {iStep + 1}: No FEB selected.\r\n"); }
                }
            }
            else if (selectedOption == "Load")
            {
                bool selected = false;
                SelectFEB();
                foreach (bool b in FEBSelection)
                {
                    if (b)
                    {
                        selected = true;
                        break;
                    }
                }
                if (selected)
                {
                    isValid = true;
                    parentForm.sequencerLog.AppendText($"Step {iStep + 1}: Load {txtLoadFile.Text} for FEB {AssembleFEBNames()}.\r\n");
                }
                else
                {
                    parentForm.sequencerLog.AppendText($"Invalid input at Step {iStep + 1}: No FEB selected.\r\n");
                }
            }
            else if (selectedOption == "Set All Bias By Bulk")
            {
                bool isDoubleInRange = false;
                if (Regex.IsMatch(txtParameter.Text, @"^(\d+(\.\d*)?|\.\d+)$"))
                {
                    double targetV = double.Parse(txtParameter.Text);
                    if (targetV < 60.0 && targetV >= 0.0)
                    {
                        isDoubleInRange = true;
                    }
                }
                bool selected = false;
                SelectFEB();
                foreach (bool b in FEBSelection)
                {
                    if (b)
                    {
                        selected = true;
                        break;
                    }
                }
                if (isDoubleInRange && selected)
                {
                    isValid = true;
                    parentForm.sequencerLog.AppendText($"Step {iStep + 1}: Set all bias (by bulk) to {txtParameter.Text} V for FEB {AssembleFEBNames()}.\r\n");
                }
                else
                {
                    if (!isDoubleInRange) { parentForm.sequencerLog.AppendText($"Invalid input at Step {iStep + 1}: Expect a double value between 0 and 60.\r\n"); }
                    if (!selected) { parentForm.sequencerLog.AppendText($"Invalid input at Step {iStep + 1}: No FEB selected.\r\n"); }
                }
            }

            else if (selectedOption == "Increase Bulk By Counts")
            {
                bool containsOnlyDigits = Regex.IsMatch(txtParameter.Text, @"^[+-]?\d+$");

                bool selected = false;
                SelectFEB();
                foreach (bool b in FEBSelection)
                {
                    if (b)
                    {
                        selected = true;
                        break;
                    }
                }

                // tally maxChange, check max bias < 60, min > 40
                bool goodValue = true;
                if (containsOnlyDigits && selected)
                {
                    double thisIncreaseBias = (double)(int.Parse(txtParameter.Text)) * PP.mVPerBulkUnit / 1000.0;
                    for (int i = 0; i < FEBSelection.Length; i++)
                    {
                        if (FEBSelection[i])
                        {
                            parentForm.totalVBiasChange[i] += thisIncreaseBias;
                        }
                    }
                    for (int i = 0; i < PP.Num_FEB_clients; i++)
                    {
                        bool thisGood = (parentForm.highestBiasAtSetup[i] + parentForm.totalVBiasChange[i] < 60.0);
                        thisGood = (thisGood && (parentForm.lowestBiasAtSetup[i] + parentForm.totalVBiasChange[i] > 40.0)); // Attempt to prevent bulk to go into 0xfff
                        goodValue = (goodValue && thisGood);
                    }
                }

                if (containsOnlyDigits && selected && goodValue)
                {
                    isValid = true;
                    parentForm.sequencerLog.AppendText($"Step {iStep + 1}: Increase all bulks by {txtParameter.Text} counts for FEB {AssembleFEBNames()}.\r\n");
                }
                else
                {
                    if (!containsOnlyDigits) { parentForm.sequencerLog.AppendText($"Invalid input at Step {iStep + 1}: Expect an integer value count.\r\n"); }
                    if (!selected) { parentForm.sequencerLog.AppendText($"Invalid input at Step {iStep + 1}: No FEB selected.\r\n"); }
                    if (!goodValue) { parentForm.sequencerLog.AppendText($"Invalid input at Step {iStep + 1}: Some SiPM bias wanders out of the (40, 60) V range.\r\n"); }
                }
            }

            else if (selectedOption == "Increase Trim By Counts")
            {
                bool containsOnlyDigits = Regex.IsMatch(txtParameter.Text, @"^[+-]?\d+$");

                bool selected = false;
                SelectFEB();
                foreach (bool b in FEBSelection)
                {
                    if (b)
                    {
                        selected = true;
                        break;
                    }
                }

                // tally changes, check trim within [0, 4095], max bias < 60, min > 40
                bool goodValue = true;
                if (containsOnlyDigits && selected)
                {
                    int thisIncreaseTrim = int.Parse(txtParameter.Text);
                    if (thisIncreaseTrim < -4095 || thisIncreaseTrim > 4095)
                    {
                        goodValue = false;
                    }
                    else
                    {
                        double thisIncreaseBias = (double)(thisIncreaseTrim) * PP.mVPerTrimUnit / 1000.0;
                        for (int i = 0; i < FEBSelection.Length; i++)
                        {
                            if (FEBSelection[i])
                            {
                                parentForm.totalTrimChange[i] += thisIncreaseTrim;
                                parentForm.totalVBiasChange[i] += thisIncreaseBias;
                            }
                        }
                        for (int i = 0; i < PP.Num_FEB_clients; i++)
                        {
                            bool thisGood = (parentForm.highestBiasAtSetup[i] + parentForm.totalVBiasChange[i] < 60.0);
                            thisGood = (thisGood && (parentForm.lowestBiasAtSetup[i] + parentForm.totalVBiasChange[i] > 40.0)); // Attempt to prevent bulk to go into 0xfff
                            thisGood = (thisGood && (parentForm.highestTrimAtSetup[i] + parentForm.totalTrimChange[i] < 4096));
                            thisGood = (thisGood && (parentForm.lowestTrimAtSetup[i] + parentForm.totalTrimChange[i] >= 0));
                            goodValue = (goodValue && thisGood);
                        }
                    }
                }

                if (containsOnlyDigits && selected && goodValue)
                {
                    isValid = true;
                    parentForm.sequencerLog.AppendText($"Step {iStep + 1}: Increase all trims by {txtParameter.Text} counts for FEB {AssembleFEBNames()}.\r\n");
                }
                else
                {
                    if (!containsOnlyDigits) { parentForm.sequencerLog.AppendText($"Invalid input at Step {iStep + 1}: Expect an integer value count.\r\n"); }
                    if (!selected) { parentForm.sequencerLog.AppendText($"Invalid input at Step {iStep + 1}: No FEB selected.\r\n"); }
                    if (!goodValue) { parentForm.sequencerLog.AppendText($"Invalid input at Step {iStep + 1}: Some trim value out of the [0, 0xFFF] range, or some SiPM bias out of the (40, 60) V range.\r\n"); }
                }
            }

            else if (selectedOption == "Set Gain To")
            {
                bool isGoodHex = Regex.IsMatch(txtParameter.Text, "^[0-9A-Fa-f]+$");

                bool selected = false;
                SelectFEB();
                foreach (bool b in FEBSelection)
                {
                    if (b)
                    {
                        selected = true;
                        break;
                    }
                }

                bool goodValue = true;
                if (isGoodHex && selected)
                {
                    int val = int.Parse(txtParameter.Text, System.Globalization.NumberStyles.HexNumber);
                    if (val < 0 || val > 4095) goodValue = false;
                }

                if (isGoodHex && selected && goodValue)
                {
                    isValid = true;
                    parentForm.sequencerLog.AppendText($"Step {iStep + 1}: Set all gains to 0x{txtParameter.Text} for FEB {AssembleFEBNames()}.\r\n");
                }
                else
                {
                    if (!isGoodHex) { parentForm.sequencerLog.AppendText($"Invalid input at Step {iStep + 1}: Expect a hex value (no 0x prefix).\r\n"); }
                    if (!selected) { parentForm.sequencerLog.AppendText($"Invalid input at Step {iStep + 1}: No FEB selected.\r\n"); }
                    if (!goodValue) { parentForm.sequencerLog.AppendText($"Invalid input at Step {iStep + 1}: gain value out of the [0, 0xFFF] range.\r\n"); }
                }
            }
            else if (selectedOption == "Take Data Run")
            {
                bool containsOnlyDigits = Regex.IsMatch(txtParameter.Text, @"^\d+$");
                bool tagPopulated = !string.IsNullOrEmpty(txtDataTag.Text);
                if (containsOnlyDigits && tagPopulated)
                {
                    isValid = true;
                    parentForm.sequencerLog.AppendText($"Step {iStep + 1}: Take a run of {txtParameter.Text} spills for all FEBs.\r\n");
                }
                else
                {
                    if (!containsOnlyDigits) { parentForm.sequencerLog.AppendText($"Invalid input at Step {iStep + 1}: Expect an integer number of spills.\r\n"); }
                    if (!tagPopulated) { parentForm.sequencerLog.AppendText($"Invalid input at Step {iStep + 1}: No data tag entered.\r\n"); }
                }
            }
            else
            {
                parentForm.sequencerLog.AppendText($"Step {iStep + 1}: No action selected.\r\n");                 
            }

            if (isValid) BlockValidated = true;
            else InvalidateBlock();
            return isValid;
        }

        private void txtParameter_TextChanged(object sender, EventArgs e)
        {
            InvalidateBlock();
        }

        private void txtDataTag_TextChanged(object sender, EventArgs e)
        {
            InvalidateBlock();
        }

        private void LogDynamicBiasChkBox_CheckedChanged(object sender, EventArgs e)
        {
            InvalidateBlock();
        }

        public string SaveSequencerBlockAsString()
        {
            string blockString = "";
            string selectedOption = comboBoxAction.SelectedItem.ToString();
            blockString += selectedOption;
            blockString += "\n";
            if (selectedOption == "Idle")
            {
                blockString += txtParameter.Text;
                blockString += "\n";
            }
            else if (selectedOption == "Load")
            {
                for (int i = 0; i < checkBoxes.Length; i++)
                {
                    if (checkBoxes[i].Checked) blockString += "1";
                    else blockString += "0";
                }
                blockString += "\n";
                blockString += txtLoadFile.Text;
                blockString += "\n";
            }
            else if (selectedOption == "Set Pipeline Delay" ||
                     selectedOption == "Set All Bias By Bulk" || 
                     selectedOption == "Increase Bulk By Counts" || 
                     selectedOption == "Increase Trim By Counts" || 
                     selectedOption == "Set Gain To")
            {
                blockString += txtParameter.Text;
                blockString += "\n";
                for (int i = 0; i < checkBoxes.Length; i++)
                {
                    if (checkBoxes[i].Checked) blockString += "1";
                    else blockString += "0";
                }
                blockString += "\n";
            }
            else if (selectedOption == "Take Data Run")
            {
                blockString += txtParameter.Text;
                blockString += "\n";
                blockString += txtDataTag.Text;
                blockString += "\n";
                if (this.DynamicVbiasChkBox.Checked) blockString += "1";
                else blockString += "0";
                if (this.LogDynamicBiasChkBox.Checked) blockString += "1";
                else blockString += "0";
                blockString += "\n";
            }
            return blockString;
        }

        // actions to perform
        public int ExecuteSequencerBlock(int iStep)
        {
            this.TimerCounter += 1;
            string selectedOption = comboBoxAction.SelectedItem.ToString();
            try
            {
                bool checkReadback = false;
                if (selectedOption == "Idle")
                {
                    if (this.InProgress)
                    {
                        this.TimerCounter += 1;
                    }
                    if (this.TimerCounter > int.Parse(txtParameter.Text))
                    {
                        parentForm.UpdateSequencerStatus($"Step {iStep + 1} done: Idled {txtParameter.Text} seconds.");
                        this.InProgress = false;
                        this.TimerCounter = 0;
                        parentForm.SequencerBusy = false;
                    }
                }
                else if (selectedOption == "Set Pipeline Delay")
                {
                    for (int i = 0; i < FEBSelection.Length; i++)
                    {
                        if (FEBSelection[i])
                        {
                            Mu2e_FEB_client selectedFEB = PP.FEB_clients[i];
                            if (selectedFEB != null && selectedFEB.ClientOpen)
                            {
                                selectedFEB.SendStr("wr 304 " + txtParameter.Text);
                            }
                            else
                            {
                                throw new Exception($"FEB {i} port not open. New pipeline delay did not load.");
                            }
                        }
                    }
                    Thread.Sleep(1000); // give 1s 
                    parentForm.UpdateSequencerStatus($"Step {iStep + 1} done: Set pipeline delays to 0x{txtParameter.Text} for FEB {AssembleFEBNames()}.");
                    this.InProgress = false;
                    this.TimerCounter = 0;
                    parentForm.SequencerBusy = false;
                }
                else if (selectedOption == "Load") 
                {
                    checkReadback = true;
                    // load to referenceFEBSettings of corresponding FEBs, write to board
                    for (int i = 0; i < FEBSelection.Length; i++)
                    {
                        if (FEBSelection[i])
                        {
                            Mu2e_FEB_client selectedFEB = PP.FEB_clients[i];
                            if (selectedFEB != null && selectedFEB.ClientOpen)
                            {
                                // load 
                                using (StreamReader cmdFile = new StreamReader(File.OpenRead(txtLoadFile.Text)))
                                {
                                    while (!cmdFile.EndOfStream)
                                    {
                                        string cmd = cmdFile.ReadLine();
                                        char commentChk = cmd.First();
                                        if (commentChk != '$' || commentChk != '#' || commentChk != '/')
                                            selectedFEB.SendStr(cmd);
                                    }
                                }
                                // update referenceFEBSettings
                                using (StreamReader cmdFile = new StreamReader(File.OpenRead(txtLoadFile.Text)))
                                {
                                    PP.referenceFEBSettings[i] = new FEBSettingsBiasGain(cmdFile, i, PP.defaultCMBTemp, PP.defaultFEBTemp);
                                    if (!PP.referenceFEBSettings[i].CheckValid())
                                    {
                                        throw new Exception($"FEB {i} loaded invalid settings. Missing bulk / trim / gain.");
                                    }
                                }
                            }
                            else
                            {
                                throw new Exception($"FEB {i} port not open. New settings did not load.");
                            }
                        }
                    }
                    Thread.Sleep(10000); // give 10s for load to finish
                    parentForm.UpdateSequencerStatus($"Step {iStep + 1} done: Loaded from settings file {txtLoadFile.Text} for FEB {AssembleFEBNames()}.");
                    // go the checkReadback part for validataion and close block
                }
                else if (selectedOption == "Set All Bias By Bulk")
                {
                    checkReadback = true;
                    double newBulk = double.Parse(txtParameter.Text);
                    int newBulkCount = Convert.ToInt32(System.Math.Round((double)newBulk * 1000.0 / PP.mVPerBulkUnit));
                    // load to referenceFEBSettings of corresponding FEBs, write to board
                    for (int i = 0; i < FEBSelection.Length; i++)
                    {
                        if (FEBSelection[i])
                        {
                            Mu2e_FEB_client selectedFEB = PP.FEB_clients[i];
                            if (selectedFEB != null && selectedFEB.ClientOpen)
                            {
                                // update referenceFEBSettings
                                PP.referenceFEBSettings[i] = PP.referenceFEBSettings[i].CopyWithNewBulk(newBulkCount);
                                // load
                                PP.referenceFEBSettings[i].FEBWriteBulks();
                            }
                            else
                            {
                                throw new Exception($"FEB {i} port not open. New settings did not load.");
                            }
                        }
                    }
                    Thread.Sleep(10000); // give 10s for load to finish
                    parentForm.UpdateSequencerStatus($"Step {iStep + 1} done: Set all bulks to {txtParameter.Text} V for FEB {AssembleFEBNames()}.");
                    // go the checkReadback part for validataion and close block
                }
                else if (selectedOption == "Increase Bulk By Counts")
                {
                    checkReadback = true;
                    int increaseBulkBy = int.Parse(txtParameter.Text);
                    // load to referenceFEBSettings of corresponding FEBs, write to board
                    for (int i = 0; i < FEBSelection.Length; i++)
                    {
                        if (FEBSelection[i])
                        {
                            Mu2e_FEB_client selectedFEB = PP.FEB_clients[i];
                            if (selectedFEB != null && selectedFEB.ClientOpen)
                            {
                                // update referenceFEBSettings
                                PP.referenceFEBSettings[i] = PP.referenceFEBSettings[i].CopyAddToBulk(increaseBulkBy);
                                // load
                                PP.referenceFEBSettings[i].FEBWriteBulks();
                            }
                            else
                            {
                                throw new Exception($"FEB {i} port not open. New settings did not load.");
                            }
                        }
                    }
                    Thread.Sleep(10000); // give 10s for load to finish
                    parentForm.UpdateSequencerStatus($"Step {iStep + 1} done: Increased all bulks by {txtParameter.Text} for FEB {AssembleFEBNames()}.");
                    // go the checkReadback part for validataion and close block
                }
                else if (selectedOption == "Increase Trim By Counts")
                {
                    checkReadback = true;
                    int increaseTrimBy = int.Parse(txtParameter.Text);
                    // load to referenceFEBSettings of corresponding FEBs, write to board
                    for (int i = 0; i < FEBSelection.Length; i++)
                    {
                        if (FEBSelection[i])
                        {
                            Mu2e_FEB_client selectedFEB = PP.FEB_clients[i];
                            if (selectedFEB != null && selectedFEB.ClientOpen)
                            {
                                // update referenceFEBSettings
                                PP.referenceFEBSettings[i] = PP.referenceFEBSettings[i].CopyAddToTrim(increaseTrimBy);
                                // load
                                PP.referenceFEBSettings[i].FEBWriteTrims();
                            }
                            else
                            {
                                throw new Exception($"FEB {i} port not open. New settings did not load.");
                            }
                        }
                    }
                    Thread.Sleep(10000); // give 10s for load to finish
                    parentForm.UpdateSequencerStatus($"Step {iStep + 1} done: Increased all trims by {txtParameter.Text} for FEB {AssembleFEBNames()}.");
                    // go the checkReadback part for validataion and close block
                }
                else if (selectedOption == "Set Gain To")
                {
                    int newGain = int.Parse(txtParameter.Text, System.Globalization.NumberStyles.HexNumber);
                    // load to referenceFEBSettings of corresponding FEBs, write to board
                    for (int i = 0; i < FEBSelection.Length; i++)
                    {
                        if (FEBSelection[i])
                        {
                            Mu2e_FEB_client selectedFEB = PP.FEB_clients[i];
                            if (selectedFEB != null && selectedFEB.ClientOpen)
                            {
                                // update referenceFEBSettings
                                PP.referenceFEBSettings[i] = PP.referenceFEBSettings[i].CopyWithNewGain(newGain);
                                // load
                                PP.referenceFEBSettings[i].FEBWriteGains();
                            }
                            else
                            {
                                throw new Exception($"FEB {i} port not open. New settings did not load.");
                            }
                        }
                    }
                    Thread.Sleep(10000); // give 10s for load to finish
                    parentForm.UpdateSequencerStatus($"Step {iStep + 1} done: Set gains to 0x{txtParameter.Text} for FEB {AssembleFEBNames()}.");
                    this.InProgress = false;
                    this.TimerCounter = 0;
                    parentForm.SequencerBusy = false;
                }
                else if (selectedOption == "Take Data Run")
                {
                    int spillToReach = int.Parse(txtParameter.Text);
                    if (this.InProgress)
                    {
                        this.TimerCounter += 1;
                        // frmMain.SpillTimer handles the actual data taking. 
                    }
                    
                    // this is the beginning of next spill
                    if (parentForm.Get_spill_num() >= spillToReach && (!PP.myRun.spill_complete))
                    {
                        parentForm.BtnStopRun_Core();
                        parentForm.UpdateSequencerStatus($"Step {iStep + 1} done: Took data run for {txtParameter.Text} spills.");
                        this.InProgress = false;
                        this.TimerCounter = 0;
                        parentForm.SequencerBusy = false;
                    }
                }
                if (checkReadback)
                {
                    parentForm.UpdateSequencerStatus($"Step {iStep + 1} checking readback:");
                    
                    for (int i = 0; i < FEBSelection.Length; i++)
                    {
                        if (FEBSelection[i])
                        {
                            // write expected V, readback
                            parentForm.UpdateSequencerStatus("Settings:"+ PP.referenceFEBSettings[i].DumpToText(true));

                            double[] newMeasuredBias = new double[8];
                            bool readBackOK = true;
                            string output = "";
                            Mu2e_FEB_client selectedFEB = PP.FEB_clients[i];
                            selectedFEB.SendStr("adc 1");
                            System.Threading.Thread.Sleep(500);
                            selectedFEB.ReadStr(out string adcrtn1, out int rtn_time1);
                            System.Threading.Thread.Sleep(1000); // first read to flush
                            selectedFEB.SendStr("adc 1");
                            System.Threading.Thread.Sleep(500); // use second read results
                            selectedFEB.ReadStr(out string adcrtn2, out int rtn_time2);
                            string[] tok2 = adcrtn2.Split(new string[] { " ", Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                            if (tok2.Length == 17)
                            { 
                                for (int j = 0; j < 8; j++)
                                {
                                    newMeasuredBias[j] = double.Parse(tok2[8 + j]);
                                    output += (newMeasuredBias[j].ToString() + " ");
                                    if (newMeasuredBias[j] > 60.0)
                                    {
                                        readBackOK = false; // if out of normal V range flag error
                                    }
                                }
                                parentForm.UpdateSequencerStatus($"Readback (bulk): {output}");
                            }
                            else
                            {
                                parentForm.UpdateSequencerStatus($"Warning: FEB{i} failed to readback bulk voltages."); 
                            }

                            // if out of normal V range set to 0 for all, abort
                            if (!readBackOK)
                            {
                                foreach (Mu2e_FEB_client FEB in PP.FEB_clients)
                                {
                                    FEB.SetVAll(0.0);
                                }
                                throw new Exception("SAFETY ABORT! Bulk voltage exceeding 60 V. All FEBs set to 0 V.");
                            }
                        }
                    }
                    
                    this.InProgress = false;
                    this.TimerCounter = 0;
                    parentForm.SequencerBusy = false;
                }
                return 0;
            }
            catch (Exception ex)
            {
                parentForm.UpdateSequencerStatus($"Error during run Step {iStep + 1}: " + ex.Message);
                this.InProgress = false;
                this.TimerCounter = 0;
                parentForm.SequencerBusy = false;
                parentForm.btnStop_Core();
                return 1;
            }
        }

        private void boxStepTag_Paint(object sender, PaintEventArgs e)
        {
            comboBoxAction.SelectionLength = 0;
        }
    }
}
