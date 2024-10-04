using System.Linq.Expressions;
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
    
    private DbSet<DockerNameMapping> DockerNameMappingSet { get; init; }
    private DbSet<CustomContainerInfo> CustomContainerInfoSet { get; init; }
    
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
        return dbSet is null ? null : await dbSet.FindAsync(key);
    }

    public async Task<T?> GetAsync<T>(Expression<Func<T, bool>> expression) where T : TableBase
    {
        var dbSet = GetDbSet<T>();
        return dbSet is null ? null : await dbSet.FirstOrDefaultAsync(expression);
    }

    public T? Get<T>(object key) where T : TableBase => 
        GetDbSet<T>()?.Find(key);

    public T? Get<T>(Expression<Func<T, bool>> expression) where T : TableBase => 
        GetDbSet<T>()?.FirstOrDefault(expression);

    public List<T>? GetAll<T>() where T : TableBase => 
        GetDbSet<T>()?.ToList();

    public bool Delete<T>(object key) where T : TableBase
    {
        var dbSet = GetDbSet<T>();
        var record = dbSet?.Find(key);

        if (dbSet is null || record is null) 
            return false;
        
        dbSet.Remove(record);
            
        return SaveChanges() != 0;
    }

    public async Task<bool> DeleteAsync<T>(object key) where T : TableBase
    {
        var dbSet = GetDbSet<T>();
        if (dbSet is null)
            return false;
        
        var record = await dbSet.FindAsync(key);
        if (record is null) 
            return false;
        
        dbSet.Remove(record);
        return await SaveChangesAsync() != 0;
    }
        
    public new bool Update<T>(T record) where T : TableBase
    {
        var dbSet = GetDbSet<T>();

        if (dbSet is null)
            return false;

        dbSet.Update(record);
        return true;
    }

    public bool Insert<T>(T entity) where T : TableBase
    {
        var dbSet = GetDbSet<T>();

        if (dbSet is null)
            return false;
        
        dbSet.Add(entity);
        return SaveChanges() != 0;
    }

    public async Task<bool> InsertAsync<T>(T entity) where T : TableBase
    {
        var dbSet = GetDbSet<T>();

        if (dbSet is null)
            return false;   
        
        await dbSet.AddAsync(entity);
        return await SaveChangesAsync() != 0;
    }

    #endregion

    #region Private methods

    private DbSet<T>? GetDbSet<T>() where T : TableBase =>
        GetType().GetProperty($"{typeof(T).Name}Set", BindingFlags.NonPublic | BindingFlags.Instance)?.GetValue(this, null) as DbSet<T>;

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