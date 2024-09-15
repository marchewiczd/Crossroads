using CliWrap;
using CliWrap.Buffered;

namespace Crossroads.Utils.CLI;

public static class CommandLine
{
    public static async Task<string> RunCommandAsync(string arguments)
    {
        var result = await Cli.Wrap("cmd.exe")
            .WithArguments(arguments)
            .ExecuteBufferedAsync();

        return result.StandardOutput;
    }
}