using FluentResults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application
{
    public interface IDeleteCompanyCommand
    {
        Task<Result> DeleteCompanyAsync(long id);
    }
}
