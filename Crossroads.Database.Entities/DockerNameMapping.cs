using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Crossroads.Database.Entities.Abstractions;

namespace Crossroads.Database.Entities;

[Table("docker_name_mapping")]
public record DockerNameMapping : TableBase
{
    [Key]
    [Column("id")]
    public required int Id { get; init; }
    
    [Column("docker_container_name")]
    public required string ContainerName { get; init; }
    
    [Column("description")]
    public required string Description { get; init; }
}