using Domain.DTO;

namespace Application
{
    public interface IGetCompanyByIdCommand
    {
        Task<CompanyDto?> GetCompanyByIdAsync(Guid id);
    }
}
