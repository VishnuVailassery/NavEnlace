using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SEO.Optimize.Core.Configurations;
using SEO.Optimize.Core.Interfaces;
using SEO.Optimize.Core.Models;
using SEO.Optimize.Postgres.Repository;
using System.Security.Claims;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace SEO.Optimize.Web.Services
{
    public class IntegrationService
    {
        private readonly string[] scopes = { "app_subscriptions:read", "assets:read", "assets:write", "authorized_user:read", "cms:read", "cms:write", "components:read", "components:write", "custom_code:read", "custom_code:write", "ecommerce:read", "ecommerce:write", "forms:read", "forms:write", "pages:read", "pages:write", "sites:read", "sites:write", "site_activity:read", "site_config:read", "site_config:write", "users:read", "users:write", "workspace:read", "workspace:write" };

        private readonly HttpClient _httpClient = new HttpClient();
        private readonly IContentRepository contentRepository;
        private readonly WebflowConfigs webflowConfigs;

        public IntegrationService(IContentRepository contentRepository, IOptions<WebflowConfigs> webflowConfigs)
        {
            this.contentRepository = contentRepository;
            this.webflowConfigs = webflowConfigs.Value;
        }

        public string AuthorizeWithWebflow()
        {
            var authorizationUrl = GetAuthorizationCodeRequestUrl()
                                   .ToString();

            return authorizationUrl;
        }

        public async Task<TokenResponse> ExchangeCodeForToken(string code, ClaimsIdentity claimsIdentity)
        {
            var email = claimsIdentity.FindFirst(ClaimTypes.Email)?.Value;
            var user = await contentRepository.GetUserByMailId(email);
            if (user == null)
            {
                throw new InvalidOperationException();
            }

            var tokenUrl = "https://api.webflow.com/oauth/access_token";
            var requestBody = new Dictionary<string, string>();
            {
                requestBody.Add("client_id", webflowConfigs.ClientId);
                requestBody.Add("client_secret", webflowConfigs.ClientSecret);
                requestBody.Add("code", code);
                requestBody.Add("redirect_uri", webflowConfigs.RedirectUrls.First());
                requestBody.Add("grant_type", "authorization_code");
            };

            var requestContent = new FormUrlEncodedContent(requestBody);

            try
            {
                var response = await _httpClient.PostAsync(tokenUrl, requestContent);
                response.EnsureSuccessStatusCode();
                var responseContent = await response.Content.ReadAsStringAsync();
                var tokenResponse = JsonSerializer.Deserialize<TokenResponse>(responseContent);

                return tokenResponse;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error exchanging code for token: {ex.Message}");
                throw;
            }
        }

        private string GetAuthorizationCodeRequestUrl()
        {
            return $"https://webflow.com/oauth/authorize?" +
                   $"client_id={webflowConfigs.ClientId}&" +
                   $"redirect_uri={webflowConfigs.RedirectUrls.First()}&" +
                   $"response_type=code&" +
                   $"scope={string.Join(" ", scopes)}&" +
                   $"state={Guid.NewGuid()}";
        }
    }
}
