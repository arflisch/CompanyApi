using Database;
using Domain;
using Domain.DTO;
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

        public async Task<List<CompanyDto>> GetAllCompaniesAsync()
        {
            var allCompanies = await repository.getAllCompaniesAsync();
            List<CompanyDto> companyDtos = new List<CompanyDto>();
            foreach (var item in allCompanies)
            {
                companyDtos.Add(new CompanyDto 
                { 
                    Id = item.Id, 
                    Name = item.Name, 
                    Vat = item.Vat 
                });
            }
            return companyDtos;
        }
    }
}
