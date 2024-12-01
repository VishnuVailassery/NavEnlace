using Microsoft.EntityFrameworkCore;
using SEO.Optimize.Postgres.Model;

namespace SEO.Optimize.Postgres.Context
{
    public class DataContext : DbContext
    {
        public DataContext()
        {

        }

        public DataContext(DbContextOptions<DataContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Site> Sites { get; set; }
        public DbSet<Token> Tokens { get; set; }
        public DbSet<CrawlInfo> CrawlInfos { get; set; }
        public DbSet<PageInfo> PageInfos { get; set; }
        public DbSet<PageStats> PageStats { get; set; }
        public DbSet<LinkDetail> LinkDetails { get; set; }
        public DbSet<Job> Jobs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // User Configuration
            modelBuilder.Entity<User>()
                .HasKey(u => u.UserId);

            modelBuilder.Entity<User>()
                .HasMany(u => u.Sites)
                .WithOne(s => s.User)
                .HasForeignKey(s => s.UserId);

            // Site Configuration
            modelBuilder.Entity<Site>()
                .HasKey(s => s.SiteId);

            modelBuilder.Entity<Site>()
                .HasOne(s => s.User)
                .WithMany(u => u.Sites)
                .HasForeignKey(s => s.UserId);

            modelBuilder.Entity<Site>()
                .HasMany(s => s.Tokens)
                .WithOne(t => t.Site)
                .HasForeignKey(t => t.SiteId);

            modelBuilder.Entity<Site>()
                .HasMany(s => s.CrawlInfos)
                .WithOne(c => c.Site)
                .HasForeignKey(c => c.SiteId);

            // PageInfo Configuration
            modelBuilder.Entity<PageInfo>()
                .HasKey(p => p.PageId);

            modelBuilder.Entity<PageInfo>()
            .HasOne(p => p.Site)
            .WithMany(s => s.PageInfos)
            .HasForeignKey(p => p.SiteId);

            modelBuilder.Entity<PageInfo>()
                .HasOne(p => p.PageStats)
                .WithOne(ps => ps.PageInfo)
                .HasForeignKey<PageStats>(ps => ps.PageId);

            modelBuilder.Entity<PageInfo>()
                .HasOne(p => p.CrawlInfo)
                .WithMany(c => c.PageInfos)
                .HasForeignKey(p => p.CrawlId); // assumes the same CrawlId for each Site

            // PageStats Configuration
            modelBuilder.Entity<PageStats>()
                .HasKey(ps => ps.StatsId);

            modelBuilder.Entity<PageStats>()
                .HasOne(ps => ps.PageInfo)
                .WithOne(pi => pi.PageStats)
                .HasForeignKey<PageStats>(ps => ps.PageId);

            // LinkDetail Configuration
            modelBuilder.Entity<LinkDetail>()
                .HasKey(ld => ld.LinkId);

            modelBuilder.Entity<LinkDetail>()
                .HasOne(ld => ld.PageInfo)
                .WithMany(pi => pi.LinkDetails)
                .HasForeignKey(ld => ld.PageId);

            // CrawlInfo Configuration
            modelBuilder.Entity<CrawlInfo>()
                .HasKey(c => c.CrawlId);

            modelBuilder.Entity<CrawlInfo>()
                .HasOne(c => c.Site)
                .WithMany(s => s.CrawlInfos)
                .HasForeignKey(c => c.SiteId);

            // Token Configuration
            modelBuilder.Entity<Token>()
                .HasKey(t => t.TokenId);

            modelBuilder.Entity<Token>()
                .HasOne(t => t.Site)
                .WithMany(s => s.Tokens)
                .HasForeignKey(t => t.SiteId);

            base.OnModelCreating(modelBuilder);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=seoptdb;Username=postgres;Password=!QAZ2wsx;");
            }
        }
    }
}
