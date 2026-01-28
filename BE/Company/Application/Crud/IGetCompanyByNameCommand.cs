using Domain.DTO;

namespace Application;

public interface IGetCompanyByNameCommand
{
    Task<CompanyDto?> GetCompanyByNameAsync(string name);
}