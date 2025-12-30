using Domain.DTO;
using FluentResults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application
{
    public interface ICreateCompanyCommand
    {
       Task<Result<CompanyDto>> CreateCompanyAsync(CreateCompanyDto companyDto);
    }
}
