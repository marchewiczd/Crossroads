using Crossroads.Database.Entities;
using Crossroads.Database.Entities.Enums;
using Crossroads.Utils.Data;
using Crossroads.Utils.Helpers;
using Crossroads.Utils.Helpers.Enums;
using Microsoft.Extensions.Logging;
using Environment = Crossroads.Utils.Helpers.Environment;

namespace Crossroads.Utils.Docker;

public class DockerStatus(ILogger<DockerStatus> logger)
{
    private const int NameIndex = 0;
    private const int StatusIndex = 1;
    private const int ImageNameIndex = 2;
    private const int PortIndex = 3;

    public List<ContainerDto> GetContainerModel(string containersInfoString, List<DockerNameMapping> nameMapping)
    {
        var ip = Environment.GetVariable(Variable.HostIpAddress);
        var list = SplitContainersInfo(containersInfoString);
        var result = new List<ContainerDto>();

        foreach (var containerStatusString in list)
        {
            logger.LogDebug("Parsing container information: {status}.", containerStatusString);
            logger.LogDebug("Length: {length}", containerStatusString.Length);
            
            if (string.IsNullOrWhiteSpace(containerStatusString))
            {
                logger.LogDebug("Empty string detected, skipping.");
                continue;
            }
            
            var containerInfoArray = containerStatusString.Split(',');
            var descriptionDto = 
                GetDescriptionFromName(containerInfoArray[NameIndex], nameMapping);
            var port = GetPort(containerInfoArray[PortIndex]);
            
            var container = new ContainerDto(
                descriptionDto.MappingId,
                descriptionDto.Description, 
                string.IsNullOrEmpty(port) ? "" : ip, 
                port, 
                GetContainerStatus(containerInfoArray[StatusIndex]),
                descriptionDto.IsMapped,
                containerInfoArray[ImageNameIndex]);
                    
            result.Add(container);
            logger.LogDebug("Created container record: {container}", container);
        }

        return result.ToList();
    }
    
    public List<ContainerDto> GetCustomContainerModel(
        List<CustomContainerInfo> customContainers, 
        List<DockerNameMapping> nameMapping)
    {
        var ip = Environment.GetVariable(Variable.HostIpAddress);
        var result = new List<ContainerDto>();

        foreach (var containerInfo in customContainers)
        {
            logger.LogDebug("Parsing custom container information: {info}", containerInfo);
            var descriptionDto = 
                GetDescriptionFromName(containerInfo.ContainerName, nameMapping);

            result.Add(new ContainerDto(
                descriptionDto.MappingId,
                descriptionDto.Description,
                ip,
                containerInfo.Port,
                containerInfo.Status,
                descriptionDto.IsMapped,
                containerInfo.DockerImageName));
        }

        return result.ToList();
    }
    
    private static DescriptionDto GetDescriptionFromName(
        string containerName, 
        List<DockerNameMapping> nameMapping)
    {
        var mapIndex = nameMapping.FindIndex(mapping => mapping.ContainerName == containerName);

        return mapIndex == -1
            ? new DescriptionDto(-1, false, containerName)
            : new DescriptionDto(nameMapping[mapIndex].Id, true, nameMapping[mapIndex].Description);
    }
        
    private static List<string> SplitContainersInfo(string containersInfo) =>
        containersInfo.Split("\n").ToList();

    private static Status GetContainerStatus(string statusString) =>
        statusString switch
        {
            not null when statusString.Contains("exited", StringComparison.InvariantCultureIgnoreCase) => Status.Exited,
            not null when statusString.Contains("up", StringComparison.InvariantCultureIgnoreCase) => Status.Running,
            not null when statusString.Contains("created", StringComparison.InvariantCultureIgnoreCase) => Status.Created,
            _ => Status.Unknown
        };

    private static string GetPort(string address)
    {
        var port = RegexHelper.Port().Match(address).Value;
        return string.IsNullOrEmpty(port) ? "" : port.Remove(port.Length - 1);
    }
}