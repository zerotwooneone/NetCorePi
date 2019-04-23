using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using MvcApp.ComPort;

namespace MvcApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ComPortController : ControllerBase
    {
        private readonly ComPortAccessor _comPortAccessor;
        private readonly ArduinoSerialConfig _arduinoSerialConfig;
        private readonly SystemConfig _systemConfig;

        // GET: api/ComPort
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return _comPortAccessor.GetSystemPortNames();
        }

        private static string _selected = null;
        
        private static ISubject<string> _serialDataSubject = new Subject<string>();
        private static IObservable<string> RawObservable => _serialDataSubject.AsObservable();

        private static IObservable<string> RawLines;
        
        static ComPortController()
        {
            var oneSource = RawObservable
                .Publish()
                .RefCount();

            var onlySep = oneSource
                .Where(c => c == Environment.NewLine);

            RawLines = oneSource
                .Buffer(onlySep)
                .Select(stringList =>
                {
                    var sb = new StringBuilder(stringList.Count);
                    foreach (var s in stringList)
                    {
                        var sanitized = s.Replace(Environment.NewLine, "");
                        sb.Append(sanitized);
                    }
                    return sb.ToString();
                })
                .Where(s=>!string.IsNullOrWhiteSpace(s));

            RawLines
                .Subscribe(s => { Debug.WriteLine(s); });
        }

        public ComPortController(ComPortAccessor comPortAccessor,
            ArduinoSerialConfig arduinoSerialConfig,
            SystemConfig systemConfig)
        {
            _comPortAccessor = comPortAccessor;
            _arduinoSerialConfig = arduinoSerialConfig;
            _systemConfig = systemConfig;
        }

        // GET: api/ComPort/Selected
        [Route("Selected")]
        [HttpGet]
        public IActionResult GetSelected()
        {
            var selected = string.IsNullOrWhiteSpace(_selected) ? _systemConfig.DefaultSerialPort : _selected;
            if (!_comPortAccessor.TryGet(selected, out var serialApi))
            {
                if (_comPortAccessor.TryCreate(selected, out serialApi, ConfigureComPort))
                {
                    if(!serialApi.IsOpen)
                    {
                        serialApi.Open();   
                        serialApi.DataReceived += DataReceived;
                    } 
                }else
                {
                    return NotFound(selected);
                }
            }

            
            return Ok(selected);
        }
        
        public void ConfigureComPort(SerialPort serialPort)
        {
            ConfigureComPort(serialPort, _arduinoSerialConfig);
        }

        public static void ConfigureComPort(SerialPort serialPort,
            ArduinoSerialConfig arduinoSerialConfig)
        {
            serialPort.BaudRate = arduinoSerialConfig.BaudRate;
            serialPort.DataBits = arduinoSerialConfig.DataBits;
            serialPort.Handshake = arduinoSerialConfig.Handshake;
            serialPort.Parity = arduinoSerialConfig.Parity;
            serialPort.StopBits = arduinoSerialConfig.StopBits;
        }

        private void DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            var serialPort = (SerialApi)sender;
 
            // Read the data that's in the serial buffer.
            var serialData = serialPort.ReadExisting();
 
            // Write to debug output.
            Debug.Write(serialData);

            _serialDataSubject.OnNext(serialData);
        }
    }
}
