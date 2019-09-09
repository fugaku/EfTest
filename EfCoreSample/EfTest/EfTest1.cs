using ConsoleApp;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrackableEntities.Common.Core;
using TrackableEntities.EF.Core;
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
        public async Task Update_OfflineMode()
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
                    var user = new User { Id = 1, Name = "name2" };
                    Assert.Empty(context.ChangeTracker.Entries());
                    user.TrackingState = TrackingState.Modified;
                    user.ModifiedProperties = new HashSet<string> { "Name" };
                    Assert.Empty(context.ChangeTracker.Entries());
                    context.ApplyChanges(user);
                    Assert.Single(context.ChangeTracker.Entries());
                    await context.SaveChangesAsync();
                    context.AcceptChanges(user);
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
        public async Task Delete_UsingSelectIncludeIdOnly()
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
                    var top = new Top() { Name = "name1" };
                    var mid1 = new Mid() { Name = "mid1", Top = top };
                    var mid2 = new Mid() { Name = "mid2", Top = top };
                    
                    var bot1 = new Bottom() { Name = "bot1", Top = top, Mid = mid1 };
                    var bot2 = new Bottom() { Name = "bot2", Top = top, Mid = mid1 };
                    var bot3 = new Bottom() { Name = "bot3", Top = top, Mid = mid2 };
                    var bot4 = new Bottom() { Name = "bot4", Top = top, Mid = mid2 };

                    context.Add(top);
                    context.Add(mid1);
                    context.Add(mid2);
                    context.Add(bot1);
                    context.Add(bot2);
                    context.Add(bot3);
                    context.Add(bot4);
                    await context.SaveChangesAsync();
                }
                
                using (var context = new SampleDbContext(options))
                {
                    Assert.Equal(1, context.Tops.Count());
                    Assert.Equal(2, context.Mids.Count());
                    Assert.Equal(4, context.Bottoms.Count());
                }

                using (var context = new SampleDbContext(options))
                {
                    var top = await context.Tops
                        .Include(x => x.Mids)
                            .ThenInclude(x => x.Bottoms)
                        .Select(x => new Top
                        {
                            Id = x.Id,
                            Mids = x.Mids.Select(y => new Mid
                            {
                                Id = y.Id,
                                Bottoms = y.Bottoms.Select(z => new Bottom
                                {
                                    Id = z.Id
                                }).ToList()
                            }).ToList()
                        })
                        .FirstOrDefaultAsync();
                    Assert.Empty(context.ChangeTracker.Entries());
                    if (top != null)
                    {
                        context.RemoveRange(top.Mids.SelectMany(x => x.Bottoms));
                        context.RemoveRange(top.Mids);
                        context.Remove(top);
                    }
                    Assert.Equal(7, context.ChangeTracker.Entries().Count());
                    await context.SaveChangesAsync();
                }

                using (var context = new SampleDbContext(options))
                {
                    Assert.False(context.Tops.Any());
                    Assert.False(context.Mids.Any());
                    Assert.False(context.Bottoms.Any());
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
