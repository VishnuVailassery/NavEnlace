using SEO.Optimize.Core.Clients;
using SEO.Optimize.Core.Interfaces;
using SEO.Optimize.Core.Models.Opportunities;
using SEO.Optimize.Core.Models.Webflow;
using System.Text.RegularExpressions;
using static System.Net.Mime.MediaTypeNames;

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

        public async Task UpdateLinksAsync(IEnumerable<Opportunities> opportunities)
        {
            var siteGroups = opportunities.GroupBy(o => o.SiteId);
            var idsToPublish = new List<string>();
            foreach (var group in siteGroups)
            {
                var tokenInfos = await contentRepository.GetTokensBySiteId(group.Key);
                var accesstoken = tokenInfos.First(o => o.TokenType == "access_token").Token;

                var collectionGroups = group.GroupBy(o => o.ExternalPageId.Split("##")[0]);
                foreach (var collection in collectionGroups)
                {
                    var pageGroups = collection.GroupBy(o => o.ExternalPageId);
                    foreach( var page in pageGroups)
                    {
                        var path = group.ToList().First().ExternalPageId.Replace("##", "/items/");
                        var item = await webflowClient.GetCollectionItemAsync(path, accesstoken);
                        foreach (var opp in page)
                        {
                            string pattern = Regex.Escape(opp.AnchorTextContainingText); // Escape keyword for regex
                            string replacement = opp.AnchorTextContainingText
                                                    .Replace(opp.AnchorText,
                                                    $"<a href=\"{opp.TargetLinkUrl}\" style=\"display: inline;\">{opp.AnchorText}</a>",
                                                    StringComparison.OrdinalIgnoreCase);

                            //string replacement = $"<a href=\"{opp.TargetLinkUrl}\">{opp.AnchorText}</a>";
                            int maxReplacements = 1;

                            // Use a custom replacement method to limit occurrences
                            item.FieldData[opp.FieldKey] = ReplaceWithLimit(item.FieldData[opp.FieldKey].ToString(), pattern, replacement, maxReplacements);
                        }
                        idsToPublish.Add(item.Id);
                        await webflowClient.UpdateCollectionItemAsync(item, path, accesstoken);
                    }

                    var publishBody = new PublishItemsBody()
                    {
                        ItemIds = idsToPublish
                    };

                    await webflowClient.PublishCollectionItems(publishBody, collection.Key, accesstoken);
                }
            }

            //var anchorLink = opportunities.AnchorTextContainingText
            //    .Replace(opportunities.AnchorText,
            //    $"<a href=\"{opportunities.TargetLinkUrl}\" style=\"display: inline;\">{opportunities.AnchorText}</a>",
            //    StringComparison.OrdinalIgnoreCase);

            //item.FieldData.PostBody = item.FieldData.PostBody.Replace(opportunities.AnchorTextContainingText, anchorLink);

            //LinkifyText(item.FieldData.PostBody,);
        }

        static string ReplaceWithLimit(string input, string pattern, string replacement, int maxReplacements)
        {
            int replacementCount = 0;
            return Regex.Replace(input, pattern, match =>
            {
                if (replacementCount < maxReplacements)
                {
                    replacementCount++;
                    return replacement;
                }
                return match.Value; // Return original text if limit is reached
            });
        }
    }
}
