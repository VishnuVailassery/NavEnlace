using Azure.Core;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using SEO.Optimize.Core.Models;
using SEO.Optimize.Core.Models.Opportunities;
using SEO.Optimize.Core.Models.Webflow;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SEO.Optimize.Core.Clients
{
    public class WebflowClient
    {
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly HttpClient httpClient;

        private readonly JsonSerializerSettings settings = new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            Formatting = Formatting.Indented
        };

        const string BASE_WEBFLOW_URL = "https://api.webflow.com/v2";

        public WebflowClient(IHttpClientFactory httpClientFactory)
        {
            httpClient = httpClientFactory.CreateClient();
        }


        public async Task<WebflowCMSCollectionList> GetAllCMSCollections(string siteId, string accessToken)
        {
            var pagesUrl = $"{BASE_WEBFLOW_URL}/sites/{siteId}/collections";
            var request = new HttpRequestMessage(HttpMethod.Get, pagesUrl);
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
            var responseMessage = await httpClient.SendAsync(request);
            responseMessage.EnsureSuccessStatusCode();
            var resStr = await responseMessage.Content.ReadAsStringAsync();
            var cmsCollectionList = JsonConvert.DeserializeObject<WebflowCMSCollectionList>(resStr);
            return cmsCollectionList;
        }

        public async Task<WebflowCollectionItemListResponse> GetCMSCollectionItems(string collectionId, string accessToken)
        {
            var pagesUrl = $"{BASE_WEBFLOW_URL}/collections/{collectionId}/items";
            var request = new HttpRequestMessage(HttpMethod.Get, pagesUrl);
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
            var responseMessage = await httpClient.SendAsync(request);
            responseMessage.EnsureSuccessStatusCode();
            var resStr = await responseMessage.Content.ReadAsStringAsync();
            var collectionItems = JsonConvert.DeserializeObject<WebflowCollectionItemListResponse>(resStr);
            return collectionItems;
        }

        public async Task<CollectionItem> GetCollectionItemAsync(string idPath, string accessToken)
        {
            //var path = item.ExternalPageId.Replace("##", "items/");
            var pagesUrl = $"{BASE_WEBFLOW_URL}/collections/{idPath}";
            var request = new HttpRequestMessage(HttpMethod.Get, pagesUrl);
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
            var responseMessage = await httpClient.SendAsync(request);
            responseMessage.EnsureSuccessStatusCode();
            var resStr = await responseMessage.Content.ReadAsStringAsync();
            var collectionItems = JsonConvert.DeserializeObject<CollectionItem>(resStr);
            return collectionItems;
        }

        public async Task UpdateCollectionItemAsync(CollectionItem collectionItem, string idPath, string accessToken)
        {
            var pagesUrl = $"{BASE_WEBFLOW_URL}/collections/{idPath}";
            var request = new HttpRequestMessage(HttpMethod.Patch, pagesUrl);
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
            request.Content = new StringContent(JsonConvert.SerializeObject(collectionItem, settings), Encoding.UTF8, "application/json");
            var responseMessage = await httpClient.SendAsync(request);
            responseMessage.EnsureSuccessStatusCode();
        }

        public async Task PublishCollectionItems(PublishItemsBody publishItemsBody, string collectionId, string accessToken)
        {
            var pagesUrl = $"{BASE_WEBFLOW_URL}/collections/{collectionId}/items/publish";
            var request = new HttpRequestMessage(HttpMethod.Post, pagesUrl);
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

            var body = JsonConvert.SerializeObject(publishItemsBody, settings);
            request.Content = new StringContent(body, Encoding.UTF8, "application/json");
            var strin = await request.Content.ReadAsStringAsync();
            var responseMessage = await httpClient.SendAsync(request);
            responseMessage.EnsureSuccessStatusCode();
        }
    }
}
