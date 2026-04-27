using System.Collections.Generic;

namespace KeyboardController
{
    /// <summary>
    /// 文本传输协议 - 通过键盘输入编码实现文本传输
    /// </summary>
    public class TextTransferProtocol
    {
        // 协议标记键码
        public const byte KEY_F11 = 0x44;
        public const byte KEY_F12 = 0x45;
        public const byte KEY_ESC = 0x29;

        // 特殊字符编码(F11 + 字母)
        private static readonly Dictionary<char, byte[]> SpecialCharMap = new()
        {
            { ' ', new byte[] { 0x04 } },       // A -> 空格
            { '\r', new byte[] { 0x05 } },     // B -> 回车
            { '\n', new byte[] { 0x05 } },     // B -> 换行(回车)
            { '\t', new byte[] { 0x06 } },     // C -> Tab
            { '!', new byte[] { 0x07 } },      // D -> !
            { '@', new byte[] { 0x08 } },      // E -> @
            { '#', new byte[] { 0x09 } },      // F -> #
            { '$', new byte[] { 0x0A } },      // G -> $
            { '%', new byte[] { 0x0B } },      // H -> %
            { '^', new byte[] { 0x0C } },      // I -> ^
            { '&', new byte[] { 0x0D } },      // J -> &
            { '*', new byte[] { 0x0E } },      // K -> *
            { '(', new byte[] { 0x0F } },      // L -> (
            { ')', new byte[] { 0x10 } },      // M -> )
            { '-', new byte[] { 0x11 } },      // N -> -
            { '_', new byte[] { 0x12 } },      // O -> _
            { '=', new byte[] { 0x13 } },      // P -> =
            { '+', new byte[] { 0x14 } },      // Q -> +
            { '[', new byte[] { 0x15 } },      // R -> [
            { ']', new byte[] { 0x16 } },      // S -> ]
            { '{', new byte[] { 0x17 } },      // T -> {
            { '}', new byte[] { 0x18 } },      // U -> }
            { '\\', new byte[] { 0x19 } },     // V -> \
            { '|', new byte[] { 0x1A } },      // W -> |
            { ';', new byte[] { 0x1B } },      // X -> ;
            { ':', new byte[] { 0x1C } },      // Y -> :
            { '\'', new byte[] { 0x1D } },     // Z -> '
            { '"', new byte[] { 0x1E } },      // F11+R -> "
            { ',', new byte[] { 0x1F } },      // F11+R -> ,
            { '<', new byte[] { 0x20 } },      // F11+S -> <
            { '.', new byte[] { 0x21 } },      // F11+T -> .
            { '>', new byte[] { 0x22 } },      // F11+U -> >
            { '/', new byte[] { 0x23 } },      // F11+V -> /
            { '?', new byte[] { 0x24 } },      // F11+W -> ?
            { '`', new byte[] { 0x25 } },      // F11+X -> `
            { '~', new byte[] { 0x26 } },      // F11+Y -> ~
            { '\b', new byte[] { 0x27 } },     // F11+Z -> Backspace
        };

        // 反向映射
        private static readonly Dictionary<byte[], char> ReverseSpecialCharMap;
        private static readonly Dictionary<string, char> ReverseCodeMap;

        static TextTransferProtocol()
        {
            ReverseSpecialCharMap = new(new ByteArrayEqualityComparer());
            foreach (var kvp in SpecialCharMap)
            {
                ReverseSpecialCharMap[kvp.Value] = kvp.Key;
            }

            ReverseCodeMap = new();
            foreach (var kvp in SpecialCharMap)
            {
                if (kvp.Value.Length == 1)
                {
                    ReverseCodeMap[kvp.Value[0].ToString()] = kvp.Key;
                }
            }
        }

        // 字节数组比较器
        private class ByteArrayEqualityComparer : IEqualityComparer<byte[]>
        {
            public bool Equals(byte[]? x, byte[]? y)
            {
                if (x == null || y == null) return x == y;
                if (x.Length != y.Length) return false;
                for (int i = 0; i < x.Length; i++)
                    if (x[i] != y[i]) return false;
                return true;
            }

            public int GetHashCode(byte[] obj)
            {
                int hash = 17;
                foreach (byte b in obj)
                    hash = hash * 31 + b;
                return hash;
            }
        }

