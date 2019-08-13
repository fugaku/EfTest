using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace ConsoleApp
{
    public class SampleDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Item> Items { get; set; }

        public SampleDbContext()
        {
        }

        public SampleDbContext(DbContextOptions<SampleDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Item>()
                .HasOne(p => p.User)
                .WithMany(b => b.Items)
                .IsRequired(false);
        }
    }

    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public List<Item> Items { get; set; }
    }

    public class Item
    {
        public int Id { get; set; }
        public User User { get; set; }
        public int UserId { get; set; }
        public string Name { get; set; }
    }
}
