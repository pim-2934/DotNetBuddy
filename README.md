[![Nuget](https://img.shields.io/nuget/v/DotNetBuddy?style=flat-square)](https://www.nuget.org/packages/DotNetBuddy/)
[![License](https://img.shields.io/github/license/pim-2934/DotNetBuddy?style=flat-square)](https://github.com/pim-2934/DotNetBuddy/blob/main/LICENSE)
[![Downloads](https://img.shields.io/nuget/dt/dotnetbuddy?style=flat-square)](https://www.nuget.org/packages/DotNetBuddy/)
[![GitHub](https://img.shields.io/badge/-source-181717.svg?logo=GitHub)](https://github.com/pim-2934/DotNetBuddy)

# DotNetBuddy

## Overview

**DotNetBuddy** is a .NET framework designed to simplify and streamline common application concerns such as configuration,
dependency injection, database access, auditing, exception handling, and data seeding.

It encourages clean architecture principles and supports out-of-the-box patterns like Unit of Work and Repository.

---

## Quick Start

### 1. Install DotNetBuddy

```csharp
builder.Services.AddBuddy<DatabaseContext>();
```

### 2. Configure Database and Auditing

```csharp
services.AddDbContext<DatabaseContext>((provider, options) =>
{
    options.AddBuddyInterceptors(provider);
});
```

### 3. Enable Global Exception Handling

```csharp
app.UseBuddyExceptions();
```

### 4. Create Your First Seeder (Optional)

```csharp
public class DevUserSeeder(IUnitOfWork uow) : ISeeder
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

Run seeders with:

```csharp
await BuddyUtils.RunSeeders(services);
```

Or allow seeders to run on boot:

```json
{
  "Buddy": {
    "RunSeedersOnBoot": true
  }
}
```

---

## Documentation

- [Audit System](./DotNetBuddy/Docs/Audit.md)
- [Configuration System](./DotNetBuddy/Docs/Configs.md)
- [Database Access](./DotNetBuddy/Docs/Database.md)
- [Exception Handling](./DotNetBuddy/Docs/Exceptions.md)
- [Installers](./DotNetBuddy/Docs/Installers.md)
- [Data Seeding](./DotNetBuddy/Docs/Seeders.md)

---

## Requirements

- .NET 8 or higher
- Microsoft.Extensions.DependencyInjection
- Microsoft.EntityFrameworkCore

---

For more examples and patterns, explore the individual markdown files linked above, or look at the example application
source code.



