using System.Reflection;
using Crossroads.Database.Entities;
using Crossroads.Database.Entities.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Crossroads.Database.Context;

public class CrossroadsContext : DbContext, ICrossroadsContext
{
    private readonly ILogger<CrossroadsContext> _logger;

    #region Constructors

    public CrossroadsContext(ILogger<CrossroadsContext> logger)
    {
        _logger = logger;
    }

    public CrossroadsContext(DbContextOptions<CrossroadsContext> options, ILogger<CrossroadsContext> logger) 
        : base(options)
    {
        _logger = logger;
    }

    #endregion

    #region DbSets
    // ReSharper disable UnusedMember.Local
    
    private DbSet<DockerNameMapping> DockerNameMappings { get; init; }
    private DbSet<CustomContainerInfo> CustomContainerInfos { get; init; }
    
    // ReSharper restore UnusedMember.Local
    #endregion

    #region Configuration

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (optionsBuilder.IsConfigured) 
            return;
        
        var dir = Environment.CurrentDirectory;
        string path;
        
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

    #endregion

    #region Access methods

    public async Task<T?> GetAsync<T>(object key) where T : TableBase
    {
        var dbSet = GetDbSet<T>();
        return dbSet is null ? null : await dbSet.FindAsync(key);;
    }

    public T? Get<T>(object key) where T : TableBase => 
        GetDbSet<T>()?.Find(key);

    public List<T>? GetAll<T>() where T : TableBase => 
        GetDbSet<T>()?.ToList();

    public bool Delete<T>(object key) where T : TableBase
    {
        var dbSet = GetDbSet<T>();
        var record = dbSet?.Find(key);

        if (dbSet is null || record is null) 
            return false;
        
        dbSet.Remove(record);
        SaveChanges();
            
        return true;
    }
        
    public new bool Update<T>(T record) where T : TableBase
    {
        var dbSet = GetDbSet<T>();

        if (dbSet is null)
            return false;

        dbSet.Update(record);
        return true;
    }

    #endregion

    #region Private methods

    private DbSet<T>? GetDbSet<T>() where T : TableBase =>
        GetType().GetProperty(typeof(T).Name, BindingFlags.NonPublic | BindingFlags.Instance)?.GetValue(this, null) as DbSet<T>;

    #endregion
}

public static class CrossroadsExtensions
{
    public static IServiceCollection AddCrossroadsContext(this IServiceCollection services, string relativePath = ".")
    {
        var databasePath = Path.Combine(relativePath, "Database", "crossroads.db");
        services.AddDbContext<ICrossroadsContext, CrossroadsContext>(options =>
        {
            options.UseSqlite($"Data Source={databasePath}");
            options.LogTo(Console.WriteLine, // Console
                new[] { Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.CommandExecuting });
        });

        return services;
    }
}