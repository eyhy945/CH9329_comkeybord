using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace KeyboardController
{
    /// <summary>
    /// CH9329 HID设备管理类
    /// 封装CH9329DLL的HID接口功能
    /// </summary>
    public class CH9329HIDManager : IDisposable
    {
        private IntPtr _deviceHandle = IntPtr.Zero;
        private bool _isConnected = false;
        private CancellationTokenSource? _receiveCts;
        private Task? _receiveTask;

        public bool IsConnected => _isConnected;

        public event EventHandler<byte[]>? DataReceived;
        public event EventHandler<string>? StatusChanged;

        #region CH9329 DLL Imports

        private const string DLL_NAME = "CH9329DLL.dll";

        [DllImport(DLL_NAME, CallingConvention = CallingConvention.Winapi)]
        private static extern bool CH9329DllInt();

        [DllImport(DLL_NAME, CallingConvention = CallingConvention.Winapi)]
        private static extern void CH9329GetHidGuid(out Guid HidGuid);

        [DllImport(DLL_NAME, CallingConvention = CallingConvention.Winapi)]
        private static extern IntPtr CH9329OpenDevice(ushort USB_VID, ushort USB_PID);

        [DllImport(DLL_NAME, CallingConvention = CallingConvention.Winapi)]
        private static extern IntPtr CH9329OpenDevicePath([MarshalAs(UnmanagedType.LPStr)] string DevicePath);

        [DllImport(DLL_NAME, CallingConvention = CallingConvention.Winapi)]
        private static extern bool CH9329GetDevicePath(uint DevIndex, [MarshalAs(UnmanagedType.LPStr)] string DevicePath, uint DevicePathLen);

        [DllImport(DLL_NAME, CallingConvention = CallingConvention.Winapi)]
        private static extern bool CH9329GetAttributes(IntPtr hDeviceHandle, out ushort pVID, out ushort pPID, out ushort pVer);

        [DllImport(DLL_NAME, CallingConvention = CallingConvention.Winapi)]
        private static extern bool CH9329GetBufferLen(IntPtr hDeviceHandle, out ushort InputReportLen, out ushort OutputReportLen);

        [DllImport(DLL_NAME, CallingConvention = CallingConvention.Winapi)]
        private static extern void CH9329SetTimeOut(IntPtr hDeviceHandle, uint ReadTimeOut, uint SendTimeOut);

        [DllImport(DLL_NAME, CallingConvention = CallingConvention.Winapi)]
        private static extern bool CH9329WriteData(IntPtr hDeviceHandle, byte[] SendBuffer, uint SentLen, IntPtr hEventObject);

        [DllImport(DLL_NAME, CallingConvention = CallingConvention.Winapi)]
        private static extern bool CH9329ReadData(IntPtr hDeviceHandle, byte[] ReadBuffer, ref uint pReadLen, IntPtr hEventObject);

        [DllImport(DLL_NAME, CallingConvention = CallingConvention.Winapi)]
        private static extern bool CH9329InitThreadData(IntPtr hDeviceHandle);

        [DllImport(DLL_NAME, CallingConvention = CallingConvention.Winapi)]
        private static extern uint CH9329GetThreadDataLen(IntPtr hDeviceHandle);

        [DllImport(DLL_NAME, CallingConvention = CallingConvention.Winapi)]
        private static extern bool CH9329ReadThreadData(IntPtr hDeviceHandle, byte[] ReadBuffer, ref uint ReadLen);

        [DllImport(DLL_NAME, CallingConvention = CallingConvention.Winapi)]
        private static extern void CH9329ClearThreadData(IntPtr hDeviceHandle);

        [DllImport(DLL_NAME, CallingConvention = CallingConvention.Winapi)]
        private static extern void CH9329StopThread(IntPtr hDeviceHandle);

        [DllImport(DLL_NAME, CallingConvention = CallingConvention.Winapi)]
        private static extern void CH9329CloseDevice(IntPtr hDeviceHandle);

        [DllImport(DLL_NAME, CallingConvention = CallingConvention.Winapi)]
        private static extern bool CH9329GetCFG(IntPtr hDeviceHandle, ref USBDevInfo pDevInfo);

        [DllImport(DLL_NAME, CallingConvention = CallingConvention.Winapi)]
        private static extern bool CH9329SetCFG(IntPtr hDeviceHandle, USBDevInfo DevInfo);

        [DllImport(DLL_NAME, CallingConvention = CallingConvention.Winapi)]
        private static extern bool CH9329SetRate(IntPtr hDeviceHandle, uint DevRate);

        [DllImport(DLL_NAME, CallingConvention = CallingConvention.Winapi)]
        private static extern bool CH9329SetDEF(IntPtr hDeviceHandle);

        [DllImport(DLL_NAME, CallingConvention = CallingConvention.Winapi)]
        private static extern bool CH9329Reset(IntPtr hDeviceHandle);

        #endregion

        #region Structures

        [StructLayout(LayoutKind.Sequential)]
        public struct USBDevInfo
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
            public byte[] Head_Flag;
            public byte Work_Mode;
            public byte UART_Mode;
            public byte UART_Addr;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public byte[] UART_BaudRate;
            public byte UART_BitNum;
            public byte UART_Check;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
            public byte[] UART_PackInteral;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
            public byte[] USB_VID;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
            public byte[] USB_PID;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
            public byte[] USB_KB_UpInteral;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
            public byte[] USB_KB_ReleaseDelay;
            public byte USB_KB_AutoEnter;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public byte[] USB_KB_EnterChar1;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public byte[] USB_KB_EnterChar2;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public byte[] USB_KB_BegEnter;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public byte[] USB_KB_EndEnter;
            public byte USB_Str_EnFlag;
            public byte USB_KB_Fast_Flag;
            public byte USB_Sleep_Enable;
            public byte USB_Speed_Mode;
            public byte USB_Touch_Enable;
            public byte USB_Polling_Enable;
            public byte USB_Keyboard_Polling_Time;
            public byte USB_Mouse_Polling_Time;
            public byte USB_Media_Polling_Time;
            public byte USB_HID_Polling_Time;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 7)]
            public byte[] USB_Str_keep;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 24)]
            public byte[] USB_Str_factory;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 24)]
            public byte[] USB_Str_product;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 24)]
            public byte[] USB_Str_sernum;
        }

        #endregion

        static CH9329HIDManager()
        {
            try
            {
                CH9329DllInt();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"CH9329DllInt初始化失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 通过VID和PID打开设备
        /// </summary>
        public bool Connect(ushort vid, ushort pid)
        {
            try
            {
                if (_isConnected)
                {
                    Disconnect();
                }

                _deviceHandle = CH9329OpenDevice(vid, pid);
                if (_deviceHandle == IntPtr.Zero || _deviceHandle == new IntPtr(-1))
                {
                    StatusChanged?.Invoke(this, $"打开设备失败 VID={vid:X4} PID={pid:X4}");
                    return false;
                }

                // 获取设备信息
                if (CH9329GetAttributes(_deviceHandle, out ushort deviceVid, out ushort devicePid, out ushort version))
                {
                    StatusChanged?.Invoke(this, $"设备已连接 VID={deviceVid:X4} PID={devicePid:X4} Ver={version:X4}");
                }

                // 设置超时
                CH9329SetTimeOut(_deviceHandle, 3000, 3000);

                _isConnected = true;

                // 启动接收线程
                StartReceiveThread();

                return true;
            }
            catch (Exception ex)
            {
                StatusChanged?.Invoke(this, $"连接失败: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// 通过设备路径打开设备
        /// </summary>
        public bool Connect(string devicePath)
        {
            try
            {
                if (_isConnected)
                {
                    Disconnect();
                }

                _deviceHandle = CH9329OpenDevicePath(devicePath);
                if (_deviceHandle == IntPtr.Zero || _deviceHandle == new IntPtr(-1))
                {
                    StatusChanged?.Invoke(this, "打开设备失败");
                    return false;
                }

                // 获取设备信息
                if (CH9329GetAttributes(_deviceHandle, out ushort vid, out ushort pid, out ushort version))
                {
                    StatusChanged?.Invoke(this, $"设备已连接 VID={vid:X4} PID={pid:X4} Ver={version:X4}");
                }

                // 设置超时
                CH9329SetTimeOut(_deviceHandle, 3000, 3000);

                _isConnected = true;

                // 启动接收线程
                StartReceiveThread();

                return true;
            }
            catch (Exception ex)
            {
                StatusChanged?.Invoke(this, $"连接失败: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// 断开连接
        /// </summary>
        public void Disconnect()
        {
            try
            {
                StopReceiveThread();

                if (_deviceHandle != IntPtr.Zero && _deviceHandle != new IntPtr(-1))
                {
                    CH9329CloseDevice(_deviceHandle);
                    _deviceHandle = IntPtr.Zero;
                }

                _isConnected = false;
                StatusChanged?.Invoke(this, "设备已断开");
            }
            catch (Exception ex)
            {
                StatusChanged?.Invoke(this, $"断开连接错误: {ex.Message}");
            }
        }

        /// <summary>
        /// 发送数据
        /// </summary>
        public bool SendData(byte[] data)
        {
            try
            {
                if (!_isConnected || _deviceHandle == IntPtr.Zero)
                {
                    StatusChanged?.Invoke(this, "设备未连接");
                    return false;
                }

                // 使用CH9329WriteData发送数据
                // 注意：CH9329WriteData需要一个事件对象，这里使用IntPtr.Zero
                bool result = CH9329WriteData(_deviceHandle, data, (uint)data.Length, IntPtr.Zero);
                return result;
            }
            catch (Exception ex)
            {
                StatusChanged?.Invoke(this, $"发送数据错误: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// 获取设备配置
        /// </summary>
        public bool GetConfiguration(out USBDevInfo config)
        {
            config = new USBDevInfo();
            try
            {
                if (!_isConnected || _deviceHandle == IntPtr.Zero)
                {
                    return false;
                }

                return CH9329GetCFG(_deviceHandle, ref config);
            }
            catch (Exception ex)
            {
                StatusChanged?.Invoke(this, $"获取配置错误: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// 设置设备配置
        /// </summary>
        public bool SetConfiguration(USBDevInfo config)
        {
            try
            {
                if (!_isConnected || _deviceHandle == IntPtr.Zero)
                {
                    return false;
                }

                return CH9329SetCFG(_deviceHandle, config);
            }
            catch (Exception ex)
            {
                StatusChanged?.Invoke(this, $"设置配置错误: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// 设置波特率
        /// </summary>
        public bool SetBaudRate(uint baudRate)
        {
            try
            {
                if (!_isConnected || _deviceHandle == IntPtr.Zero)
                {
                    return false;
                }

                return CH9329SetRate(_deviceHandle, baudRate);
            }
            catch (Exception ex)
            {
                StatusChanged?.Invoke(this, $"设置波特率错误: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// 恢复默认配置
        /// </summary>
        public bool RestoreDefaults()
        {
            try
            {
                if (!_isConnected || _deviceHandle == IntPtr.Zero)
                {
                    return false;
                }

                return CH9329SetDEF(_deviceHandle);
            }
            catch (Exception ex)
            {
                StatusChanged?.Invoke(this, $"恢复默认配置错误: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// 复位设备
        /// </summary>
        public bool ResetDevice()
        {
            try
            {
                if (!_isConnected || _deviceHandle == IntPtr.Zero)
                {
                    return false;
                }

                return CH9329Reset(_deviceHandle);
            }
            catch (Exception ex)
            {
                StatusChanged?.Invoke(this, $"复位设备错误: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// 启动接收线程
        /// </summary>
        private void StartReceiveThread()
        {
            try
            {
                if (!CH9329InitThreadData(_deviceHandle))
                {
                    StatusChanged?.Invoke(this, "初始化接收线程失败");
                    return;
                }

                _receiveCts = new CancellationTokenSource();
                _receiveTask = Task.Run(() => ReceiveLoop(_receiveCts.Token));
            }
            catch (Exception ex)
            {
                StatusChanged?.Invoke(this, $"启动接收线程错误: {ex.Message}");
            }
        }

        /// <summary>
        /// 停止接收线程
        /// </summary>
        private void StopReceiveThread()
        {
            try
            {
                _receiveCts?.Cancel();

                if (_receiveTask != null)
                {
                    // 等待线程结束，最多等待2秒
                    if (!_receiveTask.Wait(2000))
                    {
                        StatusChanged?.Invoke(this, "接收线程停止超时");
                    }
                }

                if (_deviceHandle != IntPtr.Zero)
                {
                    CH9329StopThread(_deviceHandle);
                }

                _receiveCts?.Dispose();
                _receiveCts = null;
                _receiveTask = null;
            }
            catch (Exception ex)
            {
                StatusChanged?.Invoke(this, $"停止接收线程错误: {ex.Message}");
            }
        }

        /// <summary>
        /// 接收循环
        /// </summary>
        private void ReceiveLoop(CancellationToken cancellationToken)
        {
            byte[] buffer = new byte[1024];

            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    uint len = (uint)buffer.Length;

                    if (CH9329ReadThreadData(_deviceHandle, buffer, ref len))
                    {
                        if (len > 0)
                        {
                            // 复制实际接收到的数据
                            byte[] receivedData = new byte[len];
                            Array.Copy(buffer, receivedData, len);

                            // 触发事件
                            DataReceived?.Invoke(this, receivedData);
                        }
                    }
                    else
                    {
                        // 读取失败，可能是设备断开
                        Thread.Sleep(100);
                    }
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    StatusChanged?.Invoke(this, $"接收数据错误: {ex.Message}");
                    Thread.Sleep(100);
                }
            }
        }

        /// <summary>
        /// 获取可用设备列表
        /// </summary>
        public static System.Collections.Generic.List<string> GetAvailableDevices()
        {
            var devices = new System.Collections.Generic.List<string>();

            try
            {
                uint index = 0;
                while (true)
                {
                    // 分配足够大的缓冲区
                    string devicePath = new string('\0', 512);

                    if (CH9329GetDevicePath(index, devicePath, 512))
                    {
                        // 提取实际的设备路径（去掉多余的空字符）
                        int nullIndex = devicePath.IndexOf('\0');
                        if (nullIndex > 0)
                        {
                            devicePath = devicePath.Substring(0, nullIndex);
                        }

                        if (!string.IsNullOrWhiteSpace(devicePath))
                        {
                            devices.Add(devicePath);
                        }
                    }
                    else
                    {
                        break;
                    }

                    index++;
                }
            }
            catch (Exception)
            {
                // 忽略错误
            }

            return devices;
        }

        public void Dispose()
        {
            Disconnect();
        }
    }
}
