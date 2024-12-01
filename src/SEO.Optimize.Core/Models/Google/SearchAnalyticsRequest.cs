using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SEO.Optimize.Core.Models.Google
{
    public class SearchAnalyticsRequest
    {
        public string? StartDate { get; set; }
        public string? EndDate { get; set; }
        public List<string> Dimensions { get; set; }
        public int RowLimit { get; set; }
        public string Type { get; set; }

        // Convenience methods for setting dates
        public void SetStartDate(DateTime? date)
        {
            StartDate = date?.ToString("yyyy-MM-dd");
        }

        public void SetEndDate(DateTime? date)
        {
            EndDate = date?.ToString("yyyy-MM-dd");
        }
    }
}
