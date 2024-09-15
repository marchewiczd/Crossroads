using Crossroads.Utils.Database;
using Crossroads.Utils.Database.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Crossroads.WebApi.Controllers;

[ApiController]
[Route("[controller]")]
public class MappingController(ILogger<MappingController> logger, CrossroadsContext crossroadsContext) : ControllerBase
{
    [HttpGet("/name", Name = "GetNameMapping")]
    public List<DockerNameMapping> GetNameMappingAsync()
    { 
        logger.LogDebug("GetMapping");
        return crossroadsContext.DockerNameMappings.ToList();
    }
    
    [HttpGet("/name/{container}", Name = "GetNameByContainerMapping")]
    public async Task<DockerNameMapping> GetNameMappingAsync(string container)
    { 
        logger.LogDebug("GetNameMappingAsync: {name}", container);
        return await crossroadsContext.DockerNameMappings
            .FirstAsync(entry => entry.ContainerName == container);
    }
    
    [HttpPost("/name", Name = "PostNameMapping")]
    public async Task PostNameMappingAsync(DockerNameMapping mapping)
    { 
        logger.LogDebug("PostNameMappingAsync: {mapping}", mapping);
        await crossroadsContext.DockerNameMappings.AddAsync(mapping);
        await crossroadsContext.SaveChangesAsync();
    }
    
    [HttpGet("/custom", Name = "GetCustomContainerInfo")]
    public List<CustomContainerInfo> GetCustomContainerInfoAsync()
    { 
        logger.LogDebug("GetCustomContainerInfoAsync");
        return crossroadsContext.CustomContainerInfos.ToList();
    }
    
    [HttpGet("/custom/{container}", Name = "GetCustomContainerInfoByContainer")]
    public async Task<CustomContainerInfo> GetCustomContainerInfoAsync(string container)
    { 
        logger.LogDebug("GetCustomContainerInfoAsync: {name}", container);
        return await crossroadsContext.CustomContainerInfos
            .FirstAsync(entry => entry.ContainerName == container);
    }
    
    [HttpPost("/custom", Name = "PostCustomContainerInfo")]
    public async Task PostCustomContainerInfoAsync(CustomContainerInfo customInfo)
    { 
        logger.LogDebug("PostCustomContainerInfoAsync: {customInfo}", customInfo);
        await crossroadsContext.CustomContainerInfos.AddAsync(customInfo);
        await crossroadsContext.SaveChangesAsync();
    }
}