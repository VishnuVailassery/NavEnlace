namespace SEO.Optimize.Core.Models
{
    public class UserAuthInfo
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string ExternalIdentity { get; set; }
        public required string MailId { get; set; }
        public required string ProfileImageUrl { get; set; }
        public required string Provider { get; set; }
    }
}
