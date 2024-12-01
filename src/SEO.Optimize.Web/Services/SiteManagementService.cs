using Newtonsoft.Json;
using SEO.Optimize.Core.Interfaces;
using SEO.Optimize.Core.Models;
using SEO.Optimize.Core.Models.Opportunities;
using SEO.Optimize.Core.Models.Utils;
using SEO.Optimize.Core.Models.Webflow;
using SEO.Optimize.Postgres.Repository;

namespace SEO.Optimize.Web.Services
{
    public class SiteManagementService
    {
        private readonly HttpClient _httpClient = new HttpClient();
        private readonly IContentRepository contentRepository;

        public SiteManagementService(IContentRepository contentRepository)
        {
            this.contentRepository = contentRepository;
        }

        public async Task<IEnumerable<WebflowSite>> GetAllAuthorizedSites(TokenResponse tokenResponse)
        {
            var userInfoUrl = "https://api.webflow.com/v2/sites";
            var request = new HttpRequestMessage(HttpMethod.Get, userInfoUrl);
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", tokenResponse.access_token);
            var responseMessage = await _httpClient.SendAsync(request);
            var resStr = await responseMessage.Content.ReadAsStringAsync();
            var siteInfo = JsonConvert.DeserializeObject<WebflowSiteInfo>(resStr);

            return siteInfo.Sites;
        }

        public async Task<int> SaveSiteAndTokenAsync(string userEmail, string selectedSiteId, TokenResponse tokenResponse)
        {
            var userInfoUrl = $"https://api.webflow.com/v2/sites/{selectedSiteId}";
            var request = new HttpRequestMessage(HttpMethod.Get, userInfoUrl);
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", tokenResponse.access_token);
            var responseMessage = await _httpClient.SendAsync(request);
            var resStr = await responseMessage.Content.ReadAsStringAsync();
            var siteInfo = JsonConvert.DeserializeObject<WebflowSite>(resStr);

            var siteIntegrationInfo = new SiteIntegrationInfo
            {
                SiteName = siteInfo.DisplayName,
                ExternalSiteId = siteInfo.Id,
                SiteUrl = string.Join('|', siteInfo.CustomDomains),
                AccessToken = tokenResponse.access_token,
                AccessTokenExpiry = DateTime.MaxValue,
                UserMailId = userEmail,
            };

            var siteId = await contentRepository.IntegrateNewSite(siteIntegrationInfo);
            return siteId;
        }

        public async Task<IEnumerable<PageInfo>> GetPages(int? siteId = null, string? url = null, string? title = null, int? offset = null)
        {
            PageSearchParams searchParams = new PageSearchParams();
            searchParams.Title = title;
            searchParams.Url = url;
            searchParams.SiteId = siteId;
            searchParams.Offset = 0;

           var res = await contentRepository.GetPages(searchParams);

            return res;
        }

        public async Task<IEnumerable<Opportunities>> GetOpportunities(int? siteId = null, string? url = null, string? title = null, int? offset = null)
        {
            var searchParams = new LinkSearchParams();
            var linkDetails = await contentRepository.GetOpportunities(searchParams);

            return linkDetails;
        }

        public async Task<IEnumerable<Opportunities>> GetOpportunitiesByIds(IEnumerable<int> ids)
        {
            var linkDetails = await contentRepository.GetOpportunitiesByIds(ids);

            return linkDetails;
        }
    }
}
