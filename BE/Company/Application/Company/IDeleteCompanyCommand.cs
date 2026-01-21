using FluentResults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;

namespace Application
{
    public interface IDeleteCompanyCommand
    {
        Task<Result> DeleteCompanyAsync(string id);
    }
}
