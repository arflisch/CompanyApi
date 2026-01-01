using Database;
using Domain;
using FluentResults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application
{
    public class PatchCompanyCommand : IPatchCompanyCommand
    {
        private readonly ICompanyRepository<Company> repository;

        public PatchCompanyCommand(ICompanyRepository<Company> repository)
        {
            this.repository = repository;
        }

        public async Task<Result> PatchCompanyNameAsync(long companyId, string Name)
        {
            Company company = new Company
            {
                Id = companyId,
                Name = Name
            };
            try
            {
                if (await repository.getCompanyByIdAsync(company.Id) == null)
                {
                    return Result.Fail(new ValidationError("Company Not Found"));
                }
                await repository.patchAsync(company);
                return Result.Ok();
            }
            catch (Exception ex)
            {
                return Result.Fail($"{ex.Message}");
            }
        }

        public async Task<Result> PatchCompanyVatAsync(long companyId, string Vat)
        {
            Company company = new Company
            {
                Id = companyId,
                Vat = Vat
            };
            try
            {
                if (await repository.getCompanyByIdAsync(company.Id) == null)
                {
                    return Result.Fail(new ValidationError("Company Not Found"));
                }
                await repository.patchAsync(company);
                return Result.Ok();
            }
            catch (Exception ex)
            {
                return Result.Fail($"{ex.Message}");
            }
        }
    }
}
