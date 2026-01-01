using CompanyApi.Facade.Sdk;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CompanyFrontend.Services
{
    public interface ICompanyService
    {
        Task<List<CompanyDto>> GetAllCompaniesAsync();
        Task<CompanyDto> CreateCompanyAsync(CreateCompanyDto companyDto);
        Task<FileResponse> UpdateCompanyAsync(long id, CreateCompanyDto companyDto);
        Task<FileResponse> DeleteCompanyAsync(long id);
        Task<FileResponse> PatchCompanyName(long id, string name);
        Task<FileResponse> PatchCompanyVat(long id, string vat);
    }
}
