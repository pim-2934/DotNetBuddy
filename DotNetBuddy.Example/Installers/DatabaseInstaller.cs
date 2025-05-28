using DotNetBuddy.Application;
using DotNetBuddy.Example.Repositories;
using DotNetBuddy.Example.Repositories.Interfaces;
using DotNetBuddy.Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;

namespace DotNetBuddy.Example.Installers;

public class DatabaseInstaller : IInstaller
{
    public void Install(IServiceCollection services)
    {
        services.AddScoped<IExtendedUnitOfWork, ExtendedUnitOfWork>();

        services.AddDbContext<DatabaseContext>
        (
            (_, options) =>
            {
                options
                    .AddBuddyInterceptors() // Buddy: Enable auditing (CreatedAt, UpdatedAt, etc)
                    .UseInMemoryDatabase("Example")
                    .EnableDetailedErrors();
            }
        );
    }
}