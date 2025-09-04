using DotNetBuddy.Extensions.EntityFrameworkCore.Extensions;
using DotNetBuddy.Tests.Entities;
using Microsoft.EntityFrameworkCore;

namespace DotNetBuddy.Tests;

public class TestDbContext(DbContextOptions<TestDbContext> options) : DbContext(options)
{
    public DbSet<Entity> Entities { get; set; }
    public DbSet<AuditableEntity> AuditableTestEntities { get; set; }
    public DbSet<ComplexEntity> ComplexTestEntities { get; set; }
    public DbSet<SoftDeletableEntity> SoftDeletableTestEntities { get; set; }
    public DbSet<NavigationEntity> NavigationEntities { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplySoftDeleteQueryFilters();
    }

    public static TestDbContext CreateContext(string dbName)
    {
        var options = new DbContextOptionsBuilder<TestDbContext>()
            .UseInMemoryDatabase(dbName)
            .AddBuddyInterceptors()
            .Options;

        var context = new TestDbContext(options);
        context.Database.EnsureDeleted();
        context.Database.EnsureCreated();

        return context;
    }
}