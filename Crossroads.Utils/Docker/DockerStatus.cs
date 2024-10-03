using System.Diagnostics;
using System.Text.RegularExpressions;
using Crossroads.Utils.Data;
using Crossroads.Utils.Database.Models;
using Crossroads.Utils.Docker.Enums;
using Crossroads.Utils.Helpers.Enums;
using Environment = Crossroads.Utils.Helpers.Environment;

namespace Crossroads.Utils.Docker;

public partial class DockerStatus
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
            var desc = 
                GetDescriptionFromName(containerInfoArray[NameIndex], nameMapping, out var id, out var isMapped);
            var port = GetPort(containerInfoArray[PortIndex]);
            
            Debug.WriteLine("Detected string with length 3");
            container =
                new ContainerDto(
                    id,
                    desc, 
                    string.IsNullOrEmpty(port) ? "" : ip, 
                    port, 
                    GetContainerStatus(containerInfoArray[StatusIndex]),
                    isMapped,
                    containerInfoArray[ImageNameIndex]);
                    
            result.Add(container);
            Debug.WriteLine($"Created container record: {container}");
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

    // regex: "(\d{4}|443|80)-" + remove last char
    private static string GetPort(string address)
    {
        var port = PortRegex().Match(address).Value;
        if (string.IsNullOrEmpty(port))
            return "";

        return port.Remove(port.Length - 1);
    }

    [GeneratedRegex("(\\d{4}|443|80)-")]
    private static partial Regex PortRegex();
}