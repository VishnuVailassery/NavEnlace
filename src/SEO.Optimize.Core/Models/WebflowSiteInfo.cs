using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SEO.Optimize.Core.Models
{
    public class WebflowSiteInfo
    {
        public int UserId { get; set; }
        public List<WebflowSite> Sites { get; set; }
    }

    public class WebflowSite
    {
        public string Id { get; set; }
        public string InternalSiteId { get; set; }
        public string DisplayName { get; set; }
        public string ShortName { get; set; }
        public DateTime CreatedOn { get; set; }
        public List<CustomDomain> CustomDomains { get; set; }
    }

    public class CustomDomain
    {
        public string Id { get; set; }
        public string Url { get; set; }
    }
}
