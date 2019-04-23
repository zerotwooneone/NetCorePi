using System;
using MvcApp.JsonConfig;

namespace MvcApp.ComPort
{
    public class SystemConfig :IValidatableConfig
    {
        public string DefaultSerialPort { get; set; }
        public void Validate()
        {
            if(string.IsNullOrWhiteSpace(DefaultSerialPort)) throw new ApplicationException($"{nameof(SystemConfig)}.{nameof(DefaultSerialPort)} must have a value.");
        }
    }
}