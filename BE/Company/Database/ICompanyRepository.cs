using Domain;
using Domain.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;

namespace Database
{
    public interface ICompanyRepository<Company>
    {
        Task CreateAsync(Company company);
        Task UpdateAsync(Company company);
        Task DeleteAsync(Company company);
        Task PatchName(Guid companyId, string name);
        Task PatchVat(Guid companyId, string vat);

        Task<List<Company>> GetAllCompaniesAsync(int pageNumber, int pageSize);

        Task<Company?> GetCompanyByIdAsync(Guid id);
        
        Task<Company?> GetCompanyByNameAsync(string name);
    }
}
