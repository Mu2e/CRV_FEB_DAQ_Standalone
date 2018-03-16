namespace TB_mu2e
{
    partial class frmTelnet
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
            this.txtNet = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // txtNet
            // 
            this.txtNet.AcceptsReturn = true;
            this.txtNet.AccessibleRole = System.Windows.Forms.AccessibleRole.None;
            this.txtNet.Location = new System.Drawing.Point(3, 5);
            this.txtNet.MaxLength = 255;
            this.txtNet.Multiline = true;
            this.txtNet.Name = "txtNet";
            this.txtNet.Size = new System.Drawing.Size(604, 544);
            this.txtNet.TabIndex = 0;
            // 
            // frmTelnet
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(608, 549);
            this.Controls.Add(this.txtNet);
            this.KeyPreview = true;
            this.Name = "frmTelnet";
            this.Text = "telnet";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmTelnet_FormClosing);
            this.Load += new System.EventHandler(this.frmTelnet_Load);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.frmTelnet_KeyUp);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtNet;
    }
}