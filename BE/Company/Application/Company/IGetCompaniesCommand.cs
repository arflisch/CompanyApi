using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain;
using Domain.DTO;

namespace Application
{
    public interface IGetCompaniesCommand
    {
         Task<List<CompanyDto>> GetAllCompaniesAsync();
    }
}
