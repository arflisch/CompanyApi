using Dapr;
using Domain.DTO;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace CompanyApi.Controllers
{
    [ApiController]
    [Route("")]
    public class EventController : ControllerBase
    {
        private readonly ILogger<EventController> _logger;
        public EventController(ILogger<EventController> logger)
        {
            _logger = logger;
        }

       // [Topic("rabbitmq-pubsub", "companycreated")]
        [HttpPost("events")]
        public async Task<IActionResult> HandleEvent([FromBody] CompanyDto cloudEvent)
        {
            //var data = cloudEvent.GetProperty("data");

            // Process the event
            _logger.LogInformation("Received company created event: {Data}", cloudEvent);

            return Ok();
        }
    }
}
