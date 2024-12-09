using AngleSharp;
using HtmlAgilityPack;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using OpenAI;
using SEO.Optimize.Core.Interfaces;
using SEO.Optimize.Core.Models.Jobs;
using SEO.Optimize.Core.Models.Opportunities;
using System.ClientModel;

namespace SEO.Optimize.Core.Handlers
{
    public class GitHubModelCrawlHandler : IPageCrawlModelHandler
    {
        public GitHubModelCrawlHandler()
        {
        }

        public async Task<CrawlPageOutput> CrawlContentAndFindLinks(string fieldKey, string content, List<(string, string)> keywords)
        {
            try
            {
                var links = await ExtractLinksAsync(fieldKey, content);
                var opportunities = FindKeywordsInHtml(fieldKey, content, keywords);

                var output = new CrawlPageOutput()
                {
                    ExistingLinks = links,
                    LinkOpportunities = opportunities
                };

                return output;
            }
            catch
            {
                return null;
            }
        }

        public async Task<List<LinkProperties>> ExtractLinksAsync(string fieldKey, string htmlContent)
        {
            var links = new List<LinkProperties>();
            var context = BrowsingContext.New(Configuration.Default);
            var document = await context.OpenAsync(req => req.Content(htmlContent));
            var anchorElements = document.QuerySelectorAll("a[href]");

            foreach (var element in anchorElements)
            {
                var url = element.GetAttribute("href");
                var linkProps = new LinkProperties()
                {
                    AnchorText = element.TextContent,
                    TargetUrl = url,
                    AnchorTextContainingLine = element.TextContent,
                    FieldKey = fieldKey,
                    NodeXPath = element.ParentElement.TextContent,
                    NumberOfOccurrence = 1
                };

                links.Add(linkProps);
            }

            return links;
        }

        public static List<LinkProperties> FindKeywordsInHtml(string fieldKey, string htmlContent, List<(string, string)> keywords)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(htmlContent);

            // Dictionary to store the results
            var keywordNodeMap = new Dictionary<(string, string), List<HtmlNode>>();
            var matches = new List<LinkProperties>();

            // Traverse all text nodes in the document
            foreach (var node in doc.DocumentNode.SelectNodes("//text()"))   
            {
                foreach (var keyword in keywords)
                {
                    if (!node.ChildNodes.Any() && node.InnerText.IndexOf(keyword.Item1, StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        var linkProps = new LinkProperties()
                        {
                            AnchorText = keyword.Item1,
                            AnchorTextContainingLine = node.InnerText,
                            TargetUrl = keyword.Item2,
                            FieldKey = fieldKey,
                            NodeXPath = node.XPath,
                            NumberOfOccurrence = 1
                        };

                        matches.Add(linkProps);
                    }
                }
            }

            return matches;
        }
    }
}
