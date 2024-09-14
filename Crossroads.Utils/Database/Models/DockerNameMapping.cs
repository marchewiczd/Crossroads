using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Crossroads.Utils.Database.Models;

[Table("docker_name_mapping")]
public record DockerNameMapping
{
    [Key]
    [Column("id")]
    public required int Id { get; init; }
    
    [Column("docker_container_name")]
    public required string ContainerName { get; init; }
    
    [Column("description")]
    public required string Description { get; init; }
}