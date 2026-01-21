using FluentResults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;

namespace Application
{
    public interface IPatchCompanyCommand
    {
        Task<Result> PatchCompanyNameAsync(string companyId, string Name);

        Task<Result> PatchCompanyVatAsync(string companyId, string Vat);
    }
}
