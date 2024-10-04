using Crossroads.Database.Context;
using Crossroads.Database.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Crossroads.WebApi.Controllers;

[ApiController]
[Route("api/custom")]
public class CustomContainerMappingController(
    ILogger<CustomContainerMappingController> logger, 
    ICrossroadsContext crossroadsContext) 
    : ControllerBase
{
    [HttpGet("all", Name = "GetCustomContainerInfo")]
    public List<CustomContainerInfo> GetCustomContainerInfoAsync()
    { 
        logger.LogDebug("GetCustomContainerInfoAsync");
        return crossroadsContext.GetAll<CustomContainerInfo>() ?? [];
    }
    
    [HttpGet(Name = "GetCustomContainerInfoByContainer")]
    public async Task<CustomContainerInfo?> GetCustomContainerInfoAsync([FromQuery(Name = "container")] string container)
    { 
        logger.LogDebug("GetCustomContainerInfoAsync: {name}", container);
        return await crossroadsContext
            .GetAsync<CustomContainerInfo>(entry => entry.ContainerName.Equals(container));
    }
    
    [HttpPost(Name = "PostCustomContainerInfo")]
    public async Task<bool> PostCustomContainerInfoAsync([FromBody] CustomContainerInfo customInfo)
    { 
        logger.LogDebug("PostCustomContainerInfoAsync: {customInfo}", customInfo);
        var foundDuplicate = await crossroadsContext
            .GetAsync<CustomContainerInfo>(entry => entry.ContainerName.Equals(customInfo.ContainerName));

        if (foundDuplicate is not null)
            return false;
        
        await crossroadsContext.InsertAsync(customInfo);

        return true;
    }
}