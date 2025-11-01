using Domain;
using Domain.DTO;
using Database;
using FluentResults;
using FluentValidation;
using System.Diagnostics;
using Application.Metrics;

namespace Application
{
    public class CreateCompanyCommand : ICreateCompanyCommand
    {
        private static readonly ActivitySource ActivitySource = new("CompanyApi.Application");
        
        private readonly ICompanyRepository<Company> repository;
        private readonly IValidator<CompanyDto> validator;
        private readonly CompanyMetrics companyMetrics;

        public CreateCompanyCommand(ICompanyRepository<Company> repository, IValidator<CompanyDto> validator, CompanyMetrics companyMetrics)
        {
            this.repository = repository;
            this.validator = validator;
            this.companyMetrics = companyMetrics;
        }

        public async Task<Result> CreateCompanyAsync(CompanyDto companyDto)
        {
            using var activity = ActivitySource.StartActivity("CreateCompany");
            var stopwatch = Stopwatch.StartNew();

            try 
            {
                using (var validationActivity = ActivitySource.StartActivity("ValidateCompany"))
                {
                    if (companyDto == null)
                    {
                        validationActivity?.SetStatus(ActivityStatusCode.Error, "Company data is required");
                        companyMetrics.RecordValidationError("create");
                        return Result.Fail("Company data is required.");
                    }

                    validationActivity?.SetTag("company.name", companyDto.Name);
                    validationActivity?.SetTag("company.vat", companyDto.Vat);

                    var validationResult = await validator.ValidateAsync(companyDto);
                    if (!validationResult.IsValid)
                    {
                        validationActivity?.SetStatus(ActivityStatusCode.Error, "Validation failed");
                        validationActivity?.SetTag("validation.error_count", validationResult.Errors.Count);
                        companyMetrics.RecordValidationError("create");

                        var results = Result.Fail("Validation errors occurred.");
                        foreach (var error in validationResult.Errors)
                        {
                            results.WithError(new ValidationError(error.ErrorMessage));
                            validationActivity?.AddEvent(new ActivityEvent("ValidationError",
                                tags: new ActivityTagsCollection { { "error.message", error.ErrorMessage } }));
                        }
                        return results;
                    }

                    validationActivity?.SetStatus(ActivityStatusCode.Ok);
                }

                // Span pour la persistence
                using (var persistenceActivity = ActivitySource.StartActivity("PersistCompany"))
                {
                    try
                    {
                        Company company = new()
                        {
                            Name = companyDto.Name,
                            Vat = companyDto.Vat
                        };

                        await repository.createAsync(company);

                        persistenceActivity?.SetStatus(ActivityStatusCode.Ok);
                        persistenceActivity?.SetTag("company.id", company.Id);
                        activity?.SetTag("company.id", company.Id);
                        companyMetrics.RecordCompanyCreated();

                        return Result.Ok();
                    }
                    catch (Exception ex)
                    {
                        persistenceActivity?.SetStatus(ActivityStatusCode.Error, ex.Message);
                        activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
                        companyMetrics.RecordOperationError("create");
                        return Result.Fail($"Error creating company: {ex.Message}");
                    }
                }
            }
            finally
            {
                stopwatch.Stop();
                companyMetrics.RecordOperationDuration(stopwatch.ElapsedMilliseconds, "create");
            }
        }
    }
}
