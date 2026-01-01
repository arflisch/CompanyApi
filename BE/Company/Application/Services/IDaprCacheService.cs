using Domain;

namespace Application.Services
{
    public interface IDaprCacheService
    {
        Task<Company?> GetCompanyAsync(long id);
        Task SetCompanyAsync(Company company);
        Task RemoveCompanyAsync(long id);
        Task<List<Company>?> GetAllCompaniesAsync();
        Task SetAllCompaniesAsync(List<Company> companies);
        Task InvalidateAllCompaniesAsync();
    }
}
