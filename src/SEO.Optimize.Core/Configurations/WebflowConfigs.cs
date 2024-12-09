using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SEO.Optimize.Core.Configurations
{
    public class WebflowConfigs
    {
        public required string ClientId { get; set; }

        public required string ClientSecret { get; set; }

        public IEnumerable<string> RedirectUrls { get; set; } = new List<string>();
    }
}
