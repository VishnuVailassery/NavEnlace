using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SEO.Optimize.Core.Models.Opportunities
{
    public class Opportunities
    {
        public int LinkId { get; set; }

        public int PageId { get; set; }
        public int SiteId { get; set; }
        public string ExternalPageId { get; set; }

        public string TargetLinkUrl { get; set; }

        public string AnchorText { get; set; }
        public string AnchorTextContainingText { get; set; }

        public bool IsOpportunity { get; set; }

        public string PageUrl { get; set; }

        public bool IsApplied { get; set; }
    }
}
