using Arc4u.MongoDB;
using Domain;
using MongoDB.Driver;


namespace Database
{
    public class CompanyRepository : ICompanyRepository<Company>
    {
        private readonly IMongoCollection<Company> _companies;
        public CompanyRepository(IMongoClientFactory<CompanyContext> clientFactory)
        {
            _companies = clientFactory.GetCollection<Company>();
            var indexKeysDefinition = Builders<Company>.IndexKeys.Ascending(c => c.Name);
            var indexModel = new CreateIndexModel<Company>(indexKeysDefinition);
            _companies.Indexes.CreateOne(indexModel);
        }
        
        
        public async Task CreateAsync(Company company)
        {
            await _companies.InsertOneAsync(company, new InsertOneOptions { BypassDocumentValidation = false });
        }

        public async Task DeleteAsync(Company company)
        {
            await _companies.DeleteOneAsync(Builders<Company>.Filter.Where(c => c.Id == company.Id));
        }

        public async Task PatchName(Guid companyId, string name)
        {
            var updated = Builders<Company>.Update
                .Set(c => c.Name, name);
            await _companies.UpdateOneAsync(Builders<Company>.Filter.Where(c => c.Id == companyId), updated);
        }
        
        public async Task PatchVat(Guid companyId, string vat)
        {
            var updated = Builders<Company>.Update
                .Set(c => c.Vat, vat);
            await _companies.UpdateOneAsync(Builders<Company>.Filter.Where(c => c.Id == companyId), updated);
        }

        public async Task UpdateAsync(Company company)
        {
            var updated = Builders<Company>.Update
                .Set(c => c.Name, company.Name)
                .Set(c => c.Vat, company.Vat);
            await _companies.UpdateOneAsync(Builders<Company>.Filter.Where(c => c.Id == company.Id), updated);
        }

        public async Task<Company?> GetCompanyByIdAsync(Guid id)
        {
            using var anchor = await _companies.FindAsync(Builders<Company>.Filter.Where(c => c.Id == id));
            return await anchor.FirstOrDefaultAsync();
        }

        public async Task<List<Company>> GetAllCompaniesAsync(int pageNumber, int pageSize)
        {
            var filter = Builders<Company>.Filter.Empty;

            return await _companies
                .Find(filter)
                .SortBy(c => c.Id) // Important pour la pagination
                .Skip((pageNumber - 1) * pageSize)
                .Limit(pageSize)
                .ToListAsync();
        }

        public async Task<Company?> GetCompanyByNameAsync(string name)
        {
            using var anchor = await _companies.FindAsync(Builders<Company>.Filter.Where(c => c.Name == name));
            return await anchor.FirstOrDefaultAsync();
        }
    }
}
