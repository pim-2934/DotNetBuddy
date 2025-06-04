using DotNetBuddy.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace DotNetBuddy.Tests.Unit;

public class RepositoryTests
{
    private static async Task SeedTestData(TestDbContext context, int count = 10)
    {
        for (var i = 1; i <= count; i++)
        {
            await context.TestEntities.AddAsync(new TestEntity
            {
                Id = Guid.NewGuid(),
                Name = $"Test Entity {i}",
                Description = i % 2 == 0 ? $"Description for entity {i}" : null,
                CreatedAt = DateTime.UtcNow.AddDays(-i)
            });
        }

        await context.SaveChangesAsync();
    }

    [Fact]
    public async Task GetRangeAsync_WithoutPredicate_ReturnsAllEntities()
    {
        // Arrange
        var dbContext = TestDbContext.CreateContext(nameof(GetRangeAsync_WithoutPredicate_ReturnsAllEntities));
        await SeedTestData(dbContext, 5);
        var repository = new Repository<TestEntity, Guid>(dbContext);

        // Act
        var result = await repository.GetRangeAsync();

        // Assert
        Assert.Equal(5, result.Count());
    }

    [Fact]
    public async Task GetRangeAsync_WithPredicate_ReturnsFilteredEntities()
    {
        // Arrange
        var dbContext = TestDbContext.CreateContext(nameof(GetRangeAsync_WithPredicate_ReturnsFilteredEntities));
        await SeedTestData(dbContext);
        var repository = new Repository<TestEntity, Guid>(dbContext);

        // Act
        var result = await repository.GetRangeAsync(e => e.Name.Contains("3"));

        // Assert
        Assert.Single(result);
        Assert.Contains("3", result.First().Name);
    }

    [Fact]
    public async Task GetRangeAsync_WithIds_ReturnsEntitiesWithMatchingIds()
    {
        // Arrange
        var dbContext = TestDbContext.CreateContext(nameof(GetRangeAsync_WithIds_ReturnsEntitiesWithMatchingIds));
        await SeedTestData(dbContext, 5);
        var repository = new Repository<TestEntity, Guid>(dbContext);

        // Get all entities first to get their IDs
        var allEntities = await dbContext.TestEntities.ToListAsync();
        var selectedIds = allEntities.Take(2).Select(e => e.Id).ToList();

        // Act
        var result = await repository.GetRangeAsync(selectedIds);

        // Assert
        Assert.Equal(2, result.Count());
        Assert.All(result, entity => Assert.Contains(entity.Id, selectedIds));
    }

    [Fact]
    public async Task GetPagedAsync_WithPredicate_ReturnsPaginatedResults()
    {
        // Arrange
        var dbContext = TestDbContext.CreateContext(nameof(GetPagedAsync_WithPredicate_ReturnsPaginatedResults));
        await SeedTestData(dbContext, 20);
        var repository = new Repository<TestEntity, Guid>(dbContext);

        // Act
        var result = await repository.GetPagedAsync(2, 5);

        // Assert
        Assert.Equal(5, result.Items.Count());
        Assert.Equal(2, result.PageNumber);
        Assert.Equal(5, result.PageSize);
        Assert.Equal(20, result.TotalCount);
        Assert.Equal(4, result.TotalPages);
        Assert.True(result.HasPreviousPage);
        Assert.True(result.HasNextPage);
    }

    [Fact]
    public async Task GetPagedAsync_WithIds_ReturnsPaginatedResultsForIds()
    {
        // Arrange
        var dbContext = TestDbContext.CreateContext(nameof(GetPagedAsync_WithIds_ReturnsPaginatedResultsForIds));
        await SeedTestData(dbContext, 20);
        var repository = new Repository<TestEntity, Guid>(dbContext);

        // Get a subset of IDs
        var allEntities = await dbContext.TestEntities.ToListAsync();
        var selectedIds = allEntities.Take(10).Select(e => e.Id).ToList();

        // Act
        var result = await repository.GetPagedAsync(2, 3, selectedIds);

        // Assert
        Assert.Equal(3, result.Items.Count());
        Assert.Equal(2, result.PageNumber);
        Assert.Equal(3, result.PageSize);
        Assert.Equal(10, result.TotalCount);
        Assert.Equal(4, result.TotalPages);
        Assert.True(result.HasPreviousPage);
        Assert.True(result.HasNextPage);
    }

    [Fact]
    public async Task SearchAsync_WithValidTerm_ReturnsMatchingEntities()
    {
        // Arrange
        var dbContext = TestDbContext.CreateContext(nameof(SearchAsync_WithValidTerm_ReturnsMatchingEntities));
        await SeedTestData(dbContext);
        var repository = new Repository<TestEntity, Guid>(dbContext);

        // Act
        var result = await repository.SearchAsync("Entity 5");

        // Assert
        Assert.Single(result);
        Assert.Contains("Entity 5", result.First().Name);
    }

