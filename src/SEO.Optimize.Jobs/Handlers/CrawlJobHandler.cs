using Newtonsoft.Json.Linq;
using SEO.Optimize.Core.Clients;
using SEO.Optimize.Core.Interfaces;
using SEO.Optimize.Core.Models.Google;
using SEO.Optimize.Core.Models.Jobs;
using SEO.Optimize.Core.Models.Tokens;
using SEO.Optimize.Core.Models.Webflow;

namespace SEO.Optimize.Jobs.Handlers
{
    public class CrawlJobHandler : ICrawlJobHandler
    {
        private readonly IContentRepository contentRepository;
        private readonly WebflowClient webflowClient;
        private readonly GscClient gscClient;
        private readonly IPageCrawlModelHandler pageCrawlModelHandler;

        public CrawlJobHandler(
            IContentRepository contentRepository, 
            WebflowClient webflowClient,
            GscClient gscClient,
            IPageCrawlModelHandler pageCrawlModelHandler)
        {
            this.contentRepository = contentRepository;
            this.webflowClient = webflowClient;
            this.gscClient = gscClient;
            this.pageCrawlModelHandler = pageCrawlModelHandler;
        }

        public async Task ProcessJobAsync(JobInfo jobInfo)
        {
            var dict = new Dictionary<CollectionItem, CrawlPageOutput>();

            try
            {
                await contentRepository.UpdateJobAsync(jobInfo.Id, "Running");
                var tokens = await contentRepository.GetTokensBySiteId(jobInfo.SiteId);
                var siteInfo = await contentRepository.GetSiteByIdAsync(jobInfo.SiteId);

                var token = await gscClient.EnsureTokenIsValid(tokens);
                if (token != null)
                {
                    var tokenList = new List<TokenInfo>();
                    var expiry = DateTime.UtcNow.AddSeconds(token.ExpiresInSeconds.Value);
                    tokenList.Add(new TokenInfo(token.AccessToken, "gsc_access_token", jobInfo.SiteId, expiry));

                    await contentRepository.StoreTokens(tokenList); //todo: modify to update
                }

                var accessToken = tokens.First(o => o.TokenType == "access_token").Token;
                var gscAccessToken = tokens.First(o => o.TokenType == "gsc_access_token").Token;

                var gscAnalytics = await gscClient.GetAllSearchAnalyticsBySite(siteInfo.Url, tokens);
                var collections = await webflowClient.GetAllCMSCollections(siteInfo.ExternalSiteId, accessToken);
                foreach (var collection in collections.Collections)
                {
                    var collectionId = collection.Id;
                    var pageInfo = await webflowClient.GetCMSCollectionItems(collectionId, accessToken);

                    foreach (var item in pageInfo.Items)
                    {
                        item.CollectionId = collectionId;
                        var output = await CrawlPage(item, collection, gscAnalytics);
                        if(output != null)
                        {
                            dict.Add(item, output);
                        }
                        await contentRepository.UpdateJobAsync(jobInfo.Id);
                    }
                }

                await contentRepository.AddCrawlData(jobInfo.UserId, jobInfo.SiteId, dict);
                await contentRepository.UpdateJobAsync(jobInfo.Id, "Completed");
            }
            catch(Exception ex)
            {

            }
        }

        private async Task<CrawlPageOutput> CrawlPage(CollectionItem collectionItem, WebflowCMSCollection collection, SearchAnalyticsResponse gscAnalytics)
        {
            var collectionSlug = collection.Slug;

            var gscKeys = gscAnalytics.Rows.Select(o => (o.Keys.First(), o.Keys.Last()));
            var keywords =  new List<(string, string)>()
            { 
                ("Consequatur", "https://thesample-e155a7.webflow.io/"), 
                ("temporibus","https://thesample-e155a7.webflow.io/"), 
                ("nostrum", "https://thesample-e155a7.webflow.io/"), 
                ("Repudiandae", "https://thesample-e155a7.webflow.io/") ,
                ("Quisque", "https://secondsample.webflow.io/") ,
                ("Praesent", "https://secondsample.webflow.io/") ,
                ("vibrant", "https://secondsample.webflow.io/") ,
                ("heart of the city", "https://secondsample.webflow.io/") ,
                ("Jasmin Studio Webflow", "https://secondsample.webflow.io/") ,
                ("Bright", "https://secondsample.webflow.io/") ,
            };

            CrawlPageOutput res = new CrawlPageOutput();
            foreach (var keyValue in collectionItem.FieldData)
            {
                if(keyValue.Value is null || keyValue.Value is JObject)
                {
                    continue;
                }

                var output = await pageCrawlModelHandler.CrawlContentAndFindLinks(keyValue.Key, keyValue.Value.ToString(), keywords);
                res.ExistingLinks.AddRange(output.ExistingLinks);
                res.LinkOpportunities.AddRange(output.LinkOpportunities);
            }

            return res;
        }
    }
}
