using ConsoleApp;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace EfTest
{
    public class EfTest1
    {
        [Fact]
        public async Task Update_UsingSelectIdOnly()
        {
            using (var connection = new SqliteConnection("DataSource=:memory:"))
            {
                connection.Open();

                var options = new DbContextOptionsBuilder<SampleDbContext>()
                    .UseSqlite(connection)
                    .Options;

                using (var context = new SampleDbContext(options))
                {
                    await context.Database.EnsureCreatedAsync();
                }

                using (var context = new SampleDbContext(options))
                {
                    var user = new User() { Name = "name1", Description = "description1" };
                    context.Users.Add(user);
                    await context.SaveChangesAsync();
                }

                using (var context = new SampleDbContext(options))
                {
                    var user = await context.Users.Where(x => x.Id == 1).Select(x => new User { Id = x.Id }).FirstOrDefaultAsync();
                    user.Name = "name2";
                    context.Entry(user).Property(x => x.Name).IsModified = true;
                    await context.SaveChangesAsync();
                }

                using (var context = new SampleDbContext(options))
                {
                    var user = await context.Users.Where(x => x.Id == 1).FirstOrDefaultAsync();
                    Assert.Equal("name2", user.Name);
                    Assert.Equal("description1", user.Description);
                }
            }
        }

        [Fact]
        public async Task Update_UsingSelectedPropertyOnly_ShouldNotModiyAnything()
        {
            using (var connection = new SqliteConnection("DataSource=:memory:"))
            {
                connection.Open();

                var options = new DbContextOptionsBuilder<SampleDbContext>()
                    .UseSqlite(connection)
                    .Options;

                using (var context = new SampleDbContext(options))
                {
                    await context.Database.EnsureCreatedAsync();
                }

                using (var context = new SampleDbContext(options))
                {
                    var user = new User() { Name = "name1", Description = "description1" };
                    context.Users.Add(user);
                    await context.SaveChangesAsync();
                }

                using (var context = new SampleDbContext(options))
                {
                    var user = await context.Users.Where(x => x.Id == 1).Select(x => new User { Id = x.Id, Name = x.Name }).FirstOrDefaultAsync();
                    user.Name = "name2";
                    await context.SaveChangesAsync();
                }

                using (var context = new SampleDbContext(options))
                {
                    var user = await context.Users.Where(x => x.Id == 1).FirstOrDefaultAsync();
                    Assert.Equal("name1", user.Name);
                    Assert.Equal("description1", user.Description);
                }
            }
        }

        [Fact]
        public async Task Update_UsingAttach()
        {
            using (var connection = new SqliteConnection("DataSource=:memory:"))
            {
                connection.Open();

                var options = new DbContextOptionsBuilder<SampleDbContext>()
                    .UseSqlite(connection)
                    .Options;

                using (var context = new SampleDbContext(options))
                {
                    await context.Database.EnsureCreatedAsync();
                }

                using (var context = new SampleDbContext(options))
                {
                    var user = new User() { Name = "name1", Description = "description1" };
                    context.Users.Add(user);
                    await context.SaveChangesAsync();
                }

                using (var context = new SampleDbContext(options))
                {
                    var user = await context.Users.Where(x => x.Id == 1).Select(x => new User { Id = x.Id, Name = x.Name }).FirstOrDefaultAsync();
                    Assert.Empty(context.ChangeTracker.Entries());

                    context.Attach(user);
                    user.Name = "name2";
                    Assert.Single(context.ChangeTracker.Entries());

                    await context.SaveChangesAsync();
                }

                using (var context = new SampleDbContext(options))
                {
                    var user = await context.Users.Where(x => x.Id == 1).FirstOrDefaultAsync();
                    Assert.Equal("name2", user.Name);
                    Assert.Equal("description1", user.Description);
                }
            }
        }

        [Fact]
        public async Task Delete_UsingSelectIdOnly()
        {
            using (var connection = new SqliteConnection("DataSource=:memory:"))
            {
                connection.Open();

                var options = new DbContextOptionsBuilder<SampleDbContext>()
                    .UseSqlite(connection)
                    .Options;

                using (var context = new SampleDbContext(options))
                {
                    await context.Database.EnsureCreatedAsync();
                }

                using (var context = new SampleDbContext(options))
                {
                    var user = new User() { Name = "name1", Description = "description1" };
                    context.Users.Add(user);
                    await context.SaveChangesAsync();
                }

                using (var context = new SampleDbContext(options))
                {
                    var user = await context.Users.Where(x => x.Id == 1).Select(x => new User { Id = x.Id }).FirstOrDefaultAsync();
                    context.Remove(user);
                    await context.SaveChangesAsync();
                }

                using (var context = new SampleDbContext(options))
                {
                    var user = await context.Users.FirstOrDefaultAsync();
                    Assert.Null(user);
                }
            }
        }
    }
}
