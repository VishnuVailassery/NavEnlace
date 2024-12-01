using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using SEO.Optimize.Core.Models;
using SEO.Optimize.Postgres.Model;
using SEO.Optimize.Web.Services;
using System.Security.Claims;

namespace SEO.Optimize.Web.Pages.SiteManagement
{
    public class SelectSiteModel : PageModel
    {
        private readonly SiteManagementService siteManagementService;
        private readonly AppStateCache appStateCache;

        public SelectSiteModel(SiteManagementService siteManagementService, AppStateCache appStateCache)
        {
            this.siteManagementService = siteManagementService;
            this.appStateCache = appStateCache;
        }

        public List<WebflowSite> AvailableSites { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            // Use the sites list passed from the previous action
            var tokenRes = TempData["TokenInfo"]?.ToString();
            TempData["TokenInfo"] = tokenRes;
            if (!string.IsNullOrEmpty(tokenRes))
            {
                var token = JsonConvert.DeserializeObject<TokenResponse>(tokenRes);
                var sites = await siteManagementService.GetAllAuthorizedSites(token);
                AvailableSites = sites.ToList();
                return Page();
            }
            else
            {
                throw new ArgumentException("no token res");
            }
        }

        public async Task<IActionResult> OnPostAsync(string selectedSiteId)
        {
            var tokenRes = TempData["TokenInfo"]?.ToString();
            if (string.IsNullOrWhiteSpace(tokenRes))
            {
                return RedirectToAction("Error");
            }

            var identity = User.Identity as ClaimsIdentity;
            var email = identity?.FindFirst(ClaimTypes.Email)?.Value;

            var token = JsonConvert.DeserializeObject<TokenResponse>(tokenRes);
            var siteId = await siteManagementService.SaveSiteAndTokenAsync(email, selectedSiteId, token);

            var sessionKey = identity.FindFirst("seopt_session")?.Value;
            await appStateCache.UpdateAsync(sessionKey, new Dictionary<string, string>()
            { 
                { "site_link_inprogress", bool.TrueString},
                { "site_id", siteId.ToString() },
            });

            // Redirect to the dashboard
            return RedirectToAction("LinkGSC", "Integration");
        }
    }

}
