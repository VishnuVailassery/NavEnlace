using SEO.Optimize.Core.Interfaces;
using SEO.Optimize.Postgres.Repository;

namespace SEO.Optimize.Web.Services
{
    public class JobService
    {
        private readonly IContentRepository contentRepository;

        public JobService(IContentRepository contentRepository)
        {
            this.contentRepository = contentRepository;
        }

        public async Task ScheduleCrawlJobForSite(int userId, int siteId)
        {
            await contentRepository.ScheduleCrawlJob(userId, siteId);
        }
    }
}
