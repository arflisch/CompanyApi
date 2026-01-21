using Domain;

namespace Application.Services
{
    public interface IDaprCacheService
    {
        Task<Company?> GetCompanyAsync(Guid id);
        Task SetCompanyAsync(Company company);
        Task RemoveCompanyAsync(Guid id);
        Task<List<Company>?> GetAllCompaniesAsync();
        Task SetAllCompaniesAsync(List<Company> companies);
        Task InvalidateAllCompaniesAsync();
    }
}
