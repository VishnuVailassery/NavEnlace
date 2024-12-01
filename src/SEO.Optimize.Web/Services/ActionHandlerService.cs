using SEO.Optimize.Core.Clients;
using SEO.Optimize.Core.Interfaces;
using SEO.Optimize.Core.Models.Opportunities;
using SEO.Optimize.Core.Models.Webflow;

namespace SEO.Optimize.Web.Services
{
    public class ActionHandlerService
    {
        private readonly WebflowClient webflowClient;
        private readonly IContentRepository contentRepository;

        public ActionHandlerService(WebflowClient webflowClient, IContentRepository contentRepository)
        { 
            this.webflowClient = webflowClient;
            this.contentRepository = contentRepository;
        }

        public async Task UpdateLinksAsync(Opportunities opportunities)
        {
            var path = opportunities.ExternalPageId.Replace("##", "/items/");
            var tokenInfos = await contentRepository.GetTokensBySiteId(opportunities.SiteId);
            var accesstoken = tokenInfos.First(o => o.TokenType == "access_token").Token;
            var item = await webflowClient.GetCollectionItemAsync(path, accesstoken);

            var anchorLink = opportunities.AnchorTextContainingText
                .Replace(opportunities.AnchorText, 
                $"<a href=\"{opportunities.TargetLinkUrl}\" style=\"display: inline;\">{opportunities.AnchorText}</a>", 
                StringComparison.OrdinalIgnoreCase);

            item.FieldData.PostBody = item.FieldData.PostBody.Replace(opportunities.AnchorTextContainingText, anchorLink);

            await webflowClient.UpdateCollectionItemAsync(item, path, accesstoken);

            var publishBody = new PublishItemsBody()
            {
                ItemIds = new List<string>() { item.Id }
            };
            await webflowClient.PublishCollectionItems(publishBody, opportunities.ExternalPageId.Split("##")[0], accesstoken);
        }
    }
}
