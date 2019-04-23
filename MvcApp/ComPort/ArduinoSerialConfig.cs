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

        public ArduinoSerialConfig()
        {
            //BaudRate = 115200;
            //Parity = Parity.None;
            //StopBits = StopBits.One;
            //DataBits = 8;
            //Handshake = Handshake.None;
        }

        public void Validate()
        {
            int x = 0;
        }
    }
}