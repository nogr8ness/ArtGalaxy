using ArtWebsite.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace ArtWebsite.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }

        public DbSet<Artwork> Artworks { get; set; }

        public DbSet<Story> Stories { get; set; }

        public DbSet<Comment> Comments { get; set; }

        public DbSet<Like> Likes { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Artwork>()
                .HasOne(e => e.User)
                .WithMany(e => e.Artworks);

            modelBuilder.Entity<Story>()
                .HasOne(e => e.User)
                .WithMany(e => e.Stories);


            modelBuilder.Entity<Comment>()
                .HasOne(e => e.User)
                .WithMany(e => e.Comments);


            modelBuilder.Entity<Like>()
                .HasOne(l => l.Artwork)
                .WithMany()
                .HasForeignKey(l => l.ArtworkId)
                .OnDelete(DeleteBehavior.Cascade);
            
            modelBuilder.Entity<Like>()
                .HasOne(l => l.Story)
                .WithMany()
                .HasForeignKey(l => l.StoryId)
                .OnDelete(DeleteBehavior.Cascade);

        }
    }
}