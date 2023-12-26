namespace TB_mu2e
{
    partial class SequencerBlock
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.boxStepTag = new System.Windows.Forms.GroupBox();
            this.groupBoxLoad = new System.Windows.Forms.GroupBox();
            this.btnSelectFile = new System.Windows.Forms.Button();
            this.txtLoadFile = new System.Windows.Forms.TextBox();
            this.groupBoxRun = new System.Windows.Forms.GroupBox();
            this.txtDataTag = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.LogDynamicBiasChkBox = new System.Windows.Forms.CheckBox();
            this.DynamicVbiasChkBox = new System.Windows.Forms.CheckBox();
            this.groupBoxFEB = new System.Windows.Forms.GroupBox();
            this.panelFEB = new System.Windows.Forms.Panel();
            this.btnClear = new System.Windows.Forms.Button();
            this.btnAllFEB = new System.Windows.Forms.Button();
            this.groupBoxParameter = new System.Windows.Forms.GroupBox();
            this.lblParameter = new System.Windows.Forms.Label();
            this.txtParameter = new System.Windows.Forms.TextBox();
            this.groupBoxAction = new System.Windows.Forms.GroupBox();
            this.comboBoxAction = new System.Windows.Forms.ComboBox();
            this.boxStepTag.SuspendLayout();
            this.groupBoxLoad.SuspendLayout();
            this.groupBoxRun.SuspendLayout();
            this.groupBoxFEB.SuspendLayout();
            this.groupBoxParameter.SuspendLayout();
            this.groupBoxAction.SuspendLayout();
            this.SuspendLayout();
            // 
            // boxStepTag
            // 
            this.boxStepTag.Controls.Add(this.groupBoxLoad);
            this.boxStepTag.Controls.Add(this.groupBoxRun);
            this.boxStepTag.Controls.Add(this.groupBoxFEB);
            this.boxStepTag.Controls.Add(this.groupBoxParameter);
            this.boxStepTag.Controls.Add(this.groupBoxAction);
            this.boxStepTag.Location = new System.Drawing.Point(3, 3);
            this.boxStepTag.Name = "boxStepTag";
            this.boxStepTag.Size = new System.Drawing.Size(894, 124);
            this.boxStepTag.TabIndex = 35;
            this.boxStepTag.TabStop = false;
            this.boxStepTag.Text = "(boxStepTag)";
            this.boxStepTag.Paint += new System.Windows.Forms.PaintEventHandler(this.boxStepTag_Paint);
            // 
            // groupBoxLoad
            // 
            this.groupBoxLoad.Controls.Add(this.btnSelectFile);
            this.groupBoxLoad.Controls.Add(this.txtLoadFile);
            this.groupBoxLoad.Location = new System.Drawing.Point(330, 65);
            this.groupBoxLoad.Name = "groupBoxLoad";
            this.groupBoxLoad.Size = new System.Drawing.Size(558, 54);
            this.groupBoxLoad.TabIndex = 42;
            this.groupBoxLoad.TabStop = false;
            this.groupBoxLoad.Text = "Load";
            this.groupBoxLoad.Visible = false;
            // 
            // btnSelectFile
            // 
            this.btnSelectFile.Location = new System.Drawing.Point(412, 24);
            this.btnSelectFile.Name = "btnSelectFile";
            this.btnSelectFile.Size = new System.Drawing.Size(140, 20);
            this.btnSelectFile.TabIndex = 1;
            this.btnSelectFile.Text = "Select File";
            this.btnSelectFile.UseVisualStyleBackColor = true;
            this.btnSelectFile.Click += new System.EventHandler(this.btnSelectFile_Click);
            // 
            // txtLoadFile
            // 
            this.txtLoadFile.Location = new System.Drawing.Point(6, 24);
            this.txtLoadFile.Name = "txtLoadFile";
            this.txtLoadFile.ReadOnly = true;
            this.txtLoadFile.Size = new System.Drawing.Size(400, 20);
            this.txtLoadFile.TabIndex = 0;
            // 
            // groupBoxRun
            // 
            this.groupBoxRun.Controls.Add(this.txtDataTag);
            this.groupBoxRun.Controls.Add(this.label2);
            this.groupBoxRun.Controls.Add(this.LogDynamicBiasChkBox);
            this.groupBoxRun.Controls.Add(this.DynamicVbiasChkBox);
            this.groupBoxRun.Location = new System.Drawing.Point(330, 10);
            this.groupBoxRun.Name = "groupBoxRun";
            this.groupBoxRun.Size = new System.Drawing.Size(558, 49);
            this.groupBoxRun.TabIndex = 41;
            this.groupBoxRun.TabStop = false;
            this.groupBoxRun.Text = "Data Taking";
            this.groupBoxRun.Visible = false;
            // 
            // txtDataTag
            // 
            this.txtDataTag.Location = new System.Drawing.Point(64, 17);
            this.txtDataTag.Name = "txtDataTag";
            this.txtDataTag.Size = new System.Drawing.Size(205, 20);
            this.txtDataTag.TabIndex = 52;
            this.txtDataTag.TextChanged += new System.EventHandler(this.txtDataTag_TextChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 20);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(52, 13);
            this.label2.TabIndex = 51;
            this.label2.Text = "Data Tag";
            // 
            // LogDynamicBiasChkBox
            // 
            this.LogDynamicBiasChkBox.AutoSize = true;
            this.LogDynamicBiasChkBox.Enabled = false;
            this.LogDynamicBiasChkBox.Location = new System.Drawing.Point(412, 19);
            this.LogDynamicBiasChkBox.Name = "LogDynamicBiasChkBox";
            this.LogDynamicBiasChkBox.Size = new System.Drawing.Size(108, 17);
            this.LogDynamicBiasChkBox.TabIndex = 50;
            this.LogDynamicBiasChkBox.Text = "Log Bias Settings";
            this.LogDynamicBiasChkBox.UseVisualStyleBackColor = true;
            this.LogDynamicBiasChkBox.CheckedChanged += new System.EventHandler(this.LogDynamicBiasChkBox_CheckedChanged);
            // 
            // DynamicVbiasChkBox
            // 
            this.DynamicVbiasChkBox.AutoSize = true;
            this.DynamicVbiasChkBox.Location = new System.Drawing.Point(275, 19);
            this.DynamicVbiasChkBox.Name = "DynamicVbiasChkBox";
            this.DynamicVbiasChkBox.Size = new System.Drawing.Size(131, 17);
            this.DynamicVbiasChkBox.TabIndex = 49;
            this.DynamicVbiasChkBox.Text = "Dynamic Bias Settings";
            this.DynamicVbiasChkBox.UseVisualStyleBackColor = true;
            this.DynamicVbiasChkBox.CheckedChanged += new System.EventHandler(this.DynamicVbiasChkBox_CheckedChanged);
            // 
            // groupBoxFEB
            // 
            this.groupBoxFEB.Controls.Add(this.panelFEB);
            this.groupBoxFEB.Controls.Add(this.btnClear);
            this.groupBoxFEB.Controls.Add(this.btnAllFEB);
            this.groupBoxFEB.Location = new System.Drawing.Point(164, 10);
            this.groupBoxFEB.Name = "groupBoxFEB";
            this.groupBoxFEB.Size = new System.Drawing.Size(160, 109);
            this.groupBoxFEB.TabIndex = 40;
            this.groupBoxFEB.TabStop = false;
            this.groupBoxFEB.Text = "FEB";
            this.groupBoxFEB.Visible = false;
            // 
            // panelFEB
            // 
            this.panelFEB.AutoScroll = true;
            this.panelFEB.BackColor = System.Drawing.SystemColors.Window;
            this.panelFEB.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panelFEB.Location = new System.Drawing.Point(72, 9);
            this.panelFEB.Name = "panelFEB";
            this.panelFEB.Size = new System.Drawing.Size(85, 97);
            this.panelFEB.TabIndex = 9;
            // 
            // btnClear
            // 
            this.btnClear.Location = new System.Drawing.Point(6, 61);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(60, 35);
            this.btnClear.TabIndex = 8;
            this.btnClear.Text = "CLEAR";
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // btnAllFEB
            // 
            this.btnAllFEB.Location = new System.Drawing.Point(6, 20);
            this.btnAllFEB.Name = "btnAllFEB";
            this.btnAllFEB.Size = new System.Drawing.Size(60, 35);
            this.btnAllFEB.TabIndex = 7;
            this.btnAllFEB.Text = "ALL";
            this.btnAllFEB.UseVisualStyleBackColor = true;
            this.btnAllFEB.Click += new System.EventHandler(this.btnAllFEB_Click);
            // 
            // groupBoxParameter
            // 
            this.groupBoxParameter.Controls.Add(this.lblParameter);
            this.groupBoxParameter.Controls.Add(this.txtParameter);
            this.groupBoxParameter.Location = new System.Drawing.Point(6, 72);
            this.groupBoxParameter.Name = "groupBoxParameter";
            this.groupBoxParameter.Size = new System.Drawing.Size(152, 47);
            this.groupBoxParameter.TabIndex = 39;
            this.groupBoxParameter.TabStop = false;
            this.groupBoxParameter.Text = "(ControlValue)";
            this.groupBoxParameter.Visible = false;
            // 
            // lblParameter
            // 
            this.lblParameter.AutoSize = true;
            this.lblParameter.Location = new System.Drawing.Point(6, 20);
            this.lblParameter.Name = "lblParameter";
            this.lblParameter.Size = new System.Drawing.Size(66, 13);
            this.lblParameter.TabIndex = 4;
            this.lblParameter.Text = "(LabelValue)";
            // 
            // txtParameter
            // 
            this.txtParameter.Location = new System.Drawing.Point(78, 17);
            this.txtParameter.Name = "txtParameter";
            this.txtParameter.Size = new System.Drawing.Size(68, 20);
            this.txtParameter.TabIndex = 0;
            this.txtParameter.TextChanged += new System.EventHandler(this.txtParameter_TextChanged);
            // 
            // groupBoxAction
            // 
            this.groupBoxAction.Controls.Add(this.comboBoxAction);
            this.groupBoxAction.Location = new System.Drawing.Point(6, 19);
            this.groupBoxAction.Name = "groupBoxAction";
            this.groupBoxAction.Size = new System.Drawing.Size(152, 47);
            this.groupBoxAction.TabIndex = 38;
            this.groupBoxAction.TabStop = false;
            this.groupBoxAction.Text = "Action";
            // 
            // comboBoxAction
            // 
            this.comboBoxAction.FormattingEnabled = true;
            this.comboBoxAction.Items.AddRange(new object[] {
            "Idle",
            "Execute Console Command",
            "Set Pipeline Delay",
            "Load",
            "Set All Bias By Bulk",
            "Increase Bulk By Counts",
            "Increase Trim By Counts",
            "Set Gain To",
            "Take Data Run"});
            this.comboBoxAction.Location = new System.Drawing.Point(6, 20);
            this.comboBoxAction.Name = "comboBoxAction";
            this.comboBoxAction.Size = new System.Drawing.Size(140, 21);
            this.comboBoxAction.TabIndex = 1;
            this.comboBoxAction.SelectedIndexChanged += new System.EventHandler(this.comboBoxAction_SelectedIndexChanged);
            // 
            // SequencerBlock
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.boxStepTag);
            this.Name = "SequencerBlock";
            this.Size = new System.Drawing.Size(900, 130);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.SequencerBlock_MouseDown);
            this.boxStepTag.ResumeLayout(false);
            this.groupBoxLoad.ResumeLayout(false);
            this.groupBoxLoad.PerformLayout();
            this.groupBoxRun.ResumeLayout(false);
            this.groupBoxRun.PerformLayout();
            this.groupBoxFEB.ResumeLayout(false);
            this.groupBoxParameter.ResumeLayout(false);
            this.groupBoxParameter.PerformLayout();
            this.groupBoxAction.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.GroupBox boxStepTag;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.Button btnAllFEB;
        private System.Windows.Forms.GroupBox groupBoxParameter;
        private System.Windows.Forms.Label lblParameter;
        private System.Windows.Forms.GroupBox groupBoxAction;
        private System.Windows.Forms.GroupBox groupBoxRun;
        private System.Windows.Forms.GroupBox groupBoxLoad;
        private System.Windows.Forms.Button btnSelectFile;
        private System.Windows.Forms.Label label2;
        public System.Windows.Forms.Panel panelFEB;
        public System.Windows.Forms.ComboBox comboBoxAction;
        public System.Windows.Forms.TextBox txtParameter;
        public System.Windows.Forms.TextBox txtLoadFile;
        public System.Windows.Forms.CheckBox LogDynamicBiasChkBox;
        public System.Windows.Forms.CheckBox DynamicVbiasChkBox;
        public System.Windows.Forms.TextBox txtDataTag;
        public System.Windows.Forms.GroupBox groupBoxFEB;
    }
}
