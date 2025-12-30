using CompanyApi.Facade.Sdk;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace CompanyFrontend.Services
{
    public class CompanyService : ICompanyService
    {
        private readonly CompanyClient _companyClient;

        public CompanyService(HttpClient httpClient)
        {
            _companyClient = new CompanyClient(httpClient)
            {
                BaseUrl = httpClient.BaseAddress?.ToString() ?? "https://localhost:7223"
            };
        }

        public async Task<List<CompanyDto>> GetAllCompaniesAsync()
        {
            return await _companyClient.GetAllCompaniesAsync();
        }

        public async Task<CompanyDto> CreateCompanyAsync(CreateCompanyDto companyDto)
        {
            return await _companyClient.CreateCompanyAsync(companyDto);
        }

        public async Task<FileResponse> UpdateCompanyAsync(long id, CreateCompanyDto companyDto)
        {
            return await _companyClient.UpdateCompanyAsync(id, companyDto);
        }

        public async Task<FileResponse> DeleteCompanyAsync(long id)
        {
            return await _companyClient.DeleteCompanyAsync(id);
        }

        public async Task<FileResponse> PatchCompanyName(long id, string name)
        {
            return await _companyClient.PatchCompanyNameAsync(id, name);
        }

        public async Task<FileResponse> PatchCompanyVat(long id, string vat)
        {
            return await _companyClient.PatchCompanyVatAsync(id, vat);
        }
    }
}
