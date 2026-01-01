using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompanyFrontend.Services
{
    public class AuthService : IAuthService
    {
        private IPublicClientApplication _app;

        private const string ClientId = "79b484c6-b5fa-4d84-9d28-260f108811b1";
        private const string TenantId = "f7a15417-57cb-4855-8d36-064f95aada17";
        private readonly string[] Scopes = new[] { $"api://{ClientId}/access_as_user" };
        public bool IsAdmin { get; private set; }

        public AuthService()
        {
            _app = PublicClientApplicationBuilder.Create(ClientId)
                .WithAuthority(AzureCloudInstance.AzurePublic, TenantId)
                .WithRedirectUri("http://localhost")
                .Build();
        }

        public async Task<string?> LoginAsync()
        {
            AuthenticationResult result;
            try
            {
                var accounts = await _app.GetAccountsAsync();
                result = await _app.AcquireTokenSilent(Scopes, accounts.FirstOrDefault())
                    .ExecuteAsync();
            }
            catch (MsalUiRequiredException)
            {
                // 2. Tentative interactive
                result = await _app.AcquireTokenInteractive(Scopes).ExecuteAsync();
            }

            if (result != null && !string.IsNullOrEmpty(result.AccessToken))
            {
                CheckIfAdmin(result.AccessToken);
            }

            return result?.AccessToken;
        }

        private void CheckIfAdmin(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);

            var roles = jwtToken.Claims.Where(c => c.Type == "roles").Select(c => c.Value).ToList();
            IsAdmin = roles.Contains("CompanyAdmin");
        }

        public async Task LogoutAsync()
        {
            var accounts = await _app.GetAccountsAsync();
            if (accounts.Any()) await _app.RemoveAsync(accounts.First());
        }
    }
}
