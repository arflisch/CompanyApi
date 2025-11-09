using Dapr;
using Domain.DTO;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace CompanyApi.Controllers
{
    [ApiController]
    [Route("company/events")]
    public class CompanyEventsController : ControllerBase
    {
        private readonly ILogger<CompanyEventsController> _logger;
        public CompanyEventsController(ILogger<CompanyEventsController> logger)
        {
            _logger = logger;
        }

        [Topic("rabbitmq-pubsub", "companycreated")]
        [HttpPost("created")]
        public IActionResult HandleCompanyCreated([FromBody] CompanyDto companyDto)
        {
            _logger.LogInformation("Processing CompanyCreated: {Name}", companyDto.Name);
            return Ok();
        }
    }
}
