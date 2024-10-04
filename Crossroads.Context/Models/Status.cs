using System.ComponentModel;

namespace Crossroads.Utils.Docker.Enums;

public enum Status
{
    [Description("A container that has never been started.")]
    Created,
    
    [Description("A running container, started by either docker start or docker run")]
    Running,
    
    [Description("A paused container")]
    Paused,
    
    [Description("A container which is starting due to the designated restart policy for that container.")]
    Restarting,
    
    [Description("A container which is no longer running")]
    Exited,
    
    [Description("A container which is in the process of being removed")]
    Removing,
    
    [Description("A \"defunct\" container; for example, a container that was only partially removed because resources were kept busy by an external process")]
    Dead,
    
    [Description("Cannot get container status")]
    Unknown
}