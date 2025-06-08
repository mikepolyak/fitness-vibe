using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace FitnessVibe.Infrastructure.Data;

/// <summary>
/// Factory class for creating DbContext instances at design time.
/// This is used by EF Core tools when adding migrations or updating the database.
/// </summary>
public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<FitnessVibeDbContext>
{
    /// <summary>
    /// Creates a new instance of FitnessVibeDbContext at design time
    /// </summary>
    /// <param name="args">Arguments passed to the context factory</param>
    /// <returns>A new instance of FitnessVibeDbContext</returns>
    public FitnessVibeDbContext CreateDbContext(string[] args)
    {
        // Get the environment from args or default to Development
        var environment = args.Length > 0 ? args[0] : "Development";

        // Build configuration
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false)
            .AddJsonFile($"appsettings.{environment}.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        // Configure DbContext
        var optionsBuilder = new DbContextOptionsBuilder<FitnessVibeDbContext>();
        
        // Get connection string with fallback to local development database
        var connectionString = configuration.GetConnectionString("DefaultConnection") 
            ?? "Server=(localdb)\\mssqllocaldb;Database=FitnessVibe;Trusted_Connection=True;MultipleActiveResultSets=true";

        optionsBuilder.UseSqlServer(connectionString, opts => 
            opts.EnableRetryOnFailure(
                maxRetryCount: 3,
                maxRetryDelay: TimeSpan.FromSeconds(5),
                errorNumbersToAdd: null)
        );

        return new FitnessVibeDbContext(optionsBuilder.Options);
    }
}
