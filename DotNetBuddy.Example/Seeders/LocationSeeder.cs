using DotNetBuddy.Application.Utilities;
using DotNetBuddy.Domain;
using DotNetBuddy.Domain.Attributes;
using DotNetBuddy.Example.Models;
using DotNetBuddy.Example.Repositories.Interfaces;

namespace DotNetBuddy.Example.Seeders;

[SeedPriority(900)]
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
        "Grand Canyon", "Great Barrier Reef", "Mount Everest", "Victoria Falls", "Northern Lights"
    ];

    public async Task SeedAsync()
    {
        foreach (var location in Locations)
        {
            await SeederHelper.SeedOneAsync(
                unitOfWork,
                BuddyUtils.GenerateDeterministicGuid(location),
                new Location
                {
                    Name = location
                }
            );
        }
        
        await unitOfWork.SaveAsync();
    }
}