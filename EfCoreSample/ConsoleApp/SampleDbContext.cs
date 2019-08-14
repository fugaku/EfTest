using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ConsoleApp
{
    public class SampleDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<Top> Tops { get; set; }
        public DbSet<Mid> Mids { get; set; }
        public DbSet<Bottom> Bottoms { get; set; }

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

    [Table("top")]
    public class Top
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<Mid> Mids { get; set; }
    }

    [Table("mid")]
    public class Mid
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int TopId { get; set; }
        public Top Top { get; set; }
        public List<Bottom> Bottoms { get; set; }
    }

    [Table("bottom")]
    public class Bottom
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int MidId { get; set; }
        public Mid Mid { get; set; }
        public int TopId { get; set; }
        public Top Top { get; set; }
    }
}
