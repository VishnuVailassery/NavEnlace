using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2;
using Google.Apis.SearchConsole.v1;
using Google.Apis.Auth.OAuth2.Responses;
using System.Security.Claims;

namespace SEO.Optimize.Web.Services
{
    public class GSCService
    {
        private readonly string[] scopes = { SearchConsoleService.ScopeConstants.WebmastersReadonly };
        private readonly AppStateCache appStateCache;
        private readonly TokenManagementService tokenManagementService;
        private readonly JobService jobService;

        public GSCService(AppStateCache appStateCache, TokenManagementService tokenManagementService, JobService jobService)
        {
            this.appStateCache = appStateCache;
            this.tokenManagementService = tokenManagementService;
            this.jobService = jobService;
        }

        public async Task<string> GoogleAuthorizationRequestUrl(ClaimsIdentity identity)
        {
            var state = Guid.NewGuid().ToString();
            var sessionKey = identity.FindFirst("seopt_session")?.Value;
            await appStateCache.UpdateAsync(sessionKey, new Dictionary<string, string>() { { "gsc_state", state } });

            return GoogleAuthorizationCodeRequestUrl(state);
        }

        public async Task<TokenResponse> ExchangeCodeAndStoreTokens(string code, string state, ClaimsIdentity identity)
        {
            var sessionKey = identity.FindFirst("seopt_session")?.Value;
            var sessionCache = await  appStateCache.GetAsync(sessionKey);
            if(sessionCache.TryGetValue("gsc_state", out var response) && response != state)
            {
                throw new ArgumentException("Invalid State");
            };

            if (!sessionCache.TryGetValue("site_link_inprogress", out var isSiteLinking)
                || isSiteLinking != bool.TrueString
                || !sessionCache.TryGetValue("site_id", out var siteId)
                || !int.TryParse(siteId, out var result)
            )
            {
                throw new ArgumentException("Could not find any site to link for the current session");
            }

            var clientSecrets = new ClientSecrets
            {
                ClientId = clientId,
                ClientSecret = clientSecret
            };
            
            var codeFlow = new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer
            {
                ClientSecrets = clientSecrets,
                Scopes = scopes
            });

            var tokenResponse = await codeFlow.ExchangeCodeForTokenAsync(
                userId: "user",
                code: code,
                redirectUri: redirectUri,
                CancellationToken.None
            );

            var siteIdVal = int.Parse(sessionCache["site_id"]);
            await tokenManagementService.StoreGSCTokens(tokenResponse, siteIdVal);
            await jobService.ScheduleCrawlJobForSite(1, siteIdVal); // todo

            await appStateCache.UpdateAsync(sessionKey, new Dictionary<string, string>()
            {
                {"site_link_inprogress", bool.FalseString },
                {"site_id", string.Empty },
            });

            return tokenResponse;
        }

        private string GoogleAuthorizationCodeRequestUrl(string state)
        {
            return $"https://accounts.google.com/o/oauth2/v2/auth?" +
                   $"client_id={clientId}&" +
                   $"redirect_uri={redirectUri}&" +
                   $"response_type=code&" +
                   $"scope={string.Join(" ", scopes)}&" +
                   $"access_type=offline&" +
                   $"prompt=consent&" +
                   $"state={state}";
        }
    }
}
