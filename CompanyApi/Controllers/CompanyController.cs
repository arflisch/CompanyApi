using Application;
using Domain.DTO;
using Microsoft.AspNetCore.Mvc;


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
            _logger.LogInformation("Creating a new company with Name: {CompanyName} and Vat: {CompanyVat}", companydto.Name, companydto.Vat);

            var result = await createCompanyCommand.CreateCompanyAsync(companydto);
            if (result.IsSuccess)
            {
                _logger.LogInformation("Company created successfully");
                // Fix: Use the correct syntax for CreatedAtAction
                return CreatedAtAction(nameof(CreateCompany), result);
            }

            if (result.Errors.Any(e => e is ValidationError))
            {
                var errors = result.Errors
                    .OfType<ValidationError>()
                    .Select(e => e.Message)
                    .ToArray();

                _logger.LogWarning("Validation errors occurred while creating company: {Errors}", string.Join(", ", errors));

                var validationProblemDetails = new ValidationProblemDetails
                {
                    Title = "Validation Errors",
                    Status = StatusCodes.Status400BadRequest, // Fix: Replace StatusCode.Status400BadRequest with the correct status code value
                    
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
            _logger.LogError("An error occurred while creating company: {Detail}", problemDetails.Detail);
            return BadRequest(problemDetails);
        }

        [HttpDelete("{id:long}")]
        public async Task<IActionResult> DeleteCompany(long id, [FromServices] IDeleteCompanyCommand deleteCompanyCommand)
        {
            _logger.LogInformation("Deleting company with Id: {CompanyId}", id);

            var result = await deleteCompanyCommand.DeleteCompanyAsync(id);
            if (result.IsSuccess)
            {
                _logger.LogInformation("Company with Id: {CompanyId} deleted successfully", id);
                return Ok();
            }
            if (result.Errors.Any(e => e is ValidationError))
            {
                var errors = result.Errors
                    .OfType<ValidationError>()
                    .Select(e => e.Message)
                    .ToArray();

                _logger.LogWarning("Validation errors occurred while deleting company with Id: {CompanyId}. Errors: {Errors}", id, string.Join(", ", errors));

                var validationProblemDetails = new ValidationProblemDetails
                {
                    Title = "Validation Errors",
                    Status = StatusCodes.Status400BadRequest,
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
            _logger.LogError("An error occurred while deleting company with Id: {CompanyId}. Detail: {Detail}", id, problemDetails.Detail);
            return BadRequest(problemDetails);
        }

        [HttpPut("{id:long}")]
        public async Task<IActionResult> UpdateCompany(long id, [FromBody] CompanyDto companydto, [FromServices] IUpdateCompanyCommand updateCompanyCommand)
        {
            _logger.LogInformation("Updating company with Id: {CompanyId}", id);

            var result = await updateCompanyCommand.UpdateCompanyAsync(id, companydto);
            if (result.IsSuccess)
            {
                _logger.LogInformation("Company with Id: {CompanyId} updated successfully", id);
                return Ok();
            }
            

            if (result.Errors.Any(e => e is ValidationError))
            {
                var errors = result.Errors
                    .OfType<ValidationError>()
                    .Select(e => e.Message)
                    .ToArray();

                _logger.LogWarning("Validation errors occurred while updating company with Id: {CompanyId}. Errors: {Errors}", id, string.Join(", ", errors));

                var validationProblemDetails = new ValidationProblemDetails
                {
                    Title = "Validation Errors",
                    Status = StatusCodes.Status400BadRequest, // Fix: Replace StatusCode.Status400BadRequest with the correct status code value
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
            _logger.LogError("An error occurred while updating company with Id: {CompanyId}. Detail: {Detail}", id, problemDetails.Detail);
            return BadRequest(problemDetails);
        }

        [HttpPatch("name/{id:long}/{Name}")]
        public async Task<IActionResult> PatchCompanyName(long id, string Name, [FromServices] IPatchCompanyCommand repository)
        {
            _logger.LogInformation("Patching company name for Id: {CompanyId}", id);

            var result = await repository.PatchCompanyNameAsync(id, Name);
            if (result.IsSuccess)
            {
                _logger.LogInformation("Company name patched successfully for Id: {CompanyId}", id);
                return Ok();
            }
            _logger.LogError("An error occurred while patching company name for Id: {CompanyId}. Errors: {Errors}", id, string.Join("; ", result.Errors.Select(e => e.Message)));
            return BadRequest(result.Errors);
        }

        [HttpPatch("Vat/{id:long}/{Vat}")]
        public async Task<IActionResult> PatchCompanyVat(long id, string Vat, [FromServices] IPatchCompanyCommand repository)
        {
            _logger.LogInformation("Patching company VAT for Id: {CompanyId}", id);
            var result = await repository.PatchCompanyVatAsync(id, Vat);
            if (result.IsSuccess)
            {
                _logger.LogInformation("Company VAT patched successfully for Id: {CompanyId}", id);
                return Ok();
            }
            if (result.Errors.Any(e => e is ValidationError))
            {
                var errors = result.Errors
                    .OfType<ValidationError>()
                    .Select(e => e.Message)
                    .ToArray();

                _logger.LogWarning("Validation errors occurred while patching company VAT for Id: {CompanyId}. Errors: {Errors}", id, string.Join(", ", errors));
                var validationProblemDetails = new ValidationProblemDetails
                {
                    Title = "Validation Errors",
                    Status = StatusCodes.Status400BadRequest, // Fix: Replace StatusCode.Status400BadRequest with the correct status code value
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
            _logger.LogError("An error occurred while patching company VAT for Id: {CompanyId}. Detail: {Detail}", id, problemDetails.Detail);
            return BadRequest(problemDetails);
        }
    }
}
