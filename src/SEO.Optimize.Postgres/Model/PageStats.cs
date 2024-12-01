using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SEO.Optimize.Postgres.Model
{
    public class PageStats
    {
        [Key]
        public int StatsId { get; set; }

        public int InternalLinkCount { get; set; }
        public int ExternalLinkCount { get; set; }

        public int LinkOpportunityCount { get; set; }

        // Foreign Key
        public int PageId { get; set; }

        public DateTime CreatedOn { get; set; }

        public DateTime UpdatedOn { get; set; }

        public PageInfo PageInfo { get; set; }
    }

}
