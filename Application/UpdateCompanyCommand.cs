using Database;
using Domain;
using Domain.DTO;
using FluentResults;
using FluentValidation;
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
        private readonly IValidator<CompanyDto> validator;

        public UpdateCompanyCommand(ICompanyRepository<Company> repository, IValidator<CompanyDto> validator)
        {
            this.repository = repository;
            this.validator = validator;
        }

        public async Task<Result> UpdateCompanyAsync(CompanyDto companyDto)
        {
            if (companyDto == null)
            {
                return Result.Fail("Company data is required");
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
