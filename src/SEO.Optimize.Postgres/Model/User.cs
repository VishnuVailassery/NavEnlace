using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SEO.Optimize.Postgres.Model
{
    using Microsoft.EntityFrameworkCore;
    using Newtonsoft.Json.Linq;
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class User
    {
        [Key]
        public int UserId { get; set; }

        // External Authentication Information
        public string ExternalId { get; set; } // ID from the external provider
        public string Provider { get; set; } // e.g., "Google", "Facebook", etc.

        [Required, EmailAddress]
        public string Email { get; set; }

        [Required]
        public string PasswordHash { get; set; }

        public string UserName { get; set; }

        public string ProfileImageUrl { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsActive { get; set; } = true;


        public virtual ICollection<Site> Sites { get; set; }
    }
}
