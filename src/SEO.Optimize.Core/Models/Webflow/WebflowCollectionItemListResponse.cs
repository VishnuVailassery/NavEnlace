using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SEO.Optimize.Core.Models.Webflow
{
    public class WebflowCollectionItemListResponse
    {
        public IEnumerable<CollectionItem> Items { get; set; }
    }


    public class CollectionItem
    {
        public string CollectionId { get; set; }
        public string Id { get; set; }
        public string CmsLocaleId { get; set; }
        public DateTime LastPublished { get; set; }
        public DateTime LastUpdated { get; set; }
        public DateTime CreatedOn { get; set; }
        public bool IsArchived { get; set; }
        public bool IsDraft { get; set; }
        public Dictionary<string, object> FieldData { get; set; } = new Dictionary<string, object>();
    }

    public class FieldData
    {
        public string Color { get; set; }
        public bool Featured { get; set; }
        public string Name { get; set; }

        [JsonProperty("post-body")]
        public string PostBody { get; set; }

        [JsonProperty("post-summary")]
        public string PostSummary { get; set; }
        public string Slug { get; set; }

        [JsonProperty("main-image")]
        public Image MainImage { get; set; }

        [JsonProperty("thumbnail-image")]
        public Image ThumbnailImage { get; set; }
    }

    public class Image
    {
        public string FileId { get; set; }
        public string Url { get; set; }
        public string Alt { get; set; }
    }

}
