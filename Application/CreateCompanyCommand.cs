using Domain;
using Domain.DTO;
using Database;
using FluentResults;
using FluentValidation;

namespace Application
{
    public class CreateCompanyCommand : ICreateCompanyCommand
    {
        private readonly ICompanyRepository<Company> repository;
        private readonly IValidator<CompanyDto> validator;

        public CreateCompanyCommand(ICompanyRepository<Company> repository, IValidator<CompanyDto> validator)
        {
            this.repository = repository;
            this.validator = validator;
        }

        public async Task<Result> CreateCompanyAsync(CompanyDto companyDto)
        {
            if (companyDto == null)
            {
                return Result.Fail("Company data is required.");
            }

            var validationResult = await validator.ValidateAsync(companyDto);
            if (!validationResult.IsValid)
            {
                var results = Result.Fail("Validation errors occurred.");
                foreach (var error in validationResult.Errors)
                {
                    results.WithError(new ValidationError(error.ErrorMessage));
                }
                return results;
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
