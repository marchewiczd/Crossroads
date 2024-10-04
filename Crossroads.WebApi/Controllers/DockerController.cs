using Crossroads.Utils.CLI;
using Microsoft.AspNetCore.Mvc;

namespace Crossroads.WebApi.Controllers;

[ApiController]
[Route("api/containers")]
public class DockerController(ILogger<DockerController> logger) : ControllerBase
{
    [HttpGet(Name = "GetContainers")]
    public async Task<string> GetContainers()
    { 
#if DEBUG
        logger.LogWarning("Using static file for GetContainers API");
        return await System.IO.File.ReadAllTextAsync(Path.Combine(Environment.CurrentDirectory, "..", ".testdata"));
#else
        logger.LogDebug("GetContainers");
        return await LinuxShell.RunCommandAsync("docker", "ps -a --format \"{{.Names}},{{.Status}},{{.Image}},{{.Ports}}\"");
#endif
    }
    
    // TODO: add support for controlling containers
}