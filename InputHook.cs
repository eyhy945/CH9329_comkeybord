using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace KeyboardController
{
    /// <summary>
    /// Windows输入钩子类，用于捕获键盘和鼠标事件
    /// </summary>
    public class InputHook : IDisposable
    {
        private const int WH_KEYBOARD_LL = 13;
        private const int WH_MOUSE_LL = 14;
        private const int WM_KEYDOWN = 0x0100;
        private const int WM_KEYUP = 0x0101;
        private const int WM_SYSKEYDOWN = 0x0104;
        private const int WM_SYSKEYUP = 0x0105;
        private const int WM_MOUSEMOVE = 0x0200;
        private const int WM_LBUTTONDOWN = 0x0201;
        private const int WM_LBUTTONUP = 0x0202;
        private const int WM_RBUTTONDOWN = 0x0204;
        private const int WM_RBUTTONUP = 0x0205;
        private const int WM_MOUSEWHEEL = 0x020A;

        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);
        private delegate IntPtr LowLevelMouseProc(int nCode, IntPtr wParam, IntPtr lParam);

        private IntPtr _keyboardHook = IntPtr.Zero;
        private IntPtr _mouseHook = IntPtr.Zero;
        private LowLevelKeyboardProc? _keyboardProc;
        private LowLevelMouseProc? _mouseProc;
        private bool _isInstalled = false;
        private bool _keyboardEnabled = false;
        private bool _mouseEnabled = false;

        public event EventHandler<KeyEventArgs>? KeyDown;
        public event EventHandler<KeyEventArgs>? KeyUp;
        public event EventHandler<MouseHookEventArgs>? MouseAction;

        public Func<KeyEventArgs, bool>? KeyboardInterceptor { get; set; }
        public Func<MouseHookEventArgs, bool>? MouseInterceptor { get; set; }

        public InputHook()
        {
            _keyboardProc = KeyboardHookCallback;
            _mouseProc = MouseHookCallback;
        }

        public bool Install()
        {
            if (_isInstalled)
                return true;

            try
            {
                IntPtr moduleHandle = GetModuleHandle(IntPtr.Zero);
                _keyboardHook = SetWindowsHookEx(WH_KEYBOARD_LL, _keyboardProc!, moduleHandle, 0);
                _mouseHook = SetWindowsHookEx(WH_MOUSE_LL, _mouseProc!, moduleHandle, 0);

                if (_keyboardHook == IntPtr.Zero || _mouseHook == IntPtr.Zero)
                {
                    Uninstall();
                    return false;
                }

                _isInstalled = true;
                return true;
            }
            catch (Exception)
            {
                Uninstall();
                return false;
            }
        }

        public void Uninstall()
        {
            if (_keyboardHook != IntPtr.Zero)
            {
                UnhookWindowsHookEx(_keyboardHook);
                _keyboardHook = IntPtr.Zero;
            }

            if (_mouseHook != IntPtr.Zero)
            {
                UnhookWindowsHookEx(_mouseHook);
                _mouseHook = IntPtr.Zero;
            }

            _isInstalled = false;
            _keyboardEnabled = false;
            _mouseEnabled = false;
        }

        public bool Enabled
        {
            get => _keyboardEnabled;
            set => _keyboardEnabled = value;
        }

        public bool KeyboardEnabled
        {
            get => _keyboardEnabled;
            set => _keyboardEnabled = value;
        }

        public bool MouseEnabled
        {
            get => _mouseEnabled;
            set => _mouseEnabled = value;
        }

        public bool IsInstalled => _isInstalled;

        private IntPtr KeyboardHookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode < 0)
            {
                return CallNextHookEx(_keyboardHook, nCode, wParam, lParam);
            }

            int vkCode = Marshal.ReadInt32(lParam);
            Keys key = (Keys)vkCode;

            if (wParam == (IntPtr)WM_KEYDOWN || wParam == (IntPtr)WM_SYSKEYDOWN)
            {
                KeyEventArgs args = new KeyEventArgs(key);
                if (_keyboardEnabled)
                {
                    KeyDown?.Invoke(this, args);
                    if (KeyboardInterceptor?.Invoke(args) == true)
                    {
                        return (IntPtr)1;
                    }
                }
            }
            else if (wParam == (IntPtr)WM_KEYUP || wParam == (IntPtr)WM_SYSKEYUP)
            {
                KeyEventArgs args = new KeyEventArgs(key);
                if (_keyboardEnabled)
                {
                    KeyUp?.Invoke(this, args);
                    if (KeyboardInterceptor?.Invoke(args) == true)
                    {
                        return (IntPtr)1;
                    }
                }
            }

            return CallNextHookEx(_keyboardHook, nCode, wParam, lParam);
        }

        private IntPtr MouseHookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode < 0)
            {
                return CallNextHookEx(_mouseHook, nCode, wParam, lParam);
            }

            if (!_mouseEnabled)
            {
                return CallNextHookEx(_mouseHook, nCode, wParam, lParam);
            }

            var hookStruct = Marshal.PtrToStructure<MSLLHOOKSTRUCT>(lParam);
            var args = CreateMouseHookEventArgs((int)wParam, hookStruct);
            if (args == null)
            {
                return CallNextHookEx(_mouseHook, nCode, wParam, lParam);
            }

            MouseAction?.Invoke(this, args);
            if (MouseInterceptor?.Invoke(args) == true)
            {
                return (IntPtr)1;
            }

            return CallNextHookEx(_mouseHook, nCode, wParam, lParam);
        }

        private static MouseHookEventArgs? CreateMouseHookEventArgs(int message, MSLLHOOKSTRUCT hookStruct)
        {
            Point location = new Point(hookStruct.pt.x, hookStruct.pt.y);
            return message switch
            {
                WM_MOUSEMOVE => new MouseHookEventArgs(MouseActionType.Move, location, 0),
                WM_LBUTTONDOWN => new MouseHookEventArgs(MouseActionType.LeftDown, location, 0),
                WM_LBUTTONUP => new MouseHookEventArgs(MouseActionType.LeftUp, location, 0),
                WM_RBUTTONDOWN => new MouseHookEventArgs(MouseActionType.RightDown, location, 0),
                WM_RBUTTONUP => new MouseHookEventArgs(MouseActionType.RightUp, location, 0),
                WM_MOUSEWHEEL => new MouseHookEventArgs(MouseActionType.Wheel, location, GetWheelDelta(hookStruct.mouseData)),
                _ => null
            };
        }

        private static int GetWheelDelta(uint mouseData)
        {
            return (short)((mouseData >> 16) & 0xFFFF);
        }

        public void Dispose()
        {
            Uninstall();
        }

        public sealed class MouseHookEventArgs : EventArgs
        {
            public MouseHookEventArgs(MouseActionType actionType, Point location, int wheelDelta)
            {
                ActionType = actionType;
                Location = location;
                WheelDelta = wheelDelta;
            }

            public MouseActionType ActionType { get; }
            public Point Location { get; }
            public int WheelDelta { get; }
        }

        public enum MouseActionType
        {
            Move,
            LeftDown,
            LeftUp,
            RightDown,
            RightUp,
            Wheel
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct POINT
        {
            public int x;
            public int y;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct MSLLHOOKSTRUCT
        {
            public POINT pt;
            public uint mouseData;
            public uint flags;
            public uint time;
            public IntPtr dwExtraInfo;
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelMouseProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(IntPtr lpModuleName);
    }
}
