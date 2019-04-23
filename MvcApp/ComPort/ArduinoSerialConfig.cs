using System;
using System.IO.Ports;
using MvcApp.JsonConfig;

namespace MvcApp.ComPort
{
    public class ArduinoSerialConfig : IValidatableConfig
    {
        public Parity Parity { get; set; }
        public int BaudRate { get; set; }
        public StopBits StopBits { get; set; }
        public int DataBits { get; set; }
        public Handshake Handshake { get; set; }

        public void Validate()
        {
            if((int)StopBits < 1 ||(int)StopBits> 3) throw new ApplicationException($"{nameof(StopBits)} config value is invalid.");

            if((int)Parity < 0 ||(int)Parity> 4) throw new ApplicationException($"{nameof(Parity)} config value is invalid.");

            if((int)Handshake < 0 ||(int)Handshake> 3) throw new ApplicationException($"{nameof(Handshake)} config value is invalid.");

            if(BaudRate <1) throw new ApplicationException($"{nameof(BaudRate)} config value is invalid.");

            if(DataBits <1) throw new ApplicationException($"{nameof(DataBits)} config value is invalid.");
        }
    }
}