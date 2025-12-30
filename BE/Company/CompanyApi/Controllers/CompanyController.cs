using Application;
using Domain;
using Domain.DTO;
using Microsoft.AspNetCore.Mvc;


namespace CompanyApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [ApiExplorerSettings(GroupName = "facade")]
    public class CompanyController : Controller
    {
        private readonly ILogger<CompanyController> _logger;

        public CompanyController(ILogger<CompanyController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public async Task<List<CompanyDto>> GetAllCompanies([FromServices] IGetCompaniesCommand getCompaniesCommand) 
        {
            _logger.LogInformation("Retrieving all companies");
            return await getCompaniesCommand.GetAllCompaniesAsync();
        }

        [HttpGet("{id:long}")]
        [ProducesResponseType(typeof(CompanyDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetCompanyById(
            long id, 
            [FromServices] IGetCompanyByIdCommand getCompanyByIdCommand)
        {
            _logger.LogInformation("Retrieving company with Id: {CompanyId}", id);

            var company = await getCompanyByIdCommand.GetCompanyByIdAsync(id);
            
            if (company == null)
            {
                _logger.LogWarning("Company with Id: {CompanyId} not found", id);
                return NotFound(new ProblemDetails
                {
                    Title = "Company not found",
                    Status = StatusCodes.Status404NotFound,
                    Detail = $"Company with ID {id} does not exist"
                });
            }

            _logger.LogInformation("Company with Id: {CompanyId} retrieved successfully", id);
            return Ok(company);
        }

        [ProducesResponseType(typeof(CompanyDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost]
        public async Task<IActionResult> CreateCompany([FromBody] CreateCompanyDto createCompanydto, [FromServices] ICreateCompanyCommand createCompanyCommand)
        {
            _logger.LogInformation("Creating a new company with Name: {CompanyName} and Vat: {CompanyVat}", createCompanydto.Name, createCompanydto.Vat);

            var result = await createCompanyCommand.CreateCompanyAsync(createCompanydto);
            if (result.IsSuccess)
            {
                _logger.LogInformation("Company created successfully");

                return StatusCode(StatusCodes.Status201Created, result.Value);
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
        public async Task<IActionResult> UpdateCompany(long id, [FromBody] CreateCompanyDto companydto, [FromServices] IUpdateCompanyCommand updateCompanyCommand)
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
