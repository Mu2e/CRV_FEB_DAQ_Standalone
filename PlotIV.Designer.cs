namespace TB_mu2e
{
    partial class PlotIV
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
            this.zg1 = new ZedGraph.ZedGraphControl();
            this.btnAutoScale = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.ud_MaxX = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.ud_MinX = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.ud_MaxY = new System.Windows.Forms.NumericUpDown();
            this.btn_IVscan = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.ud_MinY = new System.Windows.Forms.NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)(this.ud_MaxX)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ud_MinX)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ud_MaxY)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ud_MinY)).BeginInit();
            this.SuspendLayout();
            // 
            // zg1
            // 
            this.zg1.Location = new System.Drawing.Point(-1, 0);
            this.zg1.Margin = new System.Windows.Forms.Padding(4);
            this.zg1.Name = "zg1";
            this.zg1.ScrollGrace = 0D;
            this.zg1.ScrollMaxX = 0D;
            this.zg1.ScrollMaxY = 0D;
            this.zg1.ScrollMaxY2 = 0D;
            this.zg1.ScrollMinX = 0D;
            this.zg1.ScrollMinY = 0D;
            this.zg1.ScrollMinY2 = 0D;
            this.zg1.Size = new System.Drawing.Size(843, 558);
            this.zg1.TabIndex = 15;
            // 
            // btnAutoScale
            // 
            this.btnAutoScale.Location = new System.Drawing.Point(93, 614);
            this.btnAutoScale.Name = "btnAutoScale";
            this.btnAutoScale.Size = new System.Drawing.Size(71, 23);
            this.btnAutoScale.TabIndex = 24;
            this.btnAutoScale.Text = "auto scale";
            this.btnAutoScale.UseVisualStyleBackColor = true;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(198, 601);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(72, 15);
            this.label4.TabIndex = 23;
            this.label4.Text = "Max V (Volt)";
            // 
            // ud_MaxX
            // 
            this.ud_MaxX.Location = new System.Drawing.Point(169, 617);
            this.ud_MaxX.Maximum = new decimal(new int[] {
            4095,
            0,
            0,
            0});
            this.ud_MaxX.Name = "ud_MaxX";
            this.ud_MaxX.Size = new System.Drawing.Size(71, 20);
            this.ud_MaxX.TabIndex = 22;
            this.ud_MaxX.Value = new decimal(new int[] {
            120,
            0,
            0,
            0});
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(14, 602);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(69, 15);
            this.label3.TabIndex = 21;
            this.label3.Text = "Min V (Volt)";
            // 
            // ud_MinX
            // 
            this.ud_MinX.DecimalPlaces = 3;
            this.ud_MinX.Location = new System.Drawing.Point(16, 617);
            this.ud_MinX.Maximum = new decimal(new int[] {
            4095,
            0,
            0,
            0});
            this.ud_MinX.Name = "ud_MinX";
            this.ud_MinX.Size = new System.Drawing.Size(71, 20);
            this.ud_MinX.TabIndex = 20;
            this.ud_MinX.Value = new decimal(new int[] {
            20,
            0,
            0,
            0});
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(102, 572);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(59, 15);
            this.label1.TabIndex = 18;
            this.label1.Text = "Max I(nA)";
            // 
            // ud_MaxY
            // 
            this.ud_MaxY.Location = new System.Drawing.Point(93, 588);
            this.ud_MaxY.Maximum = new decimal(new int[] {
            16500,
            0,
            0,
            0});
            this.ud_MaxY.Name = "ud_MaxY";
            this.ud_MaxY.Size = new System.Drawing.Size(71, 20);
            this.ud_MaxY.TabIndex = 16;
            this.ud_MaxY.Value = new decimal(new int[] {
            400,
            0,
            0,
            0});
            // 
            // btn_IVscan
            // 
            this.btn_IVscan.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.18868F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_IVscan.Location = new System.Drawing.Point(463, 594);
            this.btn_IVscan.Name = "btn_IVscan";
            this.btn_IVscan.Size = new System.Drawing.Size(149, 60);
            this.btn_IVscan.TabIndex = 25;
            this.btn_IVscan.Text = "IV SCAN";
            this.btn_IVscan.UseVisualStyleBackColor = true;
            this.btn_IVscan.Click += new System.EventHandler(this.btn_IVscan_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(105, 666);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(59, 15);
            this.label2.TabIndex = 19;
            this.label2.Text = "Min I (nA)";
            // 
            // ud_MinY
            // 
            this.ud_MinY.Location = new System.Drawing.Point(93, 643);
            this.ud_MinY.Maximum = new decimal(new int[] {
            16000,
            0,
            0,
            0});
            this.ud_MinY.Name = "ud_MinY";
            this.ud_MinY.Size = new System.Drawing.Size(71, 20);
            this.ud_MinY.TabIndex = 17;
            // 
            // PlotIV
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(841, 705);
            this.Controls.Add(this.btn_IVscan);
            this.Controls.Add(this.zg1);
            this.Controls.Add(this.btnAutoScale);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.ud_MaxX);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.ud_MinX);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.ud_MinY);
            this.Controls.Add(this.ud_MaxY);
            this.Name = "PlotIV";
            this.Text = "PlotIV";
            ((System.ComponentModel.ISupportInitialize)(this.ud_MaxX)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ud_MinX)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ud_MaxY)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ud_MinY)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private ZedGraph.ZedGraphControl zg1;
        private System.Windows.Forms.Button btnAutoScale;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.NumericUpDown ud_MaxX;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown ud_MinX;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown ud_MaxY;
        private System.Windows.Forms.Button btn_IVscan;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown ud_MinY;
    }
}