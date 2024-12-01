using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SEO.Optimize.Core.Models.Google
{
    public class GscSitesResponse
    {
        public List<GscSite> SiteEntry { get; set; }
    }

    public class GscSite
    {
        public string SiteUrl { get; set; }
        public string PermissionLevel { get; set; }
    }
}
