namespace SEO.Optimize.Postgres.Model
{
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class Site
    {
        [Key]
        public int SiteId { get; set; }

        [Required]
        [ForeignKey("User")]
        public int UserId { get; set; }

        [Required]
        public string ExternalSiteId { get; set; }

        [Required, MaxLength(200)]
        public string SiteName { get; set; }

        [Required, Url]
        public string Url { get; set; }

        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
        public DateTime? LastCrawledOn { get; set; }

        public virtual User User { get; set; }
        public virtual ICollection<Token> Tokens { get; set; }
        public virtual ICollection<CrawlInfo> CrawlInfos { get; set; }
        public virtual ICollection<PageInfo> PageInfos { get; set; }
    }
}
