using Domain.DTO;
using System.Threading.Tasks;
using MongoDB.Bson;

namespace Application
{
    public interface IGetCompanyByIdCommand
    {
        Task<CompanyDto?> GetCompanyByIdAsync(string id);
    }
}
