namespace SEO.Optimize.Postgres.Model
{
    using SEO.Optimize.Core.Models;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class CrawlInfo
    {
        [Key]
        public int CrawlId { get; set; }

        [Required]
        [ForeignKey("Site")]
        public int SiteId { get; set; }

        public DateTime CrawlDate { get; set; }

        public int TotalPages { get; set; }

        public int TotalInternalLinks { get; set; }
        public int TotalExternalLinks { get; set; }
        public int TotalOpportunities { get; set; }

        public virtual Site Site { get; set; }
        public virtual ICollection<PageInfo> PageInfos { get; set; }
    }
}
