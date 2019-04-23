using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reactive.Threading.Tasks;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace MvcApp.ComPort
{
    public class ComPortService : IHostedService
    {
        private readonly ComPortAccessor _comPortAccessor;
        private readonly ArduinoSerialConfig _arduinoSerialConfig;
        private readonly SystemConfig _systemConfig;
        private readonly ISubject<string> _serialDataSubject = new Subject<string>();
        public readonly IObservable<string> RawObservable;
        public readonly IObservable<string> RawLines;
        public IObservable<double> HallValues { get; }
        public IObservable<double> TempValues { get; }
        public IObservable<double> MagSpringValues { get; }
        public IObservable<double> EmfValues { get; }

        public ComPortService(ComPortAccessor comPortAccessor,
            ArduinoSerialConfig arduinoSerialConfig,
            SystemConfig systemConfig)
        {
            _comPortAccessor = comPortAccessor;
            _arduinoSerialConfig = arduinoSerialConfig;
            _systemConfig = systemConfig;
            RawObservable = _serialDataSubject
                .AsObservable();

            var oneSource = RawObservable
                .Publish()
                .RefCount();

            RawLines = oneSource
                .Scan(seed:string.Empty, accumulator:(previous,current)=> (previous.EndsWith(Environment.NewLine) ? "" : previous) + current)
                .Where(s=>s.EndsWith(Environment.NewLine))
                .SelectMany(s=> s.Split(Environment.NewLine).Where(s2=>s2.Any()));

            //RawLines
            //    .Subscribe(s =>
            //    {
            //        Debug.WriteLine(s);
            //    });

            HallValues = FindAndRemovePrefix(RawLines, "hall:").Select(s=>double.TryParse(s, out var dVal)? dVal : double.NaN);
            TempValues = FindAndRemovePrefix(RawLines, "temp:").Select(s=>double.TryParse(s, out var dVal)? dVal : double.NaN);
            MagSpringValues = FindAndRemovePrefix(RawLines, "magSpring:").Select(s=>double.TryParse(s, out var dVal)? dVal : double.NaN);
            EmfValues = FindAndRemovePrefix(RawLines, "emf:").Select(s=>double.TryParse(s, out var dVal)? dVal : double.NaN);

            EmfValues
                .Subscribe(s =>
                {
                    Debug.WriteLine(s);
                });
        }

        private static IObservable<string> FindAndRemovePrefix(IObservable<string> observable, string prefix)
        {
            return observable.Where(s => s.StartsWith(prefix)).Select(s => s.Replace(prefix,string.Empty));
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var comPortName = _systemConfig.DefaultSerialPort;
            if (!_comPortAccessor.TryGet(comPortName, out var serialApi))
            {
                if (_comPortAccessor.TryCreate(comPortName, out serialApi, ConfigureComPort))
                {
                    if(!serialApi.IsOpen)
                    {
                        serialApi.Open();   
                        serialApi.DataReceived += DataReceived;
                    } 
                }else
                {
                    throw new ApplicationException($"Could not open the {nameof(SystemConfig.DefaultSerialPort)}.");
                }
            }

            await Task.WhenAll(RawObservable
                .ToTask(cancellationToken),
                RawLines.ToTask(cancellationToken));
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
            //Debug.Write(serialData);

            _serialDataSubject.OnNext(serialData);
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _serialDataSubject.OnCompleted();
        }
    }
}
