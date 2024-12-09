using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SEO.Optimize.Core.Clients;
using SEO.Optimize.Core.Interfaces;
using SEO.Optimize.Core.Models.Google;
using SEO.Optimize.Core.Models.Jobs;
using SEO.Optimize.Core.Models.Jobs.JobProperties;
using SEO.Optimize.Core.Models.Tokens;
using SEO.Optimize.Core.Models.Webflow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SEO.Optimize.Jobs.Handlers
{
    public class PageCrawlHandler : ICrawlJobHandler
    {
        private readonly WebflowClient webflowClient;
        private readonly IContentRepository contentRepository;
        private readonly IPageCrawlModelHandler pageCrawlModelHandler;
        private readonly GscClient gscClient;

        public PageCrawlHandler(
            WebflowClient webflowClient, 
            IContentRepository contentRepository, 
            IPageCrawlModelHandler pageCrawlModelHandler,
            GscClient gscClient)
        {
            this.webflowClient = webflowClient;
            this.contentRepository = contentRepository;
            this.pageCrawlModelHandler = pageCrawlModelHandler;
            this.gscClient = gscClient;
        }

        public async Task ProcessJobAsync(JobInfo jobInfo)
        {
            var dict = new Dictionary<CollectionItem, CrawlPageOutput>();

            var tokens = await contentRepository.GetTokensBySiteId(jobInfo.SiteId);
            var siteInfo = await contentRepository.GetSiteByIdAsync(jobInfo.SiteId);

            var token = await gscClient.EnsureTokenIsValid(tokens);
            if (token != null)
            {
                var tokenList = new List<TokenInfo>();
                var expiry = DateTime.UtcNow.AddSeconds(token.ExpiresInSeconds.Value);
                tokenList.Add(new TokenInfo(token.AccessToken, "gsc_access_token", jobInfo.SiteId, expiry));
                tokenList.Add(new TokenInfo(token.RefreshToken, "gsc_refresh_token", jobInfo.SiteId, expiry));

                await contentRepository.StoreTokens(tokenList); //todo: modify to update
            }

            var accessToken = tokens.First(o => o.TokenType == "access_token").Token;
            var gscAccessToken = tokens.First(o => o.TokenType == "gsc_access_token").Token;
            var gscRefreshToken = tokens.First(o => o.TokenType == "gsc_refresh_token").Token;

            var gscAnalytics = await gscClient.GetAllSearchAnalyticsBySite(siteInfo.Url, tokens);

            var props = JsonConvert.DeserializeObject<PageCrawlJobPropperties>(jobInfo.JobProperties);
            var idPath = $"{props.CollectionId}/items/{props.ExternalPageId}";

            if(props.AccessToken == null)
            {
                throw new Exception("missing access token");
            }

            var item = await webflowClient.GetCollectionItemAsync(idPath, props.AccessToken);

            item.CollectionId = props.CollectionId;
            dict.Add(item, await CrawlPage(item, gscAnalytics));
            await contentRepository.UpdateJobAsync(jobInfo.Id);

            await contentRepository.AddCrawlData(jobInfo.UserId, jobInfo.SiteId, dict);
            await contentRepository.UpdateJobAsync(jobInfo.Id, "Completed");
        }

        private async Task<CrawlPageOutput> CrawlPage(CollectionItem collectionItem, SearchAnalyticsResponse gscAnalytics)
        {
            //var collectionSlug = collection.Slug;

            var gscKeys = gscAnalytics.Rows.Select(o => (o.Keys.First(), o.Keys.Last()));
            var keywords = new List<(string, string)>()
            {
                ("Consequatur", "https://thesample-e155a7.webflow.io/"),
                ("temporibus","https://thesample-e155a7.webflow.io/"),
                ("nostrum", "https://thesample-e155a7.webflow.io/"),
                ("Repudiandae", "https://thesample-e155a7.webflow.io/")
            };

            CrawlPageOutput res = new CrawlPageOutput();
            foreach (var key in collectionItem.FieldData)
            {
                if (key.Value is null || key.Value is JObject)
                {
                    continue;
                }

                var output = await pageCrawlModelHandler.CrawlContentAndFindLinks(key.Key, key.Value.ToString(), keywords);
                res.ExistingLinks.Concat(output.ExistingLinks);
                res.LinkOpportunities.Concat(output.LinkOpportunities);
            }

            return res;
        }
    }
}
