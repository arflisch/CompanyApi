using FluentResults;

namespace Application
{
    public interface IPatchCompanyCommand
    {
        Task<Result> PatchCompanyNameAsync(Guid companyId, string name);

        Task<Result> PatchCompanyVatAsync(Guid companyId, string vat);
    }
}
