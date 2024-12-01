namespace SEO.Optimize.Postgres.Model
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class UserProfile
    {
        [Key, ForeignKey("User")]
        public int UserId { get; set; }

        public string ProfileImage { get; set; }
        [MaxLength(200)]
        public string FullName { get; set; }
        public DateTime? LastLoginAt { get; set; }

        public virtual User User { get; set; }
    }
}
