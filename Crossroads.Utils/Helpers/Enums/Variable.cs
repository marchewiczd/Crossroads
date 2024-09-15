using System.ComponentModel;

namespace Crossroads.Utils.Helpers.Enums;

public enum Variable
{
    [Description("CROSSROADS_API_SERVICE")]
    ApiServiceName,
    
    [Description("CROSSROADS_API_PORT")]
    ApiPort,
    
    [Description("USE_API_WITH_HTTPS")]
    ApiHttps,
    
    [Description("HOST_IP_ADDRESS")]
    HostIpAddress,
}