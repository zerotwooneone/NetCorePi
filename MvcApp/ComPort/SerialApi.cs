using System;
using System.IO.Ports;

namespace MvcApp.ComPort
{
    public class SerialApi : IDisposable
    {
        private readonly SerialPort _serialPort;

        public SerialApi(SerialPort serialPort)
        {
            _serialPort = serialPort ?? throw new ArgumentNullException(nameof(serialPort));

            _serialPort
                .DataReceived += OnDataReceived;
        }
        
        public void Dispose()
        {
            _serialPort.Dispose();
        }

        public bool IsOpen => _serialPort.IsOpen;

        public virtual void Open()
        {
            _serialPort.Open();
        }

        public virtual void Close()
        {
            _serialPort.Close();
        }

        public event SerialDataReceivedEventHandler DataReceived;

        protected virtual void RaiseOnDataReceived(SerialDataReceivedEventArgs e)
        {
            DataReceived?.Invoke(this, e);
        }

        private void OnDataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            RaiseOnDataReceived(e);
        }

        public virtual string ReadExisting()
        {
            return _serialPort.ReadExisting();
        }
    }
}