
using System.Collections.Generic;
using Application.Services;
using Database;
using Domain;
using Domain.DTO;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Application
{
    public class GetCompaniesCommand : IGetCompaniesCommand
    {
        private readonly ICompanyRepository<Company> _repository;
        private readonly IDaprCacheService _daprCacheService;
        private static readonly ActivitySource ActivitySource = new("CompanyApi.Application");

        public GetCompaniesCommand(
            ICompanyRepository<Company> repository,
            IDaprCacheService daprCacheService)
        {
            _repository = repository;
            _daprCacheService = daprCacheService;
        }

        public async Task<List<CompanyDto>> GetAllCompaniesAsync(int pageNumber, int pageSize)
        {
            using var activity = ActivitySource.StartActivity("GetAllCompanies");
            
            using (var cacheActivity = ActivitySource.StartActivity("GetFromDaprCache"))
            {
                var cachedCompanies = await _daprCacheService.GetAllCompaniesAsync();
                if (cachedCompanies != null && cachedCompanies.Count > 0)
                {
                    cacheActivity?.SetTag("cache.hit", true);
                    cacheActivity?.SetTag("companies.count", cachedCompanies.Count);
                    cacheActivity?.SetTag("cache.provider", "dapr");
                    activity?.SetTag("cache.hit", true);
                    
                    return cachedCompanies.Select(c => new CompanyDto
                    {
                        Id = c.Id,
                        Name = c.Name,
                        Vat = c.Vat
                    }).ToList();
                }
                
                cacheActivity?.SetTag("cache.hit", false);
                cacheActivity?.SetTag("cache.provider", "dapr");
                activity?.SetTag("cache.hit", false);
            }

            // Cache miss - get from database
            using (var dbActivity = ActivitySource.StartActivity("GetFromDatabase"))
            {
                System.Diagnostics.Debug.WriteLine("⚠️ Cache MISS: Loading companies from database");
                
                var allCompanies = await _repository.GetAllCompaniesAsync(pageNumber, pageSize);
                
                dbActivity?.SetTag("companies.count", allCompanies.Count);
                
                // Store in Dapr state store for next time
                await _daprCacheService.SetAllCompaniesAsync(allCompanies);
                
                System.Diagnostics.Debug.WriteLine($"✅ Stored {allCompanies.Count} companies in Dapr/Redis cache via Dapr State API");

                return allCompanies.Select(c => new CompanyDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    Vat = c.Vat
                }).ToList();
            }
        }
    }
}
