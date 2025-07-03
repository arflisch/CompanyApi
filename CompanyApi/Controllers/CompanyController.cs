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
            return BadRequest(result.Errors);
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
