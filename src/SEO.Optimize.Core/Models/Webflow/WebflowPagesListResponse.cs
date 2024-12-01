using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SEO.Optimize.Core.Models.Webflow
{
    public class WebflowPagesListResponse
    {
        public IEnumerable<WebflowPage> Pages { get; set; }
    }

    public class WebflowPage
    {
        public string Id { get; set; }
        public string SiteId { get; set; }
        public string Slug { get; set; }
        public string Title { get; set; }
        public string CollectionId { get; set; }
        public string PublishedPath { get; set; }
        public PageSeo Seo { get; set; }
    }

    public class PageSeo
    {
        public string Title { get; set; }
        public string Description { get; set; }
    }
}
