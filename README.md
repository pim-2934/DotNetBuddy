# DotNetBuddy
[![Source](https://img.shields.io/badge/-Source%20Code-181717?logo=GitHub&style=flat-square)](https://github.com/pim-2934/DotNetBuddy)
[![License](https://img.shields.io/github/license/pim-2934/DotNetBuddy?style=flat-square)](https://github.com/pim-2934/DotNetBuddy/blob/main/LICENSE)

## Overview
**DotNetBuddy** is a .NET framework designed to simplify and streamline common application concerns such as configuration,
dependency injection, database access, auditing, exception handling, and data seeding.

It encourages clean architecture principles and supports out-of-the-box patterns like Unit of Work and Repository.

## 📦 NuGet Packages

| Project                      | Version                                                                                                              | Downloads                                                                                                                |
|-----------------------------|----------------------------------------------------------------------------------------------------------------------|---------------------------------------------------------------------------------------------------------------------------|
| **DotNetBuddy.Domain**       | [![NuGet](https://img.shields.io/nuget/v/DotNetBuddy.Domain?style=flat-square)](https://www.nuget.org/packages/DotNetBuddy.Domain/)               | [![Downloads](https://img.shields.io/nuget/dt/DotNetBuddy.Domain?style=flat-square)](https://www.nuget.org/packages/DotNetBuddy.Domain/)               |
| **DotNetBuddy.Application**  | [![NuGet](https://img.shields.io/nuget/v/DotNetBuddy.Application?style=flat-square)](https://www.nuget.org/packages/DotNetBuddy.Application/)     | [![Downloads](https://img.shields.io/nuget/dt/DotNetBuddy.Application?style=flat-square)](https://www.nuget.org/packages/DotNetBuddy.Application/)     |
| **DotNetBuddy.Infrastructure** | [![NuGet](https://img.shields.io/nuget/v/DotNetBuddy.Infrastructure?style=flat-square)](https://www.nuget.org/packages/DotNetBuddy.Infrastructure/) | [![Downloads](https://img.shields.io/nuget/dt/DotNetBuddy.Infrastructure?style=flat-square)](https://www.nuget.org/packages/DotNetBuddy.Infrastructure/) |
| **DotNetBuddy.Presentation** | [![NuGet](https://img.shields.io/nuget/v/DotNetBuddy.Presentation?style=flat-square)](https://www.nuget.org/packages/DotNetBuddy.Presentation/)   | [![Downloads](https://img.shields.io/nuget/dt/DotNetBuddy.Presentation?style=flat-square)](https://www.nuget.org/packages/DotNetBuddy.Presentation/)   |

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
await BuddyUtils.RunSeeders(services, hostEnvironment);
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

- [Audit System](./Docs/Audit.md)
- [Soft Deletion](./Docs/SoftDelete.md)
- [Configuration System](./Docs/Configs.md)
- [Database Access](./Docs/Database.md)
- [Exception Handling](./Docs/Exceptions.md)
- [Installers](./Docs/Installers.md)
- [Data Seeding](./Docs/Seeders.md)

---

## Requirements

- .NET 8 or higher
- Microsoft.Extensions.DependencyInjection
- Microsoft.EntityFrameworkCore

---

For more examples and patterns, explore the individual markdown files linked above, or look at the example application
source code.



