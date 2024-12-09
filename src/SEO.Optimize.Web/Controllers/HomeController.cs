using EnsureThat;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SEO.Optimize.Web.Services;
using System.Security.Claims;

namespace SEO.Optimize.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly AuthenticationHandlerService authenticationHandlerService;
        private readonly DashboardService dashboardService;
        private readonly AppStateCache appStateCache;

        public HomeController(
            AuthenticationHandlerService authenticationHandlerService, 
            DashboardService dashboardService, 
            AppStateCache appStateCache)
        {
            this.authenticationHandlerService = authenticationHandlerService;
            this.dashboardService = dashboardService;
            this.appStateCache = appStateCache;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Login()
        {
            return Challenge(new AuthenticationProperties { RedirectUri = "/Home/PostLogin" }, OpenIdConnectDefaults.AuthenticationScheme);
        }

        public async Task<IActionResult> PostLogin()
        {
            var identity = User.Identity as ClaimsIdentity;
            EnsureArg.IsNotNull(identity, nameof(identity));

            var sessionKey = identity.FindFirst("seopt_session")?.Value;
            await appStateCache.SetAsync(
                sessionKey, 
                new Dictionary<string, string>()
                {
                    {"session_expiry",DateTime.UtcNow.AddMinutes(2).ToString("yyyy-MM-dd HH:mm:ss") }
                });

            var userId = await authenticationHandlerService.EnsureUserExistsAsync(identity);
            return RedirectToAction("Dashboard");
        }

        public async Task<IActionResult> Logout()
        {
            var identity = User.Identity as ClaimsIdentity;
            EnsureArg.IsNotNull(identity, nameof(identity));

            var sessionKey = identity.FindFirst("seopt_session")?.Value;
            await appStateCache.ClearCache(sessionKey);

            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index");
        }

        [Authorize]
        public async Task<IActionResult > Dashboard()
        {
            var identity = User.Identity as ClaimsIdentity;
            var email = identity?.FindFirst(ClaimTypes.Email)?.Value;

            var dashboardData = await dashboardService.GetDashboardData(email);

            if (dashboardData == null || !dashboardData.Any())
            {
                // Redirect to the AddSite page
                return RedirectToAction("AddSite", "SiteManagement", new { mode = "Init"});
            }

            return View(dashboardData);
        }
    }
}
