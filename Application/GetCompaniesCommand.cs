using Database;
using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application
{
    public class GetCompaniesCommand : IGetCompaniesCommand
    {
        private ICompanyRepository<Company> repository;
        public GetCompaniesCommand(ICompanyRepository<Company> repository) 
        { 
            this.repository = repository;
        }

        public async Task<List<Company>> GetAllCompaniesAsync()
        {
            return await repository.getAllCompaniesAsync();
        }
    }
}
