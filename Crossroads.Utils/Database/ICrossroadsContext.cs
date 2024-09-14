using Crossroads.Utils.Database.Models;
using Microsoft.EntityFrameworkCore;

namespace Crossroads.Utils.Database;

public interface ICrossroadsContext
{
    public DbSet<DockerNameMapping> DockerNameMappings { get; set; }
    public DbSet<CustomContainerInfo> CustomContainerInfos { get; set; }
}