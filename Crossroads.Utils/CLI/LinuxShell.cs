using CliWrap;
using CliWrap.Buffered;

namespace Crossroads.Utils.CLI;

public static class LinuxShell
{
    public static async Task<string> RunCommandAsync(string command, string arguments)
    {
        var result = await Cli.Wrap(command)
            .WithArguments(arguments)
            .ExecuteBufferedAsync();

        return result.StandardOutput;
    }
}