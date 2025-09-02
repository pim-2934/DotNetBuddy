using System.ComponentModel.DataAnnotations;
using DotNetBuddy.Extensions.EntityFrameworkCore;
using DotNetBuddy.Domain.Enums;
using DotNetBuddy.Infrastructure.Extensions;
using DotNetBuddy.Tests.RepositoryEntities;
using Microsoft.EntityFrameworkCore;
using ValidationException = DotNetBuddy.Domain.Exceptions.ValidationException;

namespace DotNetBuddy.Tests.Unit;

public class RepositoryTests
{
    private static async Task SeedTestData(TestDbContext context, int count = 10)
    {
        var entityRepo = new Repository<Entity, Guid>(context);
        var auditableRepo = new Repository<AuditableEntity, Guid>(context);
        var softDeletableRepo = new Repository<SoftDeletableEntity, Guid>(context);
        var navigationRepo = new Repository<NavigationEntity, Guid>(context);

        for (var i = 1; i <= count; i++)
        {
            await entityRepo.AddAsync(new Entity
            {
                Id = Guid.NewGuid(),
                Name = $"Test Entity {i}",
                Description = i % 2 == 0 ? $"Description for entity {i}" : null,
                CreatedAt = DateTime.UtcNow.AddDays(-i)
            });

            await auditableRepo.AddAsync(new AuditableEntity
            {
                Id = Guid.NewGuid(),
                Name = $"Auditable Entity {i}",
                Description = i % 2 == 0 ? $"Description for auditable entity {i}" : null
                // No need to set CreatedAt/UpdatedAt manually
            });

            await softDeletableRepo.AddAsync(new SoftDeletableEntity
            {
                Id = Guid.NewGuid(),
                Name = $"Soft Deletable Entity {i}",
                Description = i % 2 == 0 ? $"Description for soft deletable entity {i}" : null,
                DeletedAt = null
            });

            await navigationRepo.AddAsync(new NavigationEntity
            {
                Id = Guid.NewGuid(),
                Parent = new NavigationEntity
                {
                    Id = Guid.NewGuid(),
                    Parent = new NavigationEntity
                    {
                        Id = Guid.NewGuid(),
                    },
                    Children =
                    [
                        new NavigationEntity
                        {
                            Id = Guid.NewGuid(),
                        },
                        new NavigationEntity
                        {
                            Id = Guid.NewGuid(),
                        }
                    ]
                },
                Children =
                [
                    new NavigationEntity
                    {
                        Id = Guid.NewGuid(),
                    },
                    new NavigationEntity
                    {
                        Id = Guid.NewGuid(),
                    }
                ]
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
        var repository = new Repository<Entity, Guid>(dbContext);

        // Act
        var result = await repository.GetRangeAsync();

        // Assert
        Assert.Equal(5, result.Count);
    }

    [Fact]
    public async Task GetRangeAsync_WithPredicate_ReturnsFilteredEntities()
    {
        // Arrange
        var dbContext = TestDbContext.CreateContext(nameof(GetRangeAsync_WithPredicate_ReturnsFilteredEntities));
        await SeedTestData(dbContext);
        var repository = new Repository<Entity, Guid>(dbContext);

        // Act
        var result = await repository.GetRangeAsync(x => x.Where(e => e.Name.Contains('3')));

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
        var repository = new Repository<Entity, Guid>(dbContext);

        // Get all entities first to get their IDs using repository
        var allEntities = await repository.GetRangeAsync();
        var selectedIds = allEntities.Take(2).Select(e => e.Id).ToList();

        // Act
        var result = await repository.GetRangeAsync(selectedIds);

        // Assert
        Assert.Equal(2, result.Count);
        Assert.All(result, entity => Assert.Contains(entity.Id, selectedIds));
    }

    [Fact]
    public async Task GetPagedAsync_WithPredicate_ReturnsPaginatedResults()
    {
        // Arrange
        var dbContext = TestDbContext.CreateContext(nameof(GetPagedAsync_WithPredicate_ReturnsPaginatedResults));
        await SeedTestData(dbContext, 20);
        var repository = new Repository<Entity, Guid>(dbContext);

        // Act
        var result = await repository.GetPagedAsync(2, 5);

        // Assert
        Assert.Equal(5, result.Items.Count);
        Assert.Equal(2, result.PageNumber);
        Assert.Equal(5, result.PageSize);
        Assert.Equal(20, result.TotalCount);
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
        var repository = new Repository<Entity, Guid>(dbContext);

        // Act
        var result = await repository.GetRangeAsync(x => x.Search("Entity 5"));

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
        var repository = new Repository<Entity, Guid>(dbContext);

        // Act
        var result = await repository.GetPagedAsync(x => x.Search("Entity"), 2, 5);

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
        var repository = new Repository<Entity, Guid>(dbContext);

        // Act
        var result = await repository.GetAsync(x => x.Where(e => e.Name == "Test Entity 3"));

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
        var repository = new Repository<Entity, Guid>(dbContext);

        // Get all entities to get an ID
        var entities = await repository.GetRangeAsync();
        var entity = entities.First();

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
        var repository = new Repository<Entity, Guid>(dbContext);

        // Act
        var result = await repository.AnyAsync(x => x.Where(e => e.Name == "Test Entity 3"));

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task AnyAsync_WithId_ReturnsTrueForExistingEntityId()
    {
        // Arrange
        var dbContext = TestDbContext.CreateContext(nameof(AnyAsync_WithId_ReturnsTrueForExistingEntityId));
        await SeedTestData(dbContext);
        var repository = new Repository<Entity, Guid>(dbContext);

        // Get an entity ID from repository
        var entityId = (await repository.GetRangeAsync()).First().Id;

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
        var repository = new Repository<Entity, Guid>(dbContext);
        var entity = new Entity
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
        Assert.Equal(1, await repository.CountAsync());
        Assert.Equal("New Test Entity", (await repository.GetRangeAsync()).First().Name);
    }

    [Fact]
    public async Task AddAsync_WithMultipleEntities_AddsEntitiesToDatabase()
    {
        // Arrange
        var dbContext = TestDbContext.CreateContext(nameof(AddAsync_WithMultipleEntities_AddsEntitiesToDatabase));
        var repository = new Repository<Entity, Guid>(dbContext);
        var entities = new List<Entity>
        {
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Entity 1",
                CreatedAt = DateTime.UtcNow
            },
            new()
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
        Assert.Equal(2, result.Count);
        Assert.Equal(2, await repository.CountAsync());
    }

    [Fact]
    public async Task UpdateShallow_UpdatesEntityProperties()
    {
        // Arrange
        var dbContext = TestDbContext.CreateContext(nameof(UpdateShallow_UpdatesEntityProperties));
        await SeedTestData(dbContext, 1);
        var repository = new Repository<Entity, Guid>(dbContext);

        var entity = (await repository.GetRangeAsync()).First();
        entity.Name = "Updated Name";

        // Act
        repository.UpdateShallow(entity);
        await dbContext.SaveChangesAsync();

        // Assert
        var updatedEntity = (await repository.GetRangeAsync()).First();
        Assert.Equal("Updated Name", updatedEntity.Name);
    }

    [Fact]
    public async Task UpdateDeep_UpdatesEntityAndRelatedEntities()
    {
        // Arrange
        var dbContext = TestDbContext.CreateContext(nameof(UpdateDeep_UpdatesEntityAndRelatedEntities));
        await SeedTestData(dbContext, 1);
        var repository = new Repository<Entity, Guid>(dbContext);

        var entity = (await repository.GetRangeAsync()).First();
        entity.Name = "Deep Updated Name";

        // Act
        repository.UpdateDeep(entity);
        await dbContext.SaveChangesAsync();

        // Assert
        var updatedEntity = (await repository.GetRangeAsync()).First();
        Assert.Equal("Deep Updated Name", updatedEntity.Name);
    }

    [Fact]
    public async Task DeleteAsync_WithValidId_RemovesEntityFromDatabase()
    {
        // Arrange
        var dbContext = TestDbContext.CreateContext(nameof(DeleteAsync_WithValidId_RemovesEntityFromDatabase));
        await SeedTestData(dbContext, 3);
        var repository = new Repository<Entity, Guid>(dbContext);

        var entities = await repository.GetRangeAsync();
        var entityToDelete = entities.First();

        // Act
        await repository.DeleteAsync(entityToDelete.Id);
        await dbContext.SaveChangesAsync();

        // Assert
        Assert.Equal(2, await repository.CountAsync());
        Assert.Null(await repository.GetAsync(entityToDelete.Id));
    }

    [Fact]
    public async Task DeleteRangeAsync_WithValidIds_RemovesEntitiesFromDatabase()
    {
        // Arrange
        var dbContext = TestDbContext.CreateContext(nameof(DeleteRangeAsync_WithValidIds_RemovesEntitiesFromDatabase));
        await SeedTestData(dbContext, 5);
        var repository = new Repository<Entity, Guid>(dbContext);

        var entities = await repository.GetRangeAsync();
        var entitiesToDelete = entities.Take(2).ToList();
        var idsToDelete = entitiesToDelete.Select(e => e.Id).ToList();

        // Act
        await repository.DeleteRangeAsync(idsToDelete);
        await dbContext.SaveChangesAsync();

        // Assert
        Assert.Equal(3, await repository.CountAsync());
        Assert.Empty(await repository.GetRangeAsync(idsToDelete));
    }

    [Fact]
    public async Task CountAsync_WithoutPredicate_ReturnsTotalCount()
    {
        // Arrange
        var dbContext = TestDbContext.CreateContext(nameof(CountAsync_WithoutPredicate_ReturnsTotalCount));
        await SeedTestData(dbContext, 7);
        var repository = new Repository<Entity, Guid>(dbContext);

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
        var repository = new Repository<Entity, Guid>(dbContext);

        // Act
        var count = await repository.CountAsync(x => x.Where(e => e.Name.Contains('5')));

        // Assert
        Assert.Equal(1, count);
    }

    [Fact]
    public async Task AddAsync_WithAuditableEntity_SetsCreatedAndUpdatedDates()
    {
        // Arrange
        var dbContext = TestDbContext.CreateContext(nameof(AddAsync_WithAuditableEntity_SetsCreatedAndUpdatedDates));
        var repository = new Repository<AuditableEntity, Guid>(dbContext);
        var beforeTime = DateTime.UtcNow;

        var entity = new AuditableEntity
        {
            Id = Guid.NewGuid(),
            Name = "New Auditable Entity",
            Description = "Test Description"
            // No need to set CreatedAt/UpdatedAt - they will be handled by interceptor
        };

        // Act
        await repository.AddAsync(entity);
        await dbContext.SaveChangesAsync();
        var afterTime = DateTime.UtcNow;

        // Assert
        var savedEntity = await repository.GetAsync(entity.Id);
        Assert.NotNull(savedEntity);

        // Check that dates were set automatically by the interceptor
        Assert.True(savedEntity.CreatedAt >= beforeTime);
        Assert.True(savedEntity.CreatedAt <= afterTime);
        Assert.True(savedEntity.UpdatedAt >= beforeTime);
        Assert.True(savedEntity.UpdatedAt <= afterTime);

        // Both timestamps should be identical on creation
        Assert.Equal(savedEntity.CreatedAt, savedEntity.UpdatedAt);
    }

    [Fact]
    public async Task UpdateShallow_WithAuditableEntity_UpdatesOnlyUpdatedAtTimestamp()
    {
        // Arrange
        var dbContext =
            TestDbContext.CreateContext(nameof(UpdateShallow_WithAuditableEntity_UpdatesOnlyUpdatedAtTimestamp));
        await SeedTestData(dbContext, 1);
        var repository = new Repository<AuditableEntity, Guid>(dbContext);

        var entity = (await repository.GetRangeAsync()).First();
        var originalCreatedAt = entity.CreatedAt;
        var originalUpdatedAt = entity.UpdatedAt;

        // Wait a moment to ensure timestamp difference
        await Task.Delay(10);
        var beforeUpdateTime = DateTime.UtcNow;

        // Modify the entity
        entity.Name = "Updated Auditable Entity Name";

        // Act
        repository.UpdateShallow(entity);
        await dbContext.SaveChangesAsync();
        var afterUpdateTime = DateTime.UtcNow;

        // Assert
        var updatedEntity = await repository.GetAsync(entity.Id);
        Assert.NotNull(updatedEntity);

        // CreatedAt should remain unchanged
        Assert.Equal(originalCreatedAt, updatedEntity.CreatedAt);

        // UpdatedAt should be updated by the interceptor
        Assert.NotEqual(originalUpdatedAt, updatedEntity.UpdatedAt);
        Assert.True(updatedEntity.UpdatedAt >= beforeUpdateTime);
        Assert.True(updatedEntity.UpdatedAt <= afterUpdateTime);
    }

    [Fact]
    public async Task UpdateDeep_WithAuditableEntity_UpdatesOnlyUpdatedAtTimestamp()
    {
        // Arrange
        var dbContext =
            TestDbContext.CreateContext(nameof(UpdateDeep_WithAuditableEntity_UpdatesOnlyUpdatedAtTimestamp));
        await SeedTestData(dbContext, 1);
        var repository = new Repository<AuditableEntity, Guid>(dbContext);

        var entity = (await repository.GetRangeAsync()).First();
        var originalCreatedAt = entity.CreatedAt;
        var originalUpdatedAt = entity.UpdatedAt;

        // Wait a moment to ensure timestamp difference
        await Task.Delay(10);
        var beforeUpdateTime = DateTime.UtcNow;

        // Modify the entity
        entity.Description = "Updated Description for Deep Update";

        // Act
        repository.UpdateDeep(entity);
        await dbContext.SaveChangesAsync();
        var afterUpdateTime = DateTime.UtcNow;

        // Assert
        var updatedEntity = await repository.GetAsync(entity.Id);
        Assert.NotNull(updatedEntity);

        // CreatedAt should remain unchanged
        Assert.Equal(originalCreatedAt, updatedEntity.CreatedAt);

        // UpdatedAt should be updated by the interceptor
        Assert.NotEqual(originalUpdatedAt, updatedEntity.UpdatedAt);
        Assert.True(updatedEntity.UpdatedAt >= beforeUpdateTime);
        Assert.True(updatedEntity.UpdatedAt <= afterUpdateTime);
    }

    [Fact]
    public async Task AddRangeAsync_WithAuditableEntities_SetsAllTimestamps()
    {
        // Arrange
        var dbContext = TestDbContext.CreateContext(nameof(AddRangeAsync_WithAuditableEntities_SetsAllTimestamps));
        var repository = new Repository<AuditableEntity, Guid>(dbContext);
        var beforeTime = DateTime.UtcNow;

        var entities = new List<AuditableEntity>
        {
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Batch Entity 1",
                Description = "First in batch"
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Batch Entity 2",
                Description = "Second in batch"
            }
        };

        // Act
        await repository.AddAsync(entities);
        await dbContext.SaveChangesAsync();
        var afterTime = DateTime.UtcNow;

        // Assert
        var savedEntities = await repository.GetRangeAsync();
        Assert.Equal(2, savedEntities.Count);

        foreach (var entity in savedEntities)
        {
            // Check that dates were set automatically by the interceptor
            Assert.True(entity.CreatedAt >= beforeTime);
            Assert.True(entity.CreatedAt <= afterTime);
            Assert.True(entity.UpdatedAt >= beforeTime);
            Assert.True(entity.UpdatedAt <= afterTime);

            // Both timestamps should be identical on creation
            Assert.Equal(entity.CreatedAt, entity.UpdatedAt);
        }
    }

    [Fact]
    public async Task EntityChanges_AcrossMultipleOperations_MaintainsCorrectAuditTrail()
    {
        // Arrange
        var dbContext =
            TestDbContext.CreateContext(nameof(EntityChanges_AcrossMultipleOperations_MaintainsCorrectAuditTrail));
        var repository = new Repository<AuditableEntity, Guid>(dbContext);

        // Add an entity
        var entity = new AuditableEntity
        {
            Id = Guid.NewGuid(),
            Name = "Audit Trail Test Entity",
            Description = "Initial description"
        };

        // Act - Step 1: Create the entity
        await repository.AddAsync(entity);
        await dbContext.SaveChangesAsync();
        var creationTime = entity.CreatedAt;

        // Wait a moment to ensure timestamp difference
        await Task.Delay(10);

        // Act - Step 2: Update the entity
        var retrievedEntity = await repository.GetAsync(entity.Id);
        Assert.NotNull(retrievedEntity);
        retrievedEntity.Name = "First Update";
        repository.UpdateShallow(retrievedEntity);
        await dbContext.SaveChangesAsync();
        var firstUpdateTime = retrievedEntity.UpdatedAt;

        // Wait a moment to ensure timestamp difference
        await Task.Delay(10);

        // Act - Step 3: Update again
        retrievedEntity = await repository.GetAsync(entity.Id);
        Assert.NotNull(retrievedEntity);
        retrievedEntity.Description = "Updated description";
        repository.UpdateShallow(retrievedEntity);
        await dbContext.SaveChangesAsync();
        var secondUpdateTime = retrievedEntity.UpdatedAt;

        // Assert
        var finalEntity = await repository.GetAsync(entity.Id);
        Assert.NotNull(finalEntity);

        // CreatedAt should remain unchanged across all operations
        Assert.Equal(creationTime, finalEntity.CreatedAt);

        // UpdatedAt should change with each update through the interceptor
        Assert.NotEqual(creationTime, firstUpdateTime);
        Assert.NotEqual(firstUpdateTime, secondUpdateTime);

        // Each timestamp should be later than the previous
        Assert.True(firstUpdateTime > creationTime);
        Assert.True(secondUpdateTime > firstUpdateTime);
    }

    [Fact]
    public async Task DeleteAsync_WithSoftDeletableEntity_SetsDeletionTimeStamp()
    {
        // Arrange
        var dbContext = TestDbContext.CreateContext(nameof(DeleteAsync_WithSoftDeletableEntity_SetsDeletionTimeStamp));
        await SeedTestData(dbContext, 3);
        var repository = new Repository<SoftDeletableEntity, Guid>(dbContext);

        var entities = await repository.GetRangeAsync();
        var entityToDelete = entities.First();
        var entityId = entityToDelete.Id;

        // Act
        await repository.DeleteAsync(entityId);
        await dbContext.SaveChangesAsync();

        // Assert
        // Entity should no longer be accessible through normal repository queries
        var deletedEntity = await repository.GetAsync(entityId);
        Assert.Null(deletedEntity);

        // But we can verify it exists with DeletedAt set by using IgnoreQueryFilters
        var deletedEntityIgnoringFilters = await repository.GetAsync(
            entityId,
            QueryOptions.WithSoftDeleted
        );

        Assert.NotNull(deletedEntityIgnoringFilters);
        Assert.NotNull(deletedEntityIgnoringFilters.DeletedAt);
    }

    [Fact]
    public async Task DeleteRangeAsync_WithSoftDeletableEntities_SetsDeletionTimeStamp()
    {
        // Arrange
        var dbContext =
            TestDbContext.CreateContext(nameof(DeleteRangeAsync_WithSoftDeletableEntities_SetsDeletionTimeStamp));
        await SeedTestData(dbContext, 5);
        var repository = new Repository<SoftDeletableEntity, Guid>(dbContext);

        var entitiesToDelete = await repository.GetRangeAsync();
        var idsToDelete = entitiesToDelete.Take(3).Select(e => e.Id).ToList();

        // Act
        await repository.DeleteRangeAsync(idsToDelete);
        await dbContext.SaveChangesAsync();

        // Assert
        // Entities should no longer be accessible through normal repository queries
        var remainingEntities = await repository.GetRangeAsync(idsToDelete);
        Assert.Empty(remainingEntities);

        // But we can verify they exist with DeletedAt set by using IgnoreQueryFilters
        var deletedEntities = await repository.GetRangeAsync(
            x => x.Where(e => idsToDelete.Contains(e.Id)),
            QueryOptions.WithSoftDeleted
        );

        Assert.Equal(3, deletedEntities.Count);
        Assert.All(deletedEntities, entity => Assert.NotNull(entity.DeletedAt));
    }

    [Fact]
    public async Task DeleteAsync_WithMixedEntityTypes_HandlesEachTypeAppropriately()
    {
        // Arrange
        var dbContext =
            TestDbContext.CreateContext(nameof(DeleteAsync_WithMixedEntityTypes_HandlesEachTypeAppropriately));
        await SeedTestData(dbContext, 3);

        var regularRepository = new Repository<Entity, Guid>(dbContext);
        var softDeletableRepository = new Repository<SoftDeletableEntity, Guid>(dbContext);

        var regularEntity = (await regularRepository.GetRangeAsync()).First();
        var softDeletableEntity = (await softDeletableRepository.GetRangeAsync()).First();

        // Act
        await regularRepository.DeleteAsync(regularEntity.Id);
        await softDeletableRepository.DeleteAsync(softDeletableEntity.Id);
        await dbContext.SaveChangesAsync();

        // Assert
        // Regular entity should be physically removed
        Assert.Null(await regularRepository.GetAsync(regularEntity.Id));

        // Soft-deletable entity should still exist but be marked as deleted
        Assert.Null(await softDeletableRepository.GetAsync(softDeletableEntity.Id));

        // Verify soft-deletable entity exists when ignoring filters
        var deletedSoftEntity = await softDeletableRepository.GetAsync(
            x => x.Where(e => e.Id == softDeletableEntity.Id),
            QueryOptions.WithSoftDeleted
        );

        Assert.NotNull(deletedSoftEntity);
        Assert.NotNull(deletedSoftEntity.DeletedAt);
    }

    [Fact]
    public async Task DeleteRangeAsync_WithMixedBatch_HandlesEachEntityTypeAppropriately()
    {
        // Arrange
        var dbContext =
            TestDbContext.CreateContext(nameof(DeleteRangeAsync_WithMixedBatch_HandlesEachEntityTypeAppropriately));
        await SeedTestData(dbContext, 5);

        var regularRepository = new Repository<Entity, Guid>(dbContext);
        var softDeletableRepository = new Repository<SoftDeletableEntity, Guid>(dbContext);

        var regularEntities = await regularRepository.GetRangeAsync();
        var regularIds = regularEntities.Take(2).Select(e => e.Id).ToList();

        var softDeletableEntities = await softDeletableRepository.GetRangeAsync();
        var softDeletableIds = softDeletableEntities.Take(2).Select(e => e.Id).ToList();

        // Act
        await regularRepository.DeleteRangeAsync(regularIds);
        await softDeletableRepository.DeleteRangeAsync(softDeletableIds);
        await dbContext.SaveChangesAsync();

        // Assert
        // Regular entities should be physically removed
        Assert.Empty(await regularRepository.GetRangeAsync(regularIds));

        // Soft-deletable entities should not be visible in normal queries
        Assert.Empty(await softDeletableRepository.GetRangeAsync(softDeletableIds));

        // But they should be visible when ignoring filters
        var deletedSoftEntities = await softDeletableRepository.GetRangeAsync(
            x => x.Where(e => softDeletableIds.Contains(e.Id)),
            QueryOptions.WithSoftDeleted
        );

        Assert.Equal(2, deletedSoftEntities.Count);
        Assert.All(deletedSoftEntities, entity => Assert.NotNull(entity.DeletedAt));
    }

    [Fact]
    public async Task GetAsync_WithSoftDeletedEntity_ReturnsNull()
    {
        // Arrange
        var dbContext = TestDbContext.CreateContext(nameof(GetAsync_WithSoftDeletedEntity_ReturnsNull));
        await SeedTestData(dbContext, 3);
        var repository = new Repository<SoftDeletableEntity, Guid>(dbContext);

        var entities = await repository.GetRangeAsync();
        var entity = entities.First();

        // Soft delete the entity
        await repository.DeleteAsync(entity.Id);
        await dbContext.SaveChangesAsync();

        // Act
        var result = await repository.GetAsync(entity.Id);

        // Assert
        Assert.Null(result);

        // Verify the entity still exists when ignoring filters
        var softDeletedEntity =
            await repository.GetAsync(x => x.Where(e => e.Id == entity.Id), QueryOptions.WithSoftDeleted);
        Assert.NotNull(softDeletedEntity);
        Assert.NotNull(softDeletedEntity.DeletedAt);
    }

    [Fact]
    public async Task GetRangeAsync_ExcludesSoftDeletedEntities()
    {
        // Arrange
        var dbContext = TestDbContext.CreateContext(nameof(GetRangeAsync_ExcludesSoftDeletedEntities));
        await SeedTestData(dbContext, 5);
        var repository = new Repository<SoftDeletableEntity, Guid>(dbContext);

        // Mark 2 entities as deleted using repository
        var entities = await repository.GetRangeAsync();
        var entitiesToDelete = entities.Take(2).ToList();

        foreach (var entity in entitiesToDelete)
        {
            await repository.DeleteAsync(entity.Id);
        }

        await dbContext.SaveChangesAsync();

        // Act
        var result = await repository.GetRangeAsync();

        // Assert
        Assert.Equal(3, result.Count);

        // Verify all entities exist when ignoring filters
        var allEntities = await repository.GetRangeAsync(QueryOptions.WithSoftDeleted);
        Assert.Equal(5, allEntities.Count);
    }

    [Fact]
    public async Task ApplyQueryIncludes_SupportsThreeLevelDeepNavigation()
    {
        // Arrange
        var dbContext = TestDbContext.CreateContext(nameof(ApplyQueryIncludes_SupportsThreeLevelDeepNavigation));
        await SeedTestData(dbContext, 5);
        var repository = new Repository<NavigationEntity, Guid>(dbContext);

        // Act
        var result = await repository.GetRangeAsync(x => x
            .Where(y => y.Parent == null)
            .Include(y => y.Parent)
            .ThenInclude(y => y!.Parent)
            .Include(y => y.Parent)
            .ThenInclude(y => y!.Children)
            .Include(y => y.Children)
        );

        // Assert
        Assert.Equal(5, result.Count);

        foreach (var entity in result)
        {
            Assert.NotNull(entity.Children);
            Assert.True(entity.Children.Count > 0, "Each entity should have children based on SeedTestData");

            Assert.NotNull(entity.Children[0].Children);
            Assert.True(entity.Children[0].Children.Count > 0,
                "Each entity should have grand children based on SeedTestData");

            Assert.NotNull(entity.Children[0].Parent);
            Assert.NotNull(entity.Children[0].Children[0].Parent!.Parent);
        }
    }

    [Fact]
    public async Task AddAsync_WithInvalidComplexEntity_ThrowsValidationException()
    {
        // Arrange
        var dbContext =
            TestDbContext.CreateContext(nameof(AddAsync_WithInvalidComplexEntity_ThrowsValidationException));
        var repository = new Repository<ComplexEntity, Guid>(dbContext);

        var invalidEntity = new ComplexEntity
        {
            Id = Guid.NewGuid(),
            BaseValue = 5,
            BelowBaseValue = 10 // Invalid: Bar > Foo violates validation rule
        };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ValidationException>(async () =>
        {
            await repository.AddAsync(invalidEntity);
            await dbContext.SaveChangesAsync();
        });

        // Verify the validation message matches our expectation
        Assert.Contains("Bar is not allowed to be greater than Foo", exception.Detail);
    }

    [Fact]
    public async Task AddAsync_WithValidComplexEntity_AddsEntityToDatabase()
    {
        // Arrange
        var dbContext = TestDbContext.CreateContext(nameof(AddAsync_WithValidComplexEntity_AddsEntityToDatabase));
        var repository = new Repository<ComplexEntity, Guid>(dbContext);

        var validEntity = new ComplexEntity
        {
            Id = Guid.NewGuid(),
            BaseValue = 10,
            BelowBaseValue = 5 // Valid: Bar < Foo
        };

        // Act
        await repository.AddAsync(validEntity);
        await dbContext.SaveChangesAsync();

        // Assert
        var savedEntity = await repository.GetAsync(validEntity.Id);
        Assert.NotNull(savedEntity);
        Assert.Equal(validEntity.Id, savedEntity.Id);
        Assert.Equal(10, savedEntity.BaseValue);
        Assert.Equal(5, savedEntity.BelowBaseValue);
    }

    [Fact]
    public async Task UpdateShallow_WithInvalidComplexEntity_ThrowsValidationException()
    {
        // Arrange
        var dbContext =
            TestDbContext.CreateContext(nameof(UpdateShallow_WithInvalidComplexEntity_ThrowsValidationException));
        var repository = new Repository<ComplexEntity, Guid>(dbContext);

        // First add a valid entity
        var entity = new ComplexEntity
        {
            Id = Guid.NewGuid(),
            BaseValue = 10,
            BelowBaseValue = 5
        };

        await repository.AddAsync(entity);
        await dbContext.SaveChangesAsync();

        // Now try to update it with invalid values
        var retrievedEntity = await repository.GetAsync(entity.Id);
        Assert.NotNull(retrievedEntity);

        retrievedEntity.BaseValue = 3; // Reduce Foo to make Bar > Foo

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ValidationException>(async () =>
        {
            repository.UpdateShallow(retrievedEntity);
            await dbContext.SaveChangesAsync();
        });

        Assert.Contains("Bar is not allowed to be greater than Foo", exception.Detail);
    }

    [Fact]
    public async Task UpdateDeep_WithInvalidComplexEntity_ThrowsValidationException()
    {
        // Arrange
        var dbContext =
            TestDbContext.CreateContext(nameof(UpdateDeep_WithInvalidComplexEntity_ThrowsValidationException));
        var repository = new Repository<ComplexEntity, Guid>(dbContext);

        // First add a valid entity
        var entity = new ComplexEntity
        {
            Id = Guid.NewGuid(),
            BaseValue = 10,
            BelowBaseValue = 5
        };

        await repository.AddAsync(entity);
        await dbContext.SaveChangesAsync();

        // Now try to update it with invalid values
        var retrievedEntity = await repository.GetAsync(entity.Id);
        Assert.NotNull(retrievedEntity);

        retrievedEntity.BelowBaseValue = 15; // Increase Bar to make Bar > Foo

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ValidationException>(async () =>
        {
            repository.UpdateDeep(retrievedEntity);
            await dbContext.SaveChangesAsync();
        });

        Assert.Contains("Bar is not allowed to be greater than Foo", exception.Detail);
    }

    [Fact]
    public async Task UpdateShallow_WithChangedUnchangeableValue_ThrowsValidationException()
    {
        // Arrange
        var dbContext =
            TestDbContext.CreateContext(nameof(UpdateShallow_WithChangedUnchangeableValue_ThrowsValidationException));
        var repository = new Repository<ComplexEntity, Guid>(dbContext);

        // First add a valid entity with an initial UnchangeableValue
        var entity = new ComplexEntity
        {
            Id = Guid.NewGuid(),
            BaseValue = 10,
            BelowBaseValue = 5,
            UnchangeableValue = 42
        };

        await repository.AddAsync(entity);
        await dbContext.SaveChangesAsync();

        // Now try to update it with a modified UnchangeableValue
        var retrievedEntity = await repository.GetAsync(entity.Id);
        Assert.NotNull(retrievedEntity);

        retrievedEntity.UnchangeableValue = 100; // Try to change the unchangeable value

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ValidationException>(async () =>
        {
            repository.UpdateShallow(retrievedEntity);
            await dbContext.SaveChangesAsync();
        });

        // Verify the validation message matches our expectation
        Assert.Contains("UnchangeableValue cannot be modified once set", exception.Detail);
    }

    [Fact]
    public async Task UpdateDeep_WithChangedUnchangeableValue_ThrowsValidationException()
    {
        // Arrange
        var dbContext =
            TestDbContext.CreateContext(nameof(UpdateDeep_WithChangedUnchangeableValue_ThrowsValidationException));
        var repository = new Repository<ComplexEntity, Guid>(dbContext);

        // First add a valid entity with an initial UnchangeableValue
        var entity = new ComplexEntity
        {
            Id = Guid.NewGuid(),
            BaseValue = 10,
            BelowBaseValue = 5,
            UnchangeableValue = 42
        };

        await repository.AddAsync(entity);
        await dbContext.SaveChangesAsync();

        // Now try to update it with a modified UnchangeableValue
        var retrievedEntity = await repository.GetAsync(entity.Id);
        Assert.NotNull(retrievedEntity);

        retrievedEntity.UnchangeableValue = 100; // Try to change the unchangeable value

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ValidationException>(async () =>
        {
            repository.UpdateDeep(retrievedEntity);
            await dbContext.SaveChangesAsync();
        });

        // Verify the validation message matches our expectation
        Assert.Contains("UnchangeableValue cannot be modified once set", exception.Detail);
    }

    [Fact]
    public async Task Update_WithSameUnchangeableValue_Succeeds()
    {
        // Arrange
        var dbContext = TestDbContext.CreateContext(nameof(Update_WithSameUnchangeableValue_Succeeds));
        var repository = new Repository<ComplexEntity, Guid>(dbContext);

        // First add a valid entity with an initial UnchangeableValue
        var entity = new ComplexEntity
        {
            Id = Guid.NewGuid(),
            BaseValue = 10,
            BelowBaseValue = 5,
            UnchangeableValue = 42
        };

        await repository.AddAsync(entity);
        await dbContext.SaveChangesAsync();

        // Now try to update other properties but keep UnchangeableValue the same
        var retrievedEntity = await repository.GetAsync(entity.Id);
        Assert.NotNull(retrievedEntity);

        retrievedEntity.BaseValue = 20; // Change other property
        retrievedEntity.BelowBaseValue = 10; // Change other property
        // UnchangeableValue remains the same

        // Act
        repository.UpdateShallow(retrievedEntity);
        await dbContext.SaveChangesAsync();

        // Assert
        var updatedEntity = await repository.GetAsync(entity.Id);
        Assert.NotNull(updatedEntity);
        Assert.Equal(20, updatedEntity.BaseValue);
        Assert.Equal(10, updatedEntity.BelowBaseValue);
        Assert.Equal(42, updatedEntity.UnchangeableValue);
    }
}