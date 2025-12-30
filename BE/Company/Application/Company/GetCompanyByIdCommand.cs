using Application.Services;
using Database;
using Domain;
using Domain.DTO;
using System.Diagnostics;

namespace Application
{
    public class GetCompanyByIdCommand : IGetCompanyByIdCommand
    {
        private readonly ICompanyRepository<Company> _repository;
        private readonly IDaprCacheService _daprCacheService;
        private static readonly ActivitySource ActivitySource = new("CompanyApi.Application");

        public GetCompanyByIdCommand(
            ICompanyRepository<Company> repository,
            IDaprCacheService daprCacheService)
        {
            _repository = repository;
            _daprCacheService = daprCacheService;
        }

        public async Task<CompanyDto?> GetCompanyByIdAsync(long id)
        {
            using var activity = ActivitySource.StartActivity("GetCompanyById");
            activity?.SetTag("company.id", id);

            // Try to get from Dapr cache first
            using (var cacheActivity = ActivitySource.StartActivity("GetFromDaprCache"))
            {
                var cachedCompany = await _daprCacheService.GetCompanyAsync(id);
                if (cachedCompany != null)
                {
                    cacheActivity?.SetTag("cache.hit", true);
                    cacheActivity?.SetTag("cache.provider", "dapr");
                    activity?.SetTag("cache.hit", true);
                    
                    System.Diagnostics.Debug.WriteLine($"? Cache HIT: Retrieved company {id} from Dapr/Redis");
                    
                    return new CompanyDto
                    {
                        Id = cachedCompany.Id,
                        Name = cachedCompany.Name,
                        Vat = cachedCompany.Vat
                    };
                }
                
                cacheActivity?.SetTag("cache.hit", false);
                cacheActivity?.SetTag("cache.provider", "dapr");
                activity?.SetTag("cache.hit", false);
            }

            // Cache miss - get from database
            using (var dbActivity = ActivitySource.StartActivity("GetFromDatabase"))
            {
                System.Diagnostics.Debug.WriteLine($"?? Cache MISS: Loading company {id} from database");
                
                var company = await _repository.getCompanyByIdAsync(id);
                
                if (company == null)
                {
                    dbActivity?.SetTag("company.found", false);
                    System.Diagnostics.Debug.WriteLine($"? Company {id} not found in database");
                    return null;
                }
                
                dbActivity?.SetTag("company.found", true);
                
                // Store in Dapr cache for next time
                await _daprCacheService.SetCompanyAsync(company);
                
                System.Diagnostics.Debug.WriteLine($"? Stored company {id} in Dapr/Redis cache");

                return new CompanyDto
                {
                    Id = company.Id,
                    Name = company.Name,
                    Vat = company.Vat
                };
            }
        }
    }
}
