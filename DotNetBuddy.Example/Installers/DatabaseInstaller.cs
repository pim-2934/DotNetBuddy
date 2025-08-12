using DotNetBuddy.Application;
using DotNetBuddy.Extensions.EntityFrameworkCore.Extensions;
using DotNetBuddy.Example.Repositories;
using DotNetBuddy.Example.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace DotNetBuddy.Example.Installers;

public class DatabaseInstaller : IInstaller
{
    public void Install(IServiceCollection services)
    {
        services.TryAddScoped<IExtendedUnitOfWork, ExtendedUnitOfWork>();

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