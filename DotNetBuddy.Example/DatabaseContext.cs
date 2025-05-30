using DotNetBuddy.Example.Models;
using DotNetBuddy.Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;

namespace DotNetBuddy.Example;

public class DatabaseContext(DbContextOptions<DatabaseContext> options) : DbContext(options)
{
    public DbSet<WeatherForecast> WeatherForecasts { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.ApplySoftDeleteQueryFilters(); // Add support for soft-deletes (ISoftDeletableEntity)
    }
}