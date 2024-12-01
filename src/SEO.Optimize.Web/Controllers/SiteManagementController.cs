using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SEO.Optimize.Core.Models;
using SEO.Optimize.Core.Models.ProjectInfo;
using SEO.Optimize.Web.Services;

namespace SEO.Optimize.Web.Controllers
{
    [Authorize]
    public class SiteManagementController : Controller
    {
        private readonly SiteManagementService siteManagementService;

        public SiteManagementController(SiteManagementService siteManagementService)
        {
            this.siteManagementService = siteManagementService;
        }

        [HttpGet]
        public IActionResult AddSite(string mode)
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> ListPages()
        {
            var result = await siteManagementService.GetPages();
            var pages = new ListPagesViewModel()
            {
                Pages = result.ToList(),
                Sites = new List<string>() { "sdds", "sdsdsff"},
                PageTitle = new List<string>() { "wwere", "ghgh", "gfgf"},
            };

            return View(pages);
        }
    }
}
