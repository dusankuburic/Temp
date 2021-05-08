using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using Temp.API.Data;
using Temp.Database;

namespace Temp.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host =  CreateHostBuilder(args).Build();
            using(var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var ctx = services.GetRequiredService<ApplicationDbContext>();
                    ctx.Database.Migrate();

                    Seed.SeedOrganizations(ctx);
                    Seed.SeedGroups(ctx);
                    Seed.SeedTeams(ctx);
                    Seed.SeedEmploymentStatuses(ctx);
                    Seed.SeedWorkplaces(ctx);
                    Seed.SeedEmployees(ctx);
                    Seed.SeedAdmins(ctx);
                    Seed.SeedUsers(ctx);
                    Seed.SeedModerators(ctx);

                }
                catch(Exception exMsg)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(exMsg, "Migration error");
                }
            }

            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
