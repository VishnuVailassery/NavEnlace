using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SEO.Optimize.Core.Models.Opportunities;
using SEO.Optimize.Web.Services;

namespace SEO.Optimize.Web.Pages.LinkManagement
{
    public class LinkDetailsModel : PageModel
    {
        private readonly SiteManagementService siteManagementService;
        private readonly ActionHandlerService actionHandlerService;

        [BindProperty]
        public List<Opportunities> Links { get; set; } = new List<Opportunities>();

        public LinkDetailsModel(SiteManagementService siteManagementService, ActionHandlerService actionHandlerService)
        {
            this.siteManagementService = siteManagementService;
            this.actionHandlerService = actionHandlerService;
        }

        public async Task OnGetAsync()
        {
            var links = await siteManagementService.GetOpportunities();
            Links = links.ToList();
        }

        public async Task<IActionResult> OnPostApply(int id)
        {
            var links = (await siteManagementService.GetOpportunitiesByIds(new List<int>() { id })).ToList();
            var opportunity = links.Where(x => x.LinkId == id);

            await actionHandlerService.UpdateLinksAsync(opportunity);
            return RedirectToPage();
        }

        public IActionResult OnPostIgnore(int id)
        {
            // Logic to handle the "Ignore" action for the link with the given ID
            // Example: Mark the link as ignored in the database
            TempData["Message"] = $"Link with ID {id} has been ignored.";
            return RedirectToPage();
        }
    }

    public class LinkDetail
    {
        public int Id { get; set; }
        public string SourcePage { get; set; }
        public string AnchorText { get; set; }
        public string TargetPage { get; set; }
    }
}
