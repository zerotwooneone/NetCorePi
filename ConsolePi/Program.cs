using System;
using System.Device.Gpio;
using System.Threading;

namespace NetCorePi
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            int pin = 17;
            GpioController controller = new GpioController();
            controller.OpenPin(pin, PinMode.Output);

            int lightTimeInMilliseconds = 1000;
            int dimTimeInMilliseconds = 200;
            
            while (true)
            {
                Console.WriteLine($"Light for {lightTimeInMilliseconds}ms");
                controller.Write(pin, PinValue.High);
                Thread.Sleep(lightTimeInMilliseconds);
                Console.WriteLine($"Dim for {dimTimeInMilliseconds}ms");
                controller.Write(pin, PinValue.Low);
                Thread.Sleep(dimTimeInMilliseconds); 
            }
        }
    }
}
