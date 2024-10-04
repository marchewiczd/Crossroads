using Crossroads.Utils.Data;

namespace Crossroads.WebUI.Services;

public interface IContainerService
{
    Task<bool> CreateMapping(DockerNameMappingDto mapping);

    Task<ContainerDto?> GetAsync(int id);

    Task<IEnumerable<ContainerDto>> GetAllAsync();

    Task<bool> UpdateAsync(ContainerDto customer);

    Task<bool> DeleteAsync(int id);
}