using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace SEO.Optimize.Web.Pages.SiteManagement
{
    public class ViewDataModel : PageModel
    {
        public Dictionary<string, string> Dictionary { get; set; }

        public void OnGet()
        {
            // Example dictionary (replace with your logic)
            Dictionary = new Dictionary<string, string>
        {
            { "magnam ", "value1" },
            { "voluptatem ", "value2" },
            { "sapiente  ", "value2" }
        };
        }

        public IActionResult OnPostApply([FromForm] ApplyRequest data)
        {
            // Handle the Apply action (e.g., save to the database)
            return new JsonResult(new { message = $"Key '{data.Key}' with value '{data.Value}' applied successfully!" });
        }

        public void OnPost()
        {

        }
    }

    public class ApplyRequest
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }

}
