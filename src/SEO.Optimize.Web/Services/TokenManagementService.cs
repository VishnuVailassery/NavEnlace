using Google.Apis.Auth.OAuth2.Responses;
using Microsoft.AspNetCore.Authorization;
using SEO.Optimize.Core.Interfaces;
using SEO.Optimize.Core.Models.Tokens;
using SEO.Optimize.Postgres.Repository;
using System.Security.Claims;

namespace SEO.Optimize.Web.Services
{
    [Authorize]
    public class TokenManagementService
    {
        private readonly IContentRepository contentRepository;
        private readonly AppStateCache appStateCache;

        public TokenManagementService(IContentRepository contentRepository, AppStateCache appStateCache)
        {
            this.contentRepository = contentRepository;
            this.appStateCache = appStateCache;
        }

        public async Task StoreGSCTokens(TokenResponse tokenResponse, int siteId)
        {
            var tokenList = new List<TokenInfo>();
            var expiry = DateTime.UtcNow.AddSeconds(tokenResponse.ExpiresInSeconds.Value);

            tokenList.Add(new TokenInfo(tokenResponse.AccessToken, "gsc_access_token", siteId, expiry));
            tokenList.Add(new TokenInfo(tokenResponse.RefreshToken, "gsc_refresh_token", siteId, expiry));

            await contentRepository.StoreTokens(tokenList);
        }
    }
}
