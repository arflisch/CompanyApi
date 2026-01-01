using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CompanyFrontend.Services
{
    public interface IAuthService
    {
        Task<string?> LoginAsync();
        Task LogoutAsync();
        bool IsAdmin { get; }
    }
}
