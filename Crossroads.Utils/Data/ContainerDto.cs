using Crossroads.Utils.Docker.Enums;

namespace Crossroads.Utils.Data;

public record ContainerDto(
    int MappingId,
    string Name, 
    string Ipv4, 
    string Port, 
    Status Status,
    bool IsMapped,
    string DockerImage);