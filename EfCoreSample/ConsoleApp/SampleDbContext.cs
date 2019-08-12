using Microsoft.EntityFrameworkCore;

namespace ConsoleApp
{
    public class SampleDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }

        public SampleDbContext()
        {
        }

        public SampleDbContext(DbContextOptions<SampleDbContext> options) : base(options)
        {
        }
    }

    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
