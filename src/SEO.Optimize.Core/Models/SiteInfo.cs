using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SEO.Optimize.Core.Models
{
    public class SiteInfo
    {
        public int SiteId { get; set; }

        public int UserId { get; set; }

        [Required]
        public string ExternalSiteId { get; set; }

        [Required, MaxLength(200)]
        public string SiteName { get; set; }

        [Required, Url]
        public string Url { get; set; }

        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
        public DateTime? LastCrawledOn { get; set; }
    }
}
