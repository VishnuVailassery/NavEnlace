using SEO.Optimize.Core.Models.Opportunities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SEO.Optimize.Core.Models.Jobs
{
    public class CrawlPageOutput
    {
        public List<LinkProperties> ExistingLinks { get; set; } = new();

        public List<LinkProperties> LinkOpportunities { get; set;} = new();
    }
}
