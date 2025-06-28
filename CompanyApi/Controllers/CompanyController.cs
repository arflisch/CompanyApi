using Microsoft.AspNetCore.Mvc;
using Domain;
using Domain.DTO;
using Database;

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
        public async Task<IActionResult> CreateCompany([FromBody] CompanyDto companydto, [FromServices] ICompanyRepository<Company> repository)
        {
            if (companydto == null)
            {
                return BadRequest("Company data is required.");
            }
            try
            {
                Company company = new()
                {
                    Name = companydto.Name,
                    Vat = companydto.Vat
                };
                await repository.createAsync(company);
                return CreatedAtAction(nameof(CreateCompany), new { id = company.Id }, company);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating company");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete("{id:long}")]
        public async Task<IActionResult> DeleteCompany(long id, [FromServices] ICompanyRepository<Company> repository)
        {
            if (id <= 0)
            {
                return BadRequest("Valid Id is required");
            }

            try
            {
                var company = await repository.getCompanyByIdAsync(id);
                if (company == null)
                {
                    return NotFound($"Company with Id {id} not found.");
                }

                await repository.deleteAsync(company);
                return Ok($"Company with Id {id} deleted successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting company");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdateCompany([FromBody] CompanyDto companydto, [FromServices] ICompanyRepository<Company> repository)
        {
            if (companydto == null)
            {
                return BadRequest("Company data is required");
            }
            try
            {
                var company = await repository.getCompanyByIdAsync(companydto.Id);

                if (company == null)
                {
                    return NotFound("Company Not Found");
                }

                await repository.updateAsync(company);
                return Ok("Company updated");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting company");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPatch("name/{id:long}/{Name}")]
        public async Task<IActionResult> PatchCompanyName(long id, string Name, [FromServices] ICompanyRepository<Company> repository)
        {
            Company company = new Company
            {
                Id = id,
                Name = Name
            };
            try
            {
                if (await repository.getCompanyByIdAsync(company.Id) == null)
                {
                    return NotFound("Company Not Found");
                }
                await repository.patchAsync(company);
                return Ok("Company patched");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error patching company");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPatch("Vat/{id:long}/{Vat}")]
        public async Task<IActionResult> PatchCompanyVat(long id, string Vat, [FromServices] ICompanyRepository<Company> repository)
        {
            Company company = new Company
            {
                Id = id,
                Vat = Vat
            };
            try
            {
                if (await repository.getCompanyByIdAsync(company.Id) == null)
                {
                    return NotFound("Company Not Found");
                }
                await repository.patchAsync(company);
                return Ok("Company patched");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error patching company");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
