using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;

namespace MvcApp.ComPort
{
    public class ComPortAccessor
    {
        private static readonly IDictionary<string, SerialApi> SerialPorts = new ConcurrentDictionary<string, SerialApi>();

        public virtual IEnumerable<string> GetSystemPortNames()
        {
            return SerialPort.GetPortNames();
        }

        public virtual IEnumerable<string> GetCreatedPortNames()
        {
            return SerialPorts.Keys;
        }
        
        public virtual bool TryGet(string portName, out SerialApi result)
        {
            if (!IsValidPortName(portName))
            {
                result = null;
                return false;
            }

            return SerialPorts.TryGetValue(portName, out result);
        }

        public virtual bool TryCreate(string portName, 
            out SerialApi result, 
            Action<SerialPort> configure = null)
        {
            if (!IsValidPortName(portName) ||
            SerialPorts.ContainsKey(portName))
            {
                result = null;
                return false;
            }

            var serialPort = new SerialPort(portName);
            configure?.Invoke(serialPort);
            
            var serialApi = new SerialApi(serialPort);
            var bResult = SerialPorts.TryAdd(portName,  serialApi);
            if (bResult)
            {
                result = serialApi;
            }
            else
            {
                result = null;
                serialApi.Dispose();
            }

            return bResult;
        }

        public virtual bool IsValidPortName(string portName)
        {
            return !string.IsNullOrWhiteSpace(portName) && 
                   GetSystemPortNames().Contains(portName);
        }
    }
}