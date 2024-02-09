using iot_services.Helpers;
using iot_services.Models;
using iot_services.Services;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace iot_services.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TempSensorController : ControllerBase
    {
        private readonly InfluxDBService service;

        public TempSensorController(TempSensorInfluxDBService service)
        {
            this.service = service;
        }

        [HttpPost]
        public async Task<ActionResult> Add([FromBody] TempSensor meas)
        {
            await service.Add(meas, DateTime.UtcNow.RemoveMillisecond());

            return Ok();
        }
    }
}