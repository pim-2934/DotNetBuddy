using DotNetBuddy.Example.Models;
using Microsoft.EntityFrameworkCore;

namespace DotNetBuddy.Example;

public class DatabaseContext(DbContextOptions<DatabaseContext> options) : DbContext(options)
{
    public DbSet<WeatherForecast> WeatherForecasts { get; set; }
}