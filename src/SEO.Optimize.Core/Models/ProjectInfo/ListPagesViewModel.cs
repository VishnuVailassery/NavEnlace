using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SEO.Optimize.Core.Models.ProjectInfo
{
    public class ListPagesViewModel
    {
        public List<string> Sites { get; set; }
        public List<string> PageTitle { get; set; }
        public List<PageInfo> Pages { get; set; }
    }

    public class PageViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Summary { get; set; }
        public string Content { get; set; }
        public string Url { get; set; }
    }
}
