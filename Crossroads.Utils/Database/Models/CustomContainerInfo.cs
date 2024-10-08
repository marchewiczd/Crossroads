﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Crossroads.Utils.Docker.Enums;

namespace Crossroads.Utils.Database.Models;

[Table("custom_container_info")]
public record CustomContainerInfo
{
    [Key]
    [Column("id")]
    public required int Id { get; init; }
    
    [Column("port")]
    public required string Port { get; init; }
    
    [Column("container_name")]
    public required string ContainerName { get; init; }
    
    [Column("status")]
    public required Status Status { get; init; }
}