        /// <summary>
        /// 编码文本为键盘输入序列
        /// </summary>
        public static List<(byte controlKey, byte[] keyCodes)> EncodeText(string text)
        {
            var sequence = new List<(byte controlKey, byte[] keyCodes)>();

            // 开始传输: F12
            sequence.Add((0x00, new byte[] { KEY_F12 }));

            foreach (char ch in text)
            {
                if (ch >= 'a' && ch <= 'z')
                {
                    // 小写字母: 直接输入
                    sequence.Add((0x00, new byte[] { (byte)(0x04 + (ch - 'a')) }));
                }
                else if (ch >= 'A' && ch <= 'Z')
                {
                    // 大写字母: Shift + 字母
                    sequence.Add((0x02, new byte[] { (byte)(0x04 + (ch - 'A')) }));
                }
                else if (ch >= '0' && ch <= '9')
                {
                    // 数字: 直接输入
                    sequence.Add((0x00, new byte[] { (byte)(0x27 + (ch - '0')) }));
                }
                else if (SpecialCharMap.ContainsKey(ch))
                {
                    // 特殊字符: F11 + 编码
                    byte[] code = SpecialCharMap[ch];
                    if (code.Length == 1 && code[0] <= 0x1A)
                    {
                        // F11 + A-Z
                        sequence.Add((0x00, new byte[] { KEY_F11 }));
                        sequence.Add((0x00, new byte[] { (byte)(0x04 + code[0]) }));
                    }
                    else if (code.Length == 1 && code[0] <= 0x26)
                    {
                        // F11 + 字母循环
                        byte letterIndex = code[0] <= 0x1A ? code[0] : (byte)((code[0] - 0x1B) + 4);
                        sequence.Add((0x00, new byte[] { KEY_F11 }));
                        sequence.Add((0x00, new byte[] { letterIndex }));
                    }
                    else
                    {
                        // F11 + 字母(超过范围)
                        sequence.Add((0x00, new byte[] { KEY_F11 }));
                        byte letterIndex = (byte)(0x04 + (code[0] % 26));
                        sequence.Add((0x00, new byte[] { letterIndex }));
                    }
                }
                else
                {
                    // 不支持的字符,使用Unicode编码表示(F11 + 逐个数字)
                    sequence.Add((0x00, new byte[] { KEY_F11 }));
                    sequence.Add((0x00, new byte[] { 0x0B })); // E 表示Unicode开始
                    string hex = ((int)ch).ToString("X4");
                    foreach (char h in hex)
                    {
                        sequence.Add((0x00, new byte[] { (byte)(0x27 + (h - '0')) }));
                    }
                }
            }

            // 结束传输: F12
            sequence.Add((0x00, new byte[] { KEY_F12 }));

            return sequence;
        }

        /// <summary>
        /// 解码器状态
        /// </summary>
        public class Decoder
        {
            private enum State
            {
                Idle,
                WaitingForCode,
                DecodingUnicode
            }

            private State _state = State.Idle;
            private readonly System.Text.StringBuilder _buffer = new();

            /// <summary>
            /// 处理接收到的键盘输入
            /// </summary>
            public bool ProcessKey(byte controlKey, byte[] keyCodes)
            {
                if (keyCodes.Length == 0 || keyCodes[0] == 0x00)
                    return false;

                byte key = keyCodes[0];

                switch (_state)
                {
                    case State.Idle:
                        // F12 开始传输
                        if (key == KEY_F12 && controlKey == 0x00)
                        {
                            _state = State.WaitingForCode;
                            return false; // 不显示F12
                        }
                        break;

                    case State.WaitingForCode:
                        // F12 结束传输
                        if (key == KEY_F12 && controlKey == 0x00)
                        {
                            _state = State.Idle;
                            return true; // 返回true表示完成一次传输
                        }
                        // F11 特殊字符
                        else if (key == KEY_F11 && controlKey == 0x00)
                        {
                            // 等待下一个字符
                            return false;
                        }
                        else
                        {
                            // 普通字符
                            if (controlKey == 0x02) // Shift按下
                            {
                                if (key >= 0x04 && key <= 0x1D) // A-Z
                                {
                                    _buffer.Append((char)('A' + (key - 0x04)));
                                }
                            }
                            else if (controlKey == 0x00)
                            {
                                if (key >= 0x04 && key <= 0x1D) // a-z
                                {
                                    _buffer.Append((char)('a' + (key - 0x04)));
                                }
                                else if (key >= 0x27 && key <= 0x30) // 0-9
                                {
                                    _buffer.Append((char)('0' + (key - 0x27)));
                                }
                                else if (key >= 0x04 && key <= 0x1D)
                                {
                                    // 可能是F11后面的编码字符
                                    return false;
                                }
                            }
                        }
                        break;
                }

                return false;
            }

            /// <summary>
            /// 处理特殊字符编码(在检测到F11后调用)
            /// </summary>
            public void ProcessSpecialCode(byte key)
            {
                if (key >= 0x04 && key <= 0x1D)
                {
                    byte codeIndex = (byte)(key - 0x04);
                    
                    // 查找对应的特殊字符
                    char? specialChar = null;
                    foreach (var kvp in SpecialCharMap)
                    {
                        if (kvp.Value.Length == 1 && kvp.Value[0] == codeIndex)
                        {
                            specialChar = kvp.Key;
                            break;
                        }
                    }

                    if (specialChar.HasValue)
                    {
                        _buffer.Append(specialChar.Value);
                    }
                }
            }

            /// <summary>
            /// 获取解码的文本
            /// </summary>
            public string GetText()
            {
                return _buffer.ToString();
            }

            /// <summary>
            /// 清空缓冲区
            /// </summary>
            public void Clear()
            {
                _buffer.Clear();
                _state = State.Idle;
            }

            /// <summary>
            /// 是否正在接收
            /// </summary>
            public bool IsReceiving => _state != State.Idle;
        }
    }
}
