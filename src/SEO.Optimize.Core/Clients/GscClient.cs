using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.SearchConsole.v1;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using SEO.Optimize.Core.Configurations;
using SEO.Optimize.Core.Interfaces;
using SEO.Optimize.Core.Models;
using SEO.Optimize.Core.Models.Google;
using SEO.Optimize.Core.Models.Tokens;
using SEO.Optimize.Core.Models.Webflow;
using System.Text;
using TokenResponse = Google.Apis.Auth.OAuth2.Responses.TokenResponse;

namespace SEO.Optimize.Core.Clients
{
    public class GscClient
    {
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly HttpClient httpClient;
        private readonly IContentRepository contentRepository;
        private readonly GSCConfigs gscConfigs;
        private string? TokenEndpoint = "https://oauth2.googleapis.com/token";
        const string BASE_GSC_URL = "https://www.googleapis.com/webmasters/v3/sites";

        public GscClient(
            IHttpClientFactory httpClientFactory, 
            IContentRepository contentRepository,
            IOptions<GSCConfigs> gscConfigs)
        {
            httpClient = httpClientFactory.CreateClient();
            this.contentRepository = contentRepository;
            this.gscConfigs = gscConfigs.Value;
        }

        public async Task<SearchAnalyticsResponse> GetAllSearchAnalyticsBySite(string siteUrl, IEnumerable<TokenInfo> tokens)
        {
            var gscAccessToken = tokens.First(o => o.TokenType == "gsc_access_token").Token;
            var gscRefreshToken = tokens.First(o => o.TokenType == "gsc_refresh_token").Token;
            
            var sites = await GetAllSites(gscAccessToken);
            var gscSite = sites.SiteEntry.FirstOrDefault(o => o.SiteUrl.Contains(siteUrl));

            var url = $"{BASE_GSC_URL}/{gscSite.SiteUrl}/searchAnalytics/query";
            var request = new HttpRequestMessage(HttpMethod.Post, url);
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", gscAccessToken);

            var content = new SearchAnalyticsRequest()
            {
                Dimensions = new List<string>() { "query" },
                RowLimit = 200,
                Type = "web"
            };

            content.SetStartDate(DateTime.UtcNow.AddMonths(-3));
            content.SetEndDate(DateTime.UtcNow);

            var settings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                Formatting = Formatting.Indented
            };

            var json = JsonConvert.SerializeObject(content, settings);
            request.Content = new StringContent(json);
            var responseMessage = await httpClient.SendAsync(request);
            responseMessage.EnsureSuccessStatusCode();
            var resStr = await responseMessage.Content.ReadAsStringAsync();
            var searchAnalytics = JsonConvert.DeserializeObject<SearchAnalyticsResponse>(resStr);
            return searchAnalytics;
        }

        public async Task<GscSitesResponse> GetAllSites(string token)
        {
            var url = $"{BASE_GSC_URL}";
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var responseMessage = await httpClient.SendAsync(request);
            responseMessage.EnsureSuccessStatusCode();
            var resStr = await responseMessage.Content.ReadAsStringAsync();

            var gscSites = JsonConvert.DeserializeObject<GscSitesResponse>(resStr);
            return gscSites;
        }

        public async Task<TokenResponse> EnsureTokenIsValid(IEnumerable<TokenInfo> tokens)
        {
            var gscAccessToken = tokens.First(o => o.TokenType == "gsc_access_token");
            var gscRefreshToken = tokens.First(o => o.TokenType == "gsc_refresh_token");

            if(gscAccessToken.Expiry > DateTime.UtcNow)
            {
                return null;
            }

            var requestBody = new
            {
                client_id = gscConfigs.ClientId,
                client_secret = gscConfigs.ClientSecret,
                refresh_token = gscRefreshToken.Token,
                grant_type = "refresh_token"
            };

            string jsonPayload = JsonConvert.SerializeObject(requestBody);
            HttpResponseMessage response = await httpClient.PostAsync(
                TokenEndpoint, 
                new StringContent(jsonPayload, Encoding.UTF8, "application/json")
            );

            response.EnsureSuccessStatusCode();

            string responseContent = await response.Content.ReadAsStringAsync();
            var tokenResponse = JsonConvert.DeserializeObject<TokenResponse>(responseContent);

            return tokenResponse;
        }
    }
}
