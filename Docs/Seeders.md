# Data Seeders

## Purpose

Allows environment-specific, idempotent database seeding.

## Interface

- `ISeeder`  
  Marker interface for seeder classes.

## Usage

```csharp
public class DevSeeder(IUnitOfWork uow) : ISeeder
{
    public string[] Environments => new[] { "Development" };

    public async Task SeedAsync()
    {
        if (!await uow.Repository<User>().AnyAsync(u => u.Email == "admin@example.com"))
        {
            await uow.Repository<User>().AddAsync(new User { Email = "admin@example.com" });
            await uow.SaveAsync();
        }
    }
}
```

## Usage (Deterministic Guid, SeederHelper)
```csharp
public class DevSeeder(IUnitOfWork uow) : ISeeder
{
    public string[] Environments => new[] { "Development" };

    public async Task SeedAsync()
    {
        var guid = BuddyUtils.GenerateDeterministicGuid("admin@example.com");
        
        await SeederHelper.SeedOneAsync
        (
            unitOfWork,
            guid,
            new User 
            {
                Id = guid,
                Email = "admin@example.com"
            }
        );
    }
}
```

## Execution

Set config:

```json
{
  "Buddy": {
    "RunSeedersOnBoot": true
  }
}
```

Or run programmatically via:

```csharp
await BuddyUtils.RunSeeders(services, hostEnvironment);
```

## Notes

- Seeders only run for environments listed in `Environments`.
- Uses `IUnitOfWork` for transactions and repository access.

