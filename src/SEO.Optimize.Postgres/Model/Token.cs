namespace SEO.Optimize.Postgres.Model
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class Token
    {
        [Key]
        public int TokenId { get; set; }

        [Required]
        [ForeignKey("Site")]
        public int SiteId { get; set; }

        [Required]
        public string TokenType { get; set; }
        
        [Required]
        public string TokenValue { get; set; }

        public DateTime? TokenExpiresAt { get; set; }
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedOn { get; set; } = DateTime.UtcNow;

        public virtual Site Site { get; set; }
    }
}
