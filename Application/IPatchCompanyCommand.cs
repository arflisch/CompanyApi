﻿using FluentResults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application
{
    public interface IPatchCompanyCommand
    {
        Task<Result> PatchCompanyNameAsync(long companyId, string Name);

        Task<Result> PatchCompanyVatAsync(long companyId, string Vat);
    }
}
