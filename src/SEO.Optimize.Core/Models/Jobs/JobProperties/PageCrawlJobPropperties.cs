namespace SEO.Optimize.Core.Models.Jobs.JobProperties
{
    public class PageCrawlJobPropperties
    {
        public string? ExternalPageId { get; set; }

        public string? ExternalSiteId { get; set; }

        public string? CollectionId { get; set; }

        public bool IsStaticPage { get; set; }

        public string? AccessToken { get; set; }
    }
}
