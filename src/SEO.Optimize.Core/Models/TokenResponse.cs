namespace SEO.Optimize.Core.Models
{
    public class TokenResponse
    {
        public string refresh_token { get; set; }
        public string access_token { get; set; }
        public string token_type { get; set; }
        public string scope { get; set; }
    }
}
