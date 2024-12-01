using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SEO.Optimize.Core.Models.Webflow
{
    public class WebflowCMSCollection
    {
        public string Id { get; set; }
        public string DisplayName { get; set; }
        public string SingularName { get; set; }
        public string Slug { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime LastUpdated { get; set; }
    }

    public class WebflowCMSCollectionList
    {
        public IEnumerable<WebflowCMSCollection> Collections { get; set; }
    }
}
