namespace TB_mu2e
{
    partial class FEBBiasGainUsrCtrl
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
            this.label9 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.txtRefFEBTemp = new System.Windows.Forms.TextBox();
            this.txtRefCMBTemp = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.txtFEBRate = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.txtCMBRate = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.boxFEBTag = new System.Windows.Forms.GroupBox();
            this.loadLogBox = new System.Windows.Forms.RichTextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.chkBoxLoadSettings = new System.Windows.Forms.CheckBox();
            this.RdSettingsBtn = new System.Windows.Forms.Button();
            this.boxFEBTag.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.BackColor = System.Drawing.Color.Transparent;
            this.label9.Location = new System.Drawing.Point(383, 58);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(32, 13);
            this.label9.TabIndex = 32;
            this.label9.Text = "degC";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.BackColor = System.Drawing.Color.Transparent;
            this.label8.Location = new System.Drawing.Point(383, 33);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(32, 13);
            this.label8.TabIndex = 31;
            this.label8.Text = "degC";
            // 
            // txtRefFEBTemp
            // 
            this.txtRefFEBTemp.Location = new System.Drawing.Point(300, 54);
            this.txtRefFEBTemp.Name = "txtRefFEBTemp";
            this.txtRefFEBTemp.Size = new System.Drawing.Size(77, 20);
            this.txtRefFEBTemp.TabIndex = 30;
            // 
            // txtRefCMBTemp
            // 
            this.txtRefCMBTemp.Location = new System.Drawing.Point(300, 29);
            this.txtRefCMBTemp.Name = "txtRefCMBTemp";
            this.txtRefCMBTemp.Size = new System.Drawing.Size(77, 20);
            this.txtRefCMBTemp.TabIndex = 29;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(297, 9);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(120, 13);
            this.label7.TabIndex = 28;
            this.label7.Text = "Reference Temperature";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(219, 58);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(52, 13);
            this.label6.TabIndex = 27;
            this.label6.Text = "mV/degC";
            // 
            // txtFEBRate
            // 
            this.txtFEBRate.Location = new System.Drawing.Point(136, 54);
            this.txtFEBRate.Name = "txtFEBRate";
            this.txtFEBRate.Size = new System.Drawing.Size(77, 20);
            this.txtFEBRate.TabIndex = 26;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(219, 33);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(52, 13);
            this.label5.TabIndex = 25;
            this.label5.Text = "mV/degC";
            // 
            // txtCMBRate
            // 
            this.txtCMBRate.Location = new System.Drawing.Point(136, 29);
            this.txtCMBRate.Name = "txtCMBRate";
            this.txtCMBRate.Size = new System.Drawing.Size(77, 20);
            this.txtCMBRate.TabIndex = 24;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(24, 58);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(86, 13);
            this.label4.TabIndex = 22;
            this.label4.Text = "FEB Vover/temp";
            this.label4.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(168, 9);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(81, 13);
            this.label3.TabIndex = 21;
            this.label3.Text = "Correction Rate";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(21, 33);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(89, 13);
            this.label1.TabIndex = 18;
            this.label1.Text = "CMB Vover/temp";
            this.label1.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // boxFEBTag
            // 
            this.boxFEBTag.Controls.Add(this.loadLogBox);
            this.boxFEBTag.Controls.Add(this.groupBox1);
            this.boxFEBTag.Controls.Add(this.label9);
            this.boxFEBTag.Controls.Add(this.label8);
            this.boxFEBTag.Controls.Add(this.txtRefFEBTemp);
            this.boxFEBTag.Controls.Add(this.txtRefCMBTemp);
            this.boxFEBTag.Controls.Add(this.label7);
            this.boxFEBTag.Controls.Add(this.label6);
            this.boxFEBTag.Controls.Add(this.txtFEBRate);
            this.boxFEBTag.Controls.Add(this.label5);
            this.boxFEBTag.Controls.Add(this.txtCMBRate);
            this.boxFEBTag.Controls.Add(this.label4);
            this.boxFEBTag.Controls.Add(this.label3);
            this.boxFEBTag.Controls.Add(this.label1);
            this.boxFEBTag.Location = new System.Drawing.Point(13, 12);
            this.boxFEBTag.Name = "boxFEBTag";
            this.boxFEBTag.Size = new System.Drawing.Size(564, 256);
            this.boxFEBTag.TabIndex = 34;
            this.boxFEBTag.TabStop = false;
            this.boxFEBTag.Text = "(boxFEBTag)";
            // 
            // loadLogBox
            // 
            this.loadLogBox.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.loadLogBox.Location = new System.Drawing.Point(13, 80);
            this.loadLogBox.Name = "loadLogBox";
            this.loadLogBox.ReadOnly = true;
            this.loadLogBox.Size = new System.Drawing.Size(537, 164);
            this.loadLogBox.TabIndex = 36;
            this.loadLogBox.Text = "";
            this.loadLogBox.TextChanged += new System.EventHandler(this.LoadLog_TextChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.chkBoxLoadSettings);
            this.groupBox1.Controls.Add(this.RdSettingsBtn);
            this.groupBox1.Location = new System.Drawing.Point(448, 9);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(109, 65);
            this.groupBox1.TabIndex = 35;
            this.groupBox1.TabStop = false;
            // 
            // chkBoxLoadSettings
            // 
            this.chkBoxLoadSettings.AutoSize = true;
            this.chkBoxLoadSettings.Checked = true;
            this.chkBoxLoadSettings.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkBoxLoadSettings.Location = new System.Drawing.Point(17, 45);
            this.chkBoxLoadSettings.Name = "chkBoxLoadSettings";
            this.chkBoxLoadSettings.Size = new System.Drawing.Size(73, 17);
            this.chkBoxLoadSettings.TabIndex = 34;
            this.chkBoxLoadSettings.Text = "Also Load";
            this.chkBoxLoadSettings.UseVisualStyleBackColor = true;
            // 
            // RdSettingsBtn
            // 
            this.RdSettingsBtn.Location = new System.Drawing.Point(6, 10);
            this.RdSettingsBtn.Name = "RdSettingsBtn";
            this.RdSettingsBtn.Size = new System.Drawing.Size(97, 30);
            this.RdSettingsBtn.TabIndex = 33;
            this.RdSettingsBtn.Text = "Read Settings";
            this.RdSettingsBtn.UseVisualStyleBackColor = true;
            this.RdSettingsBtn.Click += new System.EventHandler(this.RdSettingsBtn_Click);
            // 
            // FEBBiasGainUsrCtrl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.boxFEBTag);
            this.Name = "FEBBiasGainUsrCtrl";
            this.Size = new System.Drawing.Size(590, 280);
            this.boxFEBTag.ResumeLayout(false);
            this.boxFEBTag.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button RdSettingsBtn;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox chkBoxLoadSettings;
        private System.Windows.Forms.RichTextBox loadLogBox;
        public System.Windows.Forms.GroupBox boxFEBTag;
        public System.Windows.Forms.TextBox txtRefFEBTemp;
        public System.Windows.Forms.TextBox txtRefCMBTemp;
        public System.Windows.Forms.TextBox txtFEBRate;
        public System.Windows.Forms.TextBox txtCMBRate;
    }
}
