using DotNetBuddy.Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;

namespace DotNetBuddy.Tests;

public class TestDbContext(DbContextOptions<TestDbContext> options) : DbContext(options)
{
    public DbSet<TestEntity> TestEntities { get; set; }
    public DbSet<TestAuditableEntity> AuditableTestEntities { get; set; }
    public DbSet<TestSoftDeletableEntity> SoftDeletableTestEntities { get; set; }

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