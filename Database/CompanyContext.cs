using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain;

namespace Database
{
    public class CompanyContext : ICompanyRepository<Company>
    {
        private readonly dbContext _dbContext;
        public CompanyContext(dbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task createAsync(Company company)
        {
            await _dbContext.Companys.AddAsync(company);
            await _dbContext.SaveChangesAsync();
        }

        public async Task deleteAsync(Company company)
        {
            _dbContext.Companys.Remove(company);
            await _dbContext.SaveChangesAsync();
        }

        public async Task patchAsync(Company company)
        {
            var existing = await _dbContext.Companys.FindAsync(company.Id);
            if (existing != null)
            {
                if (!string.IsNullOrEmpty(company.Name))
                   existing.Name = company.Name;
                if (!string.IsNullOrEmpty(company.Vat))
                   existing.Vat = company.Vat;
                _dbContext.Companys.Update(existing);
                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task updateAsync(Company company)
        {
            _dbContext.Companys.Update(company);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<Company?> getCompanyByIdAsync(long id)
        {
            return await _dbContext.Companys.FindAsync(id);
        }
    }
}
