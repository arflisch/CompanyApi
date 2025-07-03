using Domain;
using Domain.DTO;
using Database;
using FluentResults;

namespace Application
{
    public class CreateCompanyCommand : ICreateCompanyCommand
    {
        private readonly ICompanyRepository<Company> repository;

        public CreateCompanyCommand(ICompanyRepository<Company> repository)
        {
            this.repository = repository;
        }

        public async Task<Result> CreateCompanyAsync(CompanyDto companyDto)
        {
            if (companyDto == null)
            {
                return Result.Fail("Company data is required.");
            }
            try
            {
                Company company = new()
                {
                    Name = companyDto.Name,
                    Vat = companyDto.Vat
                };
                await repository.createAsync(company);
                return Result.Ok();
            }
            catch (Exception ex)
            {
                return Result.Fail($"Error creating company: {ex.Message}");
            }
        }
    }
}
