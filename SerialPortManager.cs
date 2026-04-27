using System;
using System.IO.Ports;

namespace KeyboardController
{
    /// <summary>
    /// 串口通信管理类
    /// </summary>
    public class SerialPortManager : IDisposable
    {
        private readonly SerialPort _serialPort;
        private bool _isConnected = false;

        public bool IsConnected => _isConnected;
        public string[] AvailablePorts => SerialPort.GetPortNames();

        public event EventHandler<byte[]>? DataReceived;
        public event EventHandler<string>? StatusChanged;

        public SerialPortManager()
        {
            _serialPort = new SerialPort();
            _serialPort.DataReceived += OnDataReceived;
        }

        private void OnDataReceived(object? sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                int bytesToRead = _serialPort.BytesToRead;
                byte[] buffer = new byte[bytesToRead];
                _serialPort.Read(buffer, 0, bytesToRead);
                DataReceived?.Invoke(this, buffer);
            }
            catch (Exception ex)
            {
                StatusChanged?.Invoke(this, $"接收数据错误: {ex.Message}");
                // 设备可能已断开，更新连接状态
                _isConnected = false;
                try
                {
                    if (_serialPort.IsOpen)
                    {
                        _serialPort.Close();
                    }
                }
                catch { }
            }
        }

        public bool Connect(string portName, int baudRate, Parity parity = Parity.None, 
            int dataBits = 8, StopBits stopBits = StopBits.One)
        {
            try
            {
                if (_isConnected)
                {
                    Disconnect();
                }

                _serialPort.PortName = portName;
                _serialPort.BaudRate = baudRate;
                _serialPort.Parity = parity;
                _serialPort.DataBits = dataBits;
                _serialPort.StopBits = stopBits;

                _serialPort.Open();
                _isConnected = true;
                StatusChanged?.Invoke(this, $"已连接到 {portName} ({baudRate} baud)");
                return true;
            }
            catch (Exception ex)
            {
                StatusChanged?.Invoke(this, $"连接失败: {ex.Message}");
                return false;
            }
        }

        public void Disconnect()
        {
            try
            {
                if (_serialPort.IsOpen)
                {
                    _serialPort.Close();
                }
                _isConnected = false;
                StatusChanged?.Invoke(this, "已断开连接");
            }
            catch (Exception ex)
            {
                StatusChanged?.Invoke(this, $"断开连接错误: {ex.Message}");
            }
        }

        public bool SendData(byte[] data)
        {
            try
            {
                if (!_isConnected || !_serialPort.IsOpen)
                {
                    StatusChanged?.Invoke(this, "串口未连接");
                    return false;
                }

                _serialPort.Write(data, 0, data.Length);
                return true;
            }
            catch (Exception ex)
            {
                StatusChanged?.Invoke(this, $"发送数据错误: {ex.Message}");
                // 设备可能已断开，更新连接状态
                _isConnected = false;
                try
                {
                    if (_serialPort.IsOpen)
                    {
                        _serialPort.Close();
                    }
                }
                catch { }
                return false;
            }
        }

        public void Dispose()
        {
            Disconnect();
            _serialPort.Dispose();
        }
    }
}
