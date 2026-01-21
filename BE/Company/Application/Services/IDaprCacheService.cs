using Domain;

namespace Application.Services
{
    public interface IDaprCacheService
    {
        Task<Company?> GetCompanyAsync(string id);
        Task SetCompanyAsync(Company company);
        Task RemoveCompanyAsync(string id);
        Task<List<Company>?> GetAllCompaniesAsync();
        Task SetAllCompaniesAsync(List<Company> companies);
        Task InvalidateAllCompaniesAsync();
    }
}
