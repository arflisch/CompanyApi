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

        public async Task<Result> PatchCompanyNameAsync(Guid companyId, string Name)
        {
            try
            {
                await repository.PatchName(companyId, Name);
                return Result.Ok();
            }
            catch (Exception ex)
            {
                return Result.Fail($"{ex.Message}");
            }
        }

        public async Task<Result> PatchCompanyVatAsync(Guid companyId, string Vat)
        {
            try
            {
                await repository.PatchVat(companyId, Vat);
                return Result.Ok();
            }
            catch (Exception ex)
            {
                return Result.Fail($"{ex.Message}");
            }
        }
    }
}
