using MvcApp.JsonConfig;

namespace MvcApp.ComPort
{
    public class SystemConfig :IValidatableConfig
    {
        public string DefaultSerialPort { get; set; }
        public void Validate()
        {
            int x = 0;
        }
    }
}