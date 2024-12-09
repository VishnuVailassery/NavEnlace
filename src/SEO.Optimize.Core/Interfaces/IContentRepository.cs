using SEO.Optimize.Core.Models;
using SEO.Optimize.Core.Models.Jobs;
using SEO.Optimize.Core.Models.Opportunities;
using SEO.Optimize.Core.Models.Tokens;
using SEO.Optimize.Core.Models.Utils;
using SEO.Optimize.Core.Models.Webflow;

namespace SEO.Optimize.Core.Interfaces
{
    public interface IContentRepository
    {
        Task CreateUserAsync(UserAuthInfo userAuthInfo);

        Task<UserAuthInfo?> GetUserByMailId(string mailId);

        Task<IEnumerable<DetailedSiteInfo>> GetGeneralSiteDetailsByUserAsync(string userId);


        Task<int> IntegrateNewSite(SiteIntegrationInfo siteIntegrationInfo);
        Task<SiteInfo> GetSiteByIdAsync(int siteId);


        Task StoreTokens(List<TokenInfo> tokens);
        Task<IEnumerable<TokenInfo>> GetTokensBySiteId(int siteId);


        Task ScheduleCrawlJob(int userId, int siteId);
        Task ScheduleCrawlForPage(JobInfo jobInfo);
        Task<IEnumerable<JobInfo>> GetAllCrawlJobs();
        Task UpdateJobAsync(int jobId, string? status = null);



        Task AddCrawlData(int userId, int siteId, Dictionary<CollectionItem, CrawlPageOutput> pageCrawlDataMapping);

        Task<IEnumerable<PageInfo>> GetPages(PageSearchParams searchParams);

        Task<IEnumerable<Opportunities>> GetOpportunities(LinkSearchParams linkSearchParams);
        Task<IEnumerable<Opportunities>> GetOpportunitiesByIds(IEnumerable<int> ids);
    }
}
