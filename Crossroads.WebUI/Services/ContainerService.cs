using Crossroads.Database.Entities;
using Crossroads.Utils.Data;
using Crossroads.Utils.Docker;

namespace Crossroads.WebUI.Services;

public class ContainerService(
    ILogger<ContainerService> logger,
    ILogger<DockerStatus> dockerStatusLogger,
    IWebApiService apiService)
    : IContainerService
{
    private readonly DockerStatus _dockerStatus = new(dockerStatusLogger);

    public async Task<bool> CreateMapping(DockerNameMappingDto mapping)
    {
        try
        {
            await apiService.PostAsync("api/name", mapping);
        }
        catch (Exception e)
        {
            logger.LogError("Exception was thrown when creating mapping: {message}, stack trace: {innerException}", 
                e.Message, e.InnerException);
            return false;
        }

        return true;
    }

    public Task<ContainerDto?> GetAsync(int id)
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<ContainerDto>> GetAllAsync()
    {
        var containersInfo = await apiService.GetAsync("api/containers");
        logger.LogDebug("Container string: {content}", containersInfo);
        
        var customMapping = await apiService.GetAsync<List<CustomContainerInfo>>("api/custom/all");
        var nameMapping = await apiService.GetAsync<List<DockerNameMapping>>("api/name/all");
        
        var containerList = _dockerStatus.GetCustomContainerModel(customMapping, nameMapping);
        containerList.AddRange(_dockerStatus.GetContainerModel(containersInfo, nameMapping));
        
        logger.LogDebug("Container list: {containerList}", containerList);

        return containerList;
    }

    public Task<bool> UpdateAsync(ContainerDto customer)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var result = false;
        
        try
        {
            result = await apiService.DeleteAsync("api/name", id);
        }
        catch (Exception e)
        {
            logger.LogError("Exception was thrown when creating mapping: {message}, stack trace: {innerException}", 
                e.Message, e.InnerException);
            return false;
        }

        return result;
    }
}