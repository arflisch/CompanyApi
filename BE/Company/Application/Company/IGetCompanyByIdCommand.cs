using Domain.DTO;
using System.Threading.Tasks;

namespace Application
{
    public interface IGetCompanyByIdCommand
    {
        Task<CompanyDto?> GetCompanyByIdAsync(long id);
    }
}
