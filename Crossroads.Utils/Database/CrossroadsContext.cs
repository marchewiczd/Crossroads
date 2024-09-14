using Crossroads.Utils.Database.Models;
using Microsoft.EntityFrameworkCore;

namespace Crossroads.Utils.Database;

public class CrossroadsContext(string dataSource) : DbContext, ICrossroadsContext
{
    public DbSet<DockerNameMapping> DockerNameMappings { get; set; }
    public DbSet<CustomContainerInfo> CustomContainerInfos { get; set; }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) =>
        optionsBuilder.UseSqlite($"Data Source={dataSource}");
}