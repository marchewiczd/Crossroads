using Crossroads.Utils.Extensions;
using Crossroads.Utils.Helpers.Enums;

namespace Crossroads.Utils.Helpers;

public static class Environment
{
    public static string GetVariable(Variable variable) => 
        System.Environment.GetEnvironmentVariable(variable.GetDescription()) ?? string.Empty;
}