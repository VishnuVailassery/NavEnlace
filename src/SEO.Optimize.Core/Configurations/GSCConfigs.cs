namespace SEO.Optimize.Core.Configurations
{
    public class GSCConfigs
    {
        public required string ClientId { get; set; }

        public required string ClientSecret { get; set; }

        public IEnumerable<string> RedirectUrls { get; set; } = new List<string>();
    }
}
