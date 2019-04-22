using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using Microsoft.AspNetCore.Mvc;

namespace MvcApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ComPortController : ControllerBase
    {
        // GET: api/ComPort
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return SerialPort.GetPortNames();
        }

        private static string _selected = null;
        private static SerialPort _serialPort = null;
        private static ISubject<string> _serialDataSubject = new Subject<string>();
        public IObservable<string> RawObservable => _serialDataSubject.AsObservable();

        private static IObservable<string> RawLines;
        
        static ComPortController()
        {
            var oneSource = _serialDataSubject
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

        // GET: api/ComPort/Selected
        [Route("Selected")]
        [HttpGet]
        public string GetSelected()
        {
            var selected = _selected;
            if (isSelectedValid(selected))
            {
                Response.Headers.Add("Error", $"{selected} is not valid");
            }
            return selected;
        }

        private static bool isSelectedValid(string selected)
        {
            return selected !=null && 
                   !SerialPort.GetPortNames().Contains(selected);
        }

        // PUT: api/ComPort/Selected
        [HttpPut]
        [Route("Selected")]
        public IActionResult PutSelected([FromBody] string value)
        {
            if (isSelectedValid(value))
            {
                return BadRequest("Com Port Not Valid");
            }

            if (_serialPort != null)
            {
                try
                {
                    _serialPort.Close();
                    
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e);
                }
                finally
                {
                    try
                    {
                        _serialPort.Dispose();
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine(e);
                    }
                    _serialPort = null;
                }
            }

            const int baudRate=115200;
            _serialPort = new SerialPort(value, baudRate, Parity.None)
            {
                StopBits = StopBits.One,
                DataBits = 8,
                Handshake = Handshake.None
            };

            _serialPort.DataReceived -= DataReceived;
            _serialPort.DataReceived += DataReceived;

            if (!_serialPort.IsOpen)
            {
                _serialPort.Open();
            }

            _selected = value;
            return Ok();
        }

        // PUT: api/ComPort/Selected
        [HttpDelete]
        [Route("Selected")]
        public IActionResult DeleteSelected()
        {
            if (string.IsNullOrWhiteSpace(_selected))
            {
                return BadRequest("No Com Port Is Selected");
            }
            
            _serialPort.Close();
            _serialPort.Dispose();
            _serialPort = null;
            
            return Ok();
        }

        private void DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            var serialPort = (SerialPort)sender;
 
            // Read the data that's in the serial buffer.
            var serialdata = serialPort.ReadExisting();
 
            // Write to debug output.
            Debug.Write(serialdata);

            _serialDataSubject.OnNext(serialdata);
        }

        //// GET: api/ComPort/5
        //[HttpGet("{id}", Name = "Get")]
        //public string Get(int id)
        //{
        //    return "value";
        //}

        //// POST: api/ComPort
        //[HttpPost]
        //public void Post([FromBody] string value)
        //{
        //}

        //// PUT: api/ComPort/5
        //[HttpPut("{id}")]
        //public void Put(int id, [FromBody] string value)
        //{
        //}

        //// DELETE: api/ApiWithActions/5
        //[HttpDelete("{id}")]
        //public void Delete(int id)
        //{
        //}
    }
}
