using Crossroads.Utils.Docker.Enums;

namespace Crossroads.Utils.Docker.Models;

public record Container(string Name, string Ipv4, string Port, Status Status);