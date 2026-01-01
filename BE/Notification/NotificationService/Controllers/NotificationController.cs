using Dapr;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NotificationService.Models.DTO;

namespace NotificationService.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private readonly ILogger<NotificationController> _logger;

        public NotificationController(ILogger<NotificationController> logger)
        {
            _logger = logger;
        }

        [Topic("rabbitmq-pubsub", "companycreated")]
        [HttpPost("send-email")]
        public async Task<IActionResult> SendNotification([FromBody] CompanyEvent company)
        {
            _logger.LogInformation("Event received for Company: {CompanyName}, VAT: {CompanyVat}", company.Name, company.Vat);

            try
            {
                SimulateEmailSending(company);
                _logger.LogInformation("Email sent for Company: {CompanyName}", company.Name);
                return Ok();
            }
            catch (Exception ex) 
            {
                _logger.LogError(ex, "Error sending email for Company: {CompanyName}", company.Name);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        private void SimulateEmailSending(CompanyEvent company)
        {
            Console.WriteLine($"[MOCK EMAIL] To: admin@vinci.be | Subject: Welcome {company.Name}");
        }
    }
}
