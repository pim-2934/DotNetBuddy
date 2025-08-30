# DotNetBuddy
[![Source](https://img.shields.io/badge/-Source%20Code-181717?logo=GitHub&style=flat-square)](https://github.com/pim-2934/DotNetBuddy)
[![License](https://img.shields.io/github/license/pim-2934/DotNetBuddy?style=flat-square)](https://github.com/pim-2934/DotNetBuddy/blob/main/LICENSE)

## Overview
**DotNetBuddy** is a .NET framework designed to simplify and streamline common application concerns such as configuration,
dependency injection, database access, auditing, exception handling, and data seeding.

It encourages clean architecture principles and supports out-of-the-box patterns like Unit of Work and Repository.

## 📦 NuGet Packages

| Project                                        | Version                                                                                                                                                                | Downloads                                                                                                                                                      |
|------------------------------------------------|------------------------------------------------------------------------------------------------------------------------------------------------------------------------|----------------------------------------------------------------------------------------------------------------------------------------------------------------|
| **DotNetBuddy.Domain**                         | [![NuGet](https://img.shields.io/nuget/v/DotNetBuddy.Domain?style=flat-square)](https://www.nuget.org/packages/DotNetBuddy.Domain/)                                    | [![Downloads](https://img.shields.io/nuget/dt/DotNetBuddy.Domain?style=flat-square)](https://www.nuget.org/packages/DotNetBuddy.Domain/)                       |
| **DotNetBuddy.Application**                    | [![NuGet](https://img.shields.io/nuget/v/DotNetBuddy.Application?style=flat-square)](https://www.nuget.org/packages/DotNetBuddy.Application/)                          | [![Downloads](https://img.shields.io/nuget/dt/DotNetBuddy.Application?style=flat-square)](https://www.nuget.org/packages/DotNetBuddy.Application/)             |
| **DotNetBuddy.Infrastructure**                 | [![NuGet](https://img.shields.io/nuget/v/DotNetBuddy.Infrastructure?style=flat-square)](https://www.nuget.org/packages/DotNetBuddy.Infrastructure/)                    | [![Downloads](https://img.shields.io/nuget/dt/DotNetBuddy.Infrastructure?style=flat-square)](https://www.nuget.org/packages/DotNetBuddy.Infrastructure/)       |
| **DotNetBuddy.Presentation**                   | [![NuGet](https://img.shields.io/nuget/v/DotNetBuddy.Presentation?style=flat-square)](https://www.nuget.org/packages/DotNetBuddy.Presentation/)                        | [![Downloads](https://img.shields.io/nuget/dt/DotNetBuddy.Presentation?style=flat-square)](https://www.nuget.org/packages/DotNetBuddy.Presentation/)           |
| **DotNetBuddy.Extensions.EntityFrameworkCore** | [![NuGet](https://img.shields.io/nuget/v/DotNetBuddy.Extensions.EntityFrameworkCore?style=flat-square)](https://www.nuget.org/packages/DotNetBuddy.Extensions.EntityFrameworkCore/) | [![Downloads](https://img.shields.io/nuget/dt/DotNetBuddy.Extensions.EntityFrameworkCore?style=flat-square)](https://www.nuget.org/packages/DotNetBuddy.Extensions.EntityFrameworkCore/) |

## Quick Start

### 1) Install packages
Using CLI:
- dotnet add package DotNetBuddy.Application
- dotnet add package DotNetBuddy.Domain
- dotnet add package DotNetBuddy.Infrastructure
- dotnet add package DotNetBuddy.Presentation

EF Core support (optional):
- dotnet add package DotNetBuddy.Extensions.EntityFrameworkCore

### 2) Register services (Program.cs)
```csharp
using DotNetBuddy.Infrastructure.Extensions;
using DotNetBuddy.Extensions.EntityFrameworkCore.Extensions; // optional EF helpers

var builder = WebApplication.CreateBuilder(args);

// Discover installers and configs, register core services
builder.Services.AddBuddy();

// EF Core helpers: IRepository/UnitOfWork for your DbContext
builder.Services.AddBuddyEfExtension<DatabaseContext>();

// Your DbContext with Buddy interceptors (audit, validation, etc.)
builder.Services.AddDbContext<DatabaseContext>((_, options) =>
{
    options
        .AddBuddyInterceptors()
        // choose your provider
        //.UseSqlServer(builder.Configuration.GetConnectionString("Default"))
        //.UseNpgsql(builder.Configuration.GetConnectionString("Default"))
        ;
});
```

### 3) Enable global exception handling
```csharp
using DotNetBuddy.Presentation.Extensions;

var app = builder.Build();
app.UseBuddyExceptions();
```

### 4) Seed data (optional)
Create a seeder by implementing ISeeder. It will be discovered and registered automatically.
```csharp
using DotNetBuddy.Domain;

public class DevUserSeeder(IUnitOfWork uow) : ISeeder
{
    public string[] Environments => new[] { "Development" };

    public async Task SeedAsync(CancellationToken ct = default)
    {
        if (!await uow.Repository<User, Guid>().AnyAsync(x => x.Where(y.Email == "admin@example.com"), ct))
        {
            await uow.Repository<User, Guid>().AddAsync(new User { Email = "admin@example.com" }, ct);
            await uow.SaveAsync(ct);
        }
    }
}
```
Run seeders manually when needed:
```csharp
using DotNetBuddy.Application.Utilities;

await BuddyUtils.RunSeeders(app.Services, app.Environment);
```
Or run them automatically on app start via configuration (appsettings.json):
```json
{
  "Buddy": {
    "RunSeedersOnBoot": true
  }
}
```

That’s it. Map your endpoints/controllers and run the app.

---

## Documentation
- [Configuration System](./Docs/Configs.md)
- [Exception Handling](./Docs/Exceptions.md)
- [Installers](./Docs/Installers.md)

## Extension: EntityFrameworkCore
- [Audit System](./Docs/Audit.md)
- [Entity Validation](./Docs/Validation.md)
- [Database Access](./Docs/Database.md)
- [Data Seeding](./Docs/Seeders.md)
- [Soft Deletion](./Docs/SoftDelete.md)

---

## Requirements

- .NET 8 or higher
- Microsoft.Extensions.DependencyInjection
- Microsoft.EntityFrameworkCore

---

For more examples and patterns, explore the individual markdown files linked above, or look at the example application
source code.



