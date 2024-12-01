using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SEO.Optimize.Core.Models.Utils
{
    public class PageSearchParams
    {
        public int? SiteId { get; set; }
        public string? Title { get; set; }

        public string? Url { get; set; }

        public int? LinkOpportunityCountGreaterThan { get; set; }
        public int? Offset { get; set; }
    }
}
