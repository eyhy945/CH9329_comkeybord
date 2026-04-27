using System;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KeyboardController
{
    public partial class MainForm : Form {
        private SerialPortManager? _serialPortManager;

        private CH9329Keyboard? _keyboardController;
        private InputHook? _inputHook;
        private bool _keyboardSyncEnabled = false;
        private bool _remoteMouseModeEnabled = false;
        private bool _suppressCurrentMouseEvent = false;
        private Point? _lastMousePosition;
        private bool _ctrlPressed = false;
        private bool _shiftPressed = false;
        private bool _altPressed = false;

        private enum SendSpeedPreset {
            Stable,
            Fast,
            Extreme
        }

        private sealed class UserPreferences {
            public SendSpeedPreset SendSpeedPreset { get; set; } = SendSpeedPreset.Stable;
        }

        private readonly record struct SendSpeedSettings(string DisplayName, int KeyReleaseDelayMs, int CharIntervalDelayMs);
        private static readonly string PreferencesFilePath = Path.Combine(AppContext.BaseDirectory, "user-preferences.json");
        private const int GlobalPasteHotkeyId = 0x1200;
        private const int GlobalSelectionHotkeyId = 0x1201;
        private const int WM_HOTKEY = 0x0312;
        private const uint MOD_ALT = 0x0001;

        public MainForm() {
            InitializeComponent();
            InitializeController();
            cmbBaudRate.SelectedIndex = 1;
            cmbSendSpeed.SelectedIndex = GetStoredSendSpeedIndex();
            RefreshSendSpeedHint();
            RefreshComPorts();
            RegisterGlobalPasteHotkey();
        }

        private void InitializeController() {
            // 默认使用串口模式
            _serialPortManager = new SerialPortManager();
            _serialPortManager.StatusChanged += OnStatusChanged;
            _keyboardController = new CH9329Keyboard(_serialPortManager);
            _keyboardController.KeyboardDataReceived += OnKeyboardDataReceived;



            // 初始化输入钩子
            _inputHook = new InputHook();
            _inputHook.Install();
            _inputHook.KeyDown += OnKeyDown;
            _inputHook.KeyUp += OnKeyUp;
            _inputHook.MouseAction += OnMouseAction;
            _inputHook.KeyboardInterceptor = ShouldBlockKeyboardEvent;
            _inputHook.MouseInterceptor = ShouldBlockMouseEvent;

            // 初始化文本解码器

        }

        private SendSpeedPreset CurrentSendSpeedPreset => cmbSendSpeed.SelectedIndex switch {
            1 => SendSpeedPreset.Fast,
            2 => SendSpeedPreset.Extreme,
            _ => SendSpeedPreset.Stable
        };

        private SendSpeedSettings GetSendSpeedSettings() {
            return CurrentSendSpeedPreset switch {
                SendSpeedPreset.Fast => new SendSpeedSettings("快速", 1, 0),
                SendSpeedPreset.Extreme => new SendSpeedSettings("极速", 0, 0),
                _ => new SendSpeedSettings("稳定", 2, 1)
            };
        }

        private int GetStoredSendSpeedIndex() {
            try {
                if (!File.Exists(PreferencesFilePath)) {
                    return 0;
                }

                var json = File.ReadAllText(PreferencesFilePath);
                var preferences = JsonSerializer.Deserialize<UserPreferences>(json);
                return preferences?.SendSpeedPreset switch {
                    SendSpeedPreset.Fast => 1,
                    SendSpeedPreset.Extreme => 2,
                    _ => 0
                };
            }
            catch {
                return 0;
            }
        }

        private void SavePreferences() {
            try {
                var preferences = new UserPreferences {
                    SendSpeedPreset = CurrentSendSpeedPreset
                };
                var json = JsonSerializer.Serialize(preferences, new JsonSerializerOptions {
                    WriteIndented = true
                });
                File.WriteAllText(PreferencesFilePath, json);
            }
            catch {
            }
        }

        private void RefreshSendSpeedHint() {
            var settings = GetSendSpeedSettings();
            lblSendSpeedHint.Text = settings.DisplayName switch {
                "快速" => "较快发送，适合大多数场景",
                "极速" => "极速测试，优先速度",
                _ => "稳定优先，兼容性更好"
            };
        }

        private void RefreshComPorts() {
            string[] ports = _serialPortManager?.AvailablePorts ?? Array.Empty<string>();
            cmbComPort.Items.Clear();
            cmbComPort.Items.AddRange(ports);
            if (ports.Length > 0) {
                cmbComPort.SelectedIndex = 0;
            }
        }





        private void OnStatusChanged(object? sender, string message) {
            if (InvokeRequired) {
                Invoke(new Action(() => OnStatusChanged(sender, message)));
                return;
            }
            lblStatus.Text = message;
        }

        private void btnConnect_Click(object sender, EventArgs e) {
            if (btnConnect.Text == "连接") {
                // 串口模式连接
                ConnectSerialMode();
            }
            else {
                // 断开连接
                DisconnectDevice();
            }
        }



        /// <summary>
        /// 串口模式连接
        /// </summary>
        private void ConnectSerialMode() {
            if (cmbComPort.SelectedItem == null) {
                MessageBox.Show("请选择串口", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string port = cmbComPort.SelectedItem.ToString()!;
            int baudRate = int.Parse(cmbBaudRate.SelectedItem.ToString()!);

            if (_serialPortManager != null && _serialPortManager.Connect(port, baudRate)) {

                _keyboardController?.SetConnectionMode(CH9329Keyboard.ConnectionMode.SerialPort, _serialPortManager);
                btnConnect.Text = "断开";
                btnConnect.BackColor = Color.FromArgb(255, 128, 128);
                RefreshComPorts();
                EnableControls(true);
            }
        }



        /// <summary>
        /// 断开设备连接
        /// </summary>
        private void DisconnectDevice() {
            // 断开串口
            _serialPortManager?.Disconnect();

            // 重置键盘控制器
            _keyboardController?.Disconnect();
            btnConnect.Text = "连接";
            btnConnect.BackColor = Color.FromArgb(128, 255, 128);
            EnableControls(false);
            RefreshComPorts();
        }

        private void EnableControls(bool enabled) {
            cmbComPort.Enabled = !enabled;
            cmbBaudRate.Enabled = !enabled;
            btnRefresh.Enabled = !enabled;
            grpSend.Enabled = enabled;
            grpQuickSend.Enabled = enabled;
            grpHotKeys.Enabled = enabled;
            grpSync.Enabled = enabled;
            grpModifiers.Enabled = enabled;
            grpRemoteMouse.Enabled = enabled;
            if (!enabled) {
                SetRemoteMouseMode(false, "远程鼠标模式已关闭");
                SetKeyboardSyncState(false, "键盘同步已关闭");
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e) {
            RefreshComPorts();
        }

        private CancellationTokenSource? _sendCts;

        private async void btnSend_Click(object sender, EventArgs e) {
            if (string.IsNullOrWhiteSpace(txtSend.Text)) {
                MessageBox.Show("请输入要发送的字符", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (_keyboardController != null) {
                // 取消之前的发送操作
                _sendCts?.Cancel();
                _sendCts = new CancellationTokenSource();

                try {
                    var speedSettings = GetSendSpeedSettings();

                    // 禁用发送按钮，启用停止按钮
                    btnSend.Enabled = false;
                    btnStopSend.Enabled = true;
                    lblStatus.Text = $"正在发送（{speedSettings.DisplayName}）...";

                    // 设置进度条
                    progressBarSend.Minimum = 0;
                    progressBarSend.Maximum = txtSend.Text.Length;
                    progressBarSend.Value = 0;
                    progressBarSend.Visible = true;

                    bool success = await _keyboardController.SendStringAsync(
                        txtSend.Text,
                        _sendCts.Token,
                        (current, total) => {
                            BeginInvoke(new Action(() => {
                                progressBarSend.Value = current;
                            }));
                        },
                        speedSettings.KeyReleaseDelayMs,
                        speedSettings.CharIntervalDelayMs);
                    lblStatus.Text = success ? $"已发送: {txtSend.Text}" : "发送失败";
                } catch (OperationCanceledException) {
                    lblStatus.Text = "发送已取消";
                } finally {
                    // 启用发送按钮，禁用停止按钮
                    btnSend.Enabled = true;
                    btnStopSend.Enabled = false;
                    _sendCts = null;
                    
                    // 重置进度条
                    progressBarSend.Value = 0;
                    progressBarSend.Visible = false;
                }
            }
        }

        private void btnOpenFile_Click(object sender, EventArgs e) {
            using OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "打开文件";
            openFileDialog.Filter = "文本文件|*.txt|脚本文件|*.bat;*.ps1;*.cmd;*.sh;*.py;*.js;*.cpp;*.c;*.h;*.java;*.cs;*.html;*.xml;*.json;*.yaml;*.yml;*.md|All Files|*.*";
            openFileDialog.FilterIndex = 1;

            if (openFileDialog.ShowDialog() == DialogResult.OK) {
                try {
                    string filePath = openFileDialog.FileName;
                    string content = File.ReadAllText(filePath);
                    txtSend.Text = content;
                    lblStatus.Text = $"已加载文件: {Path.GetFileName(filePath)}";
                } catch (Exception ex) {
                    MessageBox.Show($"读取文件失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnStopSend_Click(object sender, EventArgs e) {
            // 取消发送操作
            _sendCts?.Cancel();
            _keyboardController?.StopSending();
            lblStatus.Text = "发送已取消";
            btnSend.Enabled = true;
            btnStopSend.Enabled = false;
            
            // 重置进度条
            progressBarSend.Value = 0;
            progressBarSend.Visible = false;
        }

        private void btnSendEnter_Click(object sender, EventArgs e) {
            _keyboardController?.SendChar('\r');
            lblStatus.Text = "已发送回车键";
        }

        private void btnSendSpace_Click(object sender, EventArgs e) {
            _keyboardController?.SendChar(' ');
            lblStatus.Text = "已发送空格键";
        }

        private void btnSendBackspace_Click(object sender, EventArgs e) {
            _keyboardController?.SendChar('\b');
            lblStatus.Text = "已发送退格键";
        }

        private void cmbSendSpeed_SelectedIndexChanged(object? sender, EventArgs e) {
            RefreshSendSpeedHint();
            SavePreferences();
        }

        private void btnMouseLeft_Click(object? sender, EventArgs e) {
            if (_keyboardController?.SendMouseClick(0x01) == true) {
                lblStatus.Text = "已发送 → (鼠标左键)";
                return;
            }

            lblStatus.Text = "鼠标左键发送失败";
        }

        private void btnMouseRight_Click(object? sender, EventArgs e) {
            if (_keyboardController?.SendMouseClick(0x02) == true) {
                lblStatus.Text = "已发送 → (鼠标右键)";
                return;
            }

            lblStatus.Text = "鼠标右键发送失败";
        }

        private void btnCapsLock_Click(object? sender, EventArgs e) {
            // 发送 Caps Lock
            _keyboardController?.SendKey(0x00, new byte[] { 0x39 });
            lblStatus.Text = "已发送 → (Caps Lock)";
        }

        private void btnNumLock_Click(object? sender, EventArgs e) {
            // 发送 Num Lock
            _keyboardController?.SendKey(0x00, new byte[] { 0x53 });
            lblStatus.Text = "已发送 → (Num Lock)";
        }

        private void btnScrollLock_Click(object? sender, EventArgs e) {
            // 发送 Scroll Lock
            _keyboardController?.SendKey(0x00, new byte[] { 0x46 });
            lblStatus.Text = "已发送 → (Scroll Lock)";
        }

        private void btnClear_Click(object sender, EventArgs e) {
            txtSend.Clear();
            txtSend.Focus();
        }

        private void RegisterGlobalPasteHotkey() {
            if (!IsHandleCreated) {
                return;
            }

            if (!RegisterHotKey(Handle, GlobalPasteHotkeyId, MOD_ALT, (uint)Keys.R)) {
                lblStatus.Text = "Alt+R 全局快捷键注册失败，可能已被占用";
            }

            if (!RegisterHotKey(Handle, GlobalSelectionHotkeyId, MOD_ALT, (uint)Keys.T)) {
                lblStatus.Text = "Alt+T 全局快捷键注册失败，可能已被占用";
            }
        }

        private void UnregisterGlobalPasteHotkey() {
            if (IsHandleCreated) {
                UnregisterHotKey(Handle, GlobalPasteHotkeyId);
                UnregisterHotKey(Handle, GlobalSelectionHotkeyId);
            }
        }

        protected override void OnHandleCreated(EventArgs e) {
            base.OnHandleCreated(e);
            RegisterGlobalPasteHotkey();
        }

        protected override void OnHandleDestroyed(EventArgs e) {
            UnregisterGlobalPasteHotkey();
            base.OnHandleDestroyed(e);
        }

        protected override void WndProc(ref Message m) {
            if (m.Msg == WM_HOTKEY && m.WParam == (IntPtr)GlobalPasteHotkeyId) {
                PrepareClipboardText();
                return;
            }

            if (m.Msg == WM_HOTKEY && m.WParam == (IntPtr)GlobalSelectionHotkeyId) {
                PrepareSelectedText();
                return;
            }

            base.WndProc(ref m);
        }

        private void PrepareClipboardText() {
            if (WindowState == FormWindowState.Minimized) {
                WindowState = FormWindowState.Normal;
            }

            Show();
            Activate();
            BringToFront();
            btnClear_Click(this, EventArgs.Empty);

            if (!Clipboard.ContainsText()) {
                lblStatus.Text = "剪贴板中没有可粘贴的文本";
                return;
            }

            txtSend.Text = Clipboard.GetText();
            txtSend.Focus();
            txtSend.SelectionStart = txtSend.TextLength;
            lblStatus.Text = "已载入剪贴板文本";
        }

        private async void PrepareSelectedText() {
            string? originalClipboardText = Clipboard.ContainsText() ? Clipboard.GetText() : null;

            try {
                SendKeys.SendWait("^c");
                await Task.Delay(120);

                if (!Clipboard.ContainsText()) {
                    lblStatus.Text = "当前没有可复制的选中文本";
                    return;
                }

                PrepareClipboardText();
                lblStatus.Text = "已载入当前选中文本";
            }
            finally {
                if (originalClipboardText != null) {
                    Clipboard.SetText(originalClipboardText);
                }
            }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e) {
            UnregisterGlobalPasteHotkey();

            // 关闭同步
            if (_keyboardSyncEnabled) {
                _inputHook?.Dispose();
            }

            // 取消发送并释放所有按键
            _sendCts?.Cancel();
            _keyboardController?.StopSending();

            // 断开串口
            _serialPortManager?.Disconnect();
            _serialPortManager?.Dispose();


            // 释放键盘控制器
            _keyboardController?.Disconnect();
        }

        private void txtSend_KeyPress(object sender, KeyPressEventArgs e) {
            if (e.KeyChar == (char)Keys.Enter) {
                btnSend_Click(sender, e);
                e.Handled = true;
            }
        }

        private void txtSend_KeyDown(object? sender, KeyEventArgs e) {
            // Ctrl+A 全选
            if (e.Control && e.KeyCode == Keys.A) {
                txtSend.SelectAll();
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
        }

        private void txtSend_TextChanged(object? sender, EventArgs e) {
            // 计算行数并更新显示
            int lineCount = txtSend.Lines.Length;
            if (lblLineCount != null) {
                lblLineCount.Text = $"行数: {lineCount}";
            }
        }

        private void btnCtrlA_Click(object sender, EventArgs e) {
            _keyboardController?.SendCtrlA();
            lblStatus.Text = "已发送 Ctrl+A (全选)";
        }

        private void btnCtrlC_Click(object sender, EventArgs e) {
            _keyboardController?.SendCtrlC();
            lblStatus.Text = "已发送 Ctrl+C (复制)";
        }

        private void btnCtrlV_Click(object sender, EventArgs e) {
            _keyboardController?.SendCtrlV();
            lblStatus.Text = "已发送 Ctrl+V (粘贴)";
        }

        private void btnCtrlZ_Click(object sender, EventArgs e) {
            _keyboardController?.SendCtrlZ();
            lblStatus.Text = "已发送 Ctrl+Z (撤销)";
        }

        private void btnCtrlX_Click(object sender, EventArgs e) {
            _keyboardController?.SendCtrlX();
            lblStatus.Text = "已发送 Ctrl+X (剪切)";
        }

        private void btnWinD_Click(object sender, EventArgs e) {
            _keyboardController?.SendWinD();
            lblStatus.Text = "已发送 Win+D (显示桌面)";
        }

        private void btnAltTab_Click(object sender, EventArgs e) {
            _keyboardController?.SendAltTab();
            lblStatus.Text = "已发送 Alt+Tab (切换程序)";
        }

        private void btnArrowUp_Click(object sender, EventArgs e) {
            _keyboardController?.SendArrowUp();
            lblStatus.Text = "已发送 ↑ (上箭头)";
        }

        private void btnArrowDown_Click(object sender, EventArgs e) {
            _keyboardController?.SendArrowDown();
            lblStatus.Text = "已发送 ↓ (下箭头)";
        }

        private void btnArrowLeft_Click(object sender, EventArgs e) {
            _keyboardController?.SendArrowLeft();
            lblStatus.Text = "已发送 ← (左箭头)";
        }

        private void btnArrowRight_Click(object sender, EventArgs e) {
            _keyboardController?.SendArrowRight();
            lblStatus.Text = "已发送 → (右箭头)";
        }

        private void btnKeyboardSync_Click(object sender, EventArgs e) {
            if (_inputHook == null || _keyboardController == null)
                return;

            SetKeyboardSyncState(!_keyboardSyncEnabled);
        }

        private void SetKeyboardSyncState(bool enabled, string? statusMessage = null) {
            _keyboardSyncEnabled = enabled;
            if (_inputHook != null) {
                _inputHook.KeyboardEnabled = enabled;
            }

            if (_keyboardSyncEnabled) {
                btnKeyboardSync.Text = "关闭键盘同步";
                btnKeyboardSync.BackColor = Color.FromArgb(255, 128, 128);
                lblStatus.Text = statusMessage ?? "键盘同步已开启";
            }
            else {
                btnKeyboardSync.Text = "开启键盘同步";
                btnKeyboardSync.BackColor = Color.FromArgb(200, 200, 200);
                lblStatus.Text = statusMessage ?? "键盘同步已关闭";
            }
        }

        private void OnKeyDown(object? sender, KeyEventArgs e) {
            if (_keyboardController == null) {
                return;
            }

            bool ctrl = (Control.ModifierKeys & Keys.Control) == Keys.Control;
            bool shift = (Control.ModifierKeys & Keys.Shift) == Keys.Shift;
            bool alt = (Control.ModifierKeys & Keys.Alt) == Keys.Alt;

            if (e.KeyCode == Keys.F12) {
                SetRemoteMouseMode(false, "远程鼠标模式已关闭 (F12)");
                e.Handled = true;
                e.SuppressKeyPress = true;
                return;
            }

            if (ctrl && alt && e.KeyCode == Keys.X) {
                SetKeyboardSyncState(!_keyboardSyncEnabled, _keyboardSyncEnabled ? "键盘同步已关闭 (Ctrl+Alt+X)" : "键盘同步已开启 (Ctrl+Alt+X)");
                e.Handled = true;
                e.SuppressKeyPress = true;
                return;
            }

            if (ctrl && alt && e.KeyCode == Keys.M) {
                SetRemoteMouseMode(!_remoteMouseModeEnabled, _remoteMouseModeEnabled ? "远程鼠标模式已关闭 (Ctrl+Alt+M)" : "远程鼠标模式已开启 (Ctrl+Alt+M)");
                e.Handled = true;
                e.SuppressKeyPress = true;
                return;
            }

            if (!_keyboardSyncEnabled) {
                return;
            }

            int keyCode = (int)e.KeyCode;
            byte chKeyCode = _keyboardController.VirtualKeyToKeyCode(keyCode);

            if (chKeyCode != 0x00) {
                byte controlKey = _keyboardController.GetControlKeyState(ctrl, shift, alt, false);
                _keyboardController.KeyDown(controlKey, new byte[] { chKeyCode });
            }
        }

        private void OnKeyUp(object? sender, KeyEventArgs e) {
            if (_keyboardSyncEnabled && _keyboardController != null) {
                _keyboardController.KeyUp();
            }
        }

        private static bool IsRemoteMouseExitShortcut(KeyEventArgs e) {
            return e.KeyCode == Keys.F12;
        }

        private bool ShouldBlockKeyboardEvent(KeyEventArgs e) {
            return _remoteMouseModeEnabled && !IsRemoteMouseExitShortcut(e);
        }

        private bool ShouldBlockMouseEvent(InputHook.MouseHookEventArgs e) {
            return _suppressCurrentMouseEvent;
        }

        private void OnMouseAction(object? sender, InputHook.MouseHookEventArgs e) {
            _suppressCurrentMouseEvent = false;

            if (!_remoteMouseModeEnabled || _keyboardController == null) {
                _lastMousePosition = e.Location;
                return;
            }

            bool forwarded = e.ActionType switch {
                InputHook.MouseActionType.Move => ForwardMouseMove(e.Location),
                InputHook.MouseActionType.LeftDown => _keyboardController.SendMouseButton(0x01, true),
                InputHook.MouseActionType.LeftUp => _keyboardController.SendMouseButton(0x01, false),
                InputHook.MouseActionType.RightDown => _keyboardController.SendMouseButton(0x02, true),
                InputHook.MouseActionType.RightUp => _keyboardController.SendMouseButton(0x02, false),
                InputHook.MouseActionType.Wheel => _keyboardController.SendMouseWheel(e.WheelDelta),
                _ => false
            };

            _suppressCurrentMouseEvent = forwarded;
            if (!forwarded && _remoteMouseModeEnabled) {
                BeginInvoke(new Action(() => {
                    if (_remoteMouseModeEnabled) {
                        SetRemoteMouseMode(false, "远程鼠标转发失败，已自动退出以保护本机控制");
                    }
                }));
            }
        }

        private bool ForwardMouseMove(Point location) {
            bool forwarded = true;
            if (_lastMousePosition.HasValue) {
                int deltaX = location.X - _lastMousePosition.Value.X;
                int deltaY = location.Y - _lastMousePosition.Value.Y;
                if (deltaX != 0 || deltaY != 0) {
                    forwarded = _keyboardController!.SendMouseMoveRelative(deltaX, deltaY);
                }
            }

            _lastMousePosition = location;
            return forwarded;
        }

        private void btnRemoteMouseMode_Click(object sender, EventArgs e) {
            SetRemoteMouseMode(!_remoteMouseModeEnabled);
        }

        private void SetRemoteMouseMode(bool enabled, string? statusMessage = null) {
            _remoteMouseModeEnabled = enabled;
            _lastMousePosition = null;

            if (_inputHook != null) {
                _inputHook.MouseEnabled = enabled;
                _inputHook.KeyboardEnabled = enabled || _keyboardSyncEnabled;
            }

            if (_remoteMouseModeEnabled) {
                btnRemoteMouseMode.Text = "关闭远程鼠标模式";
                btnRemoteMouseMode.BackColor = Color.FromArgb(255, 128, 128);
                lblRemoteMouseHint.Text = "已开启，仅在转发成功时拦截本机鼠标，按 F12 立即退出";
                lblStatus.Text = statusMessage ?? "远程鼠标模式已开启";
            }
            else {
                btnRemoteMouseMode.Text = "开启远程鼠标模式";
                btnRemoteMouseMode.BackColor = Color.FromArgb(200, 200, 200);
                lblRemoteMouseHint.Text = "开启后转发移动/左键/右键/滚轮，按 F12 立即退出";
                lblStatus.Text = statusMessage ?? "远程鼠标模式已关闭";
            }
        }

        private void btnCtrl_Click(object sender, EventArgs e) {
            _ctrlPressed = !_ctrlPressed;
            if (_ctrlPressed) {
                btnCtrl.BackColor = Color.FromArgb(128, 255, 128);
                _keyboardController?.KeyDown(0x01, new byte[0]);
                lblStatus.Text = "Ctrl键已保持按下";
            }
            else {
                btnCtrl.BackColor = Color.FromArgb(200, 200, 200);
                _keyboardController?.KeyUp();
                lblStatus.Text = "Ctrl键已释放";
            }
        }

        private void btnShift_Click(object sender, EventArgs e) {
            _shiftPressed = !_shiftPressed;
            if (_shiftPressed) {
                btnShift.BackColor = Color.FromArgb(128, 255, 128);
                _keyboardController?.KeyDown(0x02, new byte[0]);
                lblStatus.Text = "Shift键已保持按下";
            }
            else {
                btnShift.BackColor = Color.FromArgb(200, 200, 200);
                _keyboardController?.KeyUp();
                lblStatus.Text = "Shift键已释放";
            }
        }

        private void btnAlt_Click(object sender, EventArgs e) {
            _altPressed = !_altPressed;
            if (_altPressed) {
                btnAlt.BackColor = Color.FromArgb(128, 255, 128);
                _keyboardController?.KeyDown(0x04, new byte[0]);
                lblStatus.Text = "Alt键已保持按下";
            }
            else {
                btnAlt.BackColor = Color.FromArgb(200, 200, 200);
                _keyboardController?.KeyUp();
                lblStatus.Text = "Alt键已释放";
            }
        }









        /// <summary>
        /// 处理CH9329回传的键盘数据
        /// </summary>
        private void OnKeyboardDataReceived(object? sender, CH9329Keyboard.KeyboardDataReceivedEventArgs e) {
            if (InvokeRequired) {
                Invoke(new Action(() => OnKeyboardDataReceived(sender, e)));
                return;
            }

            // 构建回传数据的描述
            string controlKeys = "";
            if ((e.ControlKey & 0x01) != 0) controlKeys += "Ctrl ";
            if ((e.ControlKey & 0x02) != 0) controlKeys += "Shift ";
            if ((e.ControlKey & 0x04) != 0) controlKeys += "Alt ";
            if ((e.ControlKey & 0x08) != 0) controlKeys += "Win ";

            string keys = "";
            foreach (byte keyCode in e.KeyCodes) {
                if (keyCode != 0x00) {
                    keys += $"{keyCode:X2} ";
                }
            }

            string status = e.IsPressed ? "按下" : "释放";
            string message = $"回传: {status} - 控制键: {controlKeys} - 按键: {keys}";
            
            // 显示回传数据
            lblStatus.Text = message;
            

        }

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);
    }
}
