using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SEO.Optimize.Core.Models.Opportunities
{
    public class LinkProperties
    {
        public string AnchorText { get; set; }

        public string AnchorTextContainingLine { get; set; }

        public string NodeXPath { get; set; }

        public int NumberOfOccurrence {  get; set; }

        public string FieldKey { get; set; }

        public string TargetUrl { get; set; }
    }
}
