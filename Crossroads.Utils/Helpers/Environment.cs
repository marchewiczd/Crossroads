using Crossroads.Utils.Extensions;
using Crossroads.Utils.Helpers.Enums;

namespace Crossroads.Utils.Helpers;

public static class Environment
{
    public static string GetVariable(Variable variable) => 
        System.Environment.GetEnvironmentVariable(variable.GetDescription()) ?? string.Empty;

    public static Uri GetUri()
    {
        var useHttps = 
            GetVariable(Variable.ApiHttps).Equals("true", StringComparison.InvariantCultureIgnoreCase);
        var protocol = useHttps ? "https" : "http";
        var serviceName = GetVariable(Variable.ApiServiceName);
        var servicePort = GetVariable(Variable.ApiPort);
        
        return new Uri($"{protocol}://{serviceName}:{servicePort}/", UriKind.Absolute);
    }
}