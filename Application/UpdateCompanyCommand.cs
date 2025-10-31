using Database;
using Domain;
using Domain.DTO;
using FluentResults;
using FluentValidation;
using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application
{
    public class UpdateCompanyCommand : IUpdateCompanyCommand
    {
        private static readonly ActivitySource ActivitySource = new("CompanyApi.Application");
        private readonly ICompanyRepository<Company> repository;
        private readonly IValidator<CompanyDto> validator;

        public UpdateCompanyCommand(ICompanyRepository<Company> repository, IValidator<CompanyDto> validator)
        {
            this.repository = repository;
            this.validator = validator;
        }

        public async Task<Result> UpdateCompanyAsync(CompanyDto companyDto)
        {
            using var activity = ActivitySource.StartActivity("UpdateCompanyCommand.UpdateCompanyAsync");

            using (var validationActivity = ActivitySource.StartActivity("Validate CompanyDto"))
            {
                if (companyDto == null)
                {
                    validationActivity?.SetStatus(ActivityStatusCode.Error, "Company data is null");
                    return Result.Fail("Company data is required");
                }

                validationActivity?.SetTag("CompanyDto.Id", companyDto.Id);
                validationActivity?.SetTag("CompanyDto.vat", companyDto.Vat);

                var validationResult = await validator.ValidateAsync(companyDto);
                if (!validationResult.IsValid)
                {

                    validationActivity?.SetStatus(ActivityStatusCode.Error, "Validation failed for CompanyDto");
                    validationActivity?.SetTag("ValidationErrorsCount", validationResult.Errors.Count);

                    var results = Result.Fail("Validation errors occurred.");
                    foreach (var error in validationResult.Errors)
                    {
                        results.WithError(new ValidationError(error.ErrorMessage));
                        validationActivity?.AddEvent(new ActivityEvent("ValidationError", tags: new ActivityTagsCollection
                        {
                            { "PropertyName", error.PropertyName },
                            { "ErrorMessage", error.ErrorMessage }
                        }));
                    }
                    return results;
                }
                validationActivity?.SetStatus(ActivityStatusCode.Ok, "Validation succeeded for CompanyDto");
            }
            

            using (var dbActivity = ActivitySource.StartActivity("Update Company in Database"))
            {
                try
                {
                    var company = await repository.getCompanyByIdAsync(companyDto.Id);

                    if (company == null)
                    {
                        return Result.Fail("Company Not Found");
                    }

                    await repository.updateAsync(company);

                    dbActivity?.SetStatus(ActivityStatusCode.Ok, "Company updated successfully in database");
                    dbActivity?.SetTag("CompanyId", company.Id);
                    activity?.SetTag("CompanyId", company.Id);
                    return Result.Ok();
                }
                catch (Exception ex)
                {
                    dbActivity?.SetStatus(ActivityStatusCode.Error, "Error updating company in database");
                    activity?.SetStatus(ActivityStatusCode.Error, "Error updating company in database");
                    return Result.Fail($"Error creating company: {ex.Message}");
                }
            } 
        }
    }
}
