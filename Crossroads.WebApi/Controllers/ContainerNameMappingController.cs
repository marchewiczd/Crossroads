using Crossroads.Database.Context;
using Crossroads.Database.Entities;
using Crossroads.Utils.Data;
using Microsoft.AspNetCore.Mvc;

namespace Crossroads.WebApi.Controllers;

[ApiController]
[Route("api/name")]
public class ContainerNameMappingController(
    ILogger<ContainerNameMappingController> logger, 
    ICrossroadsContext crossroadsContext) 
    : ControllerBase
{
    [HttpGet("all", Name = "GetNameMapping")]
    public List<DockerNameMapping> GetNameMappingAsync()
    { 
        logger.LogDebug("GetMapping");
        return crossroadsContext.GetAll<DockerNameMapping>() ?? [];
    }
    
    [HttpGet(Name = "GetNameByContainerMapping")]
    public async Task<DockerNameMapping?> GetNameMappingAsync([FromQuery(Name = "container")] string container)
    { 
        logger.LogDebug("GetNameMappingAsync: {name}", container);
        return await crossroadsContext
            .GetAsync<DockerNameMapping>(entry => entry.ContainerName.Equals(container));
    }
    
    [HttpPost(Name = "PostNameMapping")]
    public async Task<bool> PostNameMappingAsync([FromBody] DockerNameMappingDto dto)
    { 
        logger.LogDebug("PostNameMappingAsync: {mapping}", dto);
        var foundDuplicate = await crossroadsContext
            .GetAsync<DockerNameMapping>(entry => entry.ContainerName.Equals(dto.ContainerName));

        if (foundDuplicate is not null)
            return false;
        
        await crossroadsContext.InsertAsync(new DockerNameMapping
        {
            Id = 0,
            ContainerName = dto.ContainerName!,
            Description = dto.Description!
        });

        return true;
    }
    
    [HttpDelete(Name = "DeleteNameMapping")]
    public async Task<bool> DeleteNameMapping([FromQuery(Name = "id")] int id)
    { 
        logger.LogDebug("DeleteNameMapping: {id}", id);
        return await crossroadsContext.DeleteAsync<DockerNameMapping>(id);
    }
}