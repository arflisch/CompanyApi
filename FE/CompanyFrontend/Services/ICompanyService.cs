using System;
using CompanyApi.Facade.Sdk;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CompanyFrontend.Services
{
    public interface ICompanyService
    {
        Task<List<CompanyDto>> GetAllCompaniesAsync();
        Task<CompanyDto> CreateCompanyAsync(CreateCompanyDto companyDto);
        Task<FileResponse> UpdateCompanyAsync(Guid id, CreateCompanyDto companyDto);
        Task<FileResponse> DeleteCompanyAsync(Guid id);
        Task<FileResponse> PatchCompanyName(Guid id, string name);
        Task<FileResponse> PatchCompanyVat(Guid id, string vat);
    }
}
