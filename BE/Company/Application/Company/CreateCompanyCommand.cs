using Domain;
using Domain.DTO;
using Database;
using FluentResults;
using FluentValidation;
using System.Diagnostics;
using Application.Metrics;
using Dapr;
using Dapr.Client;
using Microsoft.Extensions.Logging;

namespace Application
{
    public class CreateCompanyCommand : ICreateCompanyCommand
    {
        private static readonly ActivitySource ActivitySource = new("CompanyApi.Application");
        
        private readonly ICompanyRepository<Company> repository;
        private readonly IValidator<CreateCompanyDto> validator;
        private readonly CompanyMetrics companyMetrics;
        private readonly DaprClient daprClient;
        private readonly ILogger<CreateCompanyCommand> logger;

        public CreateCompanyCommand(
            ICompanyRepository<Company> repository, 
            IValidator<CreateCompanyDto> validator, 
            CompanyMetrics companyMetrics, 
            DaprClient daprClient,
            ILogger<CreateCompanyCommand> logger)
        {
            this.repository = repository;
            this.validator = validator;
            this.companyMetrics = companyMetrics;
            this.daprClient = daprClient;
            this.logger = logger;
        }

        public async Task<Result<CompanyDto>> CreateCompanyAsync(CreateCompanyDto companyDto)
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

                        // Publish event with better error handling and logging
                        try
                        {
                            logger.LogInformation("Attempting to publish CompanyCreated event to Dapr. PubSub: rabbitmq-pubsub, Topic: companycreated, Company: {Name}", companyDto.Name);
                            
                            await daprClient.PublishEventAsync("rabbitmq-pubsub", "companycreated", companyDto);
                            
                            logger.LogInformation("Successfully published CompanyCreated event for company: {Name}", companyDto.Name);
                        }
                        catch (Exception publishEx)
                        {
                            logger.LogError(publishEx, "Failed to publish CompanyCreated event to Dapr for company: {Name}. PubSub: rabbitmq-pubsub, Topic: companycreated", companyDto.Name);
                        }

                        // Retourner le CompanyDto avec l'ID généré
                        var createdCompanyDto = new CompanyDto
                        {
                            Id = company.Id,
                            Name = company.Name,
                            Vat = company.Vat
                        };

                        return Result.Ok(createdCompanyDto);
                    }
                    catch (Exception ex)
                    {
                        persistenceActivity?.SetStatus(ActivityStatusCode.Error, ex.Message);
                        activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
                        companyMetrics.RecordOperationError("create");
                        logger.LogError(ex, "Error creating company: {Message}", ex.Message);
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
