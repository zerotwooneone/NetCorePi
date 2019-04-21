using System;
using System.Collections.Generic;
using System.Device.Gpio;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace MvcApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PinController : ControllerBase
    {
        private static readonly Lazy<GpioController> LazyController = new Lazy<GpioController>(() =>
        {
            GpioController controller = new GpioController();
            controller.OpenPin(DhtPin, PinMode.Output);
            return controller;
        });

        const int DhtPin = 4;
        private static readonly TimeSpan TempHumLowDelay = TimeSpan.FromMilliseconds(18);
        private static readonly TimeSpan TempHumHighDelay = TimeSpan.FromMilliseconds(40);
        private static readonly TimeSpan TempHumReadDelay = TimeSpan.FromMilliseconds(1);
        private const int MaxTimings = 85;


        // GET: api/Pin
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                Controller.SetPinMode(DhtPin, PinMode.Output);
                Controller.Write(DhtPin, PinValue.Low);
            
                await Task.Delay(TempHumLowDelay);

                Controller.Write(DhtPin, PinValue.High);

                await Task.Delay(TempHumHighDelay);

                Controller.SetPinMode(DhtPin, PinMode.Input);
                var laststate = PinValue.High;
                var dht11_dat = new byte[] {0, 0, 0, 0, 0};
                int j = 0;
                double f;

                for (int i = 0; i < MaxTimings; i++ )
                {
                    int counter = 0;
                    while ( Controller.Read(DhtPin) == laststate )
                    {
                        counter++;
                        await Task.Delay(TempHumReadDelay);
                        if ( counter == 255 )
                        {
                            break;
                        }
                    }
                    laststate = Controller.Read(DhtPin);
 
                    if ( counter == 255 )
                        break;
 
                    if ( (i >= 4) && (i % 2 == 0) )
                    {
                        dht11_dat[j / 8] <<= 1;
                        if ( counter > 16 )
                            dht11_dat[j / 8] |= 1;
                        j++;
                    }
                }
 
                f = dht11_dat[2] * 9.0 / 5.0 + 32;
                Console.WriteLine( $"dht11_dat[0].dht11_dat[1] = {dht11_dat[0]}.{dht11_dat[1]} \n dht11_dat[2].dht11_dat[3] = {dht11_dat[2]}.{dht11_dat[3]} \n F ={f} \n");

                if ( (j >= 40) &&
                     (dht11_dat[4] == ( (dht11_dat[0] + dht11_dat[1] + dht11_dat[2] + dht11_dat[3]) & 0xFF) ) )
                {
                    
                }else  {
                    Console.WriteLine( "Data not good, skip\n" );
                }
                Controller.SetPinMode(DhtPin, PinMode.Output);
                Controller.Write(DhtPin, PinValue.Low);
            }
            catch (Exception e)
            {
                Console.WriteLine($"\n{e}\n");
                throw;
            }

            return Ok(new string[] { "value1", "value2" });
        }

        private GpioController Controller => LazyController.Value;

        // GET: api/Pin/5
        [HttpGet("{id}", Name = "Get")]
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/Pin
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT: api/Pin/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE: api/Pin/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
