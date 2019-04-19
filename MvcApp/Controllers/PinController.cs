using System;
using System.Collections.Generic;
using System.Device.Gpio;
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
            controller.OpenPin(Pin, PinMode.Output);
            return controller;
        });

        const int Pin = 17;
        const int LightTimeInMilliseconds = 1000;
        const int DimTimeInMilliseconds = 200;
        
        // GET: api/Pin
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            for (int i = 0; i < 4; i++)
            {
                Console.WriteLine($"Light for {LightTimeInMilliseconds}ms");
                Controller.Write(Pin, PinValue.High);
                await Task.Delay(TimeSpan.FromMilliseconds(LightTimeInMilliseconds));
                Console.WriteLine($"Dim for {DimTimeInMilliseconds}ms");
                Controller.Write(Pin, PinValue.Low);
                await Task.Delay(TimeSpan.FromMilliseconds(DimTimeInMilliseconds));
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
