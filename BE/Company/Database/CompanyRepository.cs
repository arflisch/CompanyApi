using Domain;
using Microsoft.EntityFrameworkCore;

namespace Database
{
    public class CompanyRepository : ICompanyRepository<Company>
    {
        private readonly dbContext _dbContext;
        public CompanyRepository(dbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task CreateAsync(Company company)
        {
            await _dbContext.Companys.AddAsync(company);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(Company company)
        {
            _dbContext.Companys.Remove(company);
            await _dbContext.SaveChangesAsync();
        }

        public async Task PatchAsync(Company company)
        {
            var existing = await GetCompanyByIdAsync(company.Id);
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

        public async Task UpdateAsync(Company company)
        {
            _dbContext.Companys.Update(company);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<Company?> GetCompanyByIdAsync(Guid id)
        {
            return await _dbContext.Companys.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<List<Company>> GetAllCompaniesAsync()
        {
            return await _dbContext.Companys
                .AsNoTracking()
                .OrderBy(c => c.Id)  // Tri par ID pour un ordre cohérent
                .ToListAsync();
        }
    }
}
