using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SEO.Optimize.Postgres.Model
{
    public class Job
    {
        [Key]
        public int Id { get; set; }

        public required string Name { get; set; }

        public int UserId { get; set; }

        public int SiteId { get; set; }

        public DateTime CreatedOn {  get; set; } = DateTime.UtcNow;

        public DateTime UpdatedOn {  get; set; } = DateTime.UtcNow;

        public DateTime? CompletedOn {  get; set; } = DateTime.UtcNow;

        public string JobProperties { get; set; } = string.Empty;
    }
}
