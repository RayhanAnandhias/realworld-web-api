using Microsoft.EntityFrameworkCore;
using realworld_api.Models;

namespace realworld_api.Database
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        public DbSet<User> user { get; set; }
        public DbSet<ProfileFollowing> profile_following { get; set; }
        public DbSet<Article> article { get; set; }
        public DbSet<UserFavorite> user_favorite { get; set; }
        public DbSet<Comment> comment { get; set; }
        public DbSet<Tag> tag { get; set; }
        public DbSet<ArticleTag> article_tag { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ProfileFollowing>()
                .HasKey(u => new { u.id_user_observer, u.id_user_target });

            modelBuilder.Entity<User>()
                .HasMany(u => u.followers)
                .WithOne(f => f.user_target)
                .HasForeignKey(f => f.id_user_target)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<User>()
                .HasMany(u => u.following)
                .WithOne(f => f.user_observer)
                .HasForeignKey(f => f.id_user_observer)
                .OnDelete(DeleteBehavior.Restrict);

            //====================================================================

            modelBuilder.Entity<Article>()
                .HasOne(u => u.author)
                .WithMany(a => a.articles)
                .HasForeignKey(u => u.id_author)
                .OnDelete(DeleteBehavior.SetNull);

            //====================================================================

            modelBuilder.Entity<UserFavorite>()
                .HasKey(u => new { u.id_user, u.id_article_favorite });

            modelBuilder.Entity<User>()
                .HasMany(uf => uf.user_favorite)
                .WithOne(u => u.user)
                .HasForeignKey(f => f.id_user)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Article>()
                .HasMany(uf => uf.user_favorite)
                .WithOne(a => a.article_favorited)
                .HasForeignKey(f => f.id_article_favorite)
                .OnDelete(DeleteBehavior.Cascade);

            //====================================================================

            modelBuilder.Entity<Comment>()
                .HasOne(a => a.article)
                .WithMany(c => c.comments)
                .HasForeignKey(f => f.id_article)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Comment>()
                .HasOne(u => u.user)
                .WithMany(c => c.comments)
                .HasForeignKey(f => f.id_user)
                .OnDelete(DeleteBehavior.Cascade);

            //====================================================================

            modelBuilder.Entity<ArticleTag>()
                .HasKey(at => new { at.id_article, at.id_tag });

            modelBuilder.Entity<Article>()
                .HasMany(at => at.tags)
                .WithOne(a => a.article)
                .HasForeignKey(f => f.id_article)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Tag>()
                .HasMany(at => at.articles)
                .WithOne(t => t.tag)
                .HasForeignKey(f => f.id_tag)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
