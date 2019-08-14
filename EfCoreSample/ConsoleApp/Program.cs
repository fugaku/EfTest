using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace ConsoleApp
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var options = new DbContextOptionsBuilder<SampleDbContext>()
                .UseSqlServer("Server=localhost;Database=test;User Id=sa;Password=12345;")
                .Options;

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
                var top = await context.Tops
                    .Include(x => x.Mids)
                        .ThenInclude(x => x.Bottoms)
                    .Where(x => x.Id == 3)
                    .FirstOrDefaultAsync();
                if (top != null)
                {
                    context.RemoveRange(top.Mids.SelectMany(x => x.Bottoms));
                    context.RemoveRange(top.Mids);
                    context.Remove(top);
                }
                await context.SaveChangesAsync();
            }
        }
    }
}
