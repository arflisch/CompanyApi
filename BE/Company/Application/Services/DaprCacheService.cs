using Dapr.Client;
using Domain;
using System.Diagnostics;

namespace Application.Services
{

    public class DaprCacheService : IDaprCacheService
    {
        private readonly DaprClient _daprClient;
        private const string StateStoreName = "statestore";
        private const string CompanyKeyPrefix = "company";
        private const string AllCompaniesKey = "companies:all";
        private static readonly ActivitySource ActivitySource = new("CompanyApi.Cache");

        public DaprCacheService(DaprClient daprClient)
        {
            _daprClient = daprClient;
        }

        public async Task<Company?> GetCompanyAsync(long id)
        {
            using var activity = ActivitySource.StartActivity("GetCompanyFromCache");
            activity?.SetTag("company.id", id);

            try
            {
                var key = $"{CompanyKeyPrefix}:{id}";
                var company = await _daprClient.GetStateAsync<Company>(StateStoreName, key);
                
                var cacheHit = company != null;
                activity?.SetTag("cache.hit", cacheHit);
                
                System.Diagnostics.Debug.WriteLine(cacheHit 
                    ? $"✅ Cache HIT: Company {id} found in Dapr state store" 
                    : $"⚠️ Cache MISS: Company {id} not found in Dapr state store");
                
                return company;
            }
            catch (Exception ex)
            {
                activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
                System.Diagnostics.Debug.WriteLine($"❌ Error getting company {id} from cache: {ex.Message}");
                return null;
            }
        }

        public async Task SetCompanyAsync(Company company)
        {
            using var activity = ActivitySource.StartActivity("SetCompanyInCache");
            activity?.SetTag("company.id", company.Id);

            try
            {
                var key = $"{CompanyKeyPrefix}:{company.Id}";
                
                // Dapr State API avec TTL (Time To Live) de 10 minutes
                var metadata = new Dictionary<string, string>
                {
                    { "ttlInSeconds", "60" } // 10 minutes
                };

                await _daprClient.SaveStateAsync(StateStoreName, key, company, metadata: metadata);
                
                System.Diagnostics.Debug.WriteLine($"✅ Stored company {company.Id} in Dapr state store with 10min TTL");
            }
            catch (Exception ex)
            {
                activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
                System.Diagnostics.Debug.WriteLine($"❌ Error storing company {company.Id} in cache: {ex.Message}");
            }
        }

        public async Task RemoveCompanyAsync(long id)
        {
            using var activity = ActivitySource.StartActivity("RemoveCompanyFromCache");
            activity?.SetTag("company.id", id);

            try
            {
                var key = $"{CompanyKeyPrefix}:{id}";
                await _daprClient.DeleteStateAsync(StateStoreName, key);
                
                // Invalider aussi la liste complète
                await InvalidateAllCompaniesAsync();
                
                System.Diagnostics.Debug.WriteLine($"✅ Removed company {id} from Dapr state store");
            }
            catch (Exception ex)
            {
                activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
                System.Diagnostics.Debug.WriteLine($"❌ Error removing company {id} from cache: {ex.Message}");
            }
        }

        public async Task<List<Company>?> GetAllCompaniesAsync()
        {
            using var activity = ActivitySource.StartActivity("GetAllCompaniesFromCache");

            try
            {
                var companies = await _daprClient.GetStateAsync<List<Company>>(StateStoreName, AllCompaniesKey);
                
                var cacheHit = companies != null && companies.Count > 0;
                activity?.SetTag("cache.hit", cacheHit);
                
                if (cacheHit)
                {
                    activity?.SetTag("companies.count", companies!.Count);
                    System.Diagnostics.Debug.WriteLine($"✅ Cache HIT: Retrieved {companies.Count} companies from Dapr state store");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("⚠️ Cache MISS: All companies not found in Dapr state store");
                }
                
                return companies;
            }
            catch (Exception ex)
            {
                activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
                System.Diagnostics.Debug.WriteLine($"❌ Error getting all companies from cache: {ex.Message}");
                return null;
            }
        }

        public async Task SetAllCompaniesAsync(List<Company> companies)
        {
            using var activity = ActivitySource.StartActivity("SetAllCompaniesInCache");
            activity?.SetTag("companies.count", companies.Count);

            try
            {
                // TTL de 5 minutes pour la liste complète (change plus souvent)
                var metadata = new Dictionary<string, string>
                {
                    { "ttlInSeconds", "300" } // 5 minutes
                };

                await _daprClient.SaveStateAsync(StateStoreName, AllCompaniesKey, companies, metadata: metadata);
                
                System.Diagnostics.Debug.WriteLine($"✅ Stored {companies.Count} companies in Dapr state store with 5min TTL");
            }
            catch (Exception ex)
            {
                activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
                System.Diagnostics.Debug.WriteLine($"❌ Error storing all companies in cache: {ex.Message}");
            }
        }

        public async Task InvalidateAllCompaniesAsync()
        {
            using var activity = ActivitySource.StartActivity("InvalidateAllCompaniesCache");

            try
            {
                await _daprClient.DeleteStateAsync(StateStoreName, AllCompaniesKey);
                System.Diagnostics.Debug.WriteLine("✅ Invalidated all companies cache in Dapr state store");
            }
            catch (Exception ex)
            {
                activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
                System.Diagnostics.Debug.WriteLine($"❌ Error invalidating all companies cache: {ex.Message}");
            }
        }
    }
}
