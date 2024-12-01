using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SEO.Optimize.Core.Models
{
    public class SiteIntegrationInfo
    {
        public string ExternalSiteId { get; set; }

        public string UserMailId { get; set; }

        public string SiteName { get; set; }

        public string SiteUrl { get; set; }

        public DateTime LastCrawledOn { get; set; } = DateTime.MinValue;

        public string AccessToken { get; set; }

        public DateTime? AccessTokenExpiry { get; set; }
    }
}
