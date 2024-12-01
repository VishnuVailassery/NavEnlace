using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SEO.Optimize.Core.Models
{
    public class DetailedSiteInfo
    {
        public int SiteId { get; set; } // Primary key
        public string? ExternalSiteId { get; set; } // Primary key
        public string? SiteUrl { get; set; } // Required site URL
        public string? SiteName { get; set; } // Optional site name
        public int? UserId { get; set; } // Foreign key to UserInfo
        public int? TotalPages { get; set; }
        public int? TotalInternalLinks { get; set; }
        public int? TotalExternalLinks { get; set; }
        public int? TotalOpportunities { get; set; }
    }
}
