using Application;
using Database;
using Domain;
using Domain.DTO;
using Microsoft.AspNetCore.Mvc;
using System.Xml.Linq;

namespace CompanyApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CompanyController : Controller
    {
        private readonly ILogger<CompanyController> _logger;

        public CompanyController(ILogger<CompanyController> logger)
        {
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> CreateCompany([FromBody] CompanyDto companydto, [FromServices] ICreateCompanyCommand createCompanyCommand)
        {
            var result = await createCompanyCommand.CreateCompanyAsync(companydto);
            if (result.IsSuccess)
            {
                // Fix: Use the correct syntax for CreatedAtAction
                return CreatedAtAction(nameof(CreateCompany), new { id = companydto.Id }, result);
            }

            if (result.Errors.Any(e => e is ValidationError))
            {
                var errors = result.Errors
                    .OfType<ValidationError>()
                    .Select(e => e.Message)
                    .ToArray();

                var validationProblemDetails = new ValidationProblemDetails
                {
                    Title = "Validation Errors",
                    Status = 400, // Fix: Replace StatusCode.Status400BadRequest with the correct status code value
                    
                };
                validationProblemDetails.Errors.Add("ValidationErrors", errors);

                return ValidationProblem((ValidationProblemDetails)validationProblemDetails.Errors);
            }

            var problemDetails = new ProblemDetails
            {
                Title = "An error occurred",
                Status = StatusCodes.Status400BadRequest,
                Detail = string.Join("; ", result.Errors.Select(e => e.Message))
            };
            return BadRequest(problemDetails);
        }

        [HttpDelete("{id:long}")]
        public async Task<IActionResult> DeleteCompany(long id, [FromServices] IDeleteCompanyCommand deleteCompanyCommand)
        {
            var result = await deleteCompanyCommand.DeleteCompanyAsync(id);
            if (result.IsSuccess)
            {
                return Ok();
            }
            return BadRequest(result.Errors);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateCompany([FromBody] CompanyDto companydto, [FromServices] IUpdateCompanyCommand updateCompanyCommand)
        {
            var result = await updateCompanyCommand.UpdateCompanyAsync(companydto);
            if (result.IsSuccess)
            {
                return Ok();
            }
            return BadRequest(result.Errors);
        }

        [HttpPatch("name/{id:long}/{Name}")]
        public async Task<IActionResult> PatchCompanyName(long id, string Name, [FromServices] IPatchCompanyCommand repository)
        {
            var result = await repository.PatchCompanyNameAsync(id, Name);
            if (result.IsSuccess)
            {
                return Ok();
            }
            return BadRequest(result.Errors);
        }

        [HttpPatch("Vat/{id:long}/{Vat}")]
        public async Task<IActionResult> PatchCompanyVat(long id, string Vat, [FromServices] IPatchCompanyCommand repository)
        {
            var result = await repository.PatchCompanyVatAsync(id, Vat);
            if (result.IsSuccess)
            {
                return Ok();
            }
            return BadRequest(result.Errors);
        }
    }
}
