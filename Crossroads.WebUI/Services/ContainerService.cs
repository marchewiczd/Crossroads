using Crossroads.Utils.Data;
using Crossroads.Utils.Database.Models;
using Crossroads.Utils.Docker;
using Crossroads.Utils.Services;

namespace Crossroads.WebUI.Services;

public class ContainerService : IContainerService
{
    private readonly ILogger<ContainerService> _logger;
    private readonly WebApiService _apiService;
    private readonly DockerStatus _dockerStatus;

    public ContainerService(ILogger<ContainerService> logger, WebApiService apiService)
    {
        _logger = logger;
        _apiService = apiService;
        _dockerStatus = new DockerStatus();
    }
    
    public async Task<bool> CreateMapping(DockerNameMappingDto mapping)
    {
        try
        {
            await _apiService.PostAsync("name", mapping);
        }
        catch (Exception e)
        {
            _logger.LogError("Exception was thrown when creating mapping: {message}, stack trace: {innerException}", 
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
        var containersInfo = await _apiService.GetAsync("containers");
        var customMapping = await _apiService.GetAsync<List<CustomContainerInfo>>("custom");
        var nameMapping = await _apiService.GetAsync<List<DockerNameMapping>>("name");
        
        var containerList = _dockerStatus.GetCustomContainerModel(customMapping, nameMapping);
        containerList.AddRange(_dockerStatus.GetContainerModel(containersInfo, nameMapping));
        
        _logger.LogDebug("Response content: {content}", containersInfo);
        _logger.LogDebug("Container list: {containerList}", containerList);

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
            result = await _apiService.DeleteAsync("name", id);
        }
        catch (Exception e)
        {
            _logger.LogError("Exception was thrown when creating mapping: {message}, stack trace: {innerException}", 
                e.Message, e.InnerException);
            return false;
        }

        return result;
    }
}