using CompanyApi.Facade.Sdk;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CompanyFrontend.Services
{
    public interface ICompanyService
    {
        Task<List<Company>> GetAllCompaniesAsync();
        Task CreateCompanyAsync(CompanyDto companyDto);
        Task<FileResponse> UpdateCompanyAsync(long id, CompanyDto companyDto);
        Task<FileResponse> DeleteCompanyAsync(long id);
        Task<FileResponse> PatchCompanyName(long id, string name);
        Task<FileResponse> PatchCompanyVat(long id, string vat);
    }
}
