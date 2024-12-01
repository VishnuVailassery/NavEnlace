using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SEO.Optimize.Core.Models
{
    public class PageInfo
    {
        public int Id { get; set; }
        public string ExternalId { get; set; }
        public string Title { get; set; }
        public string Url { get; set; }

        public PageStatistics? Stats { get; set; }
    }

    public class PageStatistics
    {
        public int StatsId { get; set; }
        public int InternalLinkCount { get; set; }
        public int ExternalLinkCount { get; set; }

        public int LinkOpportunityCount { get; set; }

        public int PageId { get; set; }
    }
}
