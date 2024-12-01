using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SEO.Optimize.Core.Interfaces;
using SEO.Optimize.Postgres.Repository;
using SEO.Optimize.Web.Helpers;
using SEO.Optimize.Web.Services;
using System.Security.Claims;

namespace SEO.Optimize.Web.Controllers
{
    [Authorize]
    public class IntegrationController : Controller
    {
        private readonly IntegrationService authorizationService;
        private readonly IContentRepository contentRepository;

        public IntegrationController(IntegrationService authorizationService, IContentRepository contentRepository)
        {
            this.authorizationService = authorizationService;
            this.contentRepository = contentRepository;
        }

        [HttpGet]
        public IActionResult AuthorizeWordpress()
        {
            return View();
        }

        [HttpGet]
        public IActionResult AuthorizeWebFlow()
        {
            return Redirect(authorizationService.AuthorizeWithWebflow());
        }

        [HttpGet]
        public async Task<IActionResult> Callback(string code)
        {
            var identity = User.Identity as ClaimsIdentity;

            var tokenResponse = await authorizationService.ExchangeCodeForToken(code, identity);

            var json = JsonConvert.SerializeObject(tokenResponse);
            TempData["TokenInfo"] = json; ;

            return RedirectToAction("SelectSite", "SiteManagement");
        }

        [HttpGet]
        public IActionResult LinkGSC(string mode)
        {
            return View(mode);
        }
    }
}