    [Fact]
    public async Task SearchPagedAsync_WithValidTerm_ReturnsPaginatedResults()
    {
        // Arrange
        var dbContext = TestDbContext.CreateContext(nameof(SearchPagedAsync_WithValidTerm_ReturnsPaginatedResults));
        await SeedTestData(dbContext, 20);
        var repository = new Repository<TestEntity, Guid>(dbContext);

        // Act
        var result = await repository.SearchPagedAsync("Entity", 2, 5);

        // Assert
        Assert.Equal(5, result.Items.Count());
        Assert.Equal(2, result.PageNumber);
        Assert.Equal(5, result.PageSize);
        Assert.Equal(20, result.TotalCount);
        Assert.Equal(4, result.TotalPages);
    }

    [Fact]
    public async Task GetAsync_WithPredicate_ReturnsMatchingEntity()
    {
        // Arrange
        var dbContext = TestDbContext.CreateContext(nameof(GetAsync_WithPredicate_ReturnsMatchingEntity));
        await SeedTestData(dbContext);
        var repository = new Repository<TestEntity, Guid>(dbContext);

        // Act
        var result = await repository.GetAsync(e => e.Name == "Test Entity 3");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Test Entity 3", result.Name);
    }

    [Fact]
    public async Task GetAsync_WithId_ReturnsEntityWithMatchingId()
    {
        // Arrange
        var dbContext = TestDbContext.CreateContext(nameof(GetAsync_WithId_ReturnsEntityWithMatchingId));
        await SeedTestData(dbContext);
        var repository = new Repository<TestEntity, Guid>(dbContext);

        // Get an entity ID
        var entity = await dbContext.TestEntities.FirstAsync();

        // Act
        var result = await repository.GetAsync(entity.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(entity.Id, result.Id);
    }

    [Fact]
    public async Task AnyAsync_WithPredicate_ReturnsTrueForExistingEntity()
    {
        // Arrange
        var dbContext = TestDbContext.CreateContext(nameof(AnyAsync_WithPredicate_ReturnsTrueForExistingEntity));
        await SeedTestData(dbContext);
        var repository = new Repository<TestEntity, Guid>(dbContext);

        // Act
        var result = await repository.AnyAsync(e => e.Name == "Test Entity 3");

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task AnyAsync_WithId_ReturnsTrueForExistingEntityId()
    {
        // Arrange
        var dbContext = TestDbContext.CreateContext(nameof(AnyAsync_WithId_ReturnsTrueForExistingEntityId));
        await SeedTestData(dbContext);
        var repository = new Repository<TestEntity, Guid>(dbContext);

        // Get an entity ID
        var entityId = (await dbContext.TestEntities.FirstAsync()).Id;

        // Act
        var result = await repository.AnyAsync(entityId);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task AddAsync_WithSingleEntity_AddsEntityToDatabase()
    {
        // Arrange
        var dbContext = TestDbContext.CreateContext(nameof(AddAsync_WithSingleEntity_AddsEntityToDatabase));
        var repository = new Repository<TestEntity, Guid>(dbContext);
        var entity = new TestEntity
        {
            Id = Guid.NewGuid(),
            Name = "New Test Entity",
            Description = "Test Description",
            CreatedAt = DateTime.UtcNow
        };

        // Act
        var result = await repository.AddAsync(entity);
        await dbContext.SaveChangesAsync();

        // Assert
        Assert.Equal(entity.Id, result.Id);
        Assert.Equal(1, await dbContext.TestEntities.CountAsync());
        Assert.Equal("New Test Entity", (await dbContext.TestEntities.FirstAsync()).Name);
    }

    [Fact]
    public async Task AddAsync_WithMultipleEntities_AddsEntitiesToDatabase()
    {
        // Arrange
        var dbContext = TestDbContext.CreateContext(nameof(AddAsync_WithMultipleEntities_AddsEntitiesToDatabase));
        var repository = new Repository<TestEntity, Guid>(dbContext);
        var entities = new List<TestEntity>
        {
            new TestEntity
            {
                Id = Guid.NewGuid(),
                Name = "Entity 1",
                CreatedAt = DateTime.UtcNow
            },
            new TestEntity
            {
                Id = Guid.NewGuid(),
                Name = "Entity 2",
                CreatedAt = DateTime.UtcNow
            }
        };

        // Act
        var result = await repository.AddAsync(entities);
        await dbContext.SaveChangesAsync();

        // Assert
        Assert.Equal(2, result.Count());
        Assert.Equal(2, await dbContext.TestEntities.CountAsync());
    }

    [Fact]
    public async Task UpdateShallow_UpdatesEntityProperties()
    {
        // Arrange
        var dbContext = TestDbContext.CreateContext(nameof(UpdateShallow_UpdatesEntityProperties));
        await SeedTestData(dbContext, 1);
        var repository = new Repository<TestEntity, Guid>(dbContext);

        var entity = await dbContext.TestEntities.FirstAsync();
        entity.Name = "Updated Name";

        // Act
        repository.UpdateShallow(entity);
        await dbContext.SaveChangesAsync();

        // Assert
        var updatedEntity = await dbContext.TestEntities.FirstAsync();
        Assert.Equal("Updated Name", updatedEntity.Name);
    }

    [Fact]
    public async Task UpdateDeep_UpdatesEntityAndRelatedEntities()
    {
        // Arrange
        var dbContext = TestDbContext.CreateContext(nameof(UpdateDeep_UpdatesEntityAndRelatedEntities));
        await SeedTestData(dbContext, 1);
        var repository = new Repository<TestEntity, Guid>(dbContext);

        var entity = await dbContext.TestEntities.FirstAsync();
        entity.Name = "Deep Updated Name";

        // Act
        repository.UpdateDeep(entity);
        await dbContext.SaveChangesAsync();

        // Assert
        var updatedEntity = await dbContext.TestEntities.FirstAsync();
        Assert.Equal("Deep Updated Name", updatedEntity.Name);
    }

    [Fact]
    public async Task DeleteAsync_WithValidId_RemovesEntityFromDatabase()
    {
        // Arrange
        var dbContext = TestDbContext.CreateContext(nameof(DeleteAsync_WithValidId_RemovesEntityFromDatabase));
        await SeedTestData(dbContext, 3);
        var repository = new Repository<TestEntity, Guid>(dbContext);

        var entityToDelete = await dbContext.TestEntities.FirstAsync();

        // Act
        await repository.DeleteAsync(entityToDelete.Id);
        await dbContext.SaveChangesAsync();

        // Assert
        Assert.Equal(2, await dbContext.TestEntities.CountAsync());
        Assert.Null(await dbContext.TestEntities.FirstOrDefaultAsync(e => e.Id == entityToDelete.Id));
    }

    [Fact]
    public async Task DeleteRangeAsync_WithValidIds_RemovesEntitiesFromDatabase()
    {
        // Arrange
        var dbContext = TestDbContext.CreateContext(nameof(DeleteRangeAsync_WithValidIds_RemovesEntitiesFromDatabase));
        await SeedTestData(dbContext, 5);
        var repository = new Repository<TestEntity, Guid>(dbContext);

        var entitiesToDelete = await dbContext.TestEntities.Take(2).ToListAsync();
        var idsToDelete = entitiesToDelete.Select(e => e.Id).ToList();

        // Act
        await repository.DeleteRangeAsync(idsToDelete);
        await dbContext.SaveChangesAsync();

        // Assert
        Assert.Equal(3, await dbContext.TestEntities.CountAsync());
        Assert.Empty(await dbContext.TestEntities.Where(e => idsToDelete.Contains(e.Id)).ToListAsync());
    }

    [Fact]
    public async Task CountAsync_WithoutPredicate_ReturnsTotalCount()
    {
        // Arrange
        var dbContext = TestDbContext.CreateContext(nameof(CountAsync_WithoutPredicate_ReturnsTotalCount));
        await SeedTestData(dbContext, 7);
        var repository = new Repository<TestEntity, Guid>(dbContext);

        // Act
        var count = await repository.CountAsync();

        // Assert
        Assert.Equal(7, count);
    }

    [Fact]
    public async Task CountAsync_WithPredicate_ReturnsFilteredCount()
    {
        // Arrange
        var dbContext = TestDbContext.CreateContext(nameof(CountAsync_WithPredicate_ReturnsFilteredCount));
        await SeedTestData(dbContext);
        var repository = new Repository<TestEntity, Guid>(dbContext);

        // Act
        var count = await repository.CountAsync(e => e.Name.Contains("5"));

        // Assert
        Assert.Equal(1, count);
    }

    [Fact]
    public async Task SearchAsync_WithEmptyTerm_ReturnsEmptyCollection()
    {
        // Arrange
        var dbContext = TestDbContext.CreateContext(nameof(SearchAsync_WithEmptyTerm_ReturnsEmptyCollection));
        await SeedTestData(dbContext);
        var repository = new Repository<TestEntity, Guid>(dbContext);

        // Act
        var result = await repository.SearchAsync("");

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task SearchPagedAsync_WithEmptyTerm_ReturnsEmptyPagedResult()
    {
        // Arrange
        var dbContext = TestDbContext.CreateContext(nameof(SearchPagedAsync_WithEmptyTerm_ReturnsEmptyPagedResult));
        await SeedTestData(dbContext);
        var repository = new Repository<TestEntity, Guid>(dbContext);

        // Act
        var result = await repository.SearchPagedAsync("", 1, 10);

        // Assert
        Assert.Empty(result.Items);
        Assert.Equal(0, result.TotalCount);
    }
}