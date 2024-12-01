using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SEO.Optimize.Core.Models.Google
{
    public class SearchAnalyticsResponse
    {
        public IEnumerable<SearchAnalyticsResultRow> Rows { get; set; }
    }

    public class SearchAnalyticsResultRow
    {
        public IEnumerable<string> Keys { get; set; }
        public double Clicks { get; set; }
        public double Impressions { get; set; }
        public double Ctr {  get; set; }
        public double Position {  get; set; }
    }
}
