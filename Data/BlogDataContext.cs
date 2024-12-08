using Blogue.Data.Mappings;
using Blogue.Models;
using Microsoft.EntityFrameworkCore;

namespace Blogue.Data
{
    public class BlogDataContext : DbContext
    {
        public BlogDataContext(DbContextOptions<BlogDataContext> options) : base(options)
        {
        }
        
        public DbSet<Category> Category { get; set; }
        public DbSet<Post> Post { get; set; }
        
        public DbSet<Role> Roles { get; set; }
        
        public DbSet<Tag> Tags { get; set; }
        public DbSet<User> User { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new CategoryMap());
            modelBuilder.ApplyConfiguration(new UserMap());
            // modelBuilder.ApplyConfiguration(new PostMap());
        }
    }
}