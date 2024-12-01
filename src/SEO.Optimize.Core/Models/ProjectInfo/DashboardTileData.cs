using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SEO.Optimize.Core.Models.ProjectInfo
{
    public class DashboardTileData
    {
        public DashboardTileData(DetailedSiteInfo detailedSiteInfo)
        {
            this.SiteInfo = detailedSiteInfo;
        }

        public DetailedSiteInfo SiteInfo { get; set; }
    }
}
