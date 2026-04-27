namespace KeyboardController
{
    partial class MainForm
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
        private void InitializeComponent() {
            grpConnection = new GroupBox();
            btnRefresh = new Button();
            cmbBaudRate = new ComboBox();
            lblBaudRate = new Label();
            lblComPort = new Label();
            cmbComPort = new ComboBox();
            btnConnect = new Button();
            grpSend = new GroupBox();
            btnOpenFile = new Button();
            btnClear = new Button();
            btnStopSend = new Button();
            btnSend = new Button();
            progressBarSend = new ProgressBar();
            txtSend = new TextBox();
            lblSend = new Label();
            lblLineCount = new Label();
            lblSendSpeed = new Label();
            cmbSendSpeed = new ComboBox();
            lblSendSpeedHint = new Label();
            grpQuickSend = new GroupBox();
            btnSendBackspace = new Button();
            btnSendSpace = new Button();
            btnSendEnter = new Button();
            btnMouseLeft = new Button();
            btnMouseRight = new Button();
            btnCapsLock = new Button();
            btnNumLock = new Button();
            btnScrollLock = new Button();
            lblStatus = new Label();
            grpHotKeys = new GroupBox();
            btnArrowDown = new Button();
            btnArrowRight = new Button();
            btnArrowLeft = new Button();
            btnArrowUp = new Button();
            btnAltTab = new Button();
            btnWinD = new Button();
            btnCtrlZ = new Button();
            btnCtrlX = new Button();
            btnCtrlV = new Button();
            btnCtrlC = new Button();
            btnCtrlA = new Button();
            grpSync = new GroupBox();
            btnKeyboardSync = new Button();
            grpRemoteMouse = new GroupBox();
            btnRemoteMouseMode = new Button();
            lblRemoteMouseHint = new Label();
            grpModifiers = new GroupBox();
            btnAlt = new Button();
            btnShift = new Button();
            btnCtrl = new Button();
            grpConnection.SuspendLayout();
            grpSend.SuspendLayout();
            grpQuickSend.SuspendLayout();
            grpHotKeys.SuspendLayout();
            grpSync.SuspendLayout();
            grpRemoteMouse.SuspendLayout();
            grpModifiers.SuspendLayout();
            SuspendLayout();
            // 
            // grpConnection
            // 
            grpConnection.Controls.Add(btnRefresh);
            grpConnection.Controls.Add(cmbBaudRate);
            grpConnection.Controls.Add(lblBaudRate);
            grpConnection.Controls.Add(lblComPort);
            grpConnection.Controls.Add(cmbComPort);
            grpConnection.Controls.Add(btnConnect);
            grpConnection.Location = new Point(21, 21);
            grpConnection.Margin = new Padding(5);
            grpConnection.Name = "grpConnection";
            grpConnection.Padding = new Padding(5);
            grpConnection.Size = new Size(805, 140);
            grpConnection.TabIndex = 0;
            grpConnection.TabStop = false;
            grpConnection.Text = "连接设置";
            // 
            // btnRefresh
            // 
            btnRefresh.Location = new Point(630, 88);
            btnRefresh.Margin = new Padding(5);
            btnRefresh.Name = "btnRefresh";
            btnRefresh.Size = new Size(140, 46);
            btnRefresh.TabIndex = 5;
            btnRefresh.Text = "刷新串口";
            btnRefresh.UseVisualStyleBackColor = true;
            btnRefresh.Click += btnRefresh_Click;
            // 
            // cmbBaudRate
            // 
            cmbBaudRate.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbBaudRate.FormattingEnabled = true;
            cmbBaudRate.Items.AddRange(new object[] { "9600", "19200", "38400", "57600", "115200" });
            cmbBaudRate.Location = new Point(350, 88);
            cmbBaudRate.Margin = new Padding(5);
            cmbBaudRate.Name = "cmbBaudRate";
            cmbBaudRate.Size = new Size(207, 36);
            cmbBaudRate.TabIndex = 4;
            // 
            // lblBaudRate
            // 
            lblBaudRate.AutoSize = true;
            lblBaudRate.Location = new Point(262, 93);
            lblBaudRate.Margin = new Padding(5, 0, 5, 0);
            lblBaudRate.Name = "lblBaudRate";
            lblBaudRate.Size = new Size(75, 28);
            lblBaudRate.TabIndex = 3;
            lblBaudRate.Text = "波特率";
            // 
            // lblComPort
            // 
            lblComPort.AutoSize = true;
            lblComPort.Location = new Point(35, 93);
            lblComPort.Margin = new Padding(5, 0, 5, 0);
            lblComPort.Name = "lblComPort";
            lblComPort.Size = new Size(54, 28);
            lblComPort.TabIndex = 2;
            lblComPort.Text = "串口";
            // 
            // cmbComPort
            // 
            cmbComPort.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbComPort.FormattingEnabled = true;
            cmbComPort.Location = new Point(105, 88);
            cmbComPort.Margin = new Padding(5);
            cmbComPort.Name = "cmbComPort";
            cmbComPort.Size = new Size(137, 36);
            cmbComPort.TabIndex = 1;
            // 
            // btnConnect
            // 
            btnConnect.BackColor = Color.FromArgb(128, 255, 128);
            btnConnect.FlatStyle = FlatStyle.Flat;
            btnConnect.Font = new Font("Microsoft YaHei UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
            btnConnect.Location = new Point(35, 26);
            btnConnect.Margin = new Padding(5);
            btnConnect.Name = "btnConnect";
            btnConnect.Size = new Size(735, 46);
            btnConnect.TabIndex = 0;
            btnConnect.Text = "连接";
            btnConnect.UseVisualStyleBackColor = false;
            btnConnect.Click += btnConnect_Click;
            // 
            // grpSend
            // 
            grpSend.Controls.Add(lblSendSpeedHint);
            grpSend.Controls.Add(cmbSendSpeed);
            grpSend.Controls.Add(lblSendSpeed);
            grpSend.Controls.Add(btnOpenFile);
            grpSend.Controls.Add(btnClear);
            grpSend.Controls.Add(btnStopSend);
            grpSend.Controls.Add(btnSend);
            grpSend.Controls.Add(progressBarSend);
            grpSend.Controls.Add(txtSend);
            grpSend.Controls.Add(lblSend);
            grpSend.Controls.Add(lblLineCount);
            grpSend.Enabled = false;
            grpSend.Location = new Point(21, 172);
            grpSend.Margin = new Padding(5);
            grpSend.Name = "grpSend";
            grpSend.Padding = new Padding(5);
            grpSend.Size = new Size(805, 548);
            grpSend.TabIndex = 1;
            grpSend.TabStop = false;
            grpSend.Text = "字符发送";
            // 
            // btnOpenFile
            // 
            btnOpenFile.Location = new Point(35, 434);
            btnOpenFile.Margin = new Padding(5);
            btnOpenFile.Name = "btnOpenFile";
            btnOpenFile.Size = new Size(140, 52);
            btnOpenFile.TabIndex = 6;
            btnOpenFile.Text = "打开文件";
            btnOpenFile.UseVisualStyleBackColor = true;
            btnOpenFile.Click += btnOpenFile_Click;
            // 
            // btnClear
            // 
            btnClear.Location = new Point(630, 372);
            btnClear.Margin = new Padding(5);
            btnClear.Name = "btnClear";
            btnClear.Size = new Size(140, 52);
            btnClear.TabIndex = 3;
            btnClear.Text = "清空";
            btnClear.UseVisualStyleBackColor = true;
            btnClear.Click += btnClear_Click;
            // 
            // btnStopSend
            // 
            btnStopSend.Enabled = false;
            btnStopSend.Location = new Point(314, 372);
            btnStopSend.Margin = new Padding(5);
            btnStopSend.Name = "btnStopSend";
            btnStopSend.Size = new Size(140, 52);
            btnStopSend.TabIndex = 4;
            btnStopSend.Text = "停止发送";
            btnStopSend.UseVisualStyleBackColor = true;
            btnStopSend.Click += btnStopSend_Click;
            // 
            // btnSend
            // 
            btnSend.Location = new Point(472, 372);
            btnSend.Margin = new Padding(5);
            btnSend.Name = "btnSend";
            btnSend.Size = new Size(140, 52);
            btnSend.TabIndex = 2;
            btnSend.Text = "发送";
            btnSend.UseVisualStyleBackColor = true;
            btnSend.Click += btnSend_Click;
            // 
            // progressBarSend
            // 
            progressBarSend.Location = new Point(35, 372);
            progressBarSend.Margin = new Padding(5);
            progressBarSend.Name = "progressBarSend";
            progressBarSend.Size = new Size(260, 52);
            progressBarSend.TabIndex = 5;
            // 
            // txtSend
            // 
            txtSend.Font = new Font("Consolas", 10F, FontStyle.Regular, GraphicsUnit.Point);
            txtSend.Location = new Point(35, 70);
            txtSend.Margin = new Padding(5);
            txtSend.MaxLength = 0;
            txtSend.Multiline = true;
            txtSend.Name = "txtSend";
            txtSend.ScrollBars = ScrollBars.Both;
            txtSend.Size = new Size(732, 250);
            txtSend.TabIndex = 1;
            txtSend.TextChanged += txtSend_TextChanged;
            txtSend.KeyDown += txtSend_KeyDown;
            txtSend.KeyPress += txtSend_KeyPress;
            // 
            // lblSend
            // 
            lblSend.AutoSize = true;
            lblSend.Location = new Point(30, 30);
            lblSend.Margin = new Padding(5, 0, 5, 0);
            lblSend.Name = "lblSend";
            lblSend.Size = new Size(101, 28);
            lblSend.TabIndex = 0;
            lblSend.Text = "输入字符:";
            // 
            // lblSendSpeed
            //
            lblSendSpeed.AutoSize = true;
            lblSendSpeed.Location = new Point(35, 336);
            lblSendSpeed.Margin = new Padding(5, 0, 5, 0);
            lblSendSpeed.Name = "lblSendSpeed";
            lblSendSpeed.Size = new Size(101, 28);
            lblSendSpeed.TabIndex = 8;
            lblSendSpeed.Text = "发送速率:";
            //
            // cmbSendSpeed
            //
            cmbSendSpeed.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbSendSpeed.FormattingEnabled = true;
            cmbSendSpeed.Items.AddRange(new object[] { "稳定", "快速", "极速" });
            cmbSendSpeed.Location = new Point(145, 332);
            cmbSendSpeed.Margin = new Padding(5);
            cmbSendSpeed.Name = "cmbSendSpeed";
            cmbSendSpeed.Size = new Size(137, 36);
            cmbSendSpeed.TabIndex = 9;
            cmbSendSpeed.SelectedIndexChanged += cmbSendSpeed_SelectedIndexChanged;
            //
            // lblSendSpeedHint
            //
            lblSendSpeedHint.AutoSize = true;
            lblSendSpeedHint.ForeColor = Color.FromArgb(90, 90, 90);
            lblSendSpeedHint.Location = new Point(300, 336);
            lblSendSpeedHint.Margin = new Padding(5, 0, 5, 0);
            lblSendSpeedHint.Name = "lblSendSpeedHint";
            lblSendSpeedHint.Size = new Size(306, 28);
            lblSendSpeedHint.TabIndex = 10;
            lblSendSpeedHint.Text = "稳定优先 / 快速 / 极速测试";
            //
            // lblLineCount
            // 
            lblLineCount.AutoSize = false;
            lblLineCount.Location = new Point(650, 30);
            lblLineCount.Margin = new Padding(5, 0, 5, 0);
            lblLineCount.Name = "lblLineCount";
            lblLineCount.Size = new Size(117, 28);
            lblLineCount.TabIndex = 7;
            lblLineCount.Text = "行数: 0";
            // 
            // grpQuickSend
            // 
            grpQuickSend.Controls.Add(btnSendBackspace);
            grpQuickSend.Controls.Add(btnSendSpace);
            grpQuickSend.Controls.Add(btnSendEnter);
            grpQuickSend.Controls.Add(btnMouseLeft);
            grpQuickSend.Controls.Add(btnMouseRight);
            grpQuickSend.Controls.Add(btnCapsLock);
            grpQuickSend.Controls.Add(btnNumLock);
            grpQuickSend.Controls.Add(btnScrollLock);
            grpQuickSend.Enabled = false;
            grpQuickSend.Location = new Point(21, 730);
            grpQuickSend.Margin = new Padding(5);
            grpQuickSend.Name = "grpQuickSend";
            grpQuickSend.Padding = new Padding(5);
            grpQuickSend.Size = new Size(805, 230);
            grpQuickSend.TabIndex = 2;
            grpQuickSend.TabStop = false;
            grpQuickSend.Text = "快捷发送";
            // 
            // btnSendBackspace
            // 
            btnSendBackspace.Location = new Point(525, 26);
            btnSendBackspace.Margin = new Padding(5);
            btnSendBackspace.Name = "btnSendBackspace";
            btnSendBackspace.Size = new Size(245, 52);
            btnSendBackspace.TabIndex = 2;
            btnSendBackspace.Text = "退格 (Backspace)";
            btnSendBackspace.UseVisualStyleBackColor = true;
            btnSendBackspace.Click += btnSendBackspace_Click;
            // 
            // btnSendSpace
            // 
            btnSendSpace.Location = new Point(262, 26);
            btnSendSpace.Margin = new Padding(5);
            btnSendSpace.Name = "btnSendSpace";
            btnSendSpace.Size = new Size(245, 52);
            btnSendSpace.TabIndex = 1;
            btnSendSpace.Text = "空格 (Space)";
            btnSendSpace.UseVisualStyleBackColor = true;
            btnSendSpace.Click += btnSendSpace_Click;
            // 
            // btnSendEnter
            // 
            btnSendEnter.Location = new Point(35, 26);
            btnSendEnter.Margin = new Padding(5);
            btnSendEnter.Name = "btnSendEnter";
            btnSendEnter.Size = new Size(210, 52);
            btnSendEnter.TabIndex = 0;
            btnSendEnter.Text = "回车 (Enter)";
            btnSendEnter.UseVisualStyleBackColor = true;
            btnSendEnter.Click += btnSendEnter_Click;
            // 
            // btnMouseLeft
            // 
            btnMouseLeft.Location = new Point(35, 88);
            btnMouseLeft.Margin = new Padding(5);
            btnMouseLeft.Name = "btnMouseLeft";
            btnMouseLeft.Size = new Size(394, 52);
            btnMouseLeft.TabIndex = 3;
            btnMouseLeft.Text = "鼠标左键";
            btnMouseLeft.UseVisualStyleBackColor = true;
            btnMouseLeft.Click += btnMouseLeft_Click;
            // 
            // btnMouseRight
            // 
            btnMouseRight.Location = new Point(444, 88);
            btnMouseRight.Margin = new Padding(5);
            btnMouseRight.Name = "btnMouseRight";
            btnMouseRight.Size = new Size(326, 52);
            btnMouseRight.TabIndex = 4;
            btnMouseRight.Text = "鼠标右键";
            btnMouseRight.UseVisualStyleBackColor = true;
            btnMouseRight.Click += btnMouseRight_Click;
            // 
            // btnCapsLock
            // 
            btnCapsLock.Location = new Point(35, 150);
            btnCapsLock.Margin = new Padding(5);
            btnCapsLock.Name = "btnCapsLock";
            btnCapsLock.Size = new Size(255, 52);
            btnCapsLock.TabIndex = 5;
            btnCapsLock.Text = "Caps Lock";
            btnCapsLock.UseVisualStyleBackColor = true;
            btnCapsLock.Click += btnCapsLock_Click;
            // 
            // btnNumLock
            // 
            btnNumLock.Location = new Point(300, 150);
            btnNumLock.Margin = new Padding(5);
            btnNumLock.Name = "btnNumLock";
            btnNumLock.Size = new Size(255, 52);
            btnNumLock.TabIndex = 6;
            btnNumLock.Text = "Num Lock";
            btnNumLock.UseVisualStyleBackColor = true;
            btnNumLock.Click += btnNumLock_Click;
            // 
            // btnScrollLock
            // 
            btnScrollLock.Location = new Point(565, 150);
            btnScrollLock.Margin = new Padding(5);
            btnScrollLock.Name = "btnScrollLock";
            btnScrollLock.Size = new Size(205, 52);
            btnScrollLock.TabIndex = 7;
            btnScrollLock.Text = "Scroll Lock";
            btnScrollLock.UseVisualStyleBackColor = true;
            btnScrollLock.Click += btnScrollLock_Click;
            // 
            // lblStatus
            // 
            lblStatus.AutoSize = true;
            lblStatus.Font = new Font("Microsoft YaHei UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            lblStatus.ForeColor = Color.FromArgb(0, 100, 200);
            lblStatus.Location = new Point(21, 1642);
            lblStatus.Margin = new Padding(5, 0, 5, 0);
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new Size(54, 28);
            lblStatus.TabIndex = 4;
            lblStatus.Text = "就绪";
            // 
            // grpHotKeys
            // 
            grpHotKeys.Controls.Add(btnArrowDown);
            grpHotKeys.Controls.Add(btnArrowRight);
            grpHotKeys.Controls.Add(btnArrowLeft);
            grpHotKeys.Controls.Add(btnArrowUp);
            grpHotKeys.Controls.Add(btnAltTab);
            grpHotKeys.Controls.Add(btnWinD);
            grpHotKeys.Controls.Add(btnCtrlZ);
            grpHotKeys.Controls.Add(btnCtrlX);
            grpHotKeys.Controls.Add(btnCtrlV);
            grpHotKeys.Controls.Add(btnCtrlC);
            grpHotKeys.Controls.Add(btnCtrlA);
            grpHotKeys.Enabled = false;
            grpHotKeys.Location = new Point(21, 960);
            grpHotKeys.Margin = new Padding(5);
            grpHotKeys.Name = "grpHotKeys";
            grpHotKeys.Padding = new Padding(5);
            grpHotKeys.Size = new Size(805, 254);
            grpHotKeys.TabIndex = 3;
            grpHotKeys.TabStop = false;
            grpHotKeys.Text = "组合键";
            // 
            // btnArrowDown
            //
            btnArrowDown.Location = new Point(262, 184);
            btnArrowDown.Margin = new Padding(5);
            btnArrowDown.Name = "btnArrowDown";
            btnArrowDown.Size = new Size(70, 52);
            btnArrowDown.TabIndex = 9;
            btnArrowDown.Text = "↓";
            btnArrowDown.UseVisualStyleBackColor = true;
            btnArrowDown.Click += btnArrowDown_Click;
            //
            // btnArrowRight
            //
            btnArrowRight.Location = new Point(341, 184);
            btnArrowRight.Margin = new Padding(5);
            btnArrowRight.Name = "btnArrowRight";
            btnArrowRight.Size = new Size(70, 52);
            btnArrowRight.TabIndex = 7;
            btnArrowRight.Text = "→";
            btnArrowRight.UseVisualStyleBackColor = true;
            btnArrowRight.Click += btnArrowRight_Click;
            //
            // btnArrowLeft
            //
            btnArrowLeft.Location = new Point(184, 184);
            btnArrowLeft.Margin = new Padding(5);
            btnArrowLeft.Name = "btnArrowLeft";
            btnArrowLeft.Size = new Size(70, 52);
            btnArrowLeft.TabIndex = 8;
            btnArrowLeft.Text = "←";
            btnArrowLeft.UseVisualStyleBackColor = true;
            btnArrowLeft.Click += btnArrowLeft_Click;
            //
            // btnArrowUp
            //
            btnArrowUp.Location = new Point(262, 122);
            btnArrowUp.Margin = new Padding(5);
            btnArrowUp.Name = "btnArrowUp";
            btnArrowUp.Size = new Size(70, 52);
            btnArrowUp.TabIndex = 10;
            btnArrowUp.Text = "↑";
            btnArrowUp.UseVisualStyleBackColor = true;
            btnArrowUp.Click += btnArrowUp_Click;
            //
            // btnAltTab
            //
            btnAltTab.Location = new Point(35, 122);
            btnAltTab.Margin = new Padding(5);
            btnAltTab.Name = "btnAltTab";
            btnAltTab.Size = new Size(135, 114);
            btnAltTab.TabIndex = 6;
            btnAltTab.Text = "Alt+Tab\r\n(切换程序)";
            btnAltTab.UseVisualStyleBackColor = true;
            btnAltTab.Click += btnAltTab_Click;
            //
            // btnWinD
            //
            btnWinD.Location = new Point(428, 122);
            btnWinD.Margin = new Padding(5);
            btnWinD.Name = "btnWinD";
            btnWinD.Size = new Size(170, 114);
            btnWinD.TabIndex = 5;
            btnWinD.Text = "Win+D\r\n(显示桌面)";
            btnWinD.UseVisualStyleBackColor = true;
            btnWinD.Click += btnWinD_Click;
            //
            // btnCtrlZ
            //
            btnCtrlZ.Location = new Point(610, 122);
            btnCtrlZ.Margin = new Padding(5);
            btnCtrlZ.Name = "btnCtrlZ";
            btnCtrlZ.Size = new Size(146, 114);
            btnCtrlZ.TabIndex = 4;
            btnCtrlZ.Text = "Ctrl+Z\r\n(撤销)";
            btnCtrlZ.UseVisualStyleBackColor = true;
            btnCtrlZ.Click += btnCtrlZ_Click;
            // 
            // btnCtrlX
            // 
            btnCtrlX.Location = new Point(564, 26);
            btnCtrlX.Margin = new Padding(5);
            btnCtrlX.Name = "btnCtrlX";
            btnCtrlX.Size = new Size(192, 52);
            btnCtrlX.TabIndex = 3;
            btnCtrlX.Text = "Ctrl+X (剪切)";
            btnCtrlX.UseVisualStyleBackColor = true;
            btnCtrlX.Click += btnCtrlX_Click;
            // 
            // btnCtrlV
            // 
            btnCtrlV.Location = new Point(366, 26);
            btnCtrlV.Margin = new Padding(5);
            btnCtrlV.Name = "btnCtrlV";
            btnCtrlV.Size = new Size(166, 52);
            btnCtrlV.TabIndex = 2;
            btnCtrlV.Text = "Ctrl+V (粘贴)";
            btnCtrlV.UseVisualStyleBackColor = true;
            btnCtrlV.Click += btnCtrlV_Click;
            // 
            // btnCtrlC
            // 
            btnCtrlC.Location = new Point(184, 26);
            btnCtrlC.Margin = new Padding(5);
            btnCtrlC.Name = "btnCtrlC";
            btnCtrlC.Size = new Size(131, 52);
            btnCtrlC.TabIndex = 1;
            btnCtrlC.Text = "Ctrl+C";
            btnCtrlC.UseVisualStyleBackColor = true;
            btnCtrlC.Click += btnCtrlC_Click;
            // 
            // btnCtrlA
            // 
            btnCtrlA.Location = new Point(35, 26);
            btnCtrlA.Margin = new Padding(5);
            btnCtrlA.Name = "btnCtrlA";
            btnCtrlA.Size = new Size(131, 52);
            btnCtrlA.TabIndex = 0;
            btnCtrlA.Text = "Ctrl+A";
            btnCtrlA.UseVisualStyleBackColor = true;
            btnCtrlA.Click += btnCtrlA_Click;
            // 
            // grpSync
            // 
            grpSync.Controls.Add(btnKeyboardSync);
            grpSync.Enabled = false;
            grpSync.Location = new Point(21, 1219);
            grpSync.Margin = new Padding(5);
            grpSync.Name = "grpSync";
            grpSync.Padding = new Padding(5);
            grpSync.Size = new Size(805, 96);
            grpSync.TabIndex = 5;
            grpSync.TabStop = false;
            grpSync.Text = "键盘同步";
            // 
            // btnKeyboardSync
            // 
            btnKeyboardSync.BackColor = Color.FromArgb(200, 200, 200);
            btnKeyboardSync.FlatStyle = FlatStyle.Flat;
            btnKeyboardSync.Font = new Font("Microsoft YaHei UI", 10F, FontStyle.Bold, GraphicsUnit.Point);
            btnKeyboardSync.Location = new Point(35, 26);
            btnKeyboardSync.Margin = new Padding(5);
            btnKeyboardSync.Name = "btnKeyboardSync";
            btnKeyboardSync.Size = new Size(735, 52);
            btnKeyboardSync.TabIndex = 0;
            btnKeyboardSync.Text = "开启键盘同步";
            btnKeyboardSync.UseVisualStyleBackColor = false;
            btnKeyboardSync.Click += btnKeyboardSync_Click;
            // 
            // grpRemoteMouse
            //
            grpRemoteMouse.Controls.Add(lblRemoteMouseHint);
            grpRemoteMouse.Controls.Add(btnRemoteMouseMode);
            grpRemoteMouse.Enabled = false;
            grpRemoteMouse.Location = new Point(21, 1320);
            grpRemoteMouse.Margin = new Padding(5);
            grpRemoteMouse.Name = "grpRemoteMouse";
            grpRemoteMouse.Padding = new Padding(5);
            grpRemoteMouse.Size = new Size(805, 130);
            grpRemoteMouse.TabIndex = 6;
            grpRemoteMouse.TabStop = false;
            grpRemoteMouse.Text = "远程鼠标模式";
            //
            // btnRemoteMouseMode
            //
            btnRemoteMouseMode.BackColor = Color.FromArgb(200, 200, 200);
            btnRemoteMouseMode.FlatStyle = FlatStyle.Flat;
            btnRemoteMouseMode.Font = new Font("Microsoft YaHei UI", 10F, FontStyle.Bold, GraphicsUnit.Point);
            btnRemoteMouseMode.Location = new Point(35, 30);
            btnRemoteMouseMode.Margin = new Padding(5);
            btnRemoteMouseMode.Name = "btnRemoteMouseMode";
            btnRemoteMouseMode.Size = new Size(735, 46);
            btnRemoteMouseMode.TabIndex = 0;
            btnRemoteMouseMode.Text = "开启远程鼠标模式";
            btnRemoteMouseMode.UseVisualStyleBackColor = false;
            btnRemoteMouseMode.Click += btnRemoteMouseMode_Click;
            //
            // lblRemoteMouseHint
            //
            lblRemoteMouseHint.AutoSize = true;
            lblRemoteMouseHint.ForeColor = Color.FromArgb(90, 90, 90);
            lblRemoteMouseHint.Location = new Point(35, 86);
            lblRemoteMouseHint.Margin = new Padding(5, 0, 5, 0);
            lblRemoteMouseHint.Name = "lblRemoteMouseHint";
            lblRemoteMouseHint.Size = new Size(544, 28);
            lblRemoteMouseHint.TabIndex = 1;
            lblRemoteMouseHint.Text = "开启后转发移动/左键/右键/滚轮，按 F12 立即退出";
            //
            // grpModifiers
            // 
            grpModifiers.Controls.Add(btnAlt);
            grpModifiers.Controls.Add(btnShift);
            grpModifiers.Controls.Add(btnCtrl);
            grpModifiers.Enabled = false;
            grpModifiers.Location = new Point(21, 1458);
            grpModifiers.Margin = new Padding(5);
            grpModifiers.Name = "grpModifiers";
            grpModifiers.Padding = new Padding(5);
            grpModifiers.Size = new Size(805, 96);
            grpModifiers.TabIndex = 6;
            grpModifiers.TabStop = false;
            grpModifiers.Text = "功能键保持";
            // 
            // btnAlt
            // 
            btnAlt.BackColor = Color.FromArgb(200, 200, 200);
            btnAlt.FlatStyle = FlatStyle.Flat;
            btnAlt.Font = new Font("Microsoft YaHei UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
            btnAlt.Location = new Point(525, 26);
            btnAlt.Margin = new Padding(5);
            btnAlt.Name = "btnAlt";
            btnAlt.Size = new Size(245, 52);
            btnAlt.TabIndex = 2;
            btnAlt.Text = "Alt";
            btnAlt.UseVisualStyleBackColor = false;
            btnAlt.Click += btnAlt_Click;
            // 
            // btnShift
            // 
            btnShift.BackColor = Color.FromArgb(200, 200, 200);
            btnShift.FlatStyle = FlatStyle.Flat;
            btnShift.Font = new Font("Microsoft YaHei UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
            btnShift.Location = new Point(280, 26);
            btnShift.Margin = new Padding(5);
            btnShift.Name = "btnShift";
            btnShift.Size = new Size(228, 52);
            btnShift.TabIndex = 1;
            btnShift.Text = "Shift";
            btnShift.UseVisualStyleBackColor = false;
            btnShift.Click += btnShift_Click;
            // 
            // btnCtrl
            // 
            btnCtrl.BackColor = Color.FromArgb(200, 200, 200);
            btnCtrl.FlatStyle = FlatStyle.Flat;
            btnCtrl.Font = new Font("Microsoft YaHei UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
            btnCtrl.Location = new Point(35, 26);
            btnCtrl.Margin = new Padding(5);
            btnCtrl.Name = "btnCtrl";
            btnCtrl.Size = new Size(228, 52);
            btnCtrl.TabIndex = 0;
            btnCtrl.Text = "Ctrl";
            btnCtrl.UseVisualStyleBackColor = false;
            btnCtrl.Click += btnCtrl_Click;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(168F, 168F);
            AutoScaleMode = AutoScaleMode.Dpi;
            ClientSize = new Size(847, 1695);
            Controls.Add(lblStatus);
            Controls.Add(grpRemoteMouse);
            Controls.Add(grpSync);
            Controls.Add(grpModifiers);
            Controls.Add(grpHotKeys);
            Controls.Add(grpQuickSend);
            Controls.Add(grpSend);
            Controls.Add(grpConnection);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Margin = new Padding(5);
            MaximizeBox = false;
            Name = "MainForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "CH9329 键盘模拟控制软件";
            FormClosing += MainForm_FormClosing;
            grpConnection.ResumeLayout(false);
            grpConnection.PerformLayout();
            grpSend.ResumeLayout(false);
            grpSend.PerformLayout();
            grpQuickSend.ResumeLayout(false);
            grpHotKeys.ResumeLayout(false);
            grpSync.ResumeLayout(false);
            grpRemoteMouse.ResumeLayout(false);
            grpRemoteMouse.PerformLayout();
            grpModifiers.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.GroupBox grpConnection;
        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.ComboBox cmbComPort;
        private System.Windows.Forms.Label lblComPort;
        private System.Windows.Forms.Label lblBaudRate;
        private System.Windows.Forms.ComboBox cmbBaudRate;
        private System.Windows.Forms.Button btnRefresh;

        private System.Windows.Forms.GroupBox grpSend;
        private System.Windows.Forms.TextBox txtSend;
        private System.Windows.Forms.Button btnSend;
        private System.Windows.Forms.Button btnStopSend;
        private System.Windows.Forms.Label lblSend;
        private System.Windows.Forms.GroupBox grpQuickSend;
        private System.Windows.Forms.Button btnSendEnter;
        private System.Windows.Forms.Button btnSendSpace;
        private System.Windows.Forms.Button btnSendBackspace;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.Button btnOpenFile;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.GroupBox grpHotKeys;
        private System.Windows.Forms.Button btnAltTab;
        private System.Windows.Forms.Button btnWinD;
        private System.Windows.Forms.Button btnCtrlA;
        private System.Windows.Forms.Button btnCtrlC;
        private System.Windows.Forms.Button btnCtrlV;
        private System.Windows.Forms.Button btnCtrlX;
        private System.Windows.Forms.Button btnCtrlZ;
        private System.Windows.Forms.GroupBox grpSync;
        private System.Windows.Forms.Button btnKeyboardSync;
        private System.Windows.Forms.GroupBox grpRemoteMouse;
        private System.Windows.Forms.Button btnRemoteMouseMode;
        private System.Windows.Forms.Label lblRemoteMouseHint;
        private System.Windows.Forms.Button btnArrowUp;
        private System.Windows.Forms.Button btnArrowDown;
        private System.Windows.Forms.Button btnArrowLeft;
        private System.Windows.Forms.Button btnArrowRight;
        private System.Windows.Forms.GroupBox grpModifiers;
        private System.Windows.Forms.Button btnCtrl;
        private System.Windows.Forms.Button btnShift;
        private System.Windows.Forms.Button btnAlt;
        private System.Windows.Forms.Button btnMouseLeft;
        private System.Windows.Forms.Button btnMouseRight;
        private System.Windows.Forms.Button btnCapsLock;
        private System.Windows.Forms.Button btnNumLock;
        private System.Windows.Forms.Button btnScrollLock;
        private System.Windows.Forms.ProgressBar progressBarSend;
        private System.Windows.Forms.Label lblLineCount;
        private System.Windows.Forms.Label lblSendSpeed;
        private System.Windows.Forms.ComboBox cmbSendSpeed;
        private System.Windows.Forms.Label lblSendSpeedHint;

    }
}
