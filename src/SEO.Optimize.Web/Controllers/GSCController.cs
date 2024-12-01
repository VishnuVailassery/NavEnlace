namespace SEO.Optimize.Web.Controllers
{
    using global::SEO.Optimize.Web.Helpers;
    using global::SEO.Optimize.Web.Services;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using System.Security.Claims;

    namespace SEO.Optimize.Web.Controllers
    {
        [Route("api/[controller]")]
        [ApiController]
        [Authorize]
        public class GSCController : ControllerBase
        {
            private readonly GSCService gSCService;

            public GSCController(GSCService gSCService)
            {
                this.gSCService = gSCService;
            }

            [HttpGet]
            public async Task<IActionResult> Authorize()
            {
                var identity = User.Identity as ClaimsIdentity;
                var authorizationUrl = await gSCService.GoogleAuthorizationRequestUrl(identity);

                return Redirect(authorizationUrl);
            }


            [HttpGet("callback")]
            public async Task<IActionResult> Callback(string code, string state)
            {
                var identity = User.Identity as ClaimsIdentity;
                var tokenResponse = await gSCService.ExchangeCodeAndStoreTokens(code, state, identity);
                return RedirectToAction("Dashboard", "Home");
            }
        }
    }

}
