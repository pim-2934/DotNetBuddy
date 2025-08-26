using DotNetBuddy.Application.Utilities;
using DotNetBuddy.Extensions.EntityFrameworkCore.Extensions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace DotNetBuddy.Example.Tests;

public class TestWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            services.RemoveAll(typeof(DbContextOptions<DatabaseContext>));
            services.RemoveAll(typeof(DatabaseContext));

            var dbName = $"TestDb_{Guid.NewGuid()}";
            services.AddDbContext<DatabaseContext>(options =>
                options
                    .AddBuddyInterceptors()
                    .UseInMemoryDatabase(dbName)
            );
        });

        builder.ConfigureTestServices(services =>
        {
            using var scope = services.BuildServiceProvider().CreateScope();
            var serviceProvider = scope.ServiceProvider;

            BuddyUtils.RunSeeders(serviceProvider, serviceProvider.GetRequiredService<IWebHostEnvironment>()).Wait();
        });
    }
}