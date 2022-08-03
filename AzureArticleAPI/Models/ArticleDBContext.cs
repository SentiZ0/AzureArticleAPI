using Microsoft.EntityFrameworkCore;


namespace AzureArticleAPI.Models
{
    public class ArticleDBContext : DbContext
    {
        public ArticleDBContext (DbContextOptions options) : base(options)
        {

        }

        public DbSet<Article> Articles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Article>()
                .Property(b => b.ImagePath)
                .IsRequired(false);
        }
    }
}
