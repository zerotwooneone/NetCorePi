using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using MvcApp.ComPort;

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
            return _comPortAccessor.GetSystemPortNames();
        }

        private readonly ComPortAccessor _comPortAccessor;
        private readonly SystemConfig _systemConfig;
        
        public ComPortController(ComPortAccessor comPortAccessor,
            SystemConfig systemConfig)
        {
            _comPortAccessor = comPortAccessor;
            _systemConfig = systemConfig;
        }

        // GET: api/ComPort/Selected
        [Route("Selected")]
        [HttpGet]
        public IActionResult GetSelected()
        {
            return Ok(_systemConfig.DefaultSerialPort);
        }
    }
}
