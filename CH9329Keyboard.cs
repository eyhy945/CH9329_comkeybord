using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace KeyboardController
{
    /// <summary>
    /// CH9329键盘控制器
    /// </summary>
    public class CH9329Keyboard
    {
        private SerialPortManager? _serialPort;
        private CH9329HIDManager? _hidManager;
        private ConnectionMode _connectionMode = ConnectionMode.None;
        private CancellationTokenSource? _cts;

        public enum ConnectionMode
        {
            None,
            SerialPort,
            HID
        }

        public ConnectionMode CurrentMode => _connectionMode;

        // CH9329协议常量
        private const byte FRAME_HEAD1 = 0x57;
        private const byte FRAME_HEAD2 = 0xAB;
        private const byte ADDR_CODE = 0x00;
        private const byte CMD_KEYBOARD = 0x02;
        private const byte DATA_LENGTH = 0x08;
        private const byte CMD_KEYBOARD_RESPONSE = 0x03;
        private const byte CMD_MOUSE_ABSOLUTE = 0x04;
        private const byte CMD_MOUSE_BUTTON = 0x05;
        private const byte CMD_MOUSE_RELATIVE = 0x12;

        private const byte MOUSE_BUTTON_LEFT = 0x01;
        private const byte MOUSE_BUTTON_RIGHT = 0x02;
        private const byte MOUSE_BUTTON_MIDDLE = 0x04;

        // 控制键定义
        private const byte CTRL_LEFT = 0x01;
        private const byte SHIFT_LEFT = 0x02;
        private const byte ALT_LEFT = 0x04;
        private const byte GUI_LEFT = 0x08;

        // 常用键码
        private const byte KEY_A = 0x04;
        private const byte KEY_C = 0x06;
        private const byte KEY_D = 0x07;
        private const byte KEY_V = 0x19;
        private const byte KEY_X = 0x1B;
        private const byte KEY_Z = 0x1D;
        private const byte KEY_TAB = 0x2B;
        private const byte KEY_UP = 0x52;
        private const byte KEY_DOWN = 0x51;
        private const byte KEY_LEFT = 0x50;
        private const byte KEY_RIGHT = 0x4F;

        public event EventHandler<KeyboardDataReceivedEventArgs>? KeyboardDataReceived;

        /// <summary>
        /// 键盘数据接收事件参数
        /// </summary>
        public class KeyboardDataReceivedEventArgs : EventArgs
        {
            public byte ControlKey { get; set; }
            public byte[] KeyCodes { get; set; } = Array.Empty<byte>();
            public bool IsPressed { get; set; }
        }

        /// <summary>
        /// 使用串口模式创建
        /// </summary>
        public CH9329Keyboard(SerialPortManager serialPort)
        {
            _serialPort = serialPort;
            _serialPort.DataReceived += OnSerialDataReceived;
            _connectionMode = ConnectionMode.SerialPort;
        }

        /// <summary>
        /// 使用HID模式创建
        /// </summary>
        public CH9329Keyboard(CH9329HIDManager hidManager)
        {
            _hidManager = hidManager;
            _hidManager.DataReceived += OnHIDDataReceived;
            _connectionMode = ConnectionMode.HID;
        }

        /// <summary>
        /// 切换连接模式
        /// </summary>
        public void SetConnectionMode(ConnectionMode mode, object? manager = null)
        {
            // 断开之前的连接
            Disconnect();

            _connectionMode = mode;

            switch (mode)
            {
                case ConnectionMode.SerialPort:
                    if (manager is SerialPortManager serialPort)
                    {
                        _serialPort = serialPort;
                        _serialPort.DataReceived += OnSerialDataReceived;
                    }
                    break;

                case ConnectionMode.HID:
                    if (manager is CH9329HIDManager hidManager)
                    {
                        _hidManager = hidManager;
                        _hidManager.DataReceived += OnHIDDataReceived;
                    }
                    break;
            }
        }

        /// <summary>
        /// 断开当前连接
        /// </summary>
        public void Disconnect()
        {
            if (_serialPort != null)
            {
                _serialPort.DataReceived -= OnSerialDataReceived;
                _serialPort = null;
            }

            if (_hidManager != null)
            {
                _hidManager.DataReceived -= OnHIDDataReceived;
                _hidManager.Disconnect();
                _hidManager = null;
            }

            _connectionMode = ConnectionMode.None;
        }

        /// <summary>
        /// 处理串口数据接收
        /// </summary>
        private void OnSerialDataReceived(object? sender, byte[] data)
        {
            ParseReceivedData(data);
        }

        /// <summary>
        /// 处理HID数据接收
        /// </summary>
        private void OnHIDDataReceived(object? sender, byte[] data)
        {
            ParseReceivedData(data);
        }

        /// <summary>
        /// 计算累加和
        /// </summary>
        private byte CalculateChecksum(byte[] data, int start, int length)
        {
            int sum = 0;
            for (int i = start; i < start + length; i++)
            {
                sum += data[i];
            }
            return (byte)(sum % 256);
        }

        /// <summary>
        /// 发送键盘数据包
        /// </summary>
        private bool SendPacket(byte command, byte[] payload)
        {
            switch (_connectionMode)
            {
                case ConnectionMode.SerialPort:
                    if (_serialPort == null || !_serialPort.IsConnected)
                        return false;
                    break;
                case ConnectionMode.HID:
                    if (_hidManager == null || !_hidManager.IsConnected)
                        return false;
                    break;
                default:
                    return false;
            }

            try
            {
                byte[] packet = new byte[6 + payload.Length];
                int index = 0;
                packet[index++] = FRAME_HEAD1;
                packet[index++] = FRAME_HEAD2;
                packet[index++] = ADDR_CODE;
                packet[index++] = command;
                packet[index++] = (byte)payload.Length;
                Array.Copy(payload, 0, packet, index, payload.Length);
                packet[^1] = CalculateChecksum(packet, 0, packet.Length - 1);

                return _connectionMode switch
                {
                    ConnectionMode.SerialPort => _serialPort!.SendData(packet),
                    ConnectionMode.HID => _hidManager!.SendData(packet),
                    _ => false
                };
            }
            catch (Exception)
            {
                return false;
            }
        }

        private bool SendKeyboardPacket(byte controlKey, byte[] keyCodes)
        {
            byte[] payload = new byte[DATA_LENGTH];
            payload[0] = controlKey;
            payload[1] = 0x00;
            for (int i = 0; i < 6; i++)
            {
                payload[i + 2] = i < keyCodes.Length ? keyCodes[i] : (byte)0x00;
            }

            return SendPacket(CMD_KEYBOARD, payload);
        }

        /// <summary>
        /// 发送字符
        /// </summary>
        public bool SendChar(char ch)
        {
            // 检查是否需要Shift键
            if (NeedsShift(ch))
            {
                return SendSpecialChar(ch);
            }
            
            byte key = CharToKeyCode(ch);
            if (key == 0x00)
                return false;

            byte[] keys = new byte[] { key };
            bool result = SendKeyboardPacket(0x00, keys);
            
            // 延时后发送释放按键
            System.Threading.Thread.Sleep(20);
            SendKeyboardPacket(0x00, new byte[0]);
            
            return result;
        }

        /// <summary>
        /// 进度报告委托
        /// </summary>
        /// <param name="current">当前进度</param>
        /// <param name="total">总进度</param>
        public delegate void ProgressReport(int current, int total);

        /// <summary>
        /// 异步发送字符串
        /// </summary>
        public async Task<bool> SendStringAsync(string str, CancellationToken cancellationToken = default, ProgressReport? progressReport = null, int keyReleaseDelayMs = 2, int charIntervalDelayMs = 1)
        {
            _cts = new CancellationTokenSource();
            var token = cancellationToken != default ? cancellationToken : _cts.Token;

            try
            {
                return await Task.Run(async () =>
                {
                    bool allSuccess = true;
                    int total = str.Length;
                    int current = 0;

                    foreach (char ch in str)
                    {
                        token.ThrowIfCancellationRequested();

                        if (ch == '\r')
                        {
                            if (!await SendCharAsync(ch, token, keyReleaseDelayMs))
                                allSuccess = false;
                        }
                        else if (ch != '\n' && ch != '\0')
                        {
                            if (!await SendCharAsync(ch, token, keyReleaseDelayMs))
                                allSuccess = false;
                        }

                        current++;
                        progressReport?.Invoke(current, total);

                        if (charIntervalDelayMs > 0)
                            await Task.Delay(charIntervalDelayMs, token);
                    }
                    return allSuccess;
                }, token);
            }
            catch (OperationCanceledException)
            {
                return false;
            }
            finally
            {
                _cts = null;
            }
        }

        /// <summary>
        /// 异步发送字符
        /// </summary>
        private async Task<bool> SendCharAsync(char ch, CancellationToken cancellationToken, int keyReleaseDelayMs = 2)
        {
            // 检查取消令牌
            cancellationToken.ThrowIfCancellationRequested();

            // 检查是否需要Shift键
            if (NeedsShift(ch))
            {
                return await SendSpecialCharAsync(ch, cancellationToken, keyReleaseDelayMs);
            }

            byte key = CharToKeyCode(ch);
            if (key == 0x00)
                return false;

            byte[] keys = new byte[] { key };
            bool result = SendKeyboardPacket(0x00, keys);

            if (!result)
                return false;

            try
            {
                if (keyReleaseDelayMs > 0)
                {
                    await Task.Delay(keyReleaseDelayMs, cancellationToken);
                }
            }
            finally
            {
                KeyUp();
            }

            return true;
        }

        /// <summary>
        /// 异步发送特殊字符(包含Shift)
        /// </summary>
        private async Task<bool> SendSpecialCharAsync(char ch, CancellationToken cancellationToken, int keyReleaseDelayMs = 2)
        {
            // 检查取消令牌
            cancellationToken.ThrowIfCancellationRequested();

            if (NeedsShift(ch))
            {
                byte key = CharToKeyCode(ch);
                if (key == 0x00)
                    key = CharToSpecialKeyCode(ch);

                if (key != 0x00)
                {
                    bool result = SendCombination(0x02, new byte[] { key }); // 0x02 = Left Shift
                    if (keyReleaseDelayMs > 0)
                    {
                        await Task.Delay(keyReleaseDelayMs, cancellationToken);
                    }
                    return result;
                }
            }
            return await SendCharAsync(ch, cancellationToken, keyReleaseDelayMs);
        }

        /// <summary>
        /// 停止当前发送操作
        /// </summary>
        public void StopSending()
        {
            _cts?.Cancel();
            KeyUp();
        }

        /// <summary>
        /// 发送组合键
        /// </summary>
        public bool SendCombination(byte controlKey, byte[] keyCodes)
        {
            bool result = SendKeyboardPacket(controlKey, keyCodes);
            
            // 延时后释放所有按键
            System.Threading.Thread.Sleep(2);
            SendKeyboardPacket(0x00, new byte[0]);
            
            return result;
        }

        /// <summary>
        /// 发送 Ctrl+A 组合键
        /// </summary>
        public bool SendCtrlA()
        {
            return SendCombination(CTRL_LEFT, new byte[] { KEY_A });
        }

        /// <summary>
        /// 发送 Ctrl+C 组合键
        /// </summary>
        public bool SendCtrlC()
        {
            return SendCombination(CTRL_LEFT, new byte[] { KEY_C });
        }

        /// <summary>
        /// 发送 Ctrl+V 组合键
        /// </summary>
        public bool SendCtrlV()
        {
            return SendCombination(CTRL_LEFT, new byte[] { KEY_V });
        }

        /// <summary>
        /// 发送 Ctrl+Z 组合键
        /// </summary>
        public bool SendCtrlZ()
        {
            return SendCombination(CTRL_LEFT, new byte[] { KEY_Z });
        }

        /// <summary>
        /// 发送 Ctrl+X 组合键
        /// </summary>
        public bool SendCtrlX()
        {
            return SendCombination(CTRL_LEFT, new byte[] { KEY_X });
        }

        /// <summary>
        /// 发送 Win+D 组合键（显示桌面）
        /// </summary>
        public bool SendWinD()
        {
            return SendCombination(GUI_LEFT, new byte[] { KEY_D });
        }

        /// <summary>
        /// 发送 Alt+Tab 组合键（切换程序）
        /// 需要保持Alt键按住状态，按一次Tab切换一个程序
        /// </summary>
        public bool SendAltTab()
        {
            // 1. 按下Alt键
            SendKeyboardPacket(ALT_LEFT, new byte[0]);
            System.Threading.Thread.Sleep(20);

            // 2. 按下Tab键（保持Alt按住）
            SendKeyboardPacket(ALT_LEFT, new byte[] { KEY_TAB });
            System.Threading.Thread.Sleep(20);

            // 3. 释放Tab键（继续保持Alt按住，这样可以在窗口切换器中导航）
            SendKeyboardPacket(ALT_LEFT, new byte[0]);
            System.Threading.Thread.Sleep(20);

            // 4. 释放Alt键
            SendKeyboardPacket(0x00, new byte[0]);

            return true;
        }

        /// <summary>
        /// 发送上箭头键
        /// </summary>
        public bool SendArrowUp()
        {
            return SendCharKey(KEY_UP);
        }

        /// <summary>
        /// 发送下箭头键
        /// </summary>
        public bool SendArrowDown()
        {
            return SendCharKey(KEY_DOWN);
        }

        /// <summary>
        /// 发送左箭头键
        /// </summary>
        public bool SendArrowLeft()
        {
            return SendCharKey(KEY_LEFT);
        }

        /// <summary>
        /// 发送右箭头键
        /// </summary>
        public bool SendArrowRight()
        {
            return SendCharKey(KEY_RIGHT);
        }

        /// <summary>
        /// 发送单个按键
        /// </summary>
        private bool SendCharKey(byte keyCode)
        {
            bool result = SendKeyboardPacket(0x00, new byte[] { keyCode });
            System.Threading.Thread.Sleep(20);
            SendKeyboardPacket(0x00, new byte[0]);
            return result;
        }

        /// <summary>
        /// 字符转换为键码
        /// </summary>
        private byte CharToKeyCode(char ch)
        {
            // 数字键
            if (ch >= '0' && ch <= '9')
            {
                if (ch == '0')
                    return 0x27; // 0
                else
                    return (byte)(0x1E + (ch - '1')); // 1-9
            }

            // 大写字母
            if (ch >= 'A' && ch <= 'Z')
            {
                return (byte)(0x04 + (ch - 'A'));
            }

            // 小写字母
            if (ch >= 'a' && ch <= 'z')
            {
                return (byte)(0x04 + (ch - 'a'));
            }

            // 空格
            if (ch == ' ')
            {
                return 0x2C;
            }

            // Enter
            if (ch == '\r' || ch == '\n')
            {
                return 0x28;
            }

            // Tab
            if (ch == '\t')
            {
                return 0x2B;
            }

            // Backspace
            if (ch == '\b')
            {
                return 0x2A;
            }

            // 其他特殊字符
            return CharToSpecialKeyCode(ch);
        }

        /// <summary>
        /// 特殊字符转换
        /// </summary>
        private byte CharToSpecialKeyCode(char ch)
        {
            return ch switch
            {
                '!' => 0x1E, // Shift + 1
                '@' => 0x1F, // Shift + 2
                '#' => 0x20, // Shift + 3
                '$' => 0x21, // Shift + 4
                '%' => 0x22, // Shift + 5
                '^' => 0x23, // Shift + 6
                '&' => 0x24, // Shift + 7
                '*' => 0x25, // Shift + 8
                '(' => 0x26, // Shift + 9
                ')' => 0x27, // Shift + 0
                '-' => 0x2D,
                '_' => 0x2D, // Shift + -
                '=' => 0x2E,
                '+' => 0x2E, // Shift + =
                '[' => 0x2F,
                '{' => 0x2F, // Shift + [
                ']' => 0x30,
                '}' => 0x30, // Shift + ]
                '\\' => 0x31,
                '|' => 0x31, // Shift + \
                ';' => 0x33,
                ':' => 0x33, // Shift + ;
                '\'' => 0x34,
                '"' => 0x34, // Shift + '
                ',' => 0x36,
                '<' => 0x36, // Shift + ,
                '.' => 0x37,
                '>' => 0x37, // Shift + .
                '/' => 0x38,
                '?' => 0x38, // Shift + /
                '`' => 0x35,
                '~' => 0x35, // Shift + `
                _ => 0x00
            };
        }

        /// <summary>
        /// 需要Shift键的字符判断
        /// </summary>
        private bool NeedsShift(char ch)
        {
            return ch switch
            {
                '!' or '@' or '#' or '$' or '%' or '^' or '&' or '*' or '(' or ')' => true,
                '_' or '+' or '{' or '}' or '|' or ':' or '"' or '<' or '>' or '?' => true,
                '~' => true,
                _ when (ch >= 'A' && ch <= 'Z') => true,
                _ => false
            };
        }

        /// <summary>
        /// 发送特殊字符(包含Shift)
        /// </summary>
        public bool SendSpecialChar(char ch)
        {
            if (NeedsShift(ch))
            {
                byte key = CharToKeyCode(ch);
                if (key == 0x00)
                    key = CharToSpecialKeyCode(ch);
                
                if (key != 0x00)
                {
                    return SendCombination(0x02, new byte[] { key }); // 0x02 = Left Shift
                }
            }
            return SendChar(ch);
        }

        /// <summary>
        /// 按键按下
        /// </summary>
        public bool KeyDown(byte controlKey, byte[] keyCodes)
        {
            return SendKeyboardPacket(controlKey, keyCodes);
        }

        /// <summary>
        /// 按键释放
        /// </summary>
        public bool KeyUp()
        {
            return SendKeyboardPacket(0x00, new byte[0]);
        }

        /// <summary>
        /// 发送按键
        /// </summary>
        public bool SendKey(byte controlKey, byte[] keyCodes)
        {
            if (SendKeyboardPacket(controlKey, keyCodes))
            {
                // 短暂延迟后释放按键
                System.Threading.Thread.Sleep(15);
                return SendKeyboardPacket(0x00, new byte[0]);
            }
            return false;
        }

        /// <summary>
        /// 发送鼠标点击
        /// </summary>
        public bool SendMouseClick(byte button)
        {
            return SendMouseButton(button, true) && SendMouseButton(0x00, false);
        }

        public bool SendMouseClickLegacy(byte button)
        {
            return SendMouseClick(button);
        }

        public bool SendMouseButton(byte button, bool isPressed)
        {
            byte[] payload = new byte[7];
            payload[0] = isPressed ? button : (byte)0x00;
            payload[1] = 0x00;
            payload[2] = 0x00;
            payload[3] = 0x00;
            payload[4] = 0x00;
            payload[5] = 0x00;
            payload[6] = 0x00;
            return SendPacket(CMD_MOUSE_ABSOLUTE, payload);
        }

        public bool SendMouseWheel(int delta)
        {
            sbyte wheel = delta switch
            {
                > 0 => 1,
                < 0 => -1,
                _ => 0
            };

            if (wheel == 0)
            {
                return true;
            }

            byte[] payload = new byte[5];
            payload[0] = 0x00;
            payload[1] = 0x01;
            payload[2] = unchecked((byte)wheel);
            payload[3] = 0x00;
            payload[4] = 0x00;
            return SendPacket(CMD_MOUSE_BUTTON, payload);
        }

        public bool SendMouseMoveRelative(int deltaX, int deltaY)
        {
            if (deltaX == 0 && deltaY == 0)
            {
                return true;
            }

            bool success = true;
            int remainingX = deltaX;
            int remainingY = deltaY;

            while (remainingX != 0 || remainingY != 0)
            {
                sbyte stepX = ClampToSByte(remainingX);
                sbyte stepY = ClampToSByte(remainingY);
                byte[] payload = new byte[3];
                payload[0] = 0x00;
                payload[1] = unchecked((byte)stepX);
                payload[2] = unchecked((byte)stepY);
                success &= SendPacket(CMD_MOUSE_RELATIVE, payload);
                remainingX -= stepX;
                remainingY -= stepY;
            }

            return success;
        }

        private static sbyte ClampToSByte(int value)
        {
            if (value > sbyte.MaxValue)
            {
                return sbyte.MaxValue;
            }

            if (value < sbyte.MinValue)
            {
                return sbyte.MinValue;
            }

            return (sbyte)value;
        }

        /// <summary>
        /// 将Windows虚拟键码转换为CH9329键码
        /// </summary>
        public byte VirtualKeyToKeyCode(int virtualKey)
        {
            // A-Z
            if (virtualKey >= 0x41 && virtualKey <= 0x5A)
            {
                return (byte)(0x04 + (virtualKey - 0x41));
            }
            // 0-9 (主键盘数字行)
            if (virtualKey >= 0x30 && virtualKey <= 0x39)
            {
                // 1-9 -> 0x1E-0x26, 0 -> 0x27
                if (virtualKey == 0x30) // 0
                    return 0x27;
                else // 1-9
                    return (byte)(0x1E + (virtualKey - 0x31));
            }

            // 常用键映射
            return virtualKey switch
            {
                0x0D => 0x28, // Enter
                0x20 => 0x2C, // Space
                0x08 => 0x2A, // Backspace
                0x09 => 0x2B, // Tab
                0x1B => 0x29, // Escape
                0x2D => 0x2D, // -
                0x3D => 0x2E, // =
                0x5B => 0x2F, // [
                0x5D => 0x30, // ]
                0x5C => 0x31, // \
                0x3B => 0x33, // ;
                0xDE => 0x34, // '
                0x2C => 0x36, // ,
                0x2E => 0x37, // .
                0x2F => 0x38, // /
                0xC0 => 0x35, // `
                0x26 => 0x52, // Up Arrow
                0x28 => 0x51, // Down Arrow
                0x25 => 0x50, // Left Arrow
                0x27 => 0x4F, // Right Arrow
                0x70 => 0x3A, // F1
                0x71 => 0x3B, // F2
                0x72 => 0x3C, // F3
                0x73 => 0x3D, // F4
                0x74 => 0x3E, // F5
                0x75 => 0x3F, // F6
                0x76 => 0x40, // F7
                0x77 => 0x41, // F8
                0x78 => 0x42, // F9
                0x79 => 0x43, // F10
                0x7A => 0x44, // F11
                0x7B => 0x45, // F12
                _ => 0x00
            };
        }

        /// <summary>
        /// 获取控制键状态
        /// </summary>
        public byte GetControlKeyState(bool ctrl, bool shift, bool alt, bool win)
        {
            byte controlKey = 0x00;
            if (ctrl) controlKey |= CTRL_LEFT;
            if (shift) controlKey |= SHIFT_LEFT;
            if (alt) controlKey |= ALT_LEFT;
            if (win) controlKey |= GUI_LEFT;
            return controlKey;
        }

        /// <summary>
        /// 发送编码后的文本传输序列
        /// </summary>
        public async Task<bool> SendEncodedTextSequenceAsync(List<(byte controlKey, byte[] keyCodes)> sequence, CancellationToken cancellationToken = default)
        {
            if (_serialPort == null || !_serialPort.IsConnected)
                return false;

            try
            {
                return await Task.Run(async () =>
                {
                    foreach (var (controlKey, keyCodes) in sequence)
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        SendKeyboardPacket(controlKey, keyCodes);
                        await Task.Delay(15, cancellationToken);
                        SendKeyboardPacket(0x00, new byte[0]);
                        await Task.Delay(10, cancellationToken);
                    }
                    return true;
                }, cancellationToken);
            }
            catch (OperationCanceledException)
            {
                return false;
            }
        }

        /// <summary>
        /// 发送文本(使用传输协议)
        /// </summary>
        public async Task<bool> SendTextWithProtocolAsync(string text, CancellationToken cancellationToken = default)
        {
            var sequence = TextTransferProtocol.EncodeText(text);
            return await SendEncodedTextSequenceAsync(sequence, cancellationToken);
        }

        /// <summary>
        /// 解析接收到的数据
        /// </summary>
        private void ParseReceivedData(byte[] data)
        {
            try
            {
                // 检查数据长度是否足够
                if (data.Length < 14)
                    return;

                // 检查帧头
                int index = 0;
                if (data[index++] != FRAME_HEAD1 || data[index++] != FRAME_HEAD2)
                    return;

                // 检查地址码
                if (data[index++] != ADDR_CODE)
                    return;

                // 检查命令码
                byte cmd = data[index++];
                if (cmd != CMD_KEYBOARD_RESPONSE && cmd != CMD_KEYBOARD)
                    return;

                // 检查数据长度
                byte length = data[index++];
                if (length != DATA_LENGTH)
                    return;

                // 读取键盘数据
                byte controlKey = data[index++];
                index++; // 跳过保留字节

                // 读取6个按键位置
                byte[] keyCodes = new byte[6];
                for (int i = 0; i < 6; i++)
                {
                    keyCodes[i] = data[index++];
                }

                // 检查校验和
                byte checksum = data[index];
                byte calculatedChecksum = CalculateChecksum(data, 0, 13);
                if (checksum != calculatedChecksum)
                    return;

                // 确定是否有按键按下
                bool isPressed = controlKey != 0x00 || keyCodes.Any(k => k != 0x00);

                // 触发事件
                KeyboardDataReceived?.Invoke(this, new KeyboardDataReceivedEventArgs
                {
                    ControlKey = controlKey,
                    KeyCodes = keyCodes,
                    IsPressed = isPressed
                });
            }
            catch (Exception)
            {
                // 忽略解析错误
            }
        }
    }
}
