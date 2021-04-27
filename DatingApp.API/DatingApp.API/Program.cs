using DatingApp.API.Data;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;

namespace DatingApp.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateWebHostBuilder(args).Build();
            using (var scope = host.Services.CreateScope())
            {
                var provider = scope.ServiceProvider;

                try
                {
                    var context = provider.GetRequiredService<DatingContext>();
                    context.Database.Migrate();
                    Seed.SeedDatabase(context);
                }
                catch (Exception e)
                {
                    var logger = provider.GetRequiredService<ILogger<Program>>();
                    logger.LogError(e, "unable to seed database with data");
                }

            }

            host.Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
    }
}
