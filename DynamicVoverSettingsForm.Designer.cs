using System.Collections.Generic;

namespace TB_mu2e
{
    partial class DynamicVoverSettingsForm
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label9 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.txtRefFEBTemp = new System.Windows.Forms.TextBox();
            this.txtRefCMBTemp = new System.Windows.Forms.TextBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.btnApplyToAll = new System.Windows.Forms.Button();
            this.btnUseDefault = new System.Windows.Forms.Button();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.txtFEBRate = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.txtCMBRate = new System.Windows.Forms.TextBox();
            this.useFEBTempBox = new System.Windows.Forms.CheckBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.useCMBTempBox = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.pnlFEBSettings = new System.Windows.Forms.Panel();
            this.btnApplyAllSettings = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.txtRefFEBTemp);
            this.groupBox1.Controls.Add(this.txtRefCMBTemp);
            this.groupBox1.Controls.Add(this.groupBox2);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.txtFEBRate);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.txtCMBRate);
            this.groupBox1.Controls.Add(this.useFEBTempBox);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.useCMBTempBox);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(30, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(753, 93);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Group Action";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.BackColor = System.Drawing.Color.Transparent;
            this.label9.Location = new System.Drawing.Point(486, 65);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(32, 13);
            this.label9.TabIndex = 17;
            this.label9.Text = "degC";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.BackColor = System.Drawing.Color.Transparent;
            this.label8.Location = new System.Drawing.Point(486, 40);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(32, 13);
            this.label8.TabIndex = 16;
            this.label8.Text = "degC";
            // 
            // txtRefFEBTemp
            // 
            this.txtRefFEBTemp.Location = new System.Drawing.Point(403, 61);
            this.txtRefFEBTemp.Name = "txtRefFEBTemp";
            this.txtRefFEBTemp.Size = new System.Drawing.Size(77, 20);
            this.txtRefFEBTemp.TabIndex = 15;
            // 
            // txtRefCMBTemp
            // 
            this.txtRefCMBTemp.Location = new System.Drawing.Point(403, 36);
            this.txtRefCMBTemp.Name = "txtRefCMBTemp";
            this.txtRefCMBTemp.Size = new System.Drawing.Size(77, 20);
            this.txtRefCMBTemp.TabIndex = 14;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.btnApplyToAll);
            this.groupBox2.Controls.Add(this.btnUseDefault);
            this.groupBox2.Location = new System.Drawing.Point(586, 19);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(161, 68);
            this.groupBox2.TabIndex = 13;
            this.groupBox2.TabStop = false;
            // 
            // btnApplyToAll
            // 
            this.btnApplyToAll.Location = new System.Drawing.Point(28, 39);
            this.btnApplyToAll.Name = "btnApplyToAll";
            this.btnApplyToAll.Size = new System.Drawing.Size(113, 23);
            this.btnApplyToAll.TabIndex = 12;
            this.btnApplyToAll.Text = "Apply to All FEBs";
            this.btnApplyToAll.UseVisualStyleBackColor = true;
            this.btnApplyToAll.Click += new System.EventHandler(this.BtnApplyToAll_Click);
            // 
            // btnUseDefault
            // 
            this.btnUseDefault.Location = new System.Drawing.Point(28, 11);
            this.btnUseDefault.Name = "btnUseDefault";
            this.btnUseDefault.Size = new System.Drawing.Size(113, 23);
            this.btnUseDefault.TabIndex = 11;
            this.btnUseDefault.Text = "Use Default";
            this.btnUseDefault.UseVisualStyleBackColor = true;
            this.btnUseDefault.Click += new System.EventHandler(this.BtnUseDefault_Click);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(400, 16);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(120, 13);
            this.label7.TabIndex = 10;
            this.label7.Text = "Reference Temperature";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(284, 65);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(52, 13);
            this.label6.TabIndex = 9;
            this.label6.Text = "mV/degC";
            // 
            // txtFEBRate
            // 
            this.txtFEBRate.Location = new System.Drawing.Point(201, 61);
            this.txtFEBRate.Name = "txtFEBRate";
            this.txtFEBRate.Size = new System.Drawing.Size(77, 20);
            this.txtFEBRate.TabIndex = 8;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(284, 40);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(52, 13);
            this.label5.TabIndex = 7;
            this.label5.Text = "mV/degC";
            // 
            // txtCMBRate
            // 
            this.txtCMBRate.Location = new System.Drawing.Point(201, 36);
            this.txtCMBRate.Name = "txtCMBRate";
            this.txtCMBRate.Size = new System.Drawing.Size(77, 20);
            this.txtCMBRate.TabIndex = 6;
            // 
            // useFEBTempBox
            // 
            this.useFEBTempBox.AutoSize = true;
            this.useFEBTempBox.Location = new System.Drawing.Point(139, 64);
            this.useFEBTempBox.Name = "useFEBTempBox";
            this.useFEBTempBox.Size = new System.Drawing.Size(15, 14);
            this.useFEBTempBox.TabIndex = 5;
            this.useFEBTempBox.UseVisualStyleBackColor = true;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(9, 65);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(86, 13);
            this.label4.TabIndex = 4;
            this.label4.Text = "FEB Vover/temp";
            this.label4.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(233, 16);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(81, 13);
            this.label3.TabIndex = 3;
            this.label3.Text = "Correction Rate";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(108, 16);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(83, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Use Correction?";
            // 
            // useCMBTempBox
            // 
            this.useCMBTempBox.AutoSize = true;
            this.useCMBTempBox.Location = new System.Drawing.Point(139, 39);
            this.useCMBTempBox.Name = "useCMBTempBox";
            this.useCMBTempBox.Size = new System.Drawing.Size(15, 14);
            this.useCMBTempBox.TabIndex = 1;
            this.useCMBTempBox.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 40);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(89, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "CMB Vover/temp";
            this.label1.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // pnlFEBSettings
            // 
            this.pnlFEBSettings.AutoScroll = true;
            this.pnlFEBSettings.Location = new System.Drawing.Point(30, 110);
            this.pnlFEBSettings.Name = "pnlFEBSettings";
            this.pnlFEBSettings.Size = new System.Drawing.Size(1210, 840);
            this.pnlFEBSettings.TabIndex = 1;
            // 
            // btnApplyAllSettings
            // 
            this.btnApplyAllSettings.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnApplyAllSettings.Location = new System.Drawing.Point(843, 42);
            this.btnApplyAllSettings.Name = "btnApplyAllSettings";
            this.btnApplyAllSettings.Size = new System.Drawing.Size(287, 51);
            this.btnApplyAllSettings.TabIndex = 2;
            this.btnApplyAllSettings.Text = "APPLY ALL SETTINGS";
            this.btnApplyAllSettings.UseVisualStyleBackColor = true;
            this.btnApplyAllSettings.Click += new System.EventHandler(this.BtnApplyAllSettings_Click);
            // 
            // DynamicVoverSettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1264, 961);
            this.Controls.Add(this.btnApplyAllSettings);
            this.Controls.Add(this.pnlFEBSettings);
            this.Controls.Add(this.groupBox1);
            this.Name = "DynamicVoverSettingsForm";
            this.Text = "Dynamic Bias Voltage Settings";
            this.Load += new System.EventHandler(this.DynamicVoverSettingsForm_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private void InitializeFEBControls()
        {
            //
            // Generate bias gain user control for each FEB
            //
            FEBBiasGainUsrCtrlList.Clear();
            for (int i = 0; i < PP.Num_FEB_clients; i++)
            {
                FEBBiasGainUsrCtrl febBiasGainUsrCtrl = new FEBBiasGainUsrCtrl();
                febBiasGainUsrCtrl.FEBNumber = i;
                febBiasGainUsrCtrl.Parent = this;
                febBiasGainUsrCtrl.boxFEBTag.Text = $"FEB {febBiasGainUsrCtrl.FEBNumber}";
                febBiasGainUsrCtrl.Location = new System.Drawing.Point(5+(i%2)*600, 280*(i/2));
                FEBBiasGainUsrCtrlList.Add(febBiasGainUsrCtrl);
                FEBSettingsValidList.Add(false);
            }
            foreach (FEBBiasGainUsrCtrl febBiasGainUsrCtrl in FEBBiasGainUsrCtrlList)
            {
                this.pnlFEBSettings.Controls.Add(febBiasGainUsrCtrl);
            }
            btnApplyAllSettings.Enabled = false;

        }

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox useCMBTempBox;
        private System.Windows.Forms.CheckBox useFEBTempBox;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txtFEBRate;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtCMBRate;
        private System.Windows.Forms.Button btnApplyToAll;
        private System.Windows.Forms.Button btnUseDefault;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox txtRefFEBTemp;
        private System.Windows.Forms.TextBox txtRefCMBTemp;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Panel pnlFEBSettings;
        private System.Windows.Forms.Button btnApplyAllSettings;
    }
}