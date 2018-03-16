namespace TB_mu2e
{
    partial class frmGUI
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
            this.components = new System.ComponentModel.Container();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.udStart = new System.Windows.Forms.NumericUpDown();
            this.ud_Stop = new System.Windows.Forms.NumericUpDown();
            this.txtInFile = new System.Windows.Forms.TextBox();
            this.btn_FileInChoose = new System.Windows.Forms.Button();
            this.btn_FileOutChoose = new System.Windows.Forms.Button();
            this.txtOutFile = new System.Windows.Forms.TextBox();
            this.btn_READ = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.lblNumEvts = new System.Windows.Forms.Label();
            this.btnPLOT = new System.Windows.Forms.Button();
            this.btn_Listen = new System.Windows.Forms.Button();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.button1 = new System.Windows.Forms.Button();
            this.btnStartSpill = new System.Windows.Forms.Button();
            this.btnStopSpill = new System.Windows.Forms.Button();
            this.lblDaqMessage = new System.Windows.Forms.Label();
            this.lblDaqMessage2 = new System.Windows.Forms.Label();
            this.grpWire = new System.Windows.Forms.GroupBox();
            this.lblWC_status = new System.Windows.Forms.Label();
            this.lblLastTime = new System.Windows.Forms.Label();
            this.button4 = new System.Windows.Forms.Button();
            this.lblTrigEn = new System.Windows.Forms.Label();
            this.lblSpillStatus = new System.Windows.Forms.Label();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.btnAutomatic = new System.Windows.Forms.Button();
            this.btnSetMaxTrig = new System.Windows.Forms.Button();
            this.btnUpdateStatus = new System.Windows.Forms.Button();
            this.lblStat1 = new System.Windows.Forms.Label();
            this.lblStat2 = new System.Windows.Forms.Label();
            this.lblStat3 = new System.Windows.Forms.Label();
            this.lblStat4 = new System.Windows.Forms.Label();
            this.btnStopThis = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.udStart)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ud_Stop)).BeginInit();
            this.grpWire.SuspendLayout();
            this.SuspendLayout();
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            this.openFileDialog1.ShowReadOnly = true;
            // 
            // udStart
            // 
            this.udStart.Location = new System.Drawing.Point(107, 87);
            this.udStart.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.udStart.Name = "udStart";
            this.udStart.Size = new System.Drawing.Size(80, 20);
            this.udStart.TabIndex = 0;
            this.udStart.Visible = false;
            this.udStart.ValueChanged += new System.EventHandler(this.udStart_ValueChanged);
            // 
            // ud_Stop
            // 
            this.ud_Stop.Location = new System.Drawing.Point(107, 113);
            this.ud_Stop.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.ud_Stop.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.ud_Stop.Name = "ud_Stop";
            this.ud_Stop.Size = new System.Drawing.Size(80, 20);
            this.ud_Stop.TabIndex = 1;
            this.ud_Stop.Value = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.ud_Stop.Visible = false;
            this.ud_Stop.ValueChanged += new System.EventHandler(this.ud_Stop_ValueChanged);
            // 
            // txtInFile
            // 
            this.txtInFile.Location = new System.Drawing.Point(12, 16);
            this.txtInFile.Name = "txtInFile";
            this.txtInFile.Size = new System.Drawing.Size(392, 20);
            this.txtInFile.TabIndex = 2;
            // 
            // btn_FileInChoose
            // 
            this.btn_FileInChoose.Location = new System.Drawing.Point(410, 15);
            this.btn_FileInChoose.Name = "btn_FileInChoose";
            this.btn_FileInChoose.Size = new System.Drawing.Size(79, 20);
            this.btn_FileInChoose.TabIndex = 3;
            this.btn_FileInChoose.Text = "Input";
            this.btn_FileInChoose.UseVisualStyleBackColor = true;
            this.btn_FileInChoose.Click += new System.EventHandler(this.btn_FileInChoose_Click);
            // 
            // btn_FileOutChoose
            // 
            this.btn_FileOutChoose.Location = new System.Drawing.Point(410, 41);
            this.btn_FileOutChoose.Name = "btn_FileOutChoose";
            this.btn_FileOutChoose.Size = new System.Drawing.Size(79, 20);
            this.btn_FileOutChoose.TabIndex = 5;
            this.btn_FileOutChoose.Text = "Output";
            this.btn_FileOutChoose.UseVisualStyleBackColor = true;
            this.btn_FileOutChoose.Click += new System.EventHandler(this.btn_FileOutChoose_Click);
            // 
            // txtOutFile
            // 
            this.txtOutFile.Location = new System.Drawing.Point(12, 42);
            this.txtOutFile.Name = "txtOutFile";
            this.txtOutFile.Size = new System.Drawing.Size(392, 20);
            this.txtOutFile.TabIndex = 4;
            // 
            // btn_READ
            // 
            this.btn_READ.Location = new System.Drawing.Point(409, 77);
            this.btn_READ.Name = "btn_READ";
            this.btn_READ.Size = new System.Drawing.Size(79, 24);
            this.btn_READ.TabIndex = 6;
            this.btn_READ.Text = "READ";
            this.btn_READ.UseVisualStyleBackColor = true;
            this.btn_READ.Click += new System.EventHandler(this.btn_READ_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(20, 89);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(81, 13);
            this.label1.TabIndex = 7;
            this.label1.Text = "Read from evt#";
            this.label1.Visible = false;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(31, 115);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(51, 13);
            this.label2.TabIndex = 8;
            this.label2.Text = "Max trig#";
            this.label2.Visible = false;
            // 
            // lblNumEvts
            // 
            this.lblNumEvts.AutoSize = true;
            this.lblNumEvts.Location = new System.Drawing.Point(211, 87);
            this.lblNumEvts.Name = "lblNumEvts";
            this.lblNumEvts.Size = new System.Drawing.Size(13, 13);
            this.lblNumEvts.TabIndex = 9;
            this.lblNumEvts.Text = "0";
            // 
            // btnPLOT
            // 
            this.btnPLOT.Location = new System.Drawing.Point(410, 110);
            this.btnPLOT.Name = "btnPLOT";
            this.btnPLOT.Size = new System.Drawing.Size(76, 24);
            this.btnPLOT.TabIndex = 10;
            this.btnPLOT.Text = "PLOT";
            this.btnPLOT.UseVisualStyleBackColor = true;
            this.btnPLOT.Visible = false;
            this.btnPLOT.Click += new System.EventHandler(this.btnPLOT_Click);
            // 
            // btn_Listen
            // 
            this.btn_Listen.Location = new System.Drawing.Point(324, 76);
            this.btn_Listen.Name = "btn_Listen";
            this.btn_Listen.Size = new System.Drawing.Size(79, 24);
            this.btn_Listen.TabIndex = 11;
            this.btn_Listen.Text = "LISTEN";
            this.btn_Listen.UseVisualStyleBackColor = true;
            this.btn_Listen.Click += new System.EventHandler(this.btn_Listen_Click);
            // 
            // backgroundWorker1
            // 
            this.backgroundWorker1.WorkerReportsProgress = true;
            this.backgroundWorker1.WorkerSupportsCancellation = true;
            this.backgroundWorker1.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker1_DoWork);
            this.backgroundWorker1.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.backgroundWorker1_ProgressChanged);
            this.backgroundWorker1.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorker1_RunWorkerCompleted);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(324, 109);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(79, 24);
            this.button1.TabIndex = 12;
            this.button1.Text = "fake spill";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // btnStartSpill
            // 
            this.btnStartSpill.Location = new System.Drawing.Point(324, 139);
            this.btnStartSpill.Name = "btnStartSpill";
            this.btnStartSpill.Size = new System.Drawing.Size(79, 24);
            this.btnStartSpill.TabIndex = 13;
            this.btnStartSpill.Text = "start spill";
            this.btnStartSpill.UseVisualStyleBackColor = true;
            this.btnStartSpill.Visible = false;
            this.btnStartSpill.Click += new System.EventHandler(this.btnStartSpill_Click);
            // 
            // btnStopSpill
            // 
            this.btnStopSpill.Location = new System.Drawing.Point(407, 140);
            this.btnStopSpill.Name = "btnStopSpill";
            this.btnStopSpill.Size = new System.Drawing.Size(79, 24);
            this.btnStopSpill.TabIndex = 14;
            this.btnStopSpill.Text = "end spill";
            this.btnStopSpill.UseVisualStyleBackColor = true;
            this.btnStopSpill.Visible = false;
            this.btnStopSpill.Click += new System.EventHandler(this.btnStopSpill_Click);
            // 
            // lblDaqMessage
            // 
            this.lblDaqMessage.AutoSize = true;
            this.lblDaqMessage.Location = new System.Drawing.Point(20, 146);
            this.lblDaqMessage.Name = "lblDaqMessage";
            this.lblDaqMessage.Size = new System.Drawing.Size(64, 13);
            this.lblDaqMessage.TabIndex = 15;
            this.lblDaqMessage.Text = " daq mess 1";
            // 
            // lblDaqMessage2
            // 
            this.lblDaqMessage2.AutoSize = true;
            this.lblDaqMessage2.Location = new System.Drawing.Point(20, 174);
            this.lblDaqMessage2.Name = "lblDaqMessage2";
            this.lblDaqMessage2.Size = new System.Drawing.Size(64, 13);
            this.lblDaqMessage2.TabIndex = 16;
            this.lblDaqMessage2.Text = " daq mess 2";
            // 
            // grpWire
            // 
            this.grpWire.Controls.Add(this.lblWC_status);
            this.grpWire.Controls.Add(this.lblLastTime);
            this.grpWire.Controls.Add(this.button4);
            this.grpWire.Controls.Add(this.lblTrigEn);
            this.grpWire.Controls.Add(this.lblSpillStatus);
            this.grpWire.Location = new System.Drawing.Point(12, 202);
            this.grpWire.Name = "grpWire";
            this.grpWire.Size = new System.Drawing.Size(474, 100);
            this.grpWire.TabIndex = 17;
            this.grpWire.TabStop = false;
            this.grpWire.Text = "WireChambers";
            // 
            // lblWC_status
            // 
            this.lblWC_status.AutoSize = true;
            this.lblWC_status.Location = new System.Drawing.Point(230, 16);
            this.lblWC_status.Name = "lblWC_status";
            this.lblWC_status.Size = new System.Drawing.Size(56, 13);
            this.lblWC_status.TabIndex = 20;
            this.lblWC_status.Text = "WC status";
            // 
            // lblLastTime
            // 
            this.lblLastTime.AutoSize = true;
            this.lblLastTime.Location = new System.Drawing.Point(7, 84);
            this.lblLastTime.Name = "lblLastTime";
            this.lblLastTime.Size = new System.Drawing.Size(59, 13);
            this.lblLastTime.TabIndex = 19;
            this.lblLastTime.Text = "last update";
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(389, 27);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(79, 44);
            this.button4.TabIndex = 18;
            this.button4.Text = "Activate Connection";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Visible = false;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // lblTrigEn
            // 
            this.lblTrigEn.AutoSize = true;
            this.lblTrigEn.Location = new System.Drawing.Point(8, 58);
            this.lblTrigEn.Name = "lblTrigEn";
            this.lblTrigEn.Size = new System.Drawing.Size(56, 13);
            this.lblTrigEn.TabIndex = 17;
            this.lblTrigEn.Text = "trig enable";
            // 
            // lblSpillStatus
            // 
            this.lblSpillStatus.AutoSize = true;
            this.lblSpillStatus.Location = new System.Drawing.Point(8, 27);
            this.lblSpillStatus.Name = "lblSpillStatus";
            this.lblSpillStatus.Size = new System.Drawing.Size(55, 13);
            this.lblSpillStatus.TabIndex = 16;
            this.lblSpillStatus.Text = "spill status";
            // 
            // timer1
            // 
            this.timer1.Interval = 500;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // btnAutomatic
            // 
            this.btnAutomatic.Location = new System.Drawing.Point(324, 169);
            this.btnAutomatic.Name = "btnAutomatic";
            this.btnAutomatic.Size = new System.Drawing.Size(162, 34);
            this.btnAutomatic.TabIndex = 19;
            this.btnAutomatic.Text = "AUTOMATIC";
            this.btnAutomatic.UseVisualStyleBackColor = true;
            this.btnAutomatic.Visible = false;
            this.btnAutomatic.Click += new System.EventHandler(this.btnAutomatic_Click);
            // 
            // btnSetMaxTrig
            // 
            this.btnSetMaxTrig.Location = new System.Drawing.Point(193, 113);
            this.btnSetMaxTrig.Name = "btnSetMaxTrig";
            this.btnSetMaxTrig.Size = new System.Drawing.Size(54, 20);
            this.btnSetMaxTrig.TabIndex = 20;
            this.btnSetMaxTrig.Text = "SET";
            this.btnSetMaxTrig.UseVisualStyleBackColor = true;
            this.btnSetMaxTrig.Visible = false;
            this.btnSetMaxTrig.Click += new System.EventHandler(this.btnSetMaxTrig_Click);
            // 
            // btnUpdateStatus
            // 
            this.btnUpdateStatus.Location = new System.Drawing.Point(407, 401);
            this.btnUpdateStatus.Name = "btnUpdateStatus";
            this.btnUpdateStatus.Size = new System.Drawing.Size(79, 24);
            this.btnUpdateStatus.TabIndex = 22;
            this.btnUpdateStatus.Text = "update";
            this.btnUpdateStatus.UseVisualStyleBackColor = true;
            this.btnUpdateStatus.Visible = false;
            this.btnUpdateStatus.Click += new System.EventHandler(this.btnUpdateStatus_Click);
            // 
            // lblStat1
            // 
            this.lblStat1.AutoSize = true;
            this.lblStat1.Location = new System.Drawing.Point(19, 309);
            this.lblStat1.Name = "lblStat1";
            this.lblStat1.Size = new System.Drawing.Size(55, 13);
            this.lblStat1.TabIndex = 23;
            this.lblStat1.Text = "spill status";
            // 
            // lblStat2
            // 
            this.lblStat2.AutoSize = true;
            this.lblStat2.Location = new System.Drawing.Point(19, 330);
            this.lblStat2.Name = "lblStat2";
            this.lblStat2.Size = new System.Drawing.Size(55, 13);
            this.lblStat2.TabIndex = 24;
            this.lblStat2.Text = "spill status";
            // 
            // lblStat3
            // 
            this.lblStat3.AutoSize = true;
            this.lblStat3.Location = new System.Drawing.Point(19, 351);
            this.lblStat3.Name = "lblStat3";
            this.lblStat3.Size = new System.Drawing.Size(55, 13);
            this.lblStat3.TabIndex = 25;
            this.lblStat3.Text = "spill status";
            // 
            // lblStat4
            // 
            this.lblStat4.AutoSize = true;
            this.lblStat4.Location = new System.Drawing.Point(19, 372);
            this.lblStat4.Name = "lblStat4";
            this.lblStat4.Size = new System.Drawing.Size(55, 13);
            this.lblStat4.TabIndex = 26;
            this.lblStat4.Text = "spill status";
            // 
            // btnStopThis
            // 
            this.btnStopThis.Location = new System.Drawing.Point(324, 83);
            this.btnStopThis.Name = "btnStopThis";
            this.btnStopThis.Size = new System.Drawing.Size(79, 24);
            this.btnStopThis.TabIndex = 27;
            this.btnStopThis.Text = "stop this run";
            this.btnStopThis.UseVisualStyleBackColor = true;
            this.btnStopThis.Visible = false;
            this.btnStopThis.Click += new System.EventHandler(this.btnStopThis_Click);
            // 
            // frmGUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(501, 432);
            this.Controls.Add(this.btnStopThis);
            this.Controls.Add(this.lblStat4);
            this.Controls.Add(this.lblStat3);
            this.Controls.Add(this.lblStat2);
            this.Controls.Add(this.lblStat1);
            this.Controls.Add(this.btnUpdateStatus);
            this.Controls.Add(this.btnSetMaxTrig);
            this.Controls.Add(this.btnAutomatic);
            this.Controls.Add(this.grpWire);
            this.Controls.Add(this.lblDaqMessage2);
            this.Controls.Add(this.lblDaqMessage);
            this.Controls.Add(this.btnStopSpill);
            this.Controls.Add(this.btnStartSpill);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.btn_Listen);
            this.Controls.Add(this.btnPLOT);
            this.Controls.Add(this.lblNumEvts);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btn_READ);
            this.Controls.Add(this.btn_FileOutChoose);
            this.Controls.Add(this.txtOutFile);
            this.Controls.Add(this.btn_FileInChoose);
            this.Controls.Add(this.txtInFile);
            this.Controls.Add(this.ud_Stop);
            this.Controls.Add(this.udStart);
            this.Name = "frmGUI";
            this.Text = "PulseProc GUI";
            ((System.ComponentModel.ISupportInitialize)(this.udStart)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ud_Stop)).EndInit();
            this.grpWire.ResumeLayout(false);
            this.grpWire.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }


        #endregion

        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        public System.Windows.Forms.NumericUpDown udStart;
        public System.Windows.Forms.NumericUpDown ud_Stop;
        public System.Windows.Forms.TextBox txtInFile;
        public System.Windows.Forms.Button btn_FileInChoose;
        public System.Windows.Forms.Button btn_FileOutChoose;
        public System.Windows.Forms.TextBox txtOutFile;
        public System.Windows.Forms.Button btn_READ;
        public System.Windows.Forms.Label lblNumEvts;
        public System.Windows.Forms.Button btnPLOT;
        public System.Windows.Forms.Button btn_Listen;
        public System.ComponentModel.BackgroundWorker backgroundWorker1;
        public System.Windows.Forms.Button button1;
        public System.Windows.Forms.Button btnStartSpill;
        public System.Windows.Forms.Button btnStopSpill;
        private System.Windows.Forms.Label lblDaqMessage;
        private System.Windows.Forms.Label lblDaqMessage2;
        private System.Windows.Forms.GroupBox grpWire;
        private System.Windows.Forms.Label lblLastTime;
        public System.Windows.Forms.Button button4;
        private System.Windows.Forms.Label lblTrigEn;
        private System.Windows.Forms.Label lblSpillStatus;
        private System.Windows.Forms.Timer timer1;
        public System.Windows.Forms.Button btnAutomatic;
        private System.Windows.Forms.Label lblWC_status;
        public System.Windows.Forms.Button btnSetMaxTrig;
        public System.Windows.Forms.Button btnUpdateStatus;
        private System.Windows.Forms.Label lblStat1;
        private System.Windows.Forms.Label lblStat2;
        private System.Windows.Forms.Label lblStat3;
        private System.Windows.Forms.Label lblStat4;
        public System.Windows.Forms.Button btnStopThis;
    }
}

