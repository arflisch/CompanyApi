using CompanyApi.Facade.Sdk;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace CompanyFrontend.Services
{
    public class CompanyService : ICompanyService
    {
        private readonly HttpClient _httpClient;
        private readonly CompanyClient _companyClient;
        private readonly IAuthService _authService;

        public CompanyService(HttpClient httpClient, IAuthService authService)
        {
            _companyClient = new CompanyClient(httpClient)
            {
                BaseUrl = httpClient.BaseAddress?.ToString() ?? "https://localhost:7223"
            };
            _httpClient = httpClient;
            _authService = authService;
        }

        private async Task PrepareAuthenticatedRequestAsync()
        {
            var token = await _authService.LoginAsync();

            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);
            }
        }

        public async Task<List<CompanyDto>> GetAllCompaniesAsync()
        {
            await PrepareAuthenticatedRequestAsync();
            return await _companyClient.GetAllCompaniesAsync();
        }

        public async Task<CompanyDto> CreateCompanyAsync(CreateCompanyDto companyDto)
        {
            await PrepareAuthenticatedRequestAsync();
            return await _companyClient.CreateCompanyAsync(companyDto);
        }

        public async Task<FileResponse> UpdateCompanyAsync(long id, CreateCompanyDto companyDto)
        {
            await PrepareAuthenticatedRequestAsync();
            return await _companyClient.UpdateCompanyAsync(id, companyDto);
        }

        public async Task<FileResponse> DeleteCompanyAsync(long id)
        {
            await PrepareAuthenticatedRequestAsync();
            return await _companyClient.DeleteCompanyAsync(id);
        }

        public async Task<FileResponse> PatchCompanyName(long id, string name)
        {
            await PrepareAuthenticatedRequestAsync();
            return await _companyClient.PatchCompanyNameAsync(id, name);
        }

        public async Task<FileResponse> PatchCompanyVat(long id, string vat)
        {
            await PrepareAuthenticatedRequestAsync();
            return await _companyClient.PatchCompanyVatAsync(id, vat);
        }
    }
}
