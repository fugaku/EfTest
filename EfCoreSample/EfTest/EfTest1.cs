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
                    Assert.Empty(context.ChangeTracker.Entries());
                    user.Name = "name2";
                    context.Entry(user).Property(x => x.Name).IsModified = true;
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
                    Assert.Empty(context.ChangeTracker.Entries());
                    user.Name = "name2";
                    Assert.Empty(context.ChangeTracker.Entries());
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
                    Assert.Empty(context.ChangeTracker.Entries());
                    context.Remove(user);
                    Assert.Single(context.ChangeTracker.Entries());
                    await context.SaveChangesAsync();
                }

                using (var context = new SampleDbContext(options))
                {
                    var user = await context.Users.FirstOrDefaultAsync();
                    Assert.Null(user);
                }
            }
        }

        [Fact]
        public async Task Delete_WithoutSelectOrAttach()
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
                    var user = new User { Id = 1 };
                    Assert.Empty(context.ChangeTracker.Entries());
                    context.Remove(user);
                    Assert.Single(context.ChangeTracker.Entries());
                    await context.SaveChangesAsync();
                }

                using (var context = new SampleDbContext(options))
                {
                    var user = await context.Users.FirstOrDefaultAsync();
                    Assert.Null(user);
                }
            }
        }

        [Fact]
        public async Task SelectProjection_ShouldNotTrack()
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
                    var user = await context.Users.Where(x => x.Id == 1).Select(x => new User { Id = x.Id, Name = x.Name, Description = x.Description }).FirstOrDefaultAsync();
                    Assert.Empty(context.ChangeTracker.Entries());
                    Assert.NotNull(user);
                }
            }
        }

        [Fact]
        public async Task SelectNoProjection_ShouldTrack()
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
                    var user = await context.Users.Where(x => x.Id == 1).FirstOrDefaultAsync();
                    Assert.Single(context.ChangeTracker.Entries());
                    Assert.NotNull(user);
                }
            }
        }

        [Fact]
        public async Task SelectParentRelationship_ShouldNotBreak()
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
                    context.Add(user);
                    var item = new Item() { Name = "item1" };
                    context.Add(item);
                    await context.SaveChangesAsync();
                }

                using (var context = new SampleDbContext(options))
                {
                    var item = await context.Items.Where(x => x.Id == 1)
                        .Select(x => new ItemTest
                        {
                            Id = x.Id,
                            Name = x.Name,
                            UserId = x.UserId,
                            UserName = x.User.Name
                        }).FirstOrDefaultAsync();
                    Assert.Empty(context.ChangeTracker.Entries());
                    Assert.NotNull(item);
                    Assert.Equal(1, item.Id);
                    Assert.Equal("item1", item.Name);
                    Assert.Equal(0, item.UserId);
                    Assert.Null(item.UserName);
                }
            }
        }
    }

    public class ItemTest
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
    }
}
