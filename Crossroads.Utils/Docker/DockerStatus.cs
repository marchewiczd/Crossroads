using System.Diagnostics;
using Crossroads.Utils.Data;
using Crossroads.Utils.Database.Models;
using Crossroads.Utils.Docker.Enums;
using Crossroads.Utils.Helpers.Enums;
using Environment = Crossroads.Utils.Helpers.Environment;

namespace Crossroads.Utils.Docker;

public class DockerStatus
{
    private const int NameIndex = 0;
    private const int StatusIndex = 1;
    private const int ImageNameIndex = 2;
    private const int PortIndex = 3;
    private const int NoIpLength = 4;
    
    //ps -a --format \"{{.Names}},{{.Status}},{{.Image}},{{.Ports}}\"
    public List<ContainerDto> GetContainerModel(string containersInfoString, List<DockerNameMapping> nameMapping)
    {
        var ip = Environment.GetVariable(Variable.HostIpAddress);
        var list = SplitContainersInfo(containersInfoString);
        var result = new List<ContainerDto>();

        foreach (var containerStatusString in list)
        {
            Debug.WriteLine($"Parsing container information: {containerStatusString}");
            var containerInfoArray = containerStatusString.Split(',');
            ContainerDto? container;
            var desc = GetDescriptionFromName(containerInfoArray[NameIndex], nameMapping, out var id, out var isMapped);
            
            switch (containerInfoArray.Length)
            {
                case NoIpLength:
                    Debug.WriteLine("Detected string with length 3");
                    container =
                        new ContainerDto(
                            id,
                            desc, 
                            string.Empty, 
                            string.Empty, 
                            GetContainerStatus(containerInfoArray[StatusIndex]),
                            isMapped,
                            containerInfoArray[ImageNameIndex]);
                    
                    result.Add(container);
                    Debug.WriteLine($"Created container record: {container}");
                    break;
                
                case NoIpLength + 1:
                    Debug.WriteLine("Detected string with length 4");
                    container = new ContainerDto(
                        id,
                        desc,
                        ip,
                        GetPort(containerInfoArray[PortIndex]),
                        GetContainerStatus(containerInfoArray[StatusIndex]),
                        isMapped,
                        containerInfoArray[ImageNameIndex]);
                    
                    result.Add(container);
                    Debug.WriteLine($"Created container record: {container}");
                    break;
                
                default:
                    Debug.WriteLine($"Unknown string, length of {containerInfoArray.Length} cannot be parsed");
                    break;
            }
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
            Debug.WriteLine($"Parsing custom container information: {containerInfo}");
            var desc = GetDescriptionFromName(containerInfo.ContainerName, nameMapping, out var id, out var isMapped);

            result.Add(new ContainerDto(
                id,
                desc,
                ip,
                containerInfo.Port,
                containerInfo.Status,
                isMapped,
                containerInfo.DockerImageName));
        }

        return result.ToList();
    }
    
    private static string GetDescriptionFromName(
        string containerName, 
        List<DockerNameMapping> nameMapping, 
        out int mappingId, 
        out bool isMapped)
    {
        var mapIndex = nameMapping.FindIndex(mapping => mapping.ContainerName == containerName);
        
        if (mapIndex == -1)
        {
            mappingId = -1;
            isMapped = false;
            return containerName;
        }

        mappingId = nameMapping[mapIndex].Id;
        isMapped = true;
        return nameMapping[mapIndex].Description;
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
        var arrowIndex = address.IndexOf('>');
        if (arrowIndex == -1)
            throw new ArgumentException("IP does not contain port");
        
        return address.Substring(arrowIndex - 5, 4);
    }
}