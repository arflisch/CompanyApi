using Database;
using Domain;
using FluentResults;
using System.Diagnostics;

namespace Application
{
    public class DeleteCompanyCommand : IDeleteCompanyCommand
    {
        private static readonly ActivitySource ActivitySource = new("CompanyApi.Application");
        private readonly ICompanyRepository<Company> repository;

        public DeleteCompanyCommand(ICompanyRepository<Company> repository)
        {
            this.repository = repository;
        }

        public async Task<Result> DeleteCompanyAsync(long id)
        {
            using var activity = ActivitySource.StartActivity("DeleteCompanyCommand-DeleteCompanyAsync");
            if (id <= 0)
            {
                return Result.Fail(new ValidationError("Valid Id is required"));
            }

            Company? company;
            using (var retrievalActivity = ActivitySource.StartActivity("RetrieveCompany"))
            {
                try
                {
                    retrievalActivity?.SetTag("company.id", id);
                    company = await repository.getCompanyByIdAsync(id);

                    if (company == null)
                    {
                        retrievalActivity?.SetStatus(ActivityStatusCode.Error, "Company not found");
                        retrievalActivity?.AddEvent(new ActivityEvent("CompanyNotFound",
                            tags: new ActivityTagsCollection { { "company.id", id } }));
                        activity?.SetStatus(ActivityStatusCode.Error, "Company not found");
                        return Result.Fail($"Company with Id {id} not found.");
                    }

                    retrievalActivity?.SetTag("company.name", company.Name);
                    retrievalActivity?.SetTag("company.vat", company.Vat);
                    retrievalActivity?.SetStatus(ActivityStatusCode.Ok);
                }
                catch (Exception ex)
                {
                    retrievalActivity?.SetStatus(ActivityStatusCode.Error, ex.Message);
                    activity?.SetStatus(ActivityStatusCode.Error, "Failed to retrieve company");
                    return Result.Fail($"Error retrieving company: {ex.Message}");
                }
            }

            // Deletion span
            using (var deletionActivity = ActivitySource.StartActivity("DeleteCompanyFromDatabase"))
            {
                try
                {
                    deletionActivity?.SetTag("company.id", id);
                    deletionActivity?.SetTag("company.name", company.Name);

                    await repository.deleteAsync(company);

                    deletionActivity?.SetStatus(ActivityStatusCode.Ok);
                    deletionActivity?.AddEvent(new ActivityEvent("CompanyDeleted",
                        tags: new ActivityTagsCollection
                        {
                            { "company.id", id },
                            { "company.name", company.Name }
                        }));

                    activity?.SetStatus(ActivityStatusCode.Ok);
                    return Result.Ok();
                }
                catch (Exception ex)
                {
                    deletionActivity?.SetStatus(ActivityStatusCode.Error, ex.Message);
                    activity?.SetStatus(ActivityStatusCode.Error, "Failed to delete company");
                    return Result.Fail($"Error deleting company: {ex.Message}");
                }
            }
        }
    }
}
