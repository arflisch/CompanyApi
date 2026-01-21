using CompanyApi.Facade.Sdk;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CompanyFrontend.Services
{
    public interface ICompanyService
    {
        Task<List<CompanyDto>> GetAllCompaniesAsync();
        Task<CompanyDto> CreateCompanyAsync(CreateCompanyDto companyDto);
        Task<FileResponse> UpdateCompanyAsync(string id, CreateCompanyDto companyDto);
        Task<FileResponse> DeleteCompanyAsync(string id);
        Task<FileResponse> PatchCompanyName(string id, string name);
        Task<FileResponse> PatchCompanyVat(string id, string vat);
    }
}
