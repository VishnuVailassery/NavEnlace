namespace SEO.Optimize.Postgres.Model
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class LinkDetail
    {
        [Key]
        public int LinkId { get; set; }

        [Required]
        [ForeignKey("PageInfo")]
        public int PageId { get; set; }

        [Required, Url]
        public string LinkUrl { get; set; }

        [Required]
        [MaxLength(20)]
        public string LinkType { get; set; } // Internal or External

        public string AnchorText { get; set; }

        public string AnchorTextContainingText { get; set; }

        [Required]
        public bool IsOpportunity { get; set; }

        public virtual PageInfo PageInfo { get; set; }
    }

}
