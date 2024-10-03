using Crossroads.Utils.Data;
using Crossroads.Utils.Database.Models;

namespace Crossroads.WebUI.Services;

public interface IContainerService
{
    Task<bool> CreateMapping(DockerNameMappingDto mapping);

    Task<ContainerDto?> GetAsync(int id);

    Task<IEnumerable<ContainerDto>> GetAllAsync();

    Task<bool> UpdateAsync(ContainerDto customer);

    Task<bool> DeleteAsync(int id);
}