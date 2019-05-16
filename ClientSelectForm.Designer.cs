namespace TB_mu2e
{
    partial class ClientSelectForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ClientSelectForm));
            this.clientListBox = new System.Windows.Forms.ListBox();
            this.clientMemoryBox = new System.Windows.Forms.ListBox();
            this.clientMemoryAddBtn = new System.Windows.Forms.Button();
            this.clientMemoryRemoveBtn = new System.Windows.Forms.Button();
            this.clientIPAddBox = new System.Windows.Forms.TextBox();
            this.clientAddBtn = new System.Windows.Forms.Button();
            this.clientRemoveBtn = new System.Windows.Forms.Button();
            this.FEBClientListLabel = new System.Windows.Forms.Label();
            this.clientDefaultSave = new System.Windows.Forms.Button();
            this.clientListMoveUpBtn = new System.Windows.Forms.Button();
            this.clientListMoveDownBtn = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // clientListBox
            // 
            this.clientListBox.FormattingEnabled = true;
            this.clientListBox.Location = new System.Drawing.Point(281, 37);
            this.clientListBox.Name = "clientListBox";
            this.clientListBox.Size = new System.Drawing.Size(200, 173);
            this.clientListBox.TabIndex = 5;
            // 
            // clientMemoryBox
            // 
            this.clientMemoryBox.FormattingEnabled = true;
            this.clientMemoryBox.Location = new System.Drawing.Point(12, 37);
            this.clientMemoryBox.Name = "clientMemoryBox";
            this.clientMemoryBox.SelectionMode = System.Windows.Forms.SelectionMode.MultiSimple;
            this.clientMemoryBox.Size = new System.Drawing.Size(200, 173);
            this.clientMemoryBox.Sorted = true;
            this.clientMemoryBox.TabIndex = 2;
            // 
            // clientMemoryAddBtn
            // 
            this.clientMemoryAddBtn.Location = new System.Drawing.Point(148, 11);
            this.clientMemoryAddBtn.Name = "clientMemoryAddBtn";
            this.clientMemoryAddBtn.Size = new System.Drawing.Size(64, 22);
            this.clientMemoryAddBtn.TabIndex = 1;
            this.clientMemoryAddBtn.Text = "Add";
            this.clientMemoryAddBtn.UseVisualStyleBackColor = true;
            this.clientMemoryAddBtn.Click += new System.EventHandler(this.ClientMemoryAddBtn_Click);
            // 
            // clientMemoryRemoveBtn
            // 
            this.clientMemoryRemoveBtn.Image = ((System.Drawing.Image)(resources.GetObject("clientMemoryRemoveBtn.Image")));
            this.clientMemoryRemoveBtn.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.clientMemoryRemoveBtn.Location = new System.Drawing.Point(118, 216);
            this.clientMemoryRemoveBtn.Name = "clientMemoryRemoveBtn";
            this.clientMemoryRemoveBtn.Size = new System.Drawing.Size(94, 22);
            this.clientMemoryRemoveBtn.TabIndex = 4;
            this.clientMemoryRemoveBtn.Text = "Remove";
            this.clientMemoryRemoveBtn.UseVisualStyleBackColor = true;
            this.clientMemoryRemoveBtn.Click += new System.EventHandler(this.ClientMemoryRemoveBtn_Click);
            // 
            // clientIPAddBox
            // 
            this.clientIPAddBox.AcceptsReturn = true;
            this.clientIPAddBox.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.clientIPAddBox.Location = new System.Drawing.Point(13, 12);
            this.clientIPAddBox.MaxLength = 128;
            this.clientIPAddBox.Name = "clientIPAddBox";
            this.clientIPAddBox.Size = new System.Drawing.Size(132, 20);
            this.clientIPAddBox.TabIndex = 0;
            this.clientIPAddBox.Enter += new System.EventHandler(this.ClientIPAddBox_Enter);
            this.clientIPAddBox.Leave += new System.EventHandler(this.ClientIPAddBox_Leave);
            // 
            // clientAddBtn
            // 
            this.clientAddBtn.Location = new System.Drawing.Point(229, 86);
            this.clientAddBtn.Name = "clientAddBtn";
            this.clientAddBtn.Size = new System.Drawing.Size(35, 75);
            this.clientAddBtn.TabIndex = 3;
            this.clientAddBtn.Text = ">>";
            this.clientAddBtn.UseVisualStyleBackColor = true;
            this.clientAddBtn.Click += new System.EventHandler(this.ClientAddBtn_Click);
            // 
            // clientRemoveBtn
            // 
            this.clientRemoveBtn.Image = ((System.Drawing.Image)(resources.GetObject("clientRemoveBtn.Image")));
            this.clientRemoveBtn.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.clientRemoveBtn.Location = new System.Drawing.Point(387, 216);
            this.clientRemoveBtn.Name = "clientRemoveBtn";
            this.clientRemoveBtn.Size = new System.Drawing.Size(94, 22);
            this.clientRemoveBtn.TabIndex = 6;
            this.clientRemoveBtn.Text = "Remove";
            this.clientRemoveBtn.UseVisualStyleBackColor = true;
            this.clientRemoveBtn.Click += new System.EventHandler(this.ClientRemoveBtn_Click);
            // 
            // FEBClientListLabel
            // 
            this.FEBClientListLabel.AutoSize = true;
            this.FEBClientListLabel.Location = new System.Drawing.Point(282, 19);
            this.FEBClientListLabel.Name = "FEBClientListLabel";
            this.FEBClientListLabel.Size = new System.Drawing.Size(61, 13);
            this.FEBClientListLabel.TabIndex = 7;
            this.FEBClientListLabel.Text = "FEB Clients";
            // 
            // clientDefaultSave
            // 
            this.clientDefaultSave.Image = ((System.Drawing.Image)(resources.GetObject("clientDefaultSave.Image")));
            this.clientDefaultSave.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.clientDefaultSave.Location = new System.Drawing.Point(281, 216);
            this.clientDefaultSave.Name = "clientDefaultSave";
            this.clientDefaultSave.Size = new System.Drawing.Size(94, 22);
            this.clientDefaultSave.TabIndex = 8;
            this.clientDefaultSave.Text = "Save Default";
            this.clientDefaultSave.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.clientDefaultSave.UseVisualStyleBackColor = true;
            this.clientDefaultSave.Click += new System.EventHandler(this.ClientDefaultSave_Click);
            // 
            // clientListMoveUpBtn
            // 
            this.clientListMoveUpBtn.Image = ((System.Drawing.Image)(resources.GetObject("clientListMoveUpBtn.Image")));
            this.clientListMoveUpBtn.Location = new System.Drawing.Point(456, 8);
            this.clientListMoveUpBtn.Name = "clientListMoveUpBtn";
            this.clientListMoveUpBtn.Size = new System.Drawing.Size(25, 25);
            this.clientListMoveUpBtn.TabIndex = 9;
            this.clientListMoveUpBtn.UseVisualStyleBackColor = true;
            this.clientListMoveUpBtn.Click += new System.EventHandler(this.ClientListMoveUpBtn_Click);
            // 
            // clientListMoveDownBtn
            // 
            this.clientListMoveDownBtn.Image = ((System.Drawing.Image)(resources.GetObject("clientListMoveDownBtn.Image")));
            this.clientListMoveDownBtn.Location = new System.Drawing.Point(425, 8);
            this.clientListMoveDownBtn.Name = "clientListMoveDownBtn";
            this.clientListMoveDownBtn.Size = new System.Drawing.Size(25, 25);
            this.clientListMoveDownBtn.TabIndex = 10;
            this.clientListMoveDownBtn.UseVisualStyleBackColor = true;
            this.clientListMoveDownBtn.Click += new System.EventHandler(this.ClientListMoveDownBtn_Click);
            // 
            // ClientSelectForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(494, 247);
            this.Controls.Add(this.clientListMoveDownBtn);
            this.Controls.Add(this.clientListMoveUpBtn);
            this.Controls.Add(this.clientDefaultSave);
            this.Controls.Add(this.FEBClientListLabel);
            this.Controls.Add(this.clientRemoveBtn);
            this.Controls.Add(this.clientAddBtn);
            this.Controls.Add(this.clientIPAddBox);
            this.Controls.Add(this.clientMemoryRemoveBtn);
            this.Controls.Add(this.clientMemoryAddBtn);
            this.Controls.Add(this.clientMemoryBox);
            this.Controls.Add(this.clientListBox);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ClientSelectForm";
            this.Text = "Client Selection";
            this.TopMost = true;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox clientListBox;
        private System.Windows.Forms.ListBox clientMemoryBox;
        private System.Windows.Forms.Button clientMemoryAddBtn;
        private System.Windows.Forms.Button clientMemoryRemoveBtn;
        private System.Windows.Forms.TextBox clientIPAddBox;
        private System.Windows.Forms.Button clientAddBtn;
        private System.Windows.Forms.Button clientRemoveBtn;
        private System.Windows.Forms.Label FEBClientListLabel;
        private System.Windows.Forms.Button clientDefaultSave;
        private System.Windows.Forms.Button clientListMoveUpBtn;
        private System.Windows.Forms.Button clientListMoveDownBtn;
    }
}