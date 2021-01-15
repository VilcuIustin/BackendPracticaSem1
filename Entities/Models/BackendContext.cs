using Backend.Entities.Models;
using Microsoft.EntityFrameworkCore;


namespace Backend.Entities
{
    public class BackendContext : DbContext
    {
        public BackendContext(DbContextOptions options) : base(options)
        {

        }
        public DbSet<User> Users { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Post> Posts { get; set; }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
               .HasMany(b => b.MyPosts)
               .WithOne();
            modelBuilder.Entity<User>()
              .HasMany(b => b.PostLiked)
              .WithOne();
            modelBuilder.Entity<User>()
              .HasMany(b => b.Friends)
              .WithOne();
           /* modelBuilder.Entity<Friend>()
              .HasOne(b => b.User1)
              .WithMany();*/

        }


    }
}
