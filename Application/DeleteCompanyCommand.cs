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
    public class DeleteCompanyCommand : IDeleteCompanyCommand
    {
        private readonly ICompanyRepository<Company> repository;

        public DeleteCompanyCommand(ICompanyRepository<Company> repository)
        {
            this.repository = repository;
        }

        public async Task<Result> DeleteCompanyAsync(long id)
        {
            if (id <= 0)
            {
                return Result.Fail("Valid Id is required");
            }

            try
            {
                var company = await repository.getCompanyByIdAsync(id);
                if (company == null)
                {
                    return Result.Fail($"Company with Id {id} not found.");
                }

                await repository.deleteAsync(company);
                return Result.Ok();
            }
            catch (Exception ex)
            {
                return Result.Fail($"error {ex.Message}");
            }
        }
    }
}
