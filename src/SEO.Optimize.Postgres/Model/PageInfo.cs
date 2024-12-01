namespace SEO.Optimize.Postgres.Model
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class PageInfo
    {
        [Key]
        public int PageId { get; set; }

        public string ExternalId { get; set; }

        [Required]
        [MaxLength(255)]
        public string Url { get; set; }

        [Required]
        public string Title { get; set; }

        // Foreign Keys
        public int SiteId { get; set; }

        public int CrawlId { get; set; }

        public Site Site { get; set; }

        public CrawlInfo CrawlInfo { get; set; }

        public PageStats PageStats { get; set; }
        public ICollection<LinkDetail> LinkDetails { get; set; }
    }
}
