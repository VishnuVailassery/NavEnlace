using Microsoft.AspNetCore.Mvc;

[Route("api/external-content")]
[ApiController]
public class ExternalContentController : ControllerBase
{
    [HttpGet]
    public IActionResult GetExternalContent()
    {
        // Example: Fetch content from an external URL
        string url = "https://thesample-e155a7.webflow.io/post/10-quick-tips-about-blogging";
        using var client = new HttpClient();
        var html = client.GetStringAsync(url).Result;
        return Content(html, "text/html");
    }

    //[HttpGet]
    //public IActionResult GetExternalContent()
    //{
    //    // Your logic here
    //    return Content("<html>Your external HTML here</html>", "text/html");
    //}
}
