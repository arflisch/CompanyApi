using Duende.IdentityModel.OidcClient;
using System;
using System.Collections.Specialized;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;

namespace CompanyFrontend.Services
{
    public class AuthService : IAuthService
    {
        private readonly OidcClient _oidcClient;

        // --- NOUVEAU : On stocke les tokens directement au lieu de LoginResult ---
        private string? _accessToken;
        private string? _refreshToken;
        private string? _identityToken;
        private DateTimeOffset _tokenExpiration; // Utiliser DateTimeOffset est plus sûr

        // Configuration Azure AD
        private const string ClientId = "79b484c6-b5fa-4d84-9d28-260f108811b1";

        private const string Authority =
            $"https://login.microsoftonline.com/f7a15417-57cb-4855-8d36-064f95aada17";

        private const string RedirectUri = "http://localhost:1033";
        private readonly string Scope = $"openid profile offline_access api://{ClientId}/access_as_user";

        public bool IsAdmin { get; private set; }
        public string? LastError { get; private set; }

        public AuthService()
        {
            var options = new OidcClientOptions
            {
                Authority = Authority,
                ClientId = ClientId,
                RedirectUri = RedirectUri,
                Scope = Scope,
                LoadProfile = false, // Important : On désactive le UserInfo endpoint car notre Token est pour l'API, pas pour Graph
                Browser = new SystemBrowser(port: 1033),
                Policy = new Policy
                {
                    Discovery = new Duende.IdentityModel.Client.DiscoveryPolicy
                    {
                        ValidateIssuerName = false,
                        AllowHttpOnLoopback = true,
                        ValidateEndpoints = false,
                        DiscoveryDocumentPath = "/v2.0/.well-known/openid-configuration"
                    }
                }
            };

            _oidcClient = new OidcClient(options);
        }

        public async Task<string?> LoginAsync()
        {
            LastError = null;

            // 1. Vérification : A-t-on déjà un token valide ?
            if (!string.IsNullOrEmpty(_accessToken) && _tokenExpiration > DateTimeOffset.UtcNow)
            {
                return _accessToken;
            }

            // 2. Refresh : Si on a un Refresh Token, on l'utilise
            if (!string.IsNullOrEmpty(_refreshToken))
            {
                try
                {
                    var refreshResult = await _oidcClient.RefreshTokenAsync(_refreshToken);

                    if (!refreshResult.IsError)
                    {
                        // Mise à jour des variables locales avec le nouveau token
                        _accessToken = refreshResult.AccessToken;
                        _refreshToken = refreshResult.RefreshToken;
                        _tokenExpiration = refreshResult.AccessTokenExpiration;

                        return _accessToken;
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine($"Refresh failed: {refreshResult.Error}");
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Refresh exception: {ex.Message}");
                }
            }

            // 3. Login : Flux complet avec navigateur
            try
            {
                var loginResult = await _oidcClient.LoginAsync(new LoginRequest());

                if (loginResult.IsError)
                {
                    LastError = loginResult.Error;
                    if (!string.IsNullOrEmpty(loginResult.ErrorDescription))
                    {
                        LastError += $" | Description: {loginResult.ErrorDescription}";
                    }
                    System.Diagnostics.Debug.WriteLine($"Login Error: {LastError}");
                    return null;
                }

                // Sauvegarde des résultats dans nos variables
                _accessToken = loginResult.AccessToken;
                _refreshToken = loginResult.RefreshToken;
                _identityToken = loginResult.IdentityToken;
                _tokenExpiration = loginResult.AccessTokenExpiration;

                CheckIfAdmin(_accessToken);

                return _accessToken;
            }
            catch (Exception ex)
            {
                LastError = ex.Message;
                System.Diagnostics.Debug.WriteLine($"Exception during login: {ex.Message}");
                return null;
            }
        }

        public async Task LogoutAsync()
        {
            if (!string.IsNullOrEmpty(_identityToken))
            {
                // On utilise l'IdentityToken pour dire à Azure QUEL compte déconnecter
                await _oidcClient.LogoutAsync(new LogoutRequest { IdTokenHint = _identityToken });
            }

            // Nettoyage local
            _accessToken = null;
            _refreshToken = null;
            _identityToken = null;
            IsAdmin = false;
        }

        private void CheckIfAdmin(string token)
        {
            try
            {
                var handler = new JwtSecurityTokenHandler();
                var jwtToken = handler.ReadJwtToken(token);

                var rolesClaim = jwtToken.Claims.Where(c => c.Type == "roles" || c.Type == "role").ToList();
                if (rolesClaim.Any())
                {
                    var roles = rolesClaim.Select(c => c.Value).ToList();
                    IsAdmin = roles.Contains("CompanyAdmin");
                }
                else
                {
                    IsAdmin = false;
                }
            }
            catch
            {
                IsAdmin = false;
            }
        }
    }
}