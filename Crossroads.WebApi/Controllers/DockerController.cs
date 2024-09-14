using Crossroads.Utils.CLI;
using Microsoft.AspNetCore.Mvc;

namespace Crossroads.WebApi.Controllers;

[ApiController]
[Route("[controller]")]
public class DockerController(ILogger<DockerController> logger) : ControllerBase
{
    [HttpGet("/containers", Name = "GetContainers")]
    public async Task<string> GetContainers()
    { 
        logger.LogDebug("GetContainers");
        return await LinuxShell.RunCommandAsync("docker", "ps -a --format \"{{.Names}},{{.Ports}},{{.Status}}\"");
    }
}