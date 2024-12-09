namespace SEO.Optimize.Core.Models.Jobs
{
    public class JobInfo
    {
        public int Id { get; set; }

        public required string Name { get; set; }

        public int UserId { get; set; }

        public int SiteId { get; set; }

        public required string Status { get; set; }

        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedOn { get; set; } = DateTime.UtcNow;

        public DateTime? CompletedOn { get; set; } = DateTime.UtcNow;

        public string JobProperties { get; set; } = string.Empty;
    }
}
