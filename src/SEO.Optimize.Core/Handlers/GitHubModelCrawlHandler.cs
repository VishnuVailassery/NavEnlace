using AngleSharp;
using HtmlAgilityPack;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using OpenAI;
using SEO.Optimize.Core.Interfaces;
using SEO.Optimize.Core.Models.Jobs;
using System.ClientModel;

namespace SEO.Optimize.Core.Handlers
{
    public class GitHubModelCrawlHandler : IPageCrawlModelHandler
    {
        public GitHubModelCrawlHandler()
        {
        }

        public async Task<CrawlPageOutput> CrawlContentAndFindLinks(string content, List<(string, string)> keywords)
        {
            var links = await ExtractLinksAsync(content);
            var opportunities = FindKeywordsInHtml(content, keywords);

            var output = new CrawlPageOutput()
            {
                ExistingLinks = links,
                LinkOpportunities = opportunities
                                    .Select(a => new KeyValuePair<(string, string), List<string>>(a.Key, a.Value.Select(o => o.InnerText).ToList()))
                                    .ToDictionary()
            };

            return output;
        }

        public async Task<Dictionary<string, string>> ExtractLinksAsync(string htmlContent)
        {
            var links = new Dictionary<string, string>();
            var context = BrowsingContext.New(Configuration.Default);
            var document = await context.OpenAsync(req => req.Content(htmlContent));
            var anchorElements = document.QuerySelectorAll("a[href]");

            foreach (var element in anchorElements)
            {
                var url = element.GetAttribute("href");
                var anchorText = element.TextContent;
                links.Add(anchorText, url);
            }

            return links;
        }

        public static Dictionary<(string, string), List<HtmlNode>> FindKeywordsInHtml(string htmlContent, List<(string, string)> keywords)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(htmlContent);

            // Dictionary to store the results
            var keywordNodeMap = new Dictionary<(string, string), List<HtmlNode>>();

            // Initialize the dictionary with empty lists
            foreach (var keyword in keywords)
            {
                keywordNodeMap[keyword] = new List<HtmlNode>();
            }

            // Traverse all text nodes in the document
            foreach (var node in doc.DocumentNode.SelectNodes("//text()"))
            {
                foreach (var keyword in keywords)
                {
                    if (!node.ChildNodes.Any() && node.InnerText.IndexOf(keyword.Item1, StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        keywordNodeMap[keyword].Add(node);
                    }
                }
            }

            return keywordNodeMap;
        }
    }
}
