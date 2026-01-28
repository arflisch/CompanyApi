using Domain.DTO;
using FluentResults;


namespace Application
{
    public interface IUpdateCompanyCommand
    {
        Task<Result> UpdateCompanyAsync(Guid id, CreateCompanyDto companyDto);
    }
}
