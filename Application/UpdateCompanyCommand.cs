using Database;
using Domain;
using Domain.DTO;
using FluentResults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application
{
    public class UpdateCompanyCommand : IUpdateCompanyCommand
    {
        private readonly ICompanyRepository<Company> repository;

        public UpdateCompanyCommand(ICompanyRepository<Company> repository)
        {
            this.repository = repository;
        }

        public async Task<Result> UpdateCompanyAsync(CompanyDto companyDto)
        {
            if (companyDto == null)
            {
                return Result.Fail("Company data is required");
            }
            try
            {
                var company = await repository.getCompanyByIdAsync(companyDto.Id);

                if (company == null)
                {
                    return Result.Fail("Company Not Found");
                }

                await repository.updateAsync(company);
                return Result.Ok();
            }
            catch (Exception ex)
            {
                return Result.Fail($"Error creating company: {ex.Message}");
            }
        }
    }
}
