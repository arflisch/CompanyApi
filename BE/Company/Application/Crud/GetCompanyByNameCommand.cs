using System.Diagnostics;
using Database;
using Domain;
using Domain.DTO;

namespace Application;

public class GetCompanyByNameCommand : IGetCompanyByNameCommand
{
    private readonly ICompanyRepository<Company> _repository;
    private static readonly ActivitySource ActivitySource = new("CompanyApi.Application");

    public GetCompanyByNameCommand(ICompanyRepository<Company> repository)
    {
        _repository = repository;   
    }

    public async Task<CompanyDto?> GetCompanyByNameAsync(string name)
    {
        using var activity = ActivitySource.StartActivity("GetCompanyByNameCommand.GetCompanyByNameAsync");

        using (var dbActivity = ActivitySource.StartActivity("GetCompanyByNameCommand.GetCompanyByNameAsync.Database"))
        {
            var companyByName = await _repository.GetCompanyByNameAsync(name);

            if (companyByName == null)
            {
                dbActivity?.SetTag("company.found", false);
                return null;
            }
        
            dbActivity?.SetTag("company.found", true);
            return new CompanyDto
            {
                Id = companyByName.Id,
                Name = companyByName.Name,
                Vat = companyByName.Vat
            };
        }
        
    }
}