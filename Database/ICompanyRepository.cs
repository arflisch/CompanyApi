using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Domain.Company;

namespace Database
{
    public interface ICompanyRepository<Company>
    {
        Task createAsync(Company company);
        Task updateAsync(Company company);
        Task deleteAsync(Company company);
        Task patchAsync(Company company);

        Task<Company?> getCompanyByIdAsync(long id);
    }
}
