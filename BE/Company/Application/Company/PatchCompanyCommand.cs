using Database;
using Domain;
using FluentResults;
using MongoDB.Bson;

namespace Application
{
    public class PatchCompanyCommand : IPatchCompanyCommand
    {
        private readonly ICompanyRepository<Company> repository;

        public PatchCompanyCommand(ICompanyRepository<Company> repository)
        {
            this.repository = repository;
        }

        public async Task<Result> PatchCompanyNameAsync(string companyId, string Name)
        {
            Company company = new Company
            {
                Id = ObjectId.Parse(companyId),
                Name = Name
            };
            try
            {
                if (await repository.GetCompanyByIdAsync(company.Id.ToString()) == null)
                {
                    return Result.Fail(new ValidationError("Company Not Found"));
                }
                await repository.PatchAsync(company);
                return Result.Ok();
            }
            catch (Exception ex)
            {
                return Result.Fail($"{ex.Message}");
            }
        }

        public async Task<Result> PatchCompanyVatAsync(string companyId, string Vat)
        {
            Company company = new Company
            {
                Id = ObjectId.Parse(companyId),
                Vat = Vat
            };
            try
            {
                if (await repository.GetCompanyByIdAsync(company.Id.ToString()) == null)
                {
                    return Result.Fail(new ValidationError("Company Not Found"));
                }
                await repository.PatchAsync(company);
                return Result.Ok();
            }
            catch (Exception ex)
            {
                return Result.Fail($"{ex.Message}");
            }
        }
    }
}
