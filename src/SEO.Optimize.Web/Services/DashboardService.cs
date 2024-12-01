using SEO.Optimize.Core.Interfaces;
using SEO.Optimize.Core.Models.ProjectInfo;
using SEO.Optimize.Postgres.Repository;

namespace SEO.Optimize.Web.Services
{

    public class DashboardService
    {
        private readonly IContentRepository contentRepository;

        public DashboardService(IContentRepository contentRepository)
        {
            this.contentRepository = contentRepository;
        }

        public async Task<IEnumerable<DashboardTileData>> GetDashboardData(string userEmail)
        {
            var result = await contentRepository.GetGeneralSiteDetailsByUserAsync(userEmail);
            return result.Select(o => new DashboardTileData(o));
        }
    }
}
