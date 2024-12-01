//using Google.Apis.Auth.OAuth2;
//using Google.Apis.SearchConsole.v1;
//using Google.Apis.SearchConsole.v1.Data;
//using Google.Apis.Services;
//using Google.Apis.Util.Store;
//using Microsoft.AspNetCore.Mvc;
//using SEO.Optimize.Web.Helpers;

//namespace SEO.Optimize.Web.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class SearchConsoleController : ControllerBase
//    {
//        [HttpGet("getSites")]
//        public async Task<IActionResult> GetSites()
//        {
//            var fileManger = new TokenFileManager("token.json");

//            var credential = GoogleCredential.FromAccessToken((await fileManger.ReadTokenFromFileAsync()).AccessToken);

//            var service = new SearchConsoleService(new BaseClientService.Initializer
//            {
//                HttpClientInitializer = credential,
//                ApplicationName = "YourAppName", // Replace with your app name
//            });

//            // Make the request to get the list of verified sites
//            var siteListRequest = service.Sites.List();
//            var sites = await siteListRequest.ExecuteAsync();

//            return Ok(sites);
//        }

//        [HttpGet("site-statistics")]
//        public async Task<IActionResult> GetSiteStatistics([FromQuery] string siteUrl, [FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
//        {
//            var credential = GoogleCredential.FromAccessToken((await fileManger.ReadTokenFromFileAsync()).AccessToken);

//            var service = new SearchConsoleService(new BaseClientService.Initializer
//            {
//                HttpClientInitializer = credential,
//                ApplicationName = "YourAppName", // Replace with your app name
//            });
//            var request = service.Searchanalytics.Query(new SearchAnalyticsQueryRequest
//            {
//                StartDate = startDate.ToString("yyyy-MM-dd"),
//                EndDate = endDate.ToString("yyyy-MM-dd"),
//                Dimensions = new List<string> { "page", "query" },
//                RowLimit = 5000
//            }, siteUrl);

//            var statistics = request.Execute();
//            var result = statistics.Rows.Select(row => new
//            {
//                Page = row.Keys[0],
//                Query = row.Keys[1],
//                Clicks = row.Clicks,
//                Impressions = row.Impressions,
//                CTR = row.Ctr,
//                Position = row.Position
//            });

//            return Ok(result);
//        }
//    }
//}
