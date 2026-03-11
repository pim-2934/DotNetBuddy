using DotNetBuddy.Domain;
using DotNetBuddy.Example.Contracts;
using DotNetBuddy.Example.Entities;
using DotNetBuddy.Example.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DotNetBuddy.Example.Controllers;

[ApiController]
[Route("[controller]")]
public class LocationController(IExtendedUnitOfWork extendedUnitOfWork, IValidationService validationService)
    : ControllerBase
{
    [HttpPost("CreateLocation")]
    public async Task<ActionResult<Location>> CreateLocation(
        LocationCreateDto dto,
        CancellationToken cancellationToken = default)
    {
        var location = new Location
        {
            Id = Guid.NewGuid(),
            Name = dto.Name
        };

        await extendedUnitOfWork.Repository<Location, Guid>().AddAsync(location, cancellationToken);
        await extendedUnitOfWork.SaveAsync(cancellationToken);

        return location;
    }
}