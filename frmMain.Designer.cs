namespace TB_mu2e
{
    partial class frmMain
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMain));
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tabRUN = new System.Windows.Forms.TabPage();
            this.runLog = new System.Windows.Forms.RichTextBox();
            this.label14 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.lblMaxADC3 = new System.Windows.Forms.Label();
            this.lblMaxADC2 = new System.Windows.Forms.Label();
            this.lblMaxADC1 = new System.Windows.Forms.Label();
            this.lblMaxADC0 = new System.Windows.Forms.Label();
            this.groupBoxEvDisplay = new System.Windows.Forms.GroupBox();
            this.chkLast = new System.Windows.Forms.CheckBox();
            this.chkPersist = new System.Windows.Forms.CheckBox();
            this.btnDisplaySpill = new System.Windows.Forms.Button();
            this.txtEvent = new System.Windows.Forms.TextBox();
            this.btnNextDisp = new System.Windows.Forms.Button();
            this.btnPrevDisp = new System.Windows.Forms.Button();
            this.ud_VertMin = new System.Windows.Forms.NumericUpDown();
            this.ud_VertMax = new System.Windows.Forms.NumericUpDown();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.SpillStatusGroupBox = new System.Windows.Forms.GroupBox();
            this.lblSpillsNum = new System.Windows.Forms.Label();
            this.lblNumSpills = new System.Windows.Forms.Label();
            this.SpillStatusTable = new System.Windows.Forms.TableLayoutPanel();
            this.lblFebColumn = new System.Windows.Forms.Label();
            this.lblSpillStatus = new System.Windows.Forms.Label();
            this.lblLastSpillTrig = new System.Windows.Forms.Label();
            this.lblTotalNumTrig = new System.Windows.Forms.Label();
            this.lblTimeInSpill = new System.Windows.Forms.Label();
            this.validateParseChkBox = new System.Windows.Forms.CheckBox();
            this.saveAsciiBox = new System.Windows.Forms.CheckBox();
            this.lblRunName = new System.Windows.Forms.Label();
            this.lblTimeInRun = new System.Windows.Forms.Label();
            this.lblTxtRunName = new System.Windows.Forms.Label();
            this.lblRunTime = new System.Windows.Forms.Label();
            this.lblSpillTime = new System.Windows.Forms.Label();
            this.btnFebClientsChange = new System.Windows.Forms.Button();
            this.btnStopRun = new System.Windows.Forms.Button();
            this.btnStartRun = new System.Windows.Forms.Button();
            this.btnPrepare = new System.Windows.Forms.Button();
            this.btnConnectAll = new System.Windows.Forms.Button();
            this.zg1 = new ZedGraph.ZedGraphControl();
            this.tabConsole = new System.Windows.Forms.TabPage();
            this.loadCmdsBtn = new System.Windows.Forms.Button();
            this.btnDebugLogging = new System.Windows.Forms.Button();
            this.ConsoleBox = new System.Windows.Forms.TextBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.console_Disp = new System.Windows.Forms.RichTextBox();
            this.tabFEB1 = new System.Windows.Forms.TabPage();
            this.BDVoltsLabel15 = new System.Windows.Forms.Label();
            this.BDVoltsLabel14 = new System.Windows.Forms.Label();
            this.BDVoltsLabel13 = new System.Windows.Forms.Label();
            this.BDVoltsLabel12 = new System.Windows.Forms.Label();
            this.BDVoltsLabel7 = new System.Windows.Forms.Label();
            this.BDVoltsLabel6 = new System.Windows.Forms.Label();
            this.BDVoltsLabel5 = new System.Windows.Forms.Label();
            this.BDVoltsLabel8 = new System.Windows.Forms.Label();
            this.BDVoltsLabel11 = new System.Windows.Forms.Label();
            this.BDVoltsLabel9 = new System.Windows.Forms.Label();
            this.BDVoltsLabel10 = new System.Windows.Forms.Label();
            this.BDVoltsLabel4 = new System.Windows.Forms.Label();
            this.BDVoltsLabel3 = new System.Windows.Forms.Label();
            this.BDVoltsLabel2 = new System.Windows.Forms.Label();
            this.BDVoltsLabel1 = new System.Windows.Forms.Label();
            this.BDVoltsLabel0 = new System.Windows.Forms.Label();
            this.scanAllChanBox = new System.Windows.Forms.CheckBox();
            this.label23 = new System.Windows.Forms.Label();
            this.ShowSpect = new System.Windows.Forms.Button();
            this.ShowIV = new System.Windows.Forms.Button();
            this.zedFEB1 = new ZedGraph.ZedGraphControl();
            this.groupBoxREG1 = new System.Windows.Forms.GroupBox();
            this.lblFPGA = new System.Windows.Forms.Label();
            this.btnCHANGE = new System.Windows.Forms.Button();
            this.udFPGA = new System.Windows.Forms.NumericUpDown();
            this.btnRegWRITE = new System.Windows.Forms.Button();
            this.btnRegREAD = new System.Windows.Forms.Button();
            this.panel2 = new System.Windows.Forms.Panel();
            this.btnErase = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.udChan = new System.Windows.Forms.NumericUpDown();
            this.lblInc = new System.Windows.Forms.Label();
            this.udInterval = new System.Windows.Forms.NumericUpDown();
            this.btnSaveHistos = new System.Windows.Forms.Button();
            this.lblStop = new System.Windows.Forms.Label();
            this.btnScan = new System.Windows.Forms.Button();
            this.udStop = new System.Windows.Forms.NumericUpDown();
            this.lblStart = new System.Windows.Forms.Label();
            this.udStart = new System.Windows.Forms.NumericUpDown();
            this.panel1 = new System.Windows.Forms.Panel();
            this.chkIntegral = new System.Windows.Forms.CheckBox();
            this.chkLogY = new System.Windows.Forms.CheckBox();
            this.groupBox7 = new System.Windows.Forms.GroupBox();
            this.btnBiasWRITEALL = new System.Windows.Forms.Button();
            this.txtCMB_Temp4 = new System.Windows.Forms.TextBox();
            this.txtCMB_Temp3 = new System.Windows.Forms.TextBox();
            this.txtCMB_Temp2 = new System.Windows.Forms.TextBox();
            this.txtCMB_Temp1 = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.txtI = new System.Windows.Forms.TextBox();
            this.txtV = new System.Windows.Forms.TextBox();
            this.btnBiasREAD = new System.Windows.Forms.Button();
            this.btnBiasWRITE = new System.Windows.Forms.Button();
            this.lblI = new System.Windows.Forms.Label();
            this.lblV = new System.Windows.Forms.Label();
            this.tabWC = new System.Windows.Forms.TabPage();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.lblWCmessage = new System.Windows.Forms.Label();
            this.btnWC = new System.Windows.Forms.Button();
            this.tabQC = new System.Windows.Forms.TabPage();
            this.lightCheckGroup = new System.Windows.Forms.GroupBox();
            this.LightCheckModuleHalf = new System.Windows.Forms.ComboBox();
            this.LightCheckTypeLbl = new System.Windows.Forms.Label();
            this.LightCheckType = new System.Windows.Forms.ComboBox();
            this.lightNumChecks = new System.Windows.Forms.NumericUpDown();
            this.lightModuleSideLabel = new System.Windows.Forms.Label();
            this.LightCheckModuleHalfLbl = new System.Windows.Forms.Label();
            this.autoThreshBtn = new System.Windows.Forms.Button();
            this.lightModuleSide = new System.Windows.Forms.ComboBox();
            this.lightModuleLabelLabel = new System.Windows.Forms.Label();
            this.lightModuleLabel = new System.Windows.Forms.TextBox();
            this.lightWriteToFileBox = new System.Windows.Forms.CheckBox();
            this.lightCheckFPGApanel = new System.Windows.Forms.TableLayoutPanel();
            this.label16 = new System.Windows.Forms.Label();
            this.lightCheckResetThresh = new System.Windows.Forms.Button();
            this.lightCheckProgress = new System.Windows.Forms.ProgressBar();
            this.lightCheckChanThreshBtn = new System.Windows.Forms.Button();
            this.lightCheckChanThreshLbl = new System.Windows.Forms.Label();
            this.lightCheckChanThresh = new System.Windows.Forms.TextBox();
            this.lightCheckChanSelecLbl = new System.Windows.Forms.Label();
            this.lightCheckChanSelec = new System.Windows.Forms.NumericUpDown();
            this.globalThreshChkBox = new System.Windows.Forms.CheckBox();
            this.lightGlobalThreshLbl = new System.Windows.Forms.Label();
            this.lightGlobalThresh = new System.Windows.Forms.TextBox();
            this.lightCheckBtn = new System.Windows.Forms.Button();
            this.dicounterQCGroup = new System.Windows.Forms.GroupBox();
            this.qcOutputFileName = new System.Windows.Forms.TextBox();
            this.qaOutputFileNameLabel = new System.Windows.Forms.Label();
            this.oneReadout = new System.Windows.Forms.CheckBox();
            this.qaDiNumAvgLabel = new System.Windows.Forms.Label();
            this.qaDiIWarningThreshLabel = new System.Windows.Forms.Label();
            this.qaDiNumAvg = new System.Windows.Forms.NumericUpDown();
            this.qcDiIWarningThresh = new System.Windows.Forms.TextBox();
            this.autoDataProgress = new System.Windows.Forms.ProgressBar();
            this.qcStartButton = new System.Windows.Forms.Button();
            this.numLabel = new System.Windows.Forms.Label();
            this.dicounterNumberTextBox = new System.Windows.Forms.TextBox();
            this.qaBiasLbl = new System.Windows.Forms.Label();
            this.qcBias = new System.Windows.Forms.TextBox();
            this.tabCMBTester = new System.Windows.Forms.TabPage();
            this.cmbDataGroup = new System.Windows.Forms.GroupBox();
            this.cmbDataTable = new System.Windows.Forms.TableLayoutPanel();
            this.cmbTestControl = new System.Windows.Forms.GroupBox();
            this.cmbTesterProgresBar = new System.Windows.Forms.ProgressBar();
            this.lostCMBavgsBtn = new System.Windows.Forms.Button();
            this.updateFilesChkBox = new System.Windows.Forms.CheckBox();
            this.cmbInfoBox = new System.Windows.Forms.TextBox();
            this.requestNumTrigsLabel = new System.Windows.Forms.Label();
            this.requestNumTrigs = new System.Windows.Forms.TextBox();
            this.numTrigsDisp = new System.Windows.Forms.Label();
            this.numTrigLabel = new System.Windows.Forms.Label();
            this.sipmControl = new System.Windows.Forms.GroupBox();
            this.cmbBias = new System.Windows.Forms.TextBox();
            this.cmbBiasOverride = new System.Windows.Forms.CheckBox();
            this.cmbTestBtn = new System.Windows.Forms.Button();
            this.tabModuleQC = new System.Windows.Forms.TabPage();
            this.ModuleQCModuleNameBox = new System.Windows.Forms.TextBox();
            this.ModuleQAModuleNameLbl = new System.Windows.Forms.Label();
            this.ModuleQA_OffsetLbl = new System.Windows.Forms.Label();
            this.ModuleQC_Offset = new System.Windows.Forms.NumericUpDown();
            this.ModuleQC_flipped_Chkbox = new System.Windows.Forms.CheckBox();
            this.ModuleQCHomeResetBtn = new System.Windows.Forms.Button();
            this.ModuleQCDarkCurrentBtn = new System.Windows.Forms.Button();
            this.ModuleQAHaltBtn = new System.Windows.Forms.Button();
            this.ComPortStatusLbl = new System.Windows.Forms.Label();
            this.ComPortStatusBox = new System.Windows.Forms.TextBox();
            this.ComPortRefresh = new System.Windows.Forms.Button();
            this.ComPortDisconnectBtn = new System.Windows.Forms.Button();
            this.comPortConnectBtn = new System.Windows.Forms.Button();
            this.ComPortLbl = new System.Windows.Forms.Label();
            this.comPortBox = new System.Windows.Forms.ComboBox();
            this.ModuleQCFilenameBox = new System.Windows.Forms.TextBox();
            this.ModuleQAFileLbl = new System.Windows.Forms.Label();
            this.ModuleQASideLbl = new System.Windows.Forms.Label();
            this.ModuleQCSide = new System.Windows.Forms.ComboBox();
            this.ModuleQAFEB2Box = new System.Windows.Forms.GroupBox();
            this.ModuleQCTableFEB2 = new System.Windows.Forms.TableLayoutPanel();
            this.ModuleQAFEB1Box = new System.Windows.Forms.GroupBox();
            this.ModuleQCTableFEB1 = new System.Windows.Forms.TableLayoutPanel();
            this.ModuleQCBtn = new System.Windows.Forms.Button();
            this.FEBSelectPanel = new System.Windows.Forms.TableLayoutPanel();
            this.SpillTimer = new System.Windows.Forms.Timer(this.components);
            this.moduleQCHomingTimer = new System.Windows.Forms.Timer(this.components);
            this.moduleQCMeasurementTimer = new System.Windows.Forms.Timer(this.components);
            this.ModuleQCStepTimer = new System.Windows.Forms.Timer(this.components);
            this.LightCheckMeasurementTimer = new System.Windows.Forms.Timer(this.components);
            this.qcDiCounterMeasurementTimer = new System.Windows.Forms.Timer(this.components);
            this.FEBClientFooterBar = new System.Windows.Forms.Label();
            this.cmbTest_ShortHelperBtn = new System.Windows.Forms.Button();
            this.tabControl.SuspendLayout();
            this.tabRUN.SuspendLayout();
            this.groupBoxEvDisplay.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ud_VertMin)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ud_VertMax)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.SpillStatusGroupBox.SuspendLayout();
            this.SpillStatusTable.SuspendLayout();
            this.tabConsole.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.tabFEB1.SuspendLayout();
            this.groupBoxREG1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.udFPGA)).BeginInit();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.udChan)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.udInterval)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.udStop)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.udStart)).BeginInit();
            this.panel1.SuspendLayout();
            this.groupBox7.SuspendLayout();
            this.tabWC.SuspendLayout();
            this.groupBox6.SuspendLayout();
            this.tabQC.SuspendLayout();
            this.lightCheckGroup.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.lightNumChecks)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lightCheckChanSelec)).BeginInit();
            this.dicounterQCGroup.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.qaDiNumAvg)).BeginInit();
            this.tabCMBTester.SuspendLayout();
            this.cmbDataGroup.SuspendLayout();
            this.cmbTestControl.SuspendLayout();
            this.sipmControl.SuspendLayout();
            this.tabModuleQC.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ModuleQC_Offset)).BeginInit();
            this.ModuleQAFEB2Box.SuspendLayout();
            this.ModuleQAFEB1Box.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl
            // 
            this.tabControl.Appearance = System.Windows.Forms.TabAppearance.Buttons;
            this.tabControl.Controls.Add(this.tabRUN);
            this.tabControl.Controls.Add(this.tabConsole);
            this.tabControl.Controls.Add(this.tabFEB1);
            this.tabControl.Controls.Add(this.tabWC);
            this.tabControl.Controls.Add(this.tabQC);
            this.tabControl.Controls.Add(this.tabCMBTester);
            this.tabControl.Controls.Add(this.tabModuleQC);
            this.tabControl.Location = new System.Drawing.Point(1, 0);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(1263, 686);
            this.tabControl.SizeMode = System.Windows.Forms.TabSizeMode.FillToRight;
            this.tabControl.TabIndex = 1;
            // 
            // tabRUN
            // 
            this.tabRUN.Controls.Add(this.runLog);
            this.tabRUN.Controls.Add(this.label14);
            this.tabRUN.Controls.Add(this.label13);
            this.tabRUN.Controls.Add(this.label12);
            this.tabRUN.Controls.Add(this.label10);
            this.tabRUN.Controls.Add(this.lblMaxADC3);
            this.tabRUN.Controls.Add(this.lblMaxADC2);
            this.tabRUN.Controls.Add(this.lblMaxADC1);
            this.tabRUN.Controls.Add(this.lblMaxADC0);
            this.tabRUN.Controls.Add(this.groupBoxEvDisplay);
            this.tabRUN.Controls.Add(this.groupBox1);
            this.tabRUN.Controls.Add(this.zg1);
            this.tabRUN.Location = new System.Drawing.Point(4, 29);
            this.tabRUN.Name = "tabRUN";
            this.tabRUN.Padding = new System.Windows.Forms.Padding(3);
            this.tabRUN.Size = new System.Drawing.Size(1255, 653);
            this.tabRUN.TabIndex = 0;
            this.tabRUN.Text = "RUN";
            this.tabRUN.UseVisualStyleBackColor = true;
            // 
            // runLog
            // 
            this.runLog.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.runLog.Location = new System.Drawing.Point(3, 479);
            this.runLog.MaxLength = 10240;
            this.runLog.Name = "runLog";
            this.runLog.ReadOnly = true;
            this.runLog.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.ForcedBoth;
            this.runLog.Size = new System.Drawing.Size(1249, 171);
            this.runLog.TabIndex = 46;
            this.runLog.Text = "";
            this.runLog.WordWrap = false;
            this.runLog.TextChanged += new System.EventHandler(this.RunLog_TextChanged);
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(715, 131);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(89, 17);
            this.label14.TabIndex = 45;
            this.label14.Text = "<maxADC3>:";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(715, 113);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(89, 17);
            this.label13.TabIndex = 44;
            this.label13.Text = "<maxADC2>:";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(715, 95);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(89, 17);
            this.label12.TabIndex = 43;
            this.label12.Text = "<maxADC1>:";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(715, 77);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(89, 17);
            this.label10.TabIndex = 36;
            this.label10.Text = "<maxADC0>:";
            // 
            // lblMaxADC3
            // 
            this.lblMaxADC3.AutoSize = true;
            this.lblMaxADC3.Location = new System.Drawing.Point(810, 131);
            this.lblMaxADC3.Name = "lblMaxADC3";
            this.lblMaxADC3.Size = new System.Drawing.Size(54, 17);
            this.lblMaxADC3.TabIndex = 42;
            this.lblMaxADC3.Text = "label14";
            // 
            // lblMaxADC2
            // 
            this.lblMaxADC2.AutoSize = true;
            this.lblMaxADC2.Location = new System.Drawing.Point(810, 113);
            this.lblMaxADC2.Name = "lblMaxADC2";
            this.lblMaxADC2.Size = new System.Drawing.Size(54, 17);
            this.lblMaxADC2.TabIndex = 41;
            this.lblMaxADC2.Text = "label14";
            // 
            // lblMaxADC1
            // 
            this.lblMaxADC1.AutoSize = true;
            this.lblMaxADC1.Location = new System.Drawing.Point(810, 95);
            this.lblMaxADC1.Name = "lblMaxADC1";
            this.lblMaxADC1.Size = new System.Drawing.Size(54, 17);
            this.lblMaxADC1.TabIndex = 40;
            this.lblMaxADC1.Text = "label14";
            // 
            // lblMaxADC0
            // 
            this.lblMaxADC0.AutoSize = true;
            this.lblMaxADC0.Location = new System.Drawing.Point(810, 77);
            this.lblMaxADC0.Name = "lblMaxADC0";
            this.lblMaxADC0.Size = new System.Drawing.Size(54, 17);
            this.lblMaxADC0.TabIndex = 36;
            this.lblMaxADC0.Text = "label14";
            // 
            // groupBoxEvDisplay
            // 
            this.groupBoxEvDisplay.Controls.Add(this.chkLast);
            this.groupBoxEvDisplay.Controls.Add(this.chkPersist);
            this.groupBoxEvDisplay.Controls.Add(this.btnDisplaySpill);
            this.groupBoxEvDisplay.Controls.Add(this.txtEvent);
            this.groupBoxEvDisplay.Controls.Add(this.btnNextDisp);
            this.groupBoxEvDisplay.Controls.Add(this.btnPrevDisp);
            this.groupBoxEvDisplay.Controls.Add(this.ud_VertMin);
            this.groupBoxEvDisplay.Controls.Add(this.ud_VertMax);
            this.groupBoxEvDisplay.Location = new System.Drawing.Point(620, 359);
            this.groupBoxEvDisplay.Name = "groupBoxEvDisplay";
            this.groupBoxEvDisplay.Size = new System.Drawing.Size(623, 114);
            this.groupBoxEvDisplay.TabIndex = 35;
            this.groupBoxEvDisplay.TabStop = false;
            this.groupBoxEvDisplay.Text = "EventDisplay";
            this.groupBoxEvDisplay.Enter += new System.EventHandler(this.GroupBoxEvDisplay_Enter);
            // 
            // chkLast
            // 
            this.chkLast.AutoSize = true;
            this.chkLast.Location = new System.Drawing.Point(407, 43);
            this.chkLast.Name = "chkLast";
            this.chkLast.Size = new System.Drawing.Size(54, 21);
            this.chkLast.TabIndex = 41;
            this.chkLast.Text = "Last";
            this.chkLast.UseVisualStyleBackColor = true;
            this.chkLast.CheckedChanged += new System.EventHandler(this.ChkLast_CheckedChanged);
            // 
            // chkPersist
            // 
            this.chkPersist.AutoSize = true;
            this.chkPersist.Location = new System.Drawing.Point(407, 9);
            this.chkPersist.Name = "chkPersist";
            this.chkPersist.Size = new System.Drawing.Size(70, 21);
            this.chkPersist.TabIndex = 36;
            this.chkPersist.Text = "Persist";
            this.chkPersist.UseVisualStyleBackColor = true;
            // 
            // btnDisplaySpill
            // 
            this.btnDisplaySpill.Location = new System.Drawing.Point(407, 78);
            this.btnDisplaySpill.Name = "btnDisplaySpill";
            this.btnDisplaySpill.Size = new System.Drawing.Size(102, 30);
            this.btnDisplaySpill.TabIndex = 40;
            this.btnDisplaySpill.Text = "DISPLAY";
            this.btnDisplaySpill.UseVisualStyleBackColor = true;
            this.btnDisplaySpill.Click += new System.EventHandler(this.BtnDisplaySpill_Click);
            // 
            // txtEvent
            // 
            this.txtEvent.Location = new System.Drawing.Point(517, 40);
            this.txtEvent.Name = "txtEvent";
            this.txtEvent.Size = new System.Drawing.Size(44, 23);
            this.txtEvent.TabIndex = 38;
            // 
            // btnNextDisp
            // 
            this.btnNextDisp.Location = new System.Drawing.Point(567, 37);
            this.btnNextDisp.Name = "btnNextDisp";
            this.btnNextDisp.Size = new System.Drawing.Size(50, 30);
            this.btnNextDisp.TabIndex = 37;
            this.btnNextDisp.Text = ">>>";
            this.btnNextDisp.UseVisualStyleBackColor = true;
            this.btnNextDisp.Click += new System.EventHandler(this.BtnNextDisp_Click);
            // 
            // btnPrevDisp
            // 
            this.btnPrevDisp.Location = new System.Drawing.Point(466, 37);
            this.btnPrevDisp.Name = "btnPrevDisp";
            this.btnPrevDisp.Size = new System.Drawing.Size(50, 30);
            this.btnPrevDisp.TabIndex = 36;
            this.btnPrevDisp.Text = "<<<";
            this.btnPrevDisp.UseVisualStyleBackColor = true;
            this.btnPrevDisp.Click += new System.EventHandler(this.BtnPrevDisp_Click);
            // 
            // ud_VertMin
            // 
            this.ud_VertMin.Increment = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.ud_VertMin.Location = new System.Drawing.Point(515, 73);
            this.ud_VertMin.Maximum = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.ud_VertMin.Minimum = new decimal(new int[] {
            2500,
            0,
            0,
            -2147483648});
            this.ud_VertMin.Name = "ud_VertMin";
            this.ud_VertMin.Size = new System.Drawing.Size(66, 23);
            this.ud_VertMin.TabIndex = 35;
            this.ud_VertMin.Value = new decimal(new int[] {
            100,
            0,
            0,
            -2147483648});
            // 
            // ud_VertMax
            // 
            this.ud_VertMax.Increment = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.ud_VertMax.Location = new System.Drawing.Point(515, 7);
            this.ud_VertMax.Maximum = new decimal(new int[] {
            2500,
            0,
            0,
            0});
            this.ud_VertMax.Name = "ud_VertMax";
            this.ud_VertMax.Size = new System.Drawing.Size(66, 23);
            this.ud_VertMax.TabIndex = 34;
            this.ud_VertMax.Value = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.SpillStatusGroupBox);
            this.groupBox1.Controls.Add(this.btnFebClientsChange);
            this.groupBox1.Controls.Add(this.btnStopRun);
            this.groupBox1.Controls.Add(this.btnStartRun);
            this.groupBox1.Controls.Add(this.btnPrepare);
            this.groupBox1.Controls.Add(this.btnConnectAll);
            this.groupBox1.Location = new System.Drawing.Point(7, 39);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(608, 403);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "RUN CONTROL";
            // 
            // SpillStatusGroupBox
            // 
            this.SpillStatusGroupBox.Controls.Add(this.lblSpillsNum);
            this.SpillStatusGroupBox.Controls.Add(this.lblNumSpills);
            this.SpillStatusGroupBox.Controls.Add(this.SpillStatusTable);
            this.SpillStatusGroupBox.Controls.Add(this.lblTimeInSpill);
            this.SpillStatusGroupBox.Controls.Add(this.validateParseChkBox);
            this.SpillStatusGroupBox.Controls.Add(this.saveAsciiBox);
            this.SpillStatusGroupBox.Controls.Add(this.lblRunName);
            this.SpillStatusGroupBox.Controls.Add(this.lblTimeInRun);
            this.SpillStatusGroupBox.Controls.Add(this.lblTxtRunName);
            this.SpillStatusGroupBox.Controls.Add(this.lblRunTime);
            this.SpillStatusGroupBox.Controls.Add(this.lblSpillTime);
            this.SpillStatusGroupBox.Location = new System.Drawing.Point(176, 20);
            this.SpillStatusGroupBox.Name = "SpillStatusGroupBox";
            this.SpillStatusGroupBox.Size = new System.Drawing.Size(426, 372);
            this.SpillStatusGroupBox.TabIndex = 38;
            this.SpillStatusGroupBox.TabStop = false;
            this.SpillStatusGroupBox.Text = "Spill Status";
            // 
            // lblSpillsNum
            // 
            this.lblSpillsNum.AutoSize = true;
            this.lblSpillsNum.Location = new System.Drawing.Point(94, 140);
            this.lblSpillsNum.Name = "lblSpillsNum";
            this.lblSpillsNum.Size = new System.Drawing.Size(84, 17);
            this.lblSpillsNum.TabIndex = 41;
            this.lblSpillsNum.Text = "(Num Spills)";
            // 
            // lblNumSpills
            // 
            this.lblNumSpills.AutoSize = true;
            this.lblNumSpills.Location = new System.Drawing.Point(9, 140);
            this.lblNumSpills.Name = "lblNumSpills";
            this.lblNumSpills.Size = new System.Drawing.Size(74, 17);
            this.lblNumSpills.TabIndex = 40;
            this.lblNumSpills.Text = "Num Spills";
            this.lblNumSpills.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // SpillStatusTable
            // 
            this.SpillStatusTable.ColumnCount = 4;
            this.SpillStatusTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.SpillStatusTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.SpillStatusTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.SpillStatusTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.SpillStatusTable.Controls.Add(this.lblFebColumn, 0, 0);
            this.SpillStatusTable.Controls.Add(this.lblSpillStatus, 1, 0);
            this.SpillStatusTable.Controls.Add(this.lblLastSpillTrig, 2, 0);
            this.SpillStatusTable.Controls.Add(this.lblTotalNumTrig, 3, 0);
            this.SpillStatusTable.Location = new System.Drawing.Point(6, 177);
            this.SpillStatusTable.Name = "SpillStatusTable";
            this.SpillStatusTable.RowCount = 2;
            this.SpillStatusTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 36F));
            this.SpillStatusTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.SpillStatusTable.Size = new System.Drawing.Size(414, 189);
            this.SpillStatusTable.TabIndex = 39;
            // 
            // lblFebColumn
            // 
            this.lblFebColumn.AutoSize = true;
            this.lblFebColumn.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblFebColumn.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.18868F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblFebColumn.Location = new System.Drawing.Point(3, 0);
            this.lblFebColumn.Name = "lblFebColumn";
            this.lblFebColumn.Size = new System.Drawing.Size(97, 36);
            this.lblFebColumn.TabIndex = 40;
            this.lblFebColumn.Text = "FEB";
            this.lblFebColumn.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblSpillStatus
            // 
            this.lblSpillStatus.AutoSize = true;
            this.lblSpillStatus.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblSpillStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.18868F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSpillStatus.Location = new System.Drawing.Point(106, 0);
            this.lblSpillStatus.Name = "lblSpillStatus";
            this.lblSpillStatus.Size = new System.Drawing.Size(97, 36);
            this.lblSpillStatus.TabIndex = 10;
            this.lblSpillStatus.Text = "Spill Status";
            this.lblSpillStatus.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblLastSpillTrig
            // 
            this.lblLastSpillTrig.AutoSize = true;
            this.lblLastSpillTrig.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblLastSpillTrig.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.18868F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblLastSpillTrig.Location = new System.Drawing.Point(209, 0);
            this.lblLastSpillTrig.Name = "lblLastSpillTrig";
            this.lblLastSpillTrig.Size = new System.Drawing.Size(97, 36);
            this.lblLastSpillTrig.TabIndex = 15;
            this.lblLastSpillTrig.Text = "Last Spill Trig";
            this.lblLastSpillTrig.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblTotalNumTrig
            // 
            this.lblTotalNumTrig.AutoSize = true;
            this.lblTotalNumTrig.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblTotalNumTrig.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.18868F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTotalNumTrig.Location = new System.Drawing.Point(312, 0);
            this.lblTotalNumTrig.Name = "lblTotalNumTrig";
            this.lblTotalNumTrig.Size = new System.Drawing.Size(99, 36);
            this.lblTotalNumTrig.TabIndex = 14;
            this.lblTotalNumTrig.Text = "Total Num Trig";
            this.lblTotalNumTrig.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblTimeInSpill
            // 
            this.lblTimeInSpill.AutoSize = true;
            this.lblTimeInSpill.Location = new System.Drawing.Point(90, 113);
            this.lblTimeInSpill.Name = "lblTimeInSpill";
            this.lblTimeInSpill.Size = new System.Drawing.Size(92, 17);
            this.lblTimeInSpill.TabIndex = 38;
            this.lblTimeInSpill.Text = "(Time in spill)";
            // 
            // validateParseChkBox
            // 
            this.validateParseChkBox.AutoSize = true;
            this.validateParseChkBox.Enabled = false;
            this.validateParseChkBox.Location = new System.Drawing.Point(46, 22);
            this.validateParseChkBox.Name = "validateParseChkBox";
            this.validateParseChkBox.Size = new System.Drawing.Size(78, 21);
            this.validateParseChkBox.TabIndex = 37;
            this.validateParseChkBox.Text = "Validate";
            this.validateParseChkBox.UseVisualStyleBackColor = true;
            this.validateParseChkBox.CheckedChanged += new System.EventHandler(this.ValidateParseChkBox_CheckedChanged);
            // 
            // saveAsciiBox
            // 
            this.saveAsciiBox.AutoSize = true;
            this.saveAsciiBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.saveAsciiBox.Location = new System.Drawing.Point(164, 22);
            this.saveAsciiBox.Name = "saveAsciiBox";
            this.saveAsciiBox.Size = new System.Drawing.Size(226, 21);
            this.saveAsciiBox.TabIndex = 36;
            this.saveAsciiBox.Text = "Save data as human-readable?";
            this.saveAsciiBox.UseVisualStyleBackColor = true;
            // 
            // lblRunName
            // 
            this.lblRunName.AutoSize = true;
            this.lblRunName.Location = new System.Drawing.Point(8, 59);
            this.lblRunName.Name = "lblRunName";
            this.lblRunName.Size = new System.Drawing.Size(75, 17);
            this.lblRunName.TabIndex = 9;
            this.lblRunName.Text = "Run Name";
            // 
            // lblTimeInRun
            // 
            this.lblTimeInRun.AutoSize = true;
            this.lblTimeInRun.Location = new System.Drawing.Point(92, 86);
            this.lblTimeInRun.Name = "lblTimeInRun";
            this.lblTimeInRun.Size = new System.Drawing.Size(89, 17);
            this.lblTimeInRun.TabIndex = 11;
            this.lblTimeInRun.Text = "(Time in run)";
            // 
            // lblTxtRunName
            // 
            this.lblTxtRunName.AutoSize = true;
            this.lblTxtRunName.Location = new System.Drawing.Point(96, 59);
            this.lblTxtRunName.Name = "lblTxtRunName";
            this.lblTxtRunName.Size = new System.Drawing.Size(81, 17);
            this.lblTxtRunName.TabIndex = 5;
            this.lblTxtRunName.Text = "(RunName)";
            // 
            // lblRunTime
            // 
            this.lblRunTime.AutoSize = true;
            this.lblRunTime.Location = new System.Drawing.Point(18, 86);
            this.lblRunTime.Name = "lblRunTime";
            this.lblRunTime.Size = new System.Drawing.Size(65, 17);
            this.lblRunTime.TabIndex = 12;
            this.lblRunTime.Text = "RunTime";
            // 
            // lblSpillTime
            // 
            this.lblSpillTime.AutoSize = true;
            this.lblSpillTime.Location = new System.Drawing.Point(18, 113);
            this.lblSpillTime.Name = "lblSpillTime";
            this.lblSpillTime.Size = new System.Drawing.Size(65, 17);
            this.lblSpillTime.TabIndex = 21;
            this.lblSpillTime.Text = "SpillTime";
            // 
            // btnFebClientsChange
            // 
            this.btnFebClientsChange.Location = new System.Drawing.Point(13, 33);
            this.btnFebClientsChange.Name = "btnFebClientsChange";
            this.btnFebClientsChange.Size = new System.Drawing.Size(150, 28);
            this.btnFebClientsChange.TabIndex = 31;
            this.btnFebClientsChange.Tag = "";
            this.btnFebClientsChange.Text = "FEB Clients";
            this.btnFebClientsChange.UseVisualStyleBackColor = true;
            this.btnFebClientsChange.Click += new System.EventHandler(this.ClientChangeBtn_Click);
            // 
            // btnStopRun
            // 
            this.btnStopRun.Enabled = false;
            this.btnStopRun.Location = new System.Drawing.Point(13, 267);
            this.btnStopRun.Name = "btnStopRun";
            this.btnStopRun.Size = new System.Drawing.Size(150, 60);
            this.btnStopRun.TabIndex = 4;
            this.btnStopRun.Tag = "";
            this.btnStopRun.Text = "STOP RUN";
            this.btnStopRun.UseVisualStyleBackColor = true;
            this.btnStopRun.Click += new System.EventHandler(this.BtnStopRun_Click);
            // 
            // btnStartRun
            // 
            this.btnStartRun.Enabled = false;
            this.btnStartRun.Location = new System.Drawing.Point(13, 201);
            this.btnStartRun.Name = "btnStartRun";
            this.btnStartRun.Size = new System.Drawing.Size(150, 60);
            this.btnStartRun.TabIndex = 3;
            this.btnStartRun.Tag = "";
            this.btnStartRun.Text = "START RUN";
            this.btnStartRun.UseVisualStyleBackColor = true;
            this.btnStartRun.Click += new System.EventHandler(this.BtnStartRun_Click);
            // 
            // btnPrepare
            // 
            this.btnPrepare.Location = new System.Drawing.Point(13, 135);
            this.btnPrepare.Name = "btnPrepare";
            this.btnPrepare.Size = new System.Drawing.Size(150, 60);
            this.btnPrepare.TabIndex = 2;
            this.btnPrepare.Tag = "";
            this.btnPrepare.Text = "PREPARE FOR RUN";
            this.btnPrepare.UseVisualStyleBackColor = true;
            this.btnPrepare.Click += new System.EventHandler(this.BtnPrepare_Click);
            // 
            // btnConnectAll
            // 
            this.btnConnectAll.Location = new System.Drawing.Point(13, 67);
            this.btnConnectAll.Name = "btnConnectAll";
            this.btnConnectAll.Size = new System.Drawing.Size(150, 60);
            this.btnConnectAll.TabIndex = 1;
            this.btnConnectAll.Tag = "";
            this.btnConnectAll.Text = "CONNECT ALL";
            this.btnConnectAll.UseVisualStyleBackColor = true;
            this.btnConnectAll.Click += new System.EventHandler(this.BtnConnectAll_Click);
            // 
            // zg1
            // 
            this.zg1.Location = new System.Drawing.Point(620, 8);
            this.zg1.Margin = new System.Windows.Forms.Padding(6);
            this.zg1.Name = "zg1";
            this.zg1.ScrollGrace = 0D;
            this.zg1.ScrollMaxX = 0D;
            this.zg1.ScrollMaxY = 0D;
            this.zg1.ScrollMaxY2 = 0D;
            this.zg1.ScrollMinX = 0D;
            this.zg1.ScrollMinY = 0D;
            this.zg1.ScrollMinY2 = 0D;
            this.zg1.Size = new System.Drawing.Size(630, 342);
            this.zg1.TabIndex = 1;
            // 
            // tabConsole
            // 
            this.tabConsole.Controls.Add(this.loadCmdsBtn);
            this.tabConsole.Controls.Add(this.btnDebugLogging);
            this.tabConsole.Controls.Add(this.ConsoleBox);
            this.tabConsole.Controls.Add(this.groupBox3);
            this.tabConsole.Location = new System.Drawing.Point(4, 29);
            this.tabConsole.Name = "tabConsole";
            this.tabConsole.Size = new System.Drawing.Size(1255, 653);
            this.tabConsole.TabIndex = 7;
            this.tabConsole.Text = "Debug Console";
            this.tabConsole.UseVisualStyleBackColor = true;
            // 
            // loadCmdsBtn
            // 
            this.loadCmdsBtn.Location = new System.Drawing.Point(1101, 623);
            this.loadCmdsBtn.Name = "loadCmdsBtn";
            this.loadCmdsBtn.Size = new System.Drawing.Size(148, 25);
            this.loadCmdsBtn.TabIndex = 3;
            this.loadCmdsBtn.Text = "LOAD";
            this.loadCmdsBtn.UseVisualStyleBackColor = true;
            this.loadCmdsBtn.Click += new System.EventHandler(this.LoadCmdsBtn_Click);
            // 
            // btnDebugLogging
            // 
            this.btnDebugLogging.Location = new System.Drawing.Point(947, 623);
            this.btnDebugLogging.Name = "btnDebugLogging";
            this.btnDebugLogging.Size = new System.Drawing.Size(148, 25);
            this.btnDebugLogging.TabIndex = 2;
            this.btnDebugLogging.Text = "START LOG";
            this.btnDebugLogging.UseVisualStyleBackColor = true;
            this.btnDebugLogging.Click += new System.EventHandler(this.BtnDebugLogging_Click);
            // 
            // ConsoleBox
            // 
            this.ConsoleBox.Location = new System.Drawing.Point(5, 624);
            this.ConsoleBox.Multiline = true;
            this.ConsoleBox.Name = "ConsoleBox";
            this.ConsoleBox.Size = new System.Drawing.Size(936, 24);
            this.ConsoleBox.TabIndex = 0;
            this.ConsoleBox.TextChanged += new System.EventHandler(this.ConsoleBox_TextChanged);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.console_Disp);
            this.groupBox3.Location = new System.Drawing.Point(7, 3);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(1249, 615);
            this.groupBox3.TabIndex = 4;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "CONSOLE";
            // 
            // console_Disp
            // 
            this.console_Disp.Dock = System.Windows.Forms.DockStyle.Fill;
            this.console_Disp.Location = new System.Drawing.Point(3, 19);
            this.console_Disp.Name = "console_Disp";
            this.console_Disp.ReadOnly = true;
            this.console_Disp.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.ForcedVertical;
            this.console_Disp.Size = new System.Drawing.Size(1243, 593);
            this.console_Disp.TabIndex = 2;
            this.console_Disp.Text = "";
            this.console_Disp.TextChanged += new System.EventHandler(this.Console_Disp_TextChanged);
            // 
            // tabFEB1
            // 
            this.tabFEB1.Controls.Add(this.BDVoltsLabel15);
            this.tabFEB1.Controls.Add(this.BDVoltsLabel14);
            this.tabFEB1.Controls.Add(this.BDVoltsLabel13);
            this.tabFEB1.Controls.Add(this.BDVoltsLabel12);
            this.tabFEB1.Controls.Add(this.BDVoltsLabel7);
            this.tabFEB1.Controls.Add(this.BDVoltsLabel6);
            this.tabFEB1.Controls.Add(this.BDVoltsLabel5);
            this.tabFEB1.Controls.Add(this.BDVoltsLabel8);
            this.tabFEB1.Controls.Add(this.BDVoltsLabel11);
            this.tabFEB1.Controls.Add(this.BDVoltsLabel9);
            this.tabFEB1.Controls.Add(this.BDVoltsLabel10);
            this.tabFEB1.Controls.Add(this.BDVoltsLabel4);
            this.tabFEB1.Controls.Add(this.BDVoltsLabel3);
            this.tabFEB1.Controls.Add(this.BDVoltsLabel2);
            this.tabFEB1.Controls.Add(this.BDVoltsLabel1);
            this.tabFEB1.Controls.Add(this.BDVoltsLabel0);
            this.tabFEB1.Controls.Add(this.scanAllChanBox);
            this.tabFEB1.Controls.Add(this.label23);
            this.tabFEB1.Controls.Add(this.ShowSpect);
            this.tabFEB1.Controls.Add(this.ShowIV);
            this.tabFEB1.Controls.Add(this.zedFEB1);
            this.tabFEB1.Controls.Add(this.groupBoxREG1);
            this.tabFEB1.Controls.Add(this.panel2);
            this.tabFEB1.Controls.Add(this.panel1);
            this.tabFEB1.Controls.Add(this.groupBox7);
            this.tabFEB1.Location = new System.Drawing.Point(4, 29);
            this.tabFEB1.Name = "tabFEB1";
            this.tabFEB1.Size = new System.Drawing.Size(1255, 653);
            this.tabFEB1.TabIndex = 3;
            this.tabFEB1.Text = "FEB";
            this.tabFEB1.UseVisualStyleBackColor = true;
            // 
            // BDVoltsLabel15
            // 
            this.BDVoltsLabel15.AutoSize = true;
            this.BDVoltsLabel15.BackColor = System.Drawing.SystemColors.InactiveCaption;
            this.BDVoltsLabel15.Location = new System.Drawing.Point(1202, 612);
            this.BDVoltsLabel15.Name = "BDVoltsLabel15";
            this.BDVoltsLabel15.Size = new System.Drawing.Size(45, 17);
            this.BDVoltsLabel15.TabIndex = 99;
            this.BDVoltsLabel15.Text = "0.00V";
            // 
            // BDVoltsLabel14
            // 
            this.BDVoltsLabel14.AutoSize = true;
            this.BDVoltsLabel14.BackColor = System.Drawing.SystemColors.InactiveCaption;
            this.BDVoltsLabel14.Location = new System.Drawing.Point(1151, 612);
            this.BDVoltsLabel14.Name = "BDVoltsLabel14";
            this.BDVoltsLabel14.Size = new System.Drawing.Size(45, 17);
            this.BDVoltsLabel14.TabIndex = 98;
            this.BDVoltsLabel14.Text = "0.00V";
            // 
            // BDVoltsLabel13
            // 
            this.BDVoltsLabel13.AutoSize = true;
            this.BDVoltsLabel13.BackColor = System.Drawing.SystemColors.InactiveCaption;
            this.BDVoltsLabel13.Location = new System.Drawing.Point(1100, 612);
            this.BDVoltsLabel13.Name = "BDVoltsLabel13";
            this.BDVoltsLabel13.Size = new System.Drawing.Size(45, 17);
            this.BDVoltsLabel13.TabIndex = 97;
            this.BDVoltsLabel13.Text = "0.00V";
            // 
            // BDVoltsLabel12
            // 
            this.BDVoltsLabel12.AutoSize = true;
            this.BDVoltsLabel12.BackColor = System.Drawing.SystemColors.InactiveCaption;
            this.BDVoltsLabel12.Location = new System.Drawing.Point(1049, 612);
            this.BDVoltsLabel12.Name = "BDVoltsLabel12";
            this.BDVoltsLabel12.Size = new System.Drawing.Size(45, 17);
            this.BDVoltsLabel12.TabIndex = 96;
            this.BDVoltsLabel12.Text = "0.00V";
            // 
            // BDVoltsLabel7
            // 
            this.BDVoltsLabel7.AutoSize = true;
            this.BDVoltsLabel7.BackColor = System.Drawing.SystemColors.InactiveCaption;
            this.BDVoltsLabel7.Location = new System.Drawing.Point(1202, 578);
            this.BDVoltsLabel7.Name = "BDVoltsLabel7";
            this.BDVoltsLabel7.Size = new System.Drawing.Size(45, 17);
            this.BDVoltsLabel7.TabIndex = 95;
            this.BDVoltsLabel7.Text = "0.00V";
            // 
            // BDVoltsLabel6
            // 
            this.BDVoltsLabel6.AutoSize = true;
            this.BDVoltsLabel6.BackColor = System.Drawing.SystemColors.InactiveCaption;
            this.BDVoltsLabel6.Location = new System.Drawing.Point(1151, 578);
            this.BDVoltsLabel6.Name = "BDVoltsLabel6";
            this.BDVoltsLabel6.Size = new System.Drawing.Size(45, 17);
            this.BDVoltsLabel6.TabIndex = 94;
            this.BDVoltsLabel6.Text = "0.00V";
            // 
            // BDVoltsLabel5
            // 
            this.BDVoltsLabel5.AutoSize = true;
            this.BDVoltsLabel5.BackColor = System.Drawing.SystemColors.InactiveCaption;
            this.BDVoltsLabel5.Location = new System.Drawing.Point(1100, 578);
            this.BDVoltsLabel5.Name = "BDVoltsLabel5";
            this.BDVoltsLabel5.Size = new System.Drawing.Size(45, 17);
            this.BDVoltsLabel5.TabIndex = 93;
            this.BDVoltsLabel5.Text = "0.00V";
            // 
            // BDVoltsLabel8
            // 
            this.BDVoltsLabel8.AutoSize = true;
            this.BDVoltsLabel8.BackColor = System.Drawing.SystemColors.InactiveCaption;
            this.BDVoltsLabel8.Location = new System.Drawing.Point(1049, 595);
            this.BDVoltsLabel8.Name = "BDVoltsLabel8";
            this.BDVoltsLabel8.Size = new System.Drawing.Size(45, 17);
            this.BDVoltsLabel8.TabIndex = 92;
            this.BDVoltsLabel8.Text = "0.00V";
            // 
            // BDVoltsLabel11
            // 
            this.BDVoltsLabel11.AutoSize = true;
            this.BDVoltsLabel11.BackColor = System.Drawing.SystemColors.InactiveCaption;
            this.BDVoltsLabel11.Location = new System.Drawing.Point(1202, 595);
            this.BDVoltsLabel11.Name = "BDVoltsLabel11";
            this.BDVoltsLabel11.Size = new System.Drawing.Size(45, 17);
            this.BDVoltsLabel11.TabIndex = 91;
            this.BDVoltsLabel11.Text = "0.00V";
            // 
            // BDVoltsLabel9
            // 
            this.BDVoltsLabel9.AutoSize = true;
            this.BDVoltsLabel9.BackColor = System.Drawing.SystemColors.InactiveCaption;
            this.BDVoltsLabel9.Location = new System.Drawing.Point(1100, 595);
            this.BDVoltsLabel9.Name = "BDVoltsLabel9";
            this.BDVoltsLabel9.Size = new System.Drawing.Size(45, 17);
            this.BDVoltsLabel9.TabIndex = 90;
            this.BDVoltsLabel9.Text = "0.00V";
            // 
            // BDVoltsLabel10
            // 
            this.BDVoltsLabel10.AutoSize = true;
            this.BDVoltsLabel10.BackColor = System.Drawing.SystemColors.InactiveCaption;
            this.BDVoltsLabel10.Location = new System.Drawing.Point(1151, 595);
            this.BDVoltsLabel10.Name = "BDVoltsLabel10";
            this.BDVoltsLabel10.Size = new System.Drawing.Size(45, 17);
            this.BDVoltsLabel10.TabIndex = 89;
            this.BDVoltsLabel10.Text = "0.00V";
            // 
            // BDVoltsLabel4
            // 
            this.BDVoltsLabel4.AutoSize = true;
            this.BDVoltsLabel4.BackColor = System.Drawing.SystemColors.InactiveCaption;
            this.BDVoltsLabel4.Location = new System.Drawing.Point(1049, 578);
            this.BDVoltsLabel4.Name = "BDVoltsLabel4";
            this.BDVoltsLabel4.Size = new System.Drawing.Size(45, 17);
            this.BDVoltsLabel4.TabIndex = 88;
            this.BDVoltsLabel4.Text = "0.00V";
            // 
            // BDVoltsLabel3
            // 
            this.BDVoltsLabel3.AutoSize = true;
            this.BDVoltsLabel3.BackColor = System.Drawing.SystemColors.InactiveCaption;
            this.BDVoltsLabel3.Location = new System.Drawing.Point(1202, 561);
            this.BDVoltsLabel3.Name = "BDVoltsLabel3";
            this.BDVoltsLabel3.Size = new System.Drawing.Size(45, 17);
            this.BDVoltsLabel3.TabIndex = 87;
            this.BDVoltsLabel3.Text = "0.00V";
            // 
            // BDVoltsLabel2
            // 
            this.BDVoltsLabel2.AutoSize = true;
            this.BDVoltsLabel2.BackColor = System.Drawing.SystemColors.InactiveCaption;
            this.BDVoltsLabel2.Location = new System.Drawing.Point(1151, 561);
            this.BDVoltsLabel2.Name = "BDVoltsLabel2";
            this.BDVoltsLabel2.Size = new System.Drawing.Size(45, 17);
            this.BDVoltsLabel2.TabIndex = 86;
            this.BDVoltsLabel2.Text = "0.00V";
            // 
            // BDVoltsLabel1
            // 
            this.BDVoltsLabel1.AutoSize = true;
            this.BDVoltsLabel1.BackColor = System.Drawing.SystemColors.InactiveCaption;
            this.BDVoltsLabel1.Location = new System.Drawing.Point(1100, 561);
            this.BDVoltsLabel1.Name = "BDVoltsLabel1";
            this.BDVoltsLabel1.Size = new System.Drawing.Size(45, 17);
            this.BDVoltsLabel1.TabIndex = 85;
            this.BDVoltsLabel1.Text = "0.00V";
            // 
            // BDVoltsLabel0
            // 
            this.BDVoltsLabel0.AutoSize = true;
            this.BDVoltsLabel0.BackColor = System.Drawing.SystemColors.InactiveCaption;
            this.BDVoltsLabel0.Location = new System.Drawing.Point(1049, 561);
            this.BDVoltsLabel0.Name = "BDVoltsLabel0";
            this.BDVoltsLabel0.Size = new System.Drawing.Size(45, 17);
            this.BDVoltsLabel0.TabIndex = 84;
            this.BDVoltsLabel0.Text = "0.00V";
            // 
            // scanAllChanBox
            // 
            this.scanAllChanBox.AutoSize = true;
            this.scanAllChanBox.Location = new System.Drawing.Point(902, 598);
            this.scanAllChanBox.Name = "scanAllChanBox";
            this.scanAllChanBox.Size = new System.Drawing.Size(141, 21);
            this.scanAllChanBox.TabIndex = 83;
            this.scanAllChanBox.Text = "Scan All Channels";
            this.scanAllChanBox.UseVisualStyleBackColor = true;
            // 
            // label23
            // 
            this.label23.AutoSize = true;
            this.label23.BackColor = System.Drawing.SystemColors.Menu;
            this.label23.Location = new System.Drawing.Point(903, 578);
            this.label23.Name = "label23";
            this.label23.Size = new System.Drawing.Size(137, 17);
            this.label23.TabIndex = 66;
            this.label23.Text = "Breakdown Voltages";
            // 
            // ShowSpect
            // 
            this.ShowSpect.Location = new System.Drawing.Point(796, 522);
            this.ShowSpect.Name = "ShowSpect";
            this.ShowSpect.Size = new System.Drawing.Size(112, 42);
            this.ShowSpect.TabIndex = 64;
            this.ShowSpect.Tag = "";
            this.ShowSpect.Text = "PH Histo";
            this.ShowSpect.UseVisualStyleBackColor = true;
            this.ShowSpect.Visible = false;
            this.ShowSpect.Click += new System.EventHandler(this.ShowSpect_Click);
            // 
            // ShowIV
            // 
            this.ShowIV.Location = new System.Drawing.Point(678, 522);
            this.ShowIV.Name = "ShowIV";
            this.ShowIV.Size = new System.Drawing.Size(112, 42);
            this.ShowIV.TabIndex = 63;
            this.ShowIV.Tag = "";
            this.ShowIV.Text = "IV";
            this.ShowIV.UseVisualStyleBackColor = true;
            this.ShowIV.Click += new System.EventHandler(this.ShowIV_Click);
            // 
            // zedFEB1
            // 
            this.zedFEB1.Location = new System.Drawing.Point(653, 4);
            this.zedFEB1.Margin = new System.Windows.Forms.Padding(9, 8, 9, 8);
            this.zedFEB1.Name = "zedFEB1";
            this.zedFEB1.ScrollGrace = 0D;
            this.zedFEB1.ScrollMaxX = 0D;
            this.zedFEB1.ScrollMaxY = 0D;
            this.zedFEB1.ScrollMaxY2 = 0D;
            this.zedFEB1.ScrollMinX = 0D;
            this.zedFEB1.ScrollMinY = 0D;
            this.zedFEB1.ScrollMinY2 = 0D;
            this.zedFEB1.Size = new System.Drawing.Size(594, 436);
            this.zedFEB1.TabIndex = 60;
            // 
            // groupBoxREG1
            // 
            this.groupBoxREG1.Controls.Add(this.lblFPGA);
            this.groupBoxREG1.Controls.Add(this.btnCHANGE);
            this.groupBoxREG1.Controls.Add(this.udFPGA);
            this.groupBoxREG1.Controls.Add(this.btnRegWRITE);
            this.groupBoxREG1.Controls.Add(this.btnRegREAD);
            this.groupBoxREG1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.groupBoxREG1.Location = new System.Drawing.Point(8, 6);
            this.groupBoxREG1.Name = "groupBoxREG1";
            this.groupBoxREG1.Size = new System.Drawing.Size(435, 460);
            this.groupBoxREG1.TabIndex = 6;
            this.groupBoxREG1.TabStop = false;
            this.groupBoxREG1.Text = "REGISTERS";
            // 
            // lblFPGA
            // 
            this.lblFPGA.AutoSize = true;
            this.lblFPGA.Location = new System.Drawing.Point(10, 426);
            this.lblFPGA.Name = "lblFPGA";
            this.lblFPGA.Size = new System.Drawing.Size(45, 17);
            this.lblFPGA.TabIndex = 63;
            this.lblFPGA.Text = "FPGA";
            // 
            // btnCHANGE
            // 
            this.btnCHANGE.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.830189F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCHANGE.Location = new System.Drawing.Point(335, 423);
            this.btnCHANGE.Name = "btnCHANGE";
            this.btnCHANGE.Size = new System.Drawing.Size(83, 23);
            this.btnCHANGE.TabIndex = 14;
            this.btnCHANGE.Tag = "0";
            this.btnCHANGE.Text = "CHANGE";
            this.btnCHANGE.UseVisualStyleBackColor = true;
            this.btnCHANGE.Click += new System.EventHandler(this.BtnCHANGE_Click);
            // 
            // udFPGA
            // 
            this.udFPGA.Location = new System.Drawing.Point(76, 423);
            this.udFPGA.Maximum = new decimal(new int[] {
            3,
            0,
            0,
            0});
            this.udFPGA.Name = "udFPGA";
            this.udFPGA.Size = new System.Drawing.Size(46, 23);
            this.udFPGA.TabIndex = 13;
            this.udFPGA.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.udFPGA.ValueChanged += new System.EventHandler(this.UdFPGA_ValueChanged);
            // 
            // btnRegWRITE
            // 
            this.btnRegWRITE.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.830189F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnRegWRITE.Location = new System.Drawing.Point(239, 423);
            this.btnRegWRITE.Name = "btnRegWRITE";
            this.btnRegWRITE.Size = new System.Drawing.Size(75, 23);
            this.btnRegWRITE.TabIndex = 11;
            this.btnRegWRITE.Text = "WRITE";
            this.btnRegWRITE.UseVisualStyleBackColor = true;
            this.btnRegWRITE.Click += new System.EventHandler(this.BtnRegWRITE_Click);
            // 
            // btnRegREAD
            // 
            this.btnRegREAD.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.830189F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnRegREAD.Location = new System.Drawing.Point(143, 423);
            this.btnRegREAD.Name = "btnRegREAD";
            this.btnRegREAD.Size = new System.Drawing.Size(75, 23);
            this.btnRegREAD.TabIndex = 10;
            this.btnRegREAD.Text = "READ";
            this.btnRegREAD.UseVisualStyleBackColor = true;
            this.btnRegREAD.Click += new System.EventHandler(this.BtnRegREAD_Click);
            // 
            // panel2
            // 
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel2.Controls.Add(this.btnErase);
            this.panel2.Controls.Add(this.label1);
            this.panel2.Controls.Add(this.udChan);
            this.panel2.Controls.Add(this.lblInc);
            this.panel2.Controls.Add(this.udInterval);
            this.panel2.Controls.Add(this.btnSaveHistos);
            this.panel2.Controls.Add(this.lblStop);
            this.panel2.Controls.Add(this.btnScan);
            this.panel2.Controls.Add(this.udStop);
            this.panel2.Controls.Add(this.lblStart);
            this.panel2.Controls.Add(this.udStart);
            this.panel2.Location = new System.Drawing.Point(992, 446);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(255, 113);
            this.panel2.TabIndex = 59;
            // 
            // btnErase
            // 
            this.btnErase.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnErase.Location = new System.Drawing.Point(144, 71);
            this.btnErase.Name = "btnErase";
            this.btnErase.Size = new System.Drawing.Size(95, 33);
            this.btnErase.TabIndex = 58;
            this.btnErase.Text = "ERASE";
            this.btnErase.UseVisualStyleBackColor = true;
            this.btnErase.Click += new System.EventHandler(this.BtnErase_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(0, 83);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(39, 16);
            this.label1.TabIndex = 57;
            this.label1.Text = "Chan";
            // 
            // udChan
            // 
            this.udChan.Location = new System.Drawing.Point(82, 82);
            this.udChan.Maximum = new decimal(new int[] {
            15,
            0,
            0,
            0});
            this.udChan.Name = "udChan";
            this.udChan.Size = new System.Drawing.Size(57, 23);
            this.udChan.TabIndex = 56;
            this.udChan.ValueChanged += new System.EventHandler(this.UdChan_ValueChanged);
            // 
            // lblInc
            // 
            this.lblInc.AutoSize = true;
            this.lblInc.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblInc.Location = new System.Drawing.Point(0, 56);
            this.lblInc.Name = "lblInc";
            this.lblInc.Size = new System.Drawing.Size(68, 16);
            this.lblInc.TabIndex = 53;
            this.lblInc.Text = "Time (ms)";
            // 
            // udInterval
            // 
            this.udInterval.Location = new System.Drawing.Point(82, 55);
            this.udInterval.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.udInterval.Name = "udInterval";
            this.udInterval.Size = new System.Drawing.Size(57, 23);
            this.udInterval.TabIndex = 52;
            this.udInterval.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            // 
            // btnSaveHistos
            // 
            this.btnSaveHistos.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSaveHistos.Location = new System.Drawing.Point(145, 37);
            this.btnSaveHistos.Name = "btnSaveHistos";
            this.btnSaveHistos.Size = new System.Drawing.Size(95, 33);
            this.btnSaveHistos.TabIndex = 55;
            this.btnSaveHistos.Text = "SAVE";
            this.btnSaveHistos.UseVisualStyleBackColor = true;
            this.btnSaveHistos.Click += new System.EventHandler(this.BtnSaveHistos_Click);
            // 
            // lblStop
            // 
            this.lblStop.AutoSize = true;
            this.lblStop.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblStop.Location = new System.Drawing.Point(2, 29);
            this.lblStop.Name = "lblStop";
            this.lblStop.Size = new System.Drawing.Size(36, 16);
            this.lblStop.TabIndex = 51;
            this.lblStop.Text = "Stop";
            // 
            // btnScan
            // 
            this.btnScan.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnScan.Location = new System.Drawing.Point(144, 3);
            this.btnScan.Name = "btnScan";
            this.btnScan.Size = new System.Drawing.Size(95, 33);
            this.btnScan.TabIndex = 54;
            this.btnScan.Text = "SCAN";
            this.btnScan.UseVisualStyleBackColor = true;
            this.btnScan.Click += new System.EventHandler(this.BtnScan_Click);
            // 
            // udStop
            // 
            this.udStop.Location = new System.Drawing.Point(82, 29);
            this.udStop.Maximum = new decimal(new int[] {
            4096,
            0,
            0,
            0});
            this.udStop.Minimum = new decimal(new int[] {
            2048,
            0,
            0,
            0});
            this.udStop.Name = "udStop";
            this.udStop.Size = new System.Drawing.Size(57, 23);
            this.udStop.TabIndex = 50;
            this.udStop.Value = new decimal(new int[] {
            2100,
            0,
            0,
            0});
            // 
            // lblStart
            // 
            this.lblStart.AutoSize = true;
            this.lblStart.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblStart.Location = new System.Drawing.Point(2, 3);
            this.lblStart.Name = "lblStart";
            this.lblStart.Size = new System.Drawing.Size(35, 16);
            this.lblStart.TabIndex = 49;
            this.lblStart.Text = "Start";
            // 
            // udStart
            // 
            this.udStart.Location = new System.Drawing.Point(82, 3);
            this.udStart.Maximum = new decimal(new int[] {
            4095,
            0,
            0,
            0});
            this.udStart.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.udStart.Name = "udStart";
            this.udStart.Size = new System.Drawing.Size(56, 23);
            this.udStart.TabIndex = 48;
            this.udStart.Value = new decimal(new int[] {
            2020,
            0,
            0,
            0});
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.chkIntegral);
            this.panel1.Controls.Add(this.chkLogY);
            this.panel1.Location = new System.Drawing.Point(653, 446);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(255, 61);
            this.panel1.TabIndex = 58;
            // 
            // chkIntegral
            // 
            this.chkIntegral.AutoSize = true;
            this.chkIntegral.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkIntegral.Location = new System.Drawing.Point(3, 36);
            this.chkIntegral.Name = "chkIntegral";
            this.chkIntegral.Size = new System.Drawing.Size(123, 20);
            this.chkIntegral.TabIndex = 57;
            this.chkIntegral.Text = "Integral Spect";
            this.chkIntegral.UseVisualStyleBackColor = true;
            this.chkIntegral.CheckedChanged += new System.EventHandler(this.ChkIntegral_CheckedChanged);
            // 
            // chkLogY
            // 
            this.chkLogY.AutoSize = true;
            this.chkLogY.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkLogY.Location = new System.Drawing.Point(3, 4);
            this.chkLogY.Name = "chkLogY";
            this.chkLogY.Size = new System.Drawing.Size(63, 20);
            this.chkLogY.TabIndex = 55;
            this.chkLogY.Text = "LogY";
            this.chkLogY.UseVisualStyleBackColor = true;
            this.chkLogY.CheckedChanged += new System.EventHandler(this.ChkLogY_CheckedChanged);
            // 
            // groupBox7
            // 
            this.groupBox7.Controls.Add(this.btnBiasWRITEALL);
            this.groupBox7.Controls.Add(this.txtCMB_Temp4);
            this.groupBox7.Controls.Add(this.txtCMB_Temp3);
            this.groupBox7.Controls.Add(this.txtCMB_Temp2);
            this.groupBox7.Controls.Add(this.txtCMB_Temp1);
            this.groupBox7.Controls.Add(this.label9);
            this.groupBox7.Controls.Add(this.txtI);
            this.groupBox7.Controls.Add(this.txtV);
            this.groupBox7.Controls.Add(this.btnBiasREAD);
            this.groupBox7.Controls.Add(this.btnBiasWRITE);
            this.groupBox7.Controls.Add(this.lblI);
            this.groupBox7.Controls.Add(this.lblV);
            this.groupBox7.Location = new System.Drawing.Point(8, 472);
            this.groupBox7.Name = "groupBox7";
            this.groupBox7.Size = new System.Drawing.Size(378, 87);
            this.groupBox7.TabIndex = 5;
            this.groupBox7.TabStop = false;
            this.groupBox7.Text = "BIAS";
            // 
            // btnBiasWRITEALL
            // 
            this.btnBiasWRITEALL.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnBiasWRITEALL.Location = new System.Drawing.Point(259, 28);
            this.btnBiasWRITEALL.Name = "btnBiasWRITEALL";
            this.btnBiasWRITEALL.Size = new System.Drawing.Size(75, 23);
            this.btnBiasWRITEALL.TabIndex = 70;
            this.btnBiasWRITEALL.Text = "WRITE ALL";
            this.btnBiasWRITEALL.UseVisualStyleBackColor = true;
            this.btnBiasWRITEALL.Click += new System.EventHandler(this.BtnBiasWRITEALL_Click);
            // 
            // txtCMB_Temp4
            // 
            this.txtCMB_Temp4.Enabled = false;
            this.txtCMB_Temp4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.830189F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtCMB_Temp4.Location = new System.Drawing.Point(318, 56);
            this.txtCMB_Temp4.Name = "txtCMB_Temp4";
            this.txtCMB_Temp4.Size = new System.Drawing.Size(47, 21);
            this.txtCMB_Temp4.TabIndex = 69;
            this.txtCMB_Temp4.Text = "0.000";
            // 
            // txtCMB_Temp3
            // 
            this.txtCMB_Temp3.Enabled = false;
            this.txtCMB_Temp3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.830189F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtCMB_Temp3.Location = new System.Drawing.Point(265, 56);
            this.txtCMB_Temp3.Name = "txtCMB_Temp3";
            this.txtCMB_Temp3.Size = new System.Drawing.Size(47, 21);
            this.txtCMB_Temp3.TabIndex = 68;
            this.txtCMB_Temp3.Text = "0.000";
            // 
            // txtCMB_Temp2
            // 
            this.txtCMB_Temp2.Enabled = false;
            this.txtCMB_Temp2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.830189F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtCMB_Temp2.Location = new System.Drawing.Point(212, 56);
            this.txtCMB_Temp2.Name = "txtCMB_Temp2";
            this.txtCMB_Temp2.Size = new System.Drawing.Size(47, 21);
            this.txtCMB_Temp2.TabIndex = 67;
            this.txtCMB_Temp2.Text = "0.000";
            // 
            // txtCMB_Temp1
            // 
            this.txtCMB_Temp1.Enabled = false;
            this.txtCMB_Temp1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.830189F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtCMB_Temp1.Location = new System.Drawing.Point(159, 56);
            this.txtCMB_Temp1.Name = "txtCMB_Temp1";
            this.txtCMB_Temp1.Size = new System.Drawing.Size(47, 21);
            this.txtCMB_Temp1.TabIndex = 66;
            this.txtCMB_Temp1.Text = "0.000";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(99, 58);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(54, 17);
            this.label9.TabIndex = 65;
            this.label9.Text = "TEMP=";
            // 
            // txtI
            // 
            this.txtI.Enabled = false;
            this.txtI.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.830189F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtI.Location = new System.Drawing.Point(32, 56);
            this.txtI.Name = "txtI";
            this.txtI.Size = new System.Drawing.Size(55, 21);
            this.txtI.TabIndex = 62;
            this.txtI.Text = "0.000";
            // 
            // txtV
            // 
            this.txtV.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.830189F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtV.Location = new System.Drawing.Point(32, 29);
            this.txtV.Name = "txtV";
            this.txtV.Size = new System.Drawing.Size(55, 21);
            this.txtV.TabIndex = 61;
            this.txtV.Text = "0.000";
            // 
            // btnBiasREAD
            // 
            this.btnBiasREAD.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.830189F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnBiasREAD.Location = new System.Drawing.Point(97, 28);
            this.btnBiasREAD.Name = "btnBiasREAD";
            this.btnBiasREAD.Size = new System.Drawing.Size(75, 23);
            this.btnBiasREAD.TabIndex = 16;
            this.btnBiasREAD.Text = "READ";
            this.btnBiasREAD.UseVisualStyleBackColor = true;
            this.btnBiasREAD.Click += new System.EventHandler(this.BtnBiasREAD_Click);
            // 
            // btnBiasWRITE
            // 
            this.btnBiasWRITE.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.830189F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnBiasWRITE.Location = new System.Drawing.Point(178, 28);
            this.btnBiasWRITE.Name = "btnBiasWRITE";
            this.btnBiasWRITE.Size = new System.Drawing.Size(75, 23);
            this.btnBiasWRITE.TabIndex = 16;
            this.btnBiasWRITE.Text = "WRITE";
            this.btnBiasWRITE.UseVisualStyleBackColor = true;
            this.btnBiasWRITE.Click += new System.EventHandler(this.BtnBiasWRITE_Click);
            // 
            // lblI
            // 
            this.lblI.AutoSize = true;
            this.lblI.Location = new System.Drawing.Point(10, 58);
            this.lblI.Name = "lblI";
            this.lblI.Size = new System.Drawing.Size(19, 17);
            this.lblI.TabIndex = 5;
            this.lblI.Text = "I=";
            // 
            // lblV
            // 
            this.lblV.AutoSize = true;
            this.lblV.Location = new System.Drawing.Point(7, 31);
            this.lblV.Name = "lblV";
            this.lblV.Size = new System.Drawing.Size(25, 17);
            this.lblV.TabIndex = 4;
            this.lblV.Text = "V=";
            // 
            // tabWC
            // 
            this.tabWC.Controls.Add(this.groupBox6);
            this.tabWC.Location = new System.Drawing.Point(4, 29);
            this.tabWC.Name = "tabWC";
            this.tabWC.Size = new System.Drawing.Size(1255, 653);
            this.tabWC.TabIndex = 6;
            this.tabWC.Text = "WC";
            this.tabWC.UseVisualStyleBackColor = true;
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.lblWCmessage);
            this.groupBox6.Controls.Add(this.btnWC);
            this.groupBox6.Location = new System.Drawing.Point(3, 3);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(611, 130);
            this.groupBox6.TabIndex = 4;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "CONNECTIONS";
            // 
            // lblWCmessage
            // 
            this.lblWCmessage.AutoSize = true;
            this.lblWCmessage.Location = new System.Drawing.Point(-1, 87);
            this.lblWCmessage.Name = "lblWCmessage";
            this.lblWCmessage.Size = new System.Drawing.Size(46, 34);
            this.lblWCmessage.TabIndex = 2;
            this.lblWCmessage.Text = "label1\r\nlabel2";
            // 
            // btnWC
            // 
            this.btnWC.Location = new System.Drawing.Point(0, 23);
            this.btnWC.Name = "btnWC";
            this.btnWC.Size = new System.Drawing.Size(99, 42);
            this.btnWC.TabIndex = 0;
            this.btnWC.Tag = "";
            this.btnWC.Text = "WC";
            this.btnWC.UseVisualStyleBackColor = true;
            // 
            // tabQC
            // 
            this.tabQC.Controls.Add(this.lightCheckGroup);
            this.tabQC.Controls.Add(this.dicounterQCGroup);
            this.tabQC.Controls.Add(this.qaBiasLbl);
            this.tabQC.Controls.Add(this.qcBias);
            this.tabQC.Location = new System.Drawing.Point(4, 29);
            this.tabQC.Name = "tabQC";
            this.tabQC.Size = new System.Drawing.Size(1255, 653);
            this.tabQC.TabIndex = 0;
            this.tabQC.Text = "QC";
            this.tabQC.UseVisualStyleBackColor = true;
            // 
            // lightCheckGroup
            // 
            this.lightCheckGroup.Controls.Add(this.LightCheckModuleHalf);
            this.lightCheckGroup.Controls.Add(this.LightCheckTypeLbl);
            this.lightCheckGroup.Controls.Add(this.LightCheckType);
            this.lightCheckGroup.Controls.Add(this.lightNumChecks);
            this.lightCheckGroup.Controls.Add(this.lightModuleSideLabel);
            this.lightCheckGroup.Controls.Add(this.LightCheckModuleHalfLbl);
            this.lightCheckGroup.Controls.Add(this.autoThreshBtn);
            this.lightCheckGroup.Controls.Add(this.lightModuleSide);
            this.lightCheckGroup.Controls.Add(this.lightModuleLabelLabel);
            this.lightCheckGroup.Controls.Add(this.lightModuleLabel);
            this.lightCheckGroup.Controls.Add(this.lightWriteToFileBox);
            this.lightCheckGroup.Controls.Add(this.lightCheckFPGApanel);
            this.lightCheckGroup.Controls.Add(this.label16);
            this.lightCheckGroup.Controls.Add(this.lightCheckResetThresh);
            this.lightCheckGroup.Controls.Add(this.lightCheckProgress);
            this.lightCheckGroup.Controls.Add(this.lightCheckChanThreshBtn);
            this.lightCheckGroup.Controls.Add(this.lightCheckChanThreshLbl);
            this.lightCheckGroup.Controls.Add(this.lightCheckChanThresh);
            this.lightCheckGroup.Controls.Add(this.lightCheckChanSelecLbl);
            this.lightCheckGroup.Controls.Add(this.lightCheckChanSelec);
            this.lightCheckGroup.Controls.Add(this.globalThreshChkBox);
            this.lightCheckGroup.Controls.Add(this.lightGlobalThreshLbl);
            this.lightCheckGroup.Controls.Add(this.lightGlobalThresh);
            this.lightCheckGroup.Controls.Add(this.lightCheckBtn);
            this.lightCheckGroup.Location = new System.Drawing.Point(21, 177);
            this.lightCheckGroup.Name = "lightCheckGroup";
            this.lightCheckGroup.Size = new System.Drawing.Size(1135, 474);
            this.lightCheckGroup.TabIndex = 135;
            this.lightCheckGroup.TabStop = false;
            this.lightCheckGroup.Text = "Light Check";
            // 
            // LightCheckModuleHalf
            // 
            this.LightCheckModuleHalf.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.LightCheckModuleHalf.Enabled = false;
            this.LightCheckModuleHalf.FormattingEnabled = true;
            this.LightCheckModuleHalf.Items.AddRange(new object[] {
            "TOP",
            "BOTTOM"});
            this.LightCheckModuleHalf.Location = new System.Drawing.Point(471, 36);
            this.LightCheckModuleHalf.Name = "LightCheckModuleHalf";
            this.LightCheckModuleHalf.Size = new System.Drawing.Size(93, 25);
            this.LightCheckModuleHalf.TabIndex = 203;
            // 
            // LightCheckTypeLbl
            // 
            this.LightCheckTypeLbl.AutoSize = true;
            this.LightCheckTypeLbl.Location = new System.Drawing.Point(584, 16);
            this.LightCheckTypeLbl.Name = "LightCheckTypeLbl";
            this.LightCheckTypeLbl.Size = new System.Drawing.Size(72, 17);
            this.LightCheckTypeLbl.TabIndex = 202;
            this.LightCheckTypeLbl.Text = "Test Type";
            // 
            // LightCheckType
            // 
            this.LightCheckType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.LightCheckType.Enabled = false;
            this.LightCheckType.FormattingEnabled = true;
            this.LightCheckType.Items.AddRange(new object[] {
            "DARK",
            "LIGHT"});
            this.LightCheckType.Location = new System.Drawing.Point(584, 36);
            this.LightCheckType.Name = "LightCheckType";
            this.LightCheckType.Size = new System.Drawing.Size(72, 25);
            this.LightCheckType.TabIndex = 201;
            // 
            // lightNumChecks
            // 
            this.lightNumChecks.Location = new System.Drawing.Point(108, 111);
            this.lightNumChecks.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.lightNumChecks.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.lightNumChecks.Name = "lightNumChecks";
            this.lightNumChecks.Size = new System.Drawing.Size(63, 23);
            this.lightNumChecks.TabIndex = 200;
            this.lightNumChecks.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.lightNumChecks.UpDownAlign = System.Windows.Forms.LeftRightAlignment.Left;
            this.lightNumChecks.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.lightNumChecks.Visible = false;
            // 
            // lightModuleSideLabel
            // 
            this.lightModuleSideLabel.AutoSize = true;
            this.lightModuleSideLabel.Location = new System.Drawing.Point(402, 16);
            this.lightModuleSideLabel.Name = "lightModuleSideLabel";
            this.lightModuleSideLabel.Size = new System.Drawing.Size(36, 17);
            this.lightModuleSideLabel.TabIndex = 199;
            this.lightModuleSideLabel.Text = "Side";
            // 
            // LightCheckModuleHalfLbl
            // 
            this.LightCheckModuleHalfLbl.AutoSize = true;
            this.LightCheckModuleHalfLbl.Location = new System.Drawing.Point(471, 16);
            this.LightCheckModuleHalfLbl.Name = "LightCheckModuleHalfLbl";
            this.LightCheckModuleHalfLbl.Size = new System.Drawing.Size(83, 17);
            this.LightCheckModuleHalfLbl.TabIndex = 198;
            this.LightCheckModuleHalfLbl.Text = "Module Half";
            // 
            // autoThreshBtn
            // 
            this.autoThreshBtn.Location = new System.Drawing.Point(89, 212);
            this.autoThreshBtn.Name = "autoThreshBtn";
            this.autoThreshBtn.Size = new System.Drawing.Size(108, 34);
            this.autoThreshBtn.TabIndex = 172;
            this.autoThreshBtn.Text = "Auto Thresh";
            this.autoThreshBtn.UseVisualStyleBackColor = true;
            this.autoThreshBtn.Click += new System.EventHandler(this.AutoThreshBtn_Click);
            // 
            // lightModuleSide
            // 
            this.lightModuleSide.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.lightModuleSide.Enabled = false;
            this.lightModuleSide.FormattingEnabled = true;
            this.lightModuleSide.Items.AddRange(new object[] {
            "A",
            "B"});
            this.lightModuleSide.Location = new System.Drawing.Point(402, 36);
            this.lightModuleSide.Name = "lightModuleSide";
            this.lightModuleSide.Size = new System.Drawing.Size(49, 25);
            this.lightModuleSide.TabIndex = 196;
            // 
            // lightModuleLabelLabel
            // 
            this.lightModuleLabelLabel.AutoSize = true;
            this.lightModuleLabelLabel.Location = new System.Drawing.Point(287, 16);
            this.lightModuleLabelLabel.Name = "lightModuleLabelLabel";
            this.lightModuleLabelLabel.Size = new System.Drawing.Size(93, 17);
            this.lightModuleLabelLabel.TabIndex = 195;
            this.lightModuleLabelLabel.Text = "Module Label";
            // 
            // lightModuleLabel
            // 
            this.lightModuleLabel.BackColor = System.Drawing.Color.Yellow;
            this.lightModuleLabel.Location = new System.Drawing.Point(287, 38);
            this.lightModuleLabel.Name = "lightModuleLabel";
            this.lightModuleLabel.Size = new System.Drawing.Size(95, 23);
            this.lightModuleLabel.TabIndex = 194;
            this.lightModuleLabel.TextChanged += new System.EventHandler(this.LightModuleLabel_TextChanged);
            // 
            // lightWriteToFileBox
            // 
            this.lightWriteToFileBox.AutoSize = true;
            this.lightWriteToFileBox.CheckAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.lightWriteToFileBox.Enabled = false;
            this.lightWriteToFileBox.Location = new System.Drawing.Point(676, 22);
            this.lightWriteToFileBox.Name = "lightWriteToFileBox";
            this.lightWriteToFileBox.Size = new System.Drawing.Size(87, 35);
            this.lightWriteToFileBox.TabIndex = 193;
            this.lightWriteToFileBox.Text = "Write to File";
            this.lightWriteToFileBox.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.lightWriteToFileBox.UseVisualStyleBackColor = true;
            this.lightWriteToFileBox.CheckedChanged += new System.EventHandler(this.LightWriteToFileBox_CheckedChanged);
            // 
            // lightCheckFPGApanel
            // 
            this.lightCheckFPGApanel.ColumnCount = 2;
            this.lightCheckFPGApanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.lightCheckFPGApanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.lightCheckFPGApanel.Location = new System.Drawing.Point(286, 71);
            this.lightCheckFPGApanel.Name = "lightCheckFPGApanel";
            this.lightCheckFPGApanel.RowCount = 2;
            this.lightCheckFPGApanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.lightCheckFPGApanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.lightCheckFPGApanel.Size = new System.Drawing.Size(843, 384);
            this.lightCheckFPGApanel.TabIndex = 192;
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(43, 91);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(168, 17);
            this.label16.TabIndex = 191;
            this.label16.Text = "Number of times to check";
            this.label16.Visible = false;
            // 
            // lightCheckResetThresh
            // 
            this.lightCheckResetThresh.Location = new System.Drawing.Point(116, 252);
            this.lightCheckResetThresh.Name = "lightCheckResetThresh";
            this.lightCheckResetThresh.Size = new System.Drawing.Size(55, 34);
            this.lightCheckResetThresh.TabIndex = 181;
            this.lightCheckResetThresh.Text = "Reset";
            this.lightCheckResetThresh.UseVisualStyleBackColor = true;
            this.lightCheckResetThresh.Click += new System.EventHandler(this.LightCheckResetThresh_Click);
            // 
            // lightCheckProgress
            // 
            this.lightCheckProgress.Location = new System.Drawing.Point(24, 301);
            this.lightCheckProgress.Maximum = 64;
            this.lightCheckProgress.Name = "lightCheckProgress";
            this.lightCheckProgress.Size = new System.Drawing.Size(239, 23);
            this.lightCheckProgress.Step = 1;
            this.lightCheckProgress.TabIndex = 180;
            // 
            // lightCheckChanThreshBtn
            // 
            this.lightCheckChanThreshBtn.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lightCheckChanThreshBtn.Location = new System.Drawing.Point(199, 167);
            this.lightCheckChanThreshBtn.Name = "lightCheckChanThreshBtn";
            this.lightCheckChanThreshBtn.Size = new System.Drawing.Size(24, 23);
            this.lightCheckChanThreshBtn.TabIndex = 177;
            this.lightCheckChanThreshBtn.Text = "W";
            this.lightCheckChanThreshBtn.UseVisualStyleBackColor = true;
            this.lightCheckChanThreshBtn.Click += new System.EventHandler(this.LightCheckChanThreshBtn_Click);
            // 
            // lightCheckChanThreshLbl
            // 
            this.lightCheckChanThreshLbl.AutoSize = true;
            this.lightCheckChanThreshLbl.Location = new System.Drawing.Point(113, 148);
            this.lightCheckChanThreshLbl.Name = "lightCheckChanThreshLbl";
            this.lightCheckChanThreshLbl.Size = new System.Drawing.Size(53, 17);
            this.lightCheckChanThreshLbl.TabIndex = 176;
            this.lightCheckChanThreshLbl.Text = "Thresh";
            // 
            // lightCheckChanThresh
            // 
            this.lightCheckChanThresh.Location = new System.Drawing.Point(112, 167);
            this.lightCheckChanThresh.Name = "lightCheckChanThresh";
            this.lightCheckChanThresh.Size = new System.Drawing.Size(79, 23);
            this.lightCheckChanThresh.TabIndex = 175;
            this.lightCheckChanThresh.Text = "0.2500";
            // 
            // lightCheckChanSelecLbl
            // 
            this.lightCheckChanSelecLbl.AutoSize = true;
            this.lightCheckChanSelecLbl.Location = new System.Drawing.Point(48, 148);
            this.lightCheckChanSelecLbl.Name = "lightCheckChanSelecLbl";
            this.lightCheckChanSelecLbl.Size = new System.Drawing.Size(41, 17);
            this.lightCheckChanSelecLbl.TabIndex = 174;
            this.lightCheckChanSelecLbl.Text = "Chan";
            // 
            // lightCheckChanSelec
            // 
            this.lightCheckChanSelec.Location = new System.Drawing.Point(48, 167);
            this.lightCheckChanSelec.Maximum = new decimal(new int[] {
            63,
            0,
            0,
            0});
            this.lightCheckChanSelec.Name = "lightCheckChanSelec";
            this.lightCheckChanSelec.Size = new System.Drawing.Size(56, 23);
            this.lightCheckChanSelec.TabIndex = 173;
            this.lightCheckChanSelec.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.lightCheckChanSelec.UpDownAlign = System.Windows.Forms.LeftRightAlignment.Left;
            this.lightCheckChanSelec.ValueChanged += new System.EventHandler(this.LightCheckChanSelec_ValueChanged);
            // 
            // globalThreshChkBox
            // 
            this.globalThreshChkBox.AutoSize = true;
            this.globalThreshChkBox.Location = new System.Drawing.Point(109, 57);
            this.globalThreshChkBox.Name = "globalThreshChkBox";
            this.globalThreshChkBox.Size = new System.Drawing.Size(154, 21);
            this.globalThreshChkBox.TabIndex = 171;
            this.globalThreshChkBox.Text = "Use Global Thresh?";
            this.globalThreshChkBox.UseVisualStyleBackColor = true;
            // 
            // lightGlobalThreshLbl
            // 
            this.lightGlobalThreshLbl.AutoSize = true;
            this.lightGlobalThreshLbl.Location = new System.Drawing.Point(25, 37);
            this.lightGlobalThreshLbl.Name = "lightGlobalThreshLbl";
            this.lightGlobalThreshLbl.Size = new System.Drawing.Size(117, 17);
            this.lightGlobalThreshLbl.TabIndex = 169;
            this.lightGlobalThreshLbl.Text = "Global Threshold";
            // 
            // lightGlobalThresh
            // 
            this.lightGlobalThresh.Location = new System.Drawing.Point(31, 56);
            this.lightGlobalThresh.Name = "lightGlobalThresh";
            this.lightGlobalThresh.Size = new System.Drawing.Size(61, 23);
            this.lightGlobalThresh.TabIndex = 167;
            this.lightGlobalThresh.Text = "0.25";
            this.lightGlobalThresh.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.lightGlobalThresh.TextChanged += new System.EventHandler(this.LightGlobalThresh_TextChanged);
            // 
            // lightCheckBtn
            // 
            this.lightCheckBtn.Location = new System.Drawing.Point(775, 27);
            this.lightCheckBtn.Name = "lightCheckBtn";
            this.lightCheckBtn.Size = new System.Drawing.Size(130, 35);
            this.lightCheckBtn.TabIndex = 166;
            this.lightCheckBtn.Text = "Light Check";
            this.lightCheckBtn.UseVisualStyleBackColor = true;
            this.lightCheckBtn.Click += new System.EventHandler(this.LightCheckBtn_Click);
            // 
            // dicounterQCGroup
            // 
            this.dicounterQCGroup.Controls.Add(this.qcOutputFileName);
            this.dicounterQCGroup.Controls.Add(this.qaOutputFileNameLabel);
            this.dicounterQCGroup.Controls.Add(this.oneReadout);
            this.dicounterQCGroup.Controls.Add(this.qaDiNumAvgLabel);
            this.dicounterQCGroup.Controls.Add(this.qaDiIWarningThreshLabel);
            this.dicounterQCGroup.Controls.Add(this.qaDiNumAvg);
            this.dicounterQCGroup.Controls.Add(this.qcDiIWarningThresh);
            this.dicounterQCGroup.Controls.Add(this.autoDataProgress);
            this.dicounterQCGroup.Controls.Add(this.qcStartButton);
            this.dicounterQCGroup.Controls.Add(this.numLabel);
            this.dicounterQCGroup.Controls.Add(this.dicounterNumberTextBox);
            this.dicounterQCGroup.Location = new System.Drawing.Point(21, 60);
            this.dicounterQCGroup.Name = "dicounterQCGroup";
            this.dicounterQCGroup.Size = new System.Drawing.Size(950, 100);
            this.dicounterQCGroup.TabIndex = 134;
            this.dicounterQCGroup.TabStop = false;
            this.dicounterQCGroup.Text = "DiCounter QC";
            // 
            // qcOutputFileName
            // 
            this.qcOutputFileName.Location = new System.Drawing.Point(759, 60);
            this.qcOutputFileName.Name = "qcOutputFileName";
            this.qcOutputFileName.Size = new System.Drawing.Size(112, 23);
            this.qcOutputFileName.TabIndex = 119;
            // 
            // qaOutputFileNameLabel
            // 
            this.qaOutputFileNameLabel.AutoSize = true;
            this.qaOutputFileNameLabel.Location = new System.Drawing.Point(532, 64);
            this.qaOutputFileNameLabel.Name = "qaOutputFileNameLabel";
            this.qaOutputFileNameLabel.Size = new System.Drawing.Size(371, 17);
            this.qaOutputFileNameLabel.TabIndex = 118;
            this.qaOutputFileNameLabel.Text = "Output File Name: \"ScanningData_                              .txt\"\r\n";
            // 
            // oneReadout
            // 
            this.oneReadout.AutoSize = true;
            this.oneReadout.Location = new System.Drawing.Point(535, 30);
            this.oneReadout.Name = "oneReadout";
            this.oneReadout.Size = new System.Drawing.Size(153, 21);
            this.oneReadout.TabIndex = 117;
            this.oneReadout.Text = "First Readout Only?";
            this.oneReadout.UseVisualStyleBackColor = true;
            this.oneReadout.CheckedChanged += new System.EventHandler(this.OneReadout_CheckedChanged);
            // 
            // qaDiNumAvgLabel
            // 
            this.qaDiNumAvgLabel.AutoSize = true;
            this.qaDiNumAvgLabel.Enabled = false;
            this.qaDiNumAvgLabel.Location = new System.Drawing.Point(253, 30);
            this.qaDiNumAvgLabel.Name = "qaDiNumAvgLabel";
            this.qaDiNumAvgLabel.Size = new System.Drawing.Size(61, 17);
            this.qaDiNumAvgLabel.TabIndex = 114;
            this.qaDiNumAvgLabel.Text = "NumAvg";
            // 
            // qaDiIWarningThreshLabel
            // 
            this.qaDiIWarningThreshLabel.AutoSize = true;
            this.qaDiIWarningThreshLabel.Location = new System.Drawing.Point(320, 31);
            this.qaDiIWarningThreshLabel.Name = "qaDiIWarningThreshLabel";
            this.qaDiIWarningThreshLabel.Size = new System.Drawing.Size(59, 17);
            this.qaDiIWarningThreshLabel.TabIndex = 105;
            this.qaDiIWarningThreshLabel.Text = "I_thresh";
            // 
            // qaDiNumAvg
            // 
            this.qaDiNumAvg.Enabled = false;
            this.qaDiNumAvg.Location = new System.Drawing.Point(254, 53);
            this.qaDiNumAvg.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.qaDiNumAvg.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.qaDiNumAvg.Name = "qaDiNumAvg";
            this.qaDiNumAvg.Size = new System.Drawing.Size(57, 23);
            this.qaDiNumAvg.TabIndex = 115;
            this.qaDiNumAvg.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.qaDiNumAvg.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // qcDiIWarningThresh
            // 
            this.qcDiIWarningThresh.Location = new System.Drawing.Point(317, 53);
            this.qcDiIWarningThresh.Name = "qcDiIWarningThresh";
            this.qcDiIWarningThresh.Size = new System.Drawing.Size(62, 23);
            this.qcDiIWarningThresh.TabIndex = 104;
            this.qcDiIWarningThresh.Text = "0.1";
            this.qcDiIWarningThresh.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.qcDiIWarningThresh.TextChanged += new System.EventHandler(this.QcDiIWarningThresh_TextChanged);
            // 
            // autoDataProgress
            // 
            this.autoDataProgress.Location = new System.Drawing.Point(141, 53);
            this.autoDataProgress.Maximum = 7;
            this.autoDataProgress.Name = "autoDataProgress";
            this.autoDataProgress.Size = new System.Drawing.Size(100, 23);
            this.autoDataProgress.Step = 1;
            this.autoDataProgress.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.autoDataProgress.TabIndex = 103;
            // 
            // qcStartButton
            // 
            this.qcStartButton.Location = new System.Drawing.Point(142, 31);
            this.qcStartButton.Name = "qcStartButton";
            this.qcStartButton.Size = new System.Drawing.Size(99, 23);
            this.qcStartButton.TabIndex = 100;
            this.qcStartButton.Text = "Auto Data";
            this.qcStartButton.UseVisualStyleBackColor = true;
            this.qcStartButton.Click += new System.EventHandler(this.QcStartButton_Click);
            // 
            // numLabel
            // 
            this.numLabel.AutoSize = true;
            this.numLabel.Location = new System.Drawing.Point(13, 32);
            this.numLabel.Name = "numLabel";
            this.numLabel.Size = new System.Drawing.Size(123, 17);
            this.numLabel.TabIndex = 99;
            this.numLabel.Text = "Dicounter Number";
            // 
            // dicounterNumberTextBox
            // 
            this.dicounterNumberTextBox.Location = new System.Drawing.Point(16, 53);
            this.dicounterNumberTextBox.Name = "dicounterNumberTextBox";
            this.dicounterNumberTextBox.Size = new System.Drawing.Size(112, 23);
            this.dicounterNumberTextBox.TabIndex = 98;
            // 
            // qaBiasLbl
            // 
            this.qaBiasLbl.AutoSize = true;
            this.qaBiasLbl.Font = new System.Drawing.Font("Consolas", 15.75F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.qaBiasLbl.Location = new System.Drawing.Point(34, 18);
            this.qaBiasLbl.Name = "qaBiasLbl";
            this.qaBiasLbl.Size = new System.Drawing.Size(82, 24);
            this.qaBiasLbl.TabIndex = 132;
            this.qaBiasLbl.Text = "Bias V";
            // 
            // qcBias
            // 
            this.qcBias.BackColor = System.Drawing.Color.LightGray;
            this.qcBias.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.qcBias.Location = new System.Drawing.Point(133, 21);
            this.qcBias.Name = "qcBias";
            this.qcBias.Size = new System.Drawing.Size(79, 23);
            this.qcBias.TabIndex = 131;
            this.qcBias.Text = "57.0";
            this.qcBias.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.qcBias.TextChanged += new System.EventHandler(this.QcBias_TextChanged);
            // 
            // tabCMBTester
            // 
            this.tabCMBTester.Controls.Add(this.cmbDataGroup);
            this.tabCMBTester.Controls.Add(this.cmbTestControl);
            this.tabCMBTester.Location = new System.Drawing.Point(4, 29);
            this.tabCMBTester.Name = "tabCMBTester";
            this.tabCMBTester.Padding = new System.Windows.Forms.Padding(3);
            this.tabCMBTester.Size = new System.Drawing.Size(1255, 653);
            this.tabCMBTester.TabIndex = 8;
            this.tabCMBTester.Text = "CMB Tester";
            this.tabCMBTester.UseVisualStyleBackColor = true;
            // 
            // cmbDataGroup
            // 
            this.cmbDataGroup.Controls.Add(this.cmbDataTable);
            this.cmbDataGroup.Location = new System.Drawing.Point(16, 219);
            this.cmbDataGroup.Name = "cmbDataGroup";
            this.cmbDataGroup.Size = new System.Drawing.Size(1223, 410);
            this.cmbDataGroup.TabIndex = 1;
            this.cmbDataGroup.TabStop = false;
            this.cmbDataGroup.Text = "CMB Info";
            // 
            // cmbDataTable
            // 
            this.cmbDataTable.ColumnCount = 12;
            this.cmbDataTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 8.333333F));
            this.cmbDataTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 8.333333F));
            this.cmbDataTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 8.333333F));
            this.cmbDataTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 8.333333F));
            this.cmbDataTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 8.333333F));
            this.cmbDataTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 8.333333F));
            this.cmbDataTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 8.333333F));
            this.cmbDataTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 8.333333F));
            this.cmbDataTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 8.333333F));
            this.cmbDataTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 8.333333F));
            this.cmbDataTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 8.333333F));
            this.cmbDataTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 8.333333F));
            this.cmbDataTable.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cmbDataTable.Location = new System.Drawing.Point(3, 19);
            this.cmbDataTable.Margin = new System.Windows.Forms.Padding(0);
            this.cmbDataTable.Name = "cmbDataTable";
            this.cmbDataTable.RowCount = 16;
            this.cmbDataTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 6.25F));
            this.cmbDataTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 6.25F));
            this.cmbDataTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 6.25F));
            this.cmbDataTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 6.25F));
            this.cmbDataTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 6.25F));
            this.cmbDataTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 6.25F));
            this.cmbDataTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 6.25F));
            this.cmbDataTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 6.25F));
            this.cmbDataTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 6.25F));
            this.cmbDataTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 6.25F));
            this.cmbDataTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 6.25F));
            this.cmbDataTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 6.25F));
            this.cmbDataTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 6.25F));
            this.cmbDataTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 6.25F));
            this.cmbDataTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 6.25F));
            this.cmbDataTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 6.25F));
            this.cmbDataTable.Size = new System.Drawing.Size(1217, 388);
            this.cmbDataTable.TabIndex = 0;
            // 
            // cmbTestControl
            // 
            this.cmbTestControl.Controls.Add(this.cmbTesterProgresBar);
            this.cmbTestControl.Controls.Add(this.lostCMBavgsBtn);
            this.cmbTestControl.Controls.Add(this.updateFilesChkBox);
            this.cmbTestControl.Controls.Add(this.cmbInfoBox);
            this.cmbTestControl.Controls.Add(this.requestNumTrigsLabel);
            this.cmbTestControl.Controls.Add(this.requestNumTrigs);
            this.cmbTestControl.Controls.Add(this.numTrigsDisp);
            this.cmbTestControl.Controls.Add(this.numTrigLabel);
            this.cmbTestControl.Controls.Add(this.sipmControl);
            this.cmbTestControl.Controls.Add(this.cmbTestBtn);
            this.cmbTestControl.Location = new System.Drawing.Point(16, 15);
            this.cmbTestControl.Name = "cmbTestControl";
            this.cmbTestControl.Size = new System.Drawing.Size(515, 184);
            this.cmbTestControl.TabIndex = 0;
            this.cmbTestControl.TabStop = false;
            this.cmbTestControl.Text = "CMB Test Control";
            // 
            // cmbTesterProgresBar
            // 
            this.cmbTesterProgresBar.Location = new System.Drawing.Point(33, 141);
            this.cmbTesterProgresBar.Maximum = 160;
            this.cmbTesterProgresBar.Name = "cmbTesterProgresBar";
            this.cmbTesterProgresBar.Size = new System.Drawing.Size(147, 23);
            this.cmbTesterProgresBar.Step = 8;
            this.cmbTesterProgresBar.TabIndex = 11;
            // 
            // lostCMBavgsBtn
            // 
            this.lostCMBavgsBtn.Location = new System.Drawing.Point(323, 19);
            this.lostCMBavgsBtn.Name = "lostCMBavgsBtn";
            this.lostCMBavgsBtn.Size = new System.Drawing.Size(75, 23);
            this.lostCMBavgsBtn.TabIndex = 10;
            this.lostCMBavgsBtn.Text = "Lost File";
            this.lostCMBavgsBtn.UseVisualStyleBackColor = true;
            this.lostCMBavgsBtn.Click += new System.EventHandler(this.LostCMBavgsBtn_Click);
            // 
            // updateFilesChkBox
            // 
            this.updateFilesChkBox.AutoSize = true;
            this.updateFilesChkBox.Location = new System.Drawing.Point(209, 21);
            this.updateFilesChkBox.Name = "updateFilesChkBox";
            this.updateFilesChkBox.Size = new System.Drawing.Size(114, 21);
            this.updateFilesChkBox.TabIndex = 9;
            this.updateFilesChkBox.Text = "Update Files?";
            this.updateFilesChkBox.UseVisualStyleBackColor = true;
            // 
            // cmbInfoBox
            // 
            this.cmbInfoBox.Location = new System.Drawing.Point(33, 108);
            this.cmbInfoBox.Name = "cmbInfoBox";
            this.cmbInfoBox.ReadOnly = true;
            this.cmbInfoBox.Size = new System.Drawing.Size(147, 23);
            this.cmbInfoBox.TabIndex = 8;
            // 
            // requestNumTrigsLabel
            // 
            this.requestNumTrigsLabel.AutoSize = true;
            this.requestNumTrigsLabel.Location = new System.Drawing.Point(206, 120);
            this.requestNumTrigsLabel.Name = "requestNumTrigsLabel";
            this.requestNumTrigsLabel.Size = new System.Drawing.Size(113, 17);
            this.requestNumTrigsLabel.TabIndex = 7;
            this.requestNumTrigsLabel.Text = "Requested Trigs";
            this.requestNumTrigsLabel.Visible = false;
            // 
            // requestNumTrigs
            // 
            this.requestNumTrigs.Location = new System.Drawing.Point(325, 117);
            this.requestNumTrigs.Name = "requestNumTrigs";
            this.requestNumTrigs.Size = new System.Drawing.Size(73, 23);
            this.requestNumTrigs.TabIndex = 6;
            this.requestNumTrigs.Text = "100";
            this.requestNumTrigs.Visible = false;
            // 
            // numTrigsDisp
            // 
            this.numTrigsDisp.AutoSize = true;
            this.numTrigsDisp.Location = new System.Drawing.Point(325, 147);
            this.numTrigsDisp.Name = "numTrigsDisp";
            this.numTrigsDisp.Size = new System.Drawing.Size(16, 17);
            this.numTrigsDisp.TabIndex = 4;
            this.numTrigsDisp.Text = "0";
            this.numTrigsDisp.Visible = false;
            // 
            // numTrigLabel
            // 
            this.numTrigLabel.AutoSize = true;
            this.numTrigLabel.Location = new System.Drawing.Point(206, 147);
            this.numTrigLabel.Name = "numTrigLabel";
            this.numTrigLabel.Size = new System.Drawing.Size(73, 17);
            this.numTrigLabel.TabIndex = 3;
            this.numTrigLabel.Text = "Num Trigs";
            this.numTrigLabel.Visible = false;
            // 
            // sipmControl
            // 
            this.sipmControl.Controls.Add(this.cmbTest_ShortHelperBtn);
            this.sipmControl.Controls.Add(this.cmbBias);
            this.sipmControl.Controls.Add(this.cmbBiasOverride);
            this.sipmControl.Location = new System.Drawing.Point(209, 48);
            this.sipmControl.Name = "sipmControl";
            this.sipmControl.Size = new System.Drawing.Size(300, 55);
            this.sipmControl.TabIndex = 2;
            this.sipmControl.TabStop = false;
            this.sipmControl.Text = "SiPM Bias";
            // 
            // cmbBias
            // 
            this.cmbBias.Enabled = false;
            this.cmbBias.Location = new System.Drawing.Point(14, 22);
            this.cmbBias.Name = "cmbBias";
            this.cmbBias.Size = new System.Drawing.Size(65, 23);
            this.cmbBias.TabIndex = 1;
            this.cmbBias.Text = "53.7";
            this.cmbBias.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.cmbBias.TextChanged += new System.EventHandler(this.CmbBias_TextChanged);
            // 
            // cmbBiasOverride
            // 
            this.cmbBiasOverride.AutoSize = true;
            this.cmbBiasOverride.Location = new System.Drawing.Point(99, 24);
            this.cmbBiasOverride.Name = "cmbBiasOverride";
            this.cmbBiasOverride.Size = new System.Drawing.Size(82, 21);
            this.cmbBiasOverride.TabIndex = 0;
            this.cmbBiasOverride.Text = "Override";
            this.cmbBiasOverride.UseVisualStyleBackColor = true;
            this.cmbBiasOverride.CheckedChanged += new System.EventHandler(this.CmbBiasOverride_CheckedChanged);
            // 
            // cmbTestBtn
            // 
            this.cmbTestBtn.Location = new System.Drawing.Point(33, 43);
            this.cmbTestBtn.Name = "cmbTestBtn";
            this.cmbTestBtn.Size = new System.Drawing.Size(147, 59);
            this.cmbTestBtn.TabIndex = 0;
            this.cmbTestBtn.Text = "Test CMBs";
            this.cmbTestBtn.UseVisualStyleBackColor = true;
            this.cmbTestBtn.Click += new System.EventHandler(this.CmbTestBtn_Click);
            // 
            // tabModuleQC
            // 
            this.tabModuleQC.Controls.Add(this.ModuleQCModuleNameBox);
            this.tabModuleQC.Controls.Add(this.ModuleQAModuleNameLbl);
            this.tabModuleQC.Controls.Add(this.ModuleQA_OffsetLbl);
            this.tabModuleQC.Controls.Add(this.ModuleQC_Offset);
            this.tabModuleQC.Controls.Add(this.ModuleQC_flipped_Chkbox);
            this.tabModuleQC.Controls.Add(this.ModuleQCHomeResetBtn);
            this.tabModuleQC.Controls.Add(this.ModuleQCDarkCurrentBtn);
            this.tabModuleQC.Controls.Add(this.ModuleQAHaltBtn);
            this.tabModuleQC.Controls.Add(this.ComPortStatusLbl);
            this.tabModuleQC.Controls.Add(this.ComPortStatusBox);
            this.tabModuleQC.Controls.Add(this.ComPortRefresh);
            this.tabModuleQC.Controls.Add(this.ComPortDisconnectBtn);
            this.tabModuleQC.Controls.Add(this.comPortConnectBtn);
            this.tabModuleQC.Controls.Add(this.ComPortLbl);
            this.tabModuleQC.Controls.Add(this.comPortBox);
            this.tabModuleQC.Controls.Add(this.ModuleQCFilenameBox);
            this.tabModuleQC.Controls.Add(this.ModuleQAFileLbl);
            this.tabModuleQC.Controls.Add(this.ModuleQASideLbl);
            this.tabModuleQC.Controls.Add(this.ModuleQCSide);
            this.tabModuleQC.Controls.Add(this.ModuleQAFEB2Box);
            this.tabModuleQC.Controls.Add(this.ModuleQAFEB1Box);
            this.tabModuleQC.Controls.Add(this.ModuleQCBtn);
            this.tabModuleQC.Location = new System.Drawing.Point(4, 29);
            this.tabModuleQC.Name = "tabModuleQC";
            this.tabModuleQC.Padding = new System.Windows.Forms.Padding(3);
            this.tabModuleQC.Size = new System.Drawing.Size(1255, 653);
            this.tabModuleQC.TabIndex = 9;
            this.tabModuleQC.Text = "Module QC";
            this.tabModuleQC.UseVisualStyleBackColor = true;
            // 
            // ModuleQCModuleNameBox
            // 
            this.ModuleQCModuleNameBox.Location = new System.Drawing.Point(234, 35);
            this.ModuleQCModuleNameBox.Name = "ModuleQCModuleNameBox";
            this.ModuleQCModuleNameBox.Size = new System.Drawing.Size(100, 23);
            this.ModuleQCModuleNameBox.TabIndex = 23;
            // 
            // ModuleQAModuleNameLbl
            // 
            this.ModuleQAModuleNameLbl.AutoSize = true;
            this.ModuleQAModuleNameLbl.Location = new System.Drawing.Point(231, 17);
            this.ModuleQAModuleNameLbl.Name = "ModuleQAModuleNameLbl";
            this.ModuleQAModuleNameLbl.Size = new System.Drawing.Size(54, 17);
            this.ModuleQAModuleNameLbl.TabIndex = 22;
            this.ModuleQAModuleNameLbl.Text = "Module";
            // 
            // ModuleQA_OffsetLbl
            // 
            this.ModuleQA_OffsetLbl.AutoSize = true;
            this.ModuleQA_OffsetLbl.Location = new System.Drawing.Point(1074, 31);
            this.ModuleQA_OffsetLbl.Name = "ModuleQA_OffsetLbl";
            this.ModuleQA_OffsetLbl.Size = new System.Drawing.Size(100, 17);
            this.ModuleQA_OffsetLbl.TabIndex = 21;
            this.ModuleQA_OffsetLbl.Text = "Position Offset";
            // 
            // ModuleQC_Offset
            // 
            this.ModuleQC_Offset.Location = new System.Drawing.Point(1077, 55);
            this.ModuleQC_Offset.Maximum = new decimal(new int[] {
            53,
            0,
            0,
            0});
            this.ModuleQC_Offset.Name = "ModuleQC_Offset";
            this.ModuleQC_Offset.Size = new System.Drawing.Size(54, 23);
            this.ModuleQC_Offset.TabIndex = 20;
            // 
            // ModuleQC_flipped_Chkbox
            // 
            this.ModuleQC_flipped_Chkbox.AutoSize = true;
            this.ModuleQC_flipped_Chkbox.Location = new System.Drawing.Point(361, 66);
            this.ModuleQC_flipped_Chkbox.Name = "ModuleQC_flipped_Chkbox";
            this.ModuleQC_flipped_Chkbox.Size = new System.Drawing.Size(81, 21);
            this.ModuleQC_flipped_Chkbox.TabIndex = 19;
            this.ModuleQC_flipped_Chkbox.Text = "Flipped?";
            this.ModuleQC_flipped_Chkbox.UseVisualStyleBackColor = true;
            // 
            // ModuleQCHomeResetBtn
            // 
            this.ModuleQCHomeResetBtn.Enabled = false;
            this.ModuleQCHomeResetBtn.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ModuleQCHomeResetBtn.Location = new System.Drawing.Point(126, 45);
            this.ModuleQCHomeResetBtn.Name = "ModuleQCHomeResetBtn";
            this.ModuleQCHomeResetBtn.Size = new System.Drawing.Size(79, 29);
            this.ModuleQCHomeResetBtn.TabIndex = 18;
            this.ModuleQCHomeResetBtn.Text = "Home/Reset";
            this.ModuleQCHomeResetBtn.UseVisualStyleBackColor = true;
            this.ModuleQCHomeResetBtn.Click += new System.EventHandler(this.ModuleQCHomeResetBtn_Click);
            // 
            // ModuleQCDarkCurrentBtn
            // 
            this.ModuleQCDarkCurrentBtn.Location = new System.Drawing.Point(17, 16);
            this.ModuleQCDarkCurrentBtn.Name = "ModuleQCDarkCurrentBtn";
            this.ModuleQCDarkCurrentBtn.Size = new System.Drawing.Size(103, 28);
            this.ModuleQCDarkCurrentBtn.TabIndex = 17;
            this.ModuleQCDarkCurrentBtn.Text = "Dark Current";
            this.ModuleQCDarkCurrentBtn.UseVisualStyleBackColor = true;
            this.ModuleQCDarkCurrentBtn.Click += new System.EventHandler(this.ModuleQCDarkCurrentBtn_Click);
            // 
            // ModuleQAHaltBtn
            // 
            this.ModuleQAHaltBtn.BackColor = System.Drawing.Color.Red;
            this.ModuleQAHaltBtn.Location = new System.Drawing.Point(126, 17);
            this.ModuleQAHaltBtn.Name = "ModuleQAHaltBtn";
            this.ModuleQAHaltBtn.Size = new System.Drawing.Size(79, 27);
            this.ModuleQAHaltBtn.TabIndex = 16;
            this.ModuleQAHaltBtn.Text = "STOP";
            this.ModuleQAHaltBtn.UseVisualStyleBackColor = false;
            this.ModuleQAHaltBtn.Click += new System.EventHandler(this.ModuleQCHaltBtn_Click);
            // 
            // ComPortStatusLbl
            // 
            this.ComPortStatusLbl.AutoSize = true;
            this.ComPortStatusLbl.Location = new System.Drawing.Point(831, 61);
            this.ComPortStatusLbl.Name = "ComPortStatusLbl";
            this.ComPortStatusLbl.Size = new System.Drawing.Size(52, 17);
            this.ComPortStatusLbl.TabIndex = 15;
            this.ComPortStatusLbl.Text = "Status:";
            // 
            // ComPortStatusBox
            // 
            this.ComPortStatusBox.Location = new System.Drawing.Point(885, 58);
            this.ComPortStatusBox.Name = "ComPortStatusBox";
            this.ComPortStatusBox.ReadOnly = true;
            this.ComPortStatusBox.Size = new System.Drawing.Size(109, 23);
            this.ComPortStatusBox.TabIndex = 14;
            // 
            // ComPortRefresh
            // 
            this.ComPortRefresh.Location = new System.Drawing.Point(747, 58);
            this.ComPortRefresh.Name = "ComPortRefresh";
            this.ComPortRefresh.Size = new System.Drawing.Size(75, 23);
            this.ComPortRefresh.TabIndex = 13;
            this.ComPortRefresh.Text = "Refresh";
            this.ComPortRefresh.UseVisualStyleBackColor = true;
            this.ComPortRefresh.Click += new System.EventHandler(this.ComPortRefresh_Click);
            // 
            // ComPortDisconnectBtn
            // 
            this.ComPortDisconnectBtn.Location = new System.Drawing.Point(907, 29);
            this.ComPortDisconnectBtn.Name = "ComPortDisconnectBtn";
            this.ComPortDisconnectBtn.Size = new System.Drawing.Size(87, 23);
            this.ComPortDisconnectBtn.TabIndex = 12;
            this.ComPortDisconnectBtn.Text = "Disconnect";
            this.ComPortDisconnectBtn.UseVisualStyleBackColor = true;
            this.ComPortDisconnectBtn.Click += new System.EventHandler(this.ComPortDisconnectBtn_Click);
            // 
            // comPortConnectBtn
            // 
            this.comPortConnectBtn.Location = new System.Drawing.Point(826, 29);
            this.comPortConnectBtn.Name = "comPortConnectBtn";
            this.comPortConnectBtn.Size = new System.Drawing.Size(75, 23);
            this.comPortConnectBtn.TabIndex = 11;
            this.comPortConnectBtn.Text = "Connect";
            this.comPortConnectBtn.UseVisualStyleBackColor = true;
            this.comPortConnectBtn.Click += new System.EventHandler(this.ComPortConnectBtn_Click);
            // 
            // ComPortLbl
            // 
            this.ComPortLbl.AutoSize = true;
            this.ComPortLbl.Location = new System.Drawing.Point(744, 9);
            this.ComPortLbl.Name = "ComPortLbl";
            this.ComPortLbl.Size = new System.Drawing.Size(76, 17);
            this.ComPortLbl.TabIndex = 10;
            this.ComPortLbl.Text = "COM Ports";
            this.ComPortLbl.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // comPortBox
            // 
            this.comPortBox.FormattingEnabled = true;
            this.comPortBox.Location = new System.Drawing.Point(747, 28);
            this.comPortBox.Name = "comPortBox";
            this.comPortBox.Size = new System.Drawing.Size(73, 25);
            this.comPortBox.TabIndex = 9;
            // 
            // ModuleQCFilenameBox
            // 
            this.ModuleQCFilenameBox.Location = new System.Drawing.Point(575, 33);
            this.ModuleQCFilenameBox.Name = "ModuleQCFilenameBox";
            this.ModuleQCFilenameBox.Size = new System.Drawing.Size(117, 23);
            this.ModuleQCFilenameBox.TabIndex = 2;
            this.ModuleQCFilenameBox.Text = "ModuleQC";
            // 
            // ModuleQAFileLbl
            // 
            this.ModuleQAFileLbl.AutoSize = true;
            this.ModuleQAFileLbl.Location = new System.Drawing.Point(468, 17);
            this.ModuleQAFileLbl.Name = "ModuleQAFileLbl";
            this.ModuleQAFileLbl.Size = new System.Drawing.Size(253, 34);
            this.ModuleQAFileLbl.TabIndex = 8;
            this.ModuleQAFileLbl.Text = "Output File Name:\r\n\"ScanningData_                              .txt\"";
            // 
            // ModuleQASideLbl
            // 
            this.ModuleQASideLbl.AutoSize = true;
            this.ModuleQASideLbl.Location = new System.Drawing.Point(358, 17);
            this.ModuleQASideLbl.Name = "ModuleQASideLbl";
            this.ModuleQASideLbl.Size = new System.Drawing.Size(36, 17);
            this.ModuleQASideLbl.TabIndex = 6;
            this.ModuleQASideLbl.Text = "Side";
            // 
            // ModuleQCSide
            // 
            this.ModuleQCSide.FormattingEnabled = true;
            this.ModuleQCSide.Items.AddRange(new object[] {
            "A",
            "B",
            "Middle"});
            this.ModuleQCSide.Location = new System.Drawing.Point(361, 35);
            this.ModuleQCSide.Name = "ModuleQCSide";
            this.ModuleQCSide.Size = new System.Drawing.Size(91, 25);
            this.ModuleQCSide.TabIndex = 1;
            // 
            // ModuleQAFEB2Box
            // 
            this.ModuleQAFEB2Box.Controls.Add(this.ModuleQCTableFEB2);
            this.ModuleQAFEB2Box.Location = new System.Drawing.Point(7, 288);
            this.ModuleQAFEB2Box.Name = "ModuleQAFEB2Box";
            this.ModuleQAFEB2Box.Size = new System.Drawing.Size(1242, 195);
            this.ModuleQAFEB2Box.TabIndex = 4;
            this.ModuleQAFEB2Box.TabStop = false;
            this.ModuleQAFEB2Box.Text = "FEB2";
            // 
            // ModuleQCTableFEB2
            // 
            this.ModuleQCTableFEB2.ColumnCount = 16;
            this.ModuleQCTableFEB2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 6.25F));
            this.ModuleQCTableFEB2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 6.25F));
            this.ModuleQCTableFEB2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 6.25F));
            this.ModuleQCTableFEB2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 6.25F));
            this.ModuleQCTableFEB2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 6.25F));
            this.ModuleQCTableFEB2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 6.25F));
            this.ModuleQCTableFEB2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 6.25F));
            this.ModuleQCTableFEB2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 6.25F));
            this.ModuleQCTableFEB2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 6.25F));
            this.ModuleQCTableFEB2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 6.25F));
            this.ModuleQCTableFEB2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 6.25F));
            this.ModuleQCTableFEB2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 6.25F));
            this.ModuleQCTableFEB2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 6.25F));
            this.ModuleQCTableFEB2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 6.25F));
            this.ModuleQCTableFEB2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 6.25F));
            this.ModuleQCTableFEB2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 6.25F));
            this.ModuleQCTableFEB2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ModuleQCTableFEB2.GrowStyle = System.Windows.Forms.TableLayoutPanelGrowStyle.FixedSize;
            this.ModuleQCTableFEB2.Location = new System.Drawing.Point(3, 19);
            this.ModuleQCTableFEB2.Name = "ModuleQCTableFEB2";
            this.ModuleQCTableFEB2.RowCount = 4;
            this.ModuleQCTableFEB2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.ModuleQCTableFEB2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.ModuleQCTableFEB2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.ModuleQCTableFEB2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.ModuleQCTableFEB2.Size = new System.Drawing.Size(1236, 173);
            this.ModuleQCTableFEB2.TabIndex = 2;
            // 
            // ModuleQAFEB1Box
            // 
            this.ModuleQAFEB1Box.Controls.Add(this.ModuleQCTableFEB1);
            this.ModuleQAFEB1Box.Location = new System.Drawing.Point(7, 87);
            this.ModuleQAFEB1Box.Name = "ModuleQAFEB1Box";
            this.ModuleQAFEB1Box.Size = new System.Drawing.Size(1242, 195);
            this.ModuleQAFEB1Box.TabIndex = 3;
            this.ModuleQAFEB1Box.TabStop = false;
            this.ModuleQAFEB1Box.Text = "FEB1";
            // 
            // ModuleQCTableFEB1
            // 
            this.ModuleQCTableFEB1.ColumnCount = 16;
            this.ModuleQCTableFEB1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 6.25F));
            this.ModuleQCTableFEB1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 6.25F));
            this.ModuleQCTableFEB1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 6.25F));
            this.ModuleQCTableFEB1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 6.25F));
            this.ModuleQCTableFEB1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 6.25F));
            this.ModuleQCTableFEB1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 6.25F));
            this.ModuleQCTableFEB1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 6.25F));
            this.ModuleQCTableFEB1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 6.25F));
            this.ModuleQCTableFEB1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 6.25F));
            this.ModuleQCTableFEB1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 6.25F));
            this.ModuleQCTableFEB1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 6.25F));
            this.ModuleQCTableFEB1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 6.25F));
            this.ModuleQCTableFEB1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 6.25F));
            this.ModuleQCTableFEB1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 6.25F));
            this.ModuleQCTableFEB1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 6.25F));
            this.ModuleQCTableFEB1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 6.25F));
            this.ModuleQCTableFEB1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ModuleQCTableFEB1.GrowStyle = System.Windows.Forms.TableLayoutPanelGrowStyle.FixedSize;
            this.ModuleQCTableFEB1.Location = new System.Drawing.Point(3, 19);
            this.ModuleQCTableFEB1.Name = "ModuleQCTableFEB1";
            this.ModuleQCTableFEB1.RowCount = 4;
            this.ModuleQCTableFEB1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.ModuleQCTableFEB1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.ModuleQCTableFEB1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.ModuleQCTableFEB1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.ModuleQCTableFEB1.Size = new System.Drawing.Size(1236, 173);
            this.ModuleQCTableFEB1.TabIndex = 1;
            // 
            // ModuleQCBtn
            // 
            this.ModuleQCBtn.Enabled = false;
            this.ModuleQCBtn.Location = new System.Drawing.Point(17, 45);
            this.ModuleQCBtn.Name = "ModuleQCBtn";
            this.ModuleQCBtn.Size = new System.Drawing.Size(103, 29);
            this.ModuleQCBtn.TabIndex = 0;
            this.ModuleQCBtn.Text = "Source Test";
            this.ModuleQCBtn.UseVisualStyleBackColor = true;
            this.ModuleQCBtn.Click += new System.EventHandler(this.ModuleQCBtn_Click);
            // 
            // FEBSelectPanel
            // 
            this.FEBSelectPanel.ColumnCount = 8;
            this.FEBSelectPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 12.5F));
            this.FEBSelectPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 12.5F));
            this.FEBSelectPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 12.5F));
            this.FEBSelectPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 12.5F));
            this.FEBSelectPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 12.5F));
            this.FEBSelectPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 12.5F));
            this.FEBSelectPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 12.5F));
            this.FEBSelectPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 12.5F));
            this.FEBSelectPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.FEBSelectPanel.GrowStyle = System.Windows.Forms.TableLayoutPanelGrowStyle.AddColumns;
            this.FEBSelectPanel.Location = new System.Drawing.Point(0, 688);
            this.FEBSelectPanel.Name = "FEBSelectPanel";
            this.FEBSelectPanel.RowCount = 1;
            this.FEBSelectPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.FEBSelectPanel.Size = new System.Drawing.Size(1264, 40);
            this.FEBSelectPanel.TabIndex = 9;
            // 
            // SpillTimer
            // 
            this.SpillTimer.Interval = 1000;
            this.SpillTimer.Tick += new System.EventHandler(this.SpillTimer_Tick);
            // 
            // moduleQCHomingTimer
            // 
            this.moduleQCHomingTimer.Interval = 500;
            this.moduleQCHomingTimer.Tick += new System.EventHandler(this.ModuleQCHomingTimer_Tick);
            // 
            // moduleQCMeasurementTimer
            // 
            this.moduleQCMeasurementTimer.Interval = 10;
            this.moduleQCMeasurementTimer.Tick += new System.EventHandler(this.ModuleQCMeasurementTimer_Tick);
            // 
            // ModuleQCStepTimer
            // 
            this.ModuleQCStepTimer.Interval = 500;
            this.ModuleQCStepTimer.Tick += new System.EventHandler(this.ModuleQCStepTimer_Tick);
            // 
            // LightCheckMeasurementTimer
            // 
            this.LightCheckMeasurementTimer.Interval = 50;
            this.LightCheckMeasurementTimer.Tick += new System.EventHandler(this.LightCheckMeasurementTimer_Tick);
            // 
            // qcDiCounterMeasurementTimer
            // 
            this.qcDiCounterMeasurementTimer.Tick += new System.EventHandler(this.QcDiCounterMeasurementTimer_Tick);
            // 
            // FEBClientFooterBar
            // 
            this.FEBClientFooterBar.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.FEBClientFooterBar.Location = new System.Drawing.Point(2, 685);
            this.FEBClientFooterBar.Name = "FEBClientFooterBar";
            this.FEBClientFooterBar.Size = new System.Drawing.Size(1260, 2);
            this.FEBClientFooterBar.TabIndex = 47;
            // 
            // cmbTest_ShortHelperBtn
            // 
            this.cmbTest_ShortHelperBtn.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmbTest_ShortHelperBtn.Location = new System.Drawing.Point(187, 17);
            this.cmbTest_ShortHelperBtn.Name = "cmbTest_ShortHelperBtn";
            this.cmbTest_ShortHelperBtn.Size = new System.Drawing.Size(107, 32);
            this.cmbTest_ShortHelperBtn.TabIndex = 12;
            this.cmbTest_ShortHelperBtn.Text = "Short Suspected";
            this.cmbTest_ShortHelperBtn.UseVisualStyleBackColor = true;
            this.cmbTest_ShortHelperBtn.Click += new System.EventHandler(this.CmbTest_ShortHelperBtn_Click);
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1264, 728);
            this.Controls.Add(this.FEBClientFooterBar);
            this.Controls.Add(this.FEBSelectPanel);
            this.Controls.Add(this.tabControl);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.18868F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximizeBox = false;
            this.Name = "frmMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Mu2e_TB";
            this.Load += new System.EventHandler(this.FrmMain_Load);
            this.tabControl.ResumeLayout(false);
            this.tabRUN.ResumeLayout(false);
            this.tabRUN.PerformLayout();
            this.groupBoxEvDisplay.ResumeLayout(false);
            this.groupBoxEvDisplay.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ud_VertMin)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ud_VertMax)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.SpillStatusGroupBox.ResumeLayout(false);
            this.SpillStatusGroupBox.PerformLayout();
            this.SpillStatusTable.ResumeLayout(false);
            this.SpillStatusTable.PerformLayout();
            this.tabConsole.ResumeLayout(false);
            this.tabConsole.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.tabFEB1.ResumeLayout(false);
            this.tabFEB1.PerformLayout();
            this.groupBoxREG1.ResumeLayout(false);
            this.groupBoxREG1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.udFPGA)).EndInit();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.udChan)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.udInterval)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.udStop)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.udStart)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.groupBox7.ResumeLayout(false);
            this.groupBox7.PerformLayout();
            this.tabWC.ResumeLayout(false);
            this.groupBox6.ResumeLayout(false);
            this.groupBox6.PerformLayout();
            this.tabQC.ResumeLayout(false);
            this.tabQC.PerformLayout();
            this.lightCheckGroup.ResumeLayout(false);
            this.lightCheckGroup.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.lightNumChecks)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lightCheckChanSelec)).EndInit();
            this.dicounterQCGroup.ResumeLayout(false);
            this.dicounterQCGroup.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.qaDiNumAvg)).EndInit();
            this.tabCMBTester.ResumeLayout(false);
            this.cmbDataGroup.ResumeLayout(false);
            this.cmbTestControl.ResumeLayout(false);
            this.cmbTestControl.PerformLayout();
            this.sipmControl.ResumeLayout(false);
            this.sipmControl.PerformLayout();
            this.tabModuleQC.ResumeLayout(false);
            this.tabModuleQC.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ModuleQC_Offset)).EndInit();
            this.ModuleQAFEB2Box.ResumeLayout(false);
            this.ModuleQAFEB1Box.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage tabRUN;
        private System.Windows.Forms.GroupBox groupBox1;
        private ZedGraph.ZedGraphControl zg1;
        private System.Windows.Forms.TabPage tabFEB1;
        private System.Windows.Forms.TabPage tabWC;
        private System.Windows.Forms.GroupBox groupBox6;
        private System.Windows.Forms.Label lblWCmessage;
        private System.Windows.Forms.Button btnWC;
        private System.Windows.Forms.GroupBox groupBox7;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label lblInc;
        private System.Windows.Forms.NumericUpDown udInterval;
        private System.Windows.Forms.Button btnSaveHistos;
        private System.Windows.Forms.Label lblStop;
        public System.Windows.Forms.Button btnScan;
        private System.Windows.Forms.NumericUpDown udStop;
        private System.Windows.Forms.Label lblStart;
        private System.Windows.Forms.NumericUpDown udStart;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.CheckBox chkIntegral;
        private System.Windows.Forms.CheckBox chkLogY;
        private System.Windows.Forms.Label lblI;
        private System.Windows.Forms.Label lblV;
        private System.Windows.Forms.Button btnBiasWRITE;
        private System.Windows.Forms.TabPage tabConsole;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.TextBox ConsoleBox;
        private System.Windows.Forms.Button btnBiasREAD;
        private System.Windows.Forms.TextBox txtI;
        private System.Windows.Forms.TextBox txtV;
        private System.Windows.Forms.GroupBox groupBoxREG1;
        private System.Windows.Forms.NumericUpDown udFPGA;
        private System.Windows.Forms.Button btnRegWRITE;
        private System.Windows.Forms.Button btnRegREAD;
        private ZedGraph.ZedGraphControl zedFEB1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown udChan;
        private System.Windows.Forms.Button btnErase;
        private System.Windows.Forms.Button btnCHANGE;
        private System.Windows.Forms.Label lblFPGA;
        private System.Windows.Forms.Button ShowSpect;
        private System.Windows.Forms.Button ShowIV;
        private System.Windows.Forms.Button btnConnectAll;
        private System.Windows.Forms.Button btnStopRun;
        private System.Windows.Forms.Button btnStartRun;
        private System.Windows.Forms.Button btnPrepare;
        private System.Windows.Forms.Label lblTxtRunName;
        private System.Windows.Forms.Label lblSpillTime;
        private System.Windows.Forms.Label lblLastSpillTrig;
        private System.Windows.Forms.Label lblTotalNumTrig;
        private System.Windows.Forms.Label lblRunTime;
        private System.Windows.Forms.Label lblTimeInRun;
        private System.Windows.Forms.Label lblSpillStatus;
        private System.Windows.Forms.Label lblRunName;
        public System.Windows.Forms.Timer SpillTimer;
        private System.Windows.Forms.Button btnFebClientsChange;
        private System.Windows.Forms.NumericUpDown ud_VertMax;
        private System.Windows.Forms.GroupBox groupBoxEvDisplay;
        private System.Windows.Forms.TextBox txtEvent;
        private System.Windows.Forms.Button btnNextDisp;
        private System.Windows.Forms.Button btnPrevDisp;
        private System.Windows.Forms.NumericUpDown ud_VertMin;
        private System.Windows.Forms.Button btnDisplaySpill;
        private System.Windows.Forms.Button loadCmdsBtn;
        private System.Windows.Forms.Button btnDebugLogging;
        private System.Windows.Forms.CheckBox chkPersist;
        private System.Windows.Forms.CheckBox chkLast;
        private System.Windows.Forms.TextBox txtCMB_Temp1;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox txtCMB_Temp4;
        private System.Windows.Forms.TextBox txtCMB_Temp3;
        private System.Windows.Forms.TextBox txtCMB_Temp2;
        private System.Windows.Forms.Label lblMaxADC0;
        private System.Windows.Forms.Label lblMaxADC1;
        private System.Windows.Forms.Label lblMaxADC3;
        private System.Windows.Forms.Label lblMaxADC2;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.TabPage tabQC;
        private System.Windows.Forms.Label qaBiasLbl;
        private System.Windows.Forms.TextBox qcBias;
        private System.Windows.Forms.GroupBox dicounterQCGroup;
        private System.Windows.Forms.RadioButton[] qcDiButtons;
        private System.Windows.Forms.Label qaDiIWarningThreshLabel;
        private System.Windows.Forms.TextBox qcDiIWarningThresh;
        private System.Windows.Forms.ProgressBar autoDataProgress;
        private System.Windows.Forms.Button qcStartButton;
        private System.Windows.Forms.Label numLabel;
        private System.Windows.Forms.TextBox dicounterNumberTextBox;
        private System.Windows.Forms.GroupBox lightCheckGroup;
        private System.Windows.Forms.GroupBox[] lightCheckGroupFPGAs;
        private System.Windows.Forms.Button lightCheckBtn;
        private System.Windows.Forms.RadioButton[] lightButtons;
        private System.Windows.Forms.TextBox lightGlobalThresh;
        private System.Windows.Forms.CheckBox globalThreshChkBox;
        private System.Windows.Forms.Label lightGlobalThreshLbl;
        private System.Windows.Forms.Button lightCheckChanThreshBtn;
        private System.Windows.Forms.Label lightCheckChanThreshLbl;
        private System.Windows.Forms.TextBox lightCheckChanThresh;
        private System.Windows.Forms.Label lightCheckChanSelecLbl;
        private System.Windows.Forms.NumericUpDown lightCheckChanSelec;
        private System.Windows.Forms.Button autoThreshBtn;
        private System.Windows.Forms.ProgressBar lightCheckProgress;
        private System.Windows.Forms.Button lightCheckResetThresh;
        private System.Windows.Forms.Label[] lightCMBlabels;
        private System.Windows.Forms.Label[] qcFPGALabels;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Label qaDiNumAvgLabel;
        private System.Windows.Forms.NumericUpDown qaDiNumAvg;
        private System.Windows.Forms.CheckBox oneReadout;
        private System.Windows.Forms.Label BDVoltsLabel15;
        private System.Windows.Forms.Label BDVoltsLabel14;
        private System.Windows.Forms.Label BDVoltsLabel13;
        private System.Windows.Forms.Label BDVoltsLabel12;
        private System.Windows.Forms.Label BDVoltsLabel7;
        private System.Windows.Forms.Label BDVoltsLabel6;
        private System.Windows.Forms.Label BDVoltsLabel5;
        private System.Windows.Forms.Label BDVoltsLabel8;
        private System.Windows.Forms.Label BDVoltsLabel11;
        private System.Windows.Forms.Label BDVoltsLabel9;
        private System.Windows.Forms.Label BDVoltsLabel10;
        private System.Windows.Forms.Label BDVoltsLabel4;
        private System.Windows.Forms.Label BDVoltsLabel3;
        private System.Windows.Forms.Label BDVoltsLabel2;
        private System.Windows.Forms.Label BDVoltsLabel1;
        private System.Windows.Forms.Label BDVoltsLabel0;
        private System.Windows.Forms.CheckBox scanAllChanBox;
        private System.Windows.Forms.Label label23;
        private System.Windows.Forms.TabPage tabCMBTester;
        private System.Windows.Forms.GroupBox cmbTestControl;
        private System.Windows.Forms.Button cmbTestBtn;
        private System.Windows.Forms.GroupBox sipmControl;
        private System.Windows.Forms.TextBox cmbBias;
        private System.Windows.Forms.CheckBox cmbBiasOverride;
        private System.Windows.Forms.Label numTrigsDisp;
        private System.Windows.Forms.Label numTrigLabel;
        private System.Windows.Forms.TextBox requestNumTrigs;
        private System.Windows.Forms.Label requestNumTrigsLabel;
        private System.Windows.Forms.TextBox cmbInfoBox;
        private System.Windows.Forms.CheckBox updateFilesChkBox;
        private System.Windows.Forms.GroupBox cmbDataGroup;
        private System.Windows.Forms.TextBox qcOutputFileName;
        private System.Windows.Forms.Label qaOutputFileNameLabel;
        private System.Windows.Forms.TableLayoutPanel lightCheckFPGApanel;
        private System.Windows.Forms.CheckBox lightWriteToFileBox;
        private System.Windows.Forms.ComboBox lightModuleSide;
        private System.Windows.Forms.Label lightModuleLabelLabel;
        private System.Windows.Forms.TextBox lightModuleLabel;
        private System.Windows.Forms.Label lightModuleSideLabel;
        private System.Windows.Forms.Label LightCheckModuleHalfLbl;
        private System.Windows.Forms.NumericUpDown lightNumChecks;
        private System.Windows.Forms.RichTextBox console_Disp;
        private System.Windows.Forms.RichTextBox runLog;
        private System.Windows.Forms.TableLayoutPanel cmbDataTable;
        private System.Windows.Forms.Label[][] cmbInfoLabels;
        private System.Windows.Forms.CheckBox saveAsciiBox;
        private System.Windows.Forms.Button lostCMBavgsBtn;
        private System.Windows.Forms.TabPage tabModuleQC;
        private System.Windows.Forms.TableLayoutPanel ModuleQCTableFEB2;
        private System.Windows.Forms.TableLayoutPanel ModuleQCTableFEB1;
        private System.Windows.Forms.Button ModuleQCBtn;
        private System.Windows.Forms.GroupBox ModuleQAFEB2Box;
        private System.Windows.Forms.GroupBox ModuleQAFEB1Box;
        private System.Windows.Forms.Label ModuleQASideLbl;
        private System.Windows.Forms.ComboBox ModuleQCSide;
        private System.Windows.Forms.TextBox ModuleQCFilenameBox;
        private System.Windows.Forms.Label ModuleQAFileLbl;
        private System.Windows.Forms.Label[][] ModuleQCLabels;
        private System.Windows.Forms.Label ComPortLbl;
        private System.Windows.Forms.ComboBox comPortBox;
        private System.Windows.Forms.Button comPortConnectBtn;
        private System.Windows.Forms.Button ComPortDisconnectBtn;
        private System.Windows.Forms.Button ComPortRefresh;
        private System.Windows.Forms.Timer moduleQCHomingTimer;
        private System.Windows.Forms.Label ComPortStatusLbl;
        private System.Windows.Forms.TextBox ComPortStatusBox;
        private System.Windows.Forms.Timer moduleQCMeasurementTimer;
        private System.Windows.Forms.Button ModuleQAHaltBtn;
        private System.Windows.Forms.Button ModuleQCDarkCurrentBtn;
        private System.Windows.Forms.Timer ModuleQCStepTimer;
        private System.Windows.Forms.Button ModuleQCHomeResetBtn;
        private System.Windows.Forms.CheckBox validateParseChkBox;
        private System.Windows.Forms.Timer LightCheckMeasurementTimer;
        private System.Windows.Forms.Timer qcDiCounterMeasurementTimer;
        private System.Windows.Forms.CheckBox ModuleQC_flipped_Chkbox;
        private System.Windows.Forms.Label ModuleQA_OffsetLbl;
        private System.Windows.Forms.NumericUpDown ModuleQC_Offset;
        private System.Windows.Forms.TableLayoutPanel FEBSelectPanel;
        private System.Windows.Forms.GroupBox SpillStatusGroupBox;
        private System.Windows.Forms.Label lblTimeInSpill;
        private System.Windows.Forms.TableLayoutPanel SpillStatusTable;
        private System.Windows.Forms.Label lblFebColumn;
        private System.Windows.Forms.Label lblNumSpills;
        private System.Windows.Forms.Label lblSpillsNum;
        private System.Windows.Forms.Label FEBClientFooterBar;
        private System.Windows.Forms.Button btnBiasWRITEALL;
        private System.Windows.Forms.TextBox ModuleQCModuleNameBox;
        private System.Windows.Forms.Label ModuleQAModuleNameLbl;
        private System.Windows.Forms.ComboBox LightCheckType;
        private System.Windows.Forms.Label LightCheckTypeLbl;
        private System.Windows.Forms.ComboBox LightCheckModuleHalf;
        private System.Windows.Forms.ProgressBar cmbTesterProgresBar;
        private System.Windows.Forms.Button cmbTest_ShortHelperBtn;
    }
}