using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SEO.Optimize.Core.Models.Jobs
{
    public class CrawlPageOutput
    {
        public Dictionary<string, string> ExistingLinks { get; set; } = new();

        public Dictionary<(string, string), List<string>> LinkOpportunities { get; set;} = new();
    }
}
