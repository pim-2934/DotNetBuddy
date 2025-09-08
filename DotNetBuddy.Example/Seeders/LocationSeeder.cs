using DotNetBuddy.Application.Utilities;
using DotNetBuddy.Domain;
using DotNetBuddy.Domain.Attributes;
using DotNetBuddy.Example.Entities;
using DotNetBuddy.Example.Repositories.Interfaces;

namespace DotNetBuddy.Example.Seeders;

[SeedPriority(2000000000)]
public class LocationSeeder(IExtendedUnitOfWork unitOfWork) : ISeeder
{
    public string[] Environments =>
    [
        "Development",
        "Local",
        "Staging",
        "Production"
    ];

    private static readonly string[] Locations =
    [
        "Grand Canyon",
        "Great Barrier Reef",
        "Mount Everest",
        "Victoria Falls",
        "Northern Lights"
    ];

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        foreach (var location in Locations)
        {
            await SeederHelper.SeedOneAsync(
                unitOfWork,
                BuddyUtils.GenerateDeterministicGuid(location),
                new Location
                {
                    Name = location
                },
                cancellationToken
            );
        }

        await unitOfWork.SaveAsync(cancellationToken);
    }
}