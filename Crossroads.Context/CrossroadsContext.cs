using Crossroads.Context.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Crossroads.Context;

public class CrossroadsContext : DbContext
{
    private readonly ILogger<CrossroadsContext> _logger;
    
    public CrossroadsContext(ILogger<CrossroadsContext> logger)
    {
        _logger = logger;
    }

    public CrossroadsContext(DbContextOptions<CrossroadsContext> options, ILogger<CrossroadsContext> logger) 
        : base(options)
    {
        _logger = logger;
    }
    
    public DbSet<DockerNameMapping> DockerNameMappings { get; set; }
    public DbSet<CustomContainerInfo> CustomContainerInfos { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (optionsBuilder.IsConfigured) 
            return;
        
        var dir = Environment.CurrentDirectory;
        var path = string.Empty;
        
        if (dir.Contains("source") || dir.Contains("repo") || dir.Contains("repos"))
        {
            path = Path.Combine(dir, "bin", "Debug", "net7.0", "Database", "crossroads.db");
        }
        else
        {
            // Running in the <project> directory.
            path = Path.Combine(dir, "Database", "crossroads.db");
        }
        _logger.LogDebug("Database path: {path}", path);
        
        optionsBuilder.UseSqlite($"Filename={path}");
    }
}

public static class CrossroadsExtensions
{
    public static IServiceCollection AddCrossroadsContext(this IServiceCollection services, string relativePath = ".")
    {
        var databasePath = Path.Combine(relativePath, "Database", "crossroads.db");
        services.AddDbContext<CrossroadsContext>(options =>
        {
            options.UseSqlite($"Data Source={databasePath}");
            options.LogTo(Console.WriteLine, // Console
                new[] { Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.CommandExecuting });
        });

        return services;
    }
}