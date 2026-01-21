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
        Task PatchAsync(Company company);

        Task<List<Company>> GetAllCompaniesAsync();

        Task<Company?> GetCompanyByIdAsync(string id);
    }
}
