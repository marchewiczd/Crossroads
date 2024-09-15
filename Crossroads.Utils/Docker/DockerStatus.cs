using System.Diagnostics;
using Crossroads.Utils.Database.Models;
using Crossroads.Utils.Docker.Enums;
using Crossroads.Utils.Docker.Models;
using Crossroads.Utils.Helpers.Enums;
using Environment = Crossroads.Utils.Helpers.Environment;

namespace Crossroads.Utils.Docker;

public class DockerStatus()
{
    public List<Container> GetContainerInformation(string containersInfoString)
    {
        var ip = Environment.GetVariable(Variable.HostIpAddress);
        var list = SplitContainersInfo(containersInfoString);
        var result = new List<Container>();

        foreach (var containerStatusString in list)
        {
            Debug.WriteLine($"Parsing container information: {containerStatusString}");
            var containerInfoArray = containerStatusString.Split(',');
            Container? container;

            switch (containerInfoArray.Length)
            {
                case 3:
                    Debug.WriteLine("Detected string with length 3");
                    container =
                        new Container(
                            containerInfoArray[0], 
                            string.Empty, 
                            string.Empty, 
                            GetContainerStatus(containerInfoArray[2]));
                    
                    result.Add(container);
                    Debug.WriteLine($"Created container record: {container}");
                    break;
                
                case 4:
                    Debug.WriteLine("Detected string with length 4");
                    container = new Container(
                        containerInfoArray[0],
                        ip,
                        GetPort(containerInfoArray[1]),
                        GetContainerStatus(containerInfoArray[3]));
                    
                    result.Add(container);
                    Debug.WriteLine($"Created container record: {container}");
                    break;
                
                default:
                    Debug.WriteLine($"Unknown string, length of {containerInfoArray.Length} cannot be parsed");
                    break;
            }
        }

        return result;
    }    
    
    public List<Container> GetContainerModel(string containersInfoString, List<DockerNameMapping> nameMapping)
    {
        var ip = Environment.GetVariable(Variable.HostIpAddress);
        var list = SplitContainersInfo(containersInfoString);
        var result = new List<Container>();

        foreach (var containerStatusString in list)
        {
            Debug.WriteLine($"Parsing container information: {containerStatusString}");
            var containerInfoArray = containerStatusString.Split(',');
            Container? container;

            switch (containerInfoArray.Length)
            {
                case 3:
                    Debug.WriteLine("Detected string with length 3");
                    container =
                        new Container(
                            GetDescriptionFromName(containerInfoArray[0], nameMapping), 
                            string.Empty, 
                            string.Empty, 
                            GetContainerStatus(containerInfoArray[2]));
                    
                    result.Add(container);
                    Debug.WriteLine($"Created container record: {container}");
                    break;
                
                case 4:
                    Debug.WriteLine("Detected string with length 4");
                    container = new Container(
                        GetDescriptionFromName(containerInfoArray[0], nameMapping),
                        ip,
                        GetPort(containerInfoArray[1]),
                        GetContainerStatus(containerInfoArray[3]));
                    
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
    
    public List<Container> GetCustomContainerModel(
        List<CustomContainerInfo> customContainers, 
        List<DockerNameMapping> nameMapping)
    {
        var ip = Environment.GetVariable(Variable.HostIpAddress);
        var result = new List<Container>();

        foreach (var containerInfo in customContainers)
        {
            Debug.WriteLine($"Parsing custom container information: {containerInfo}");

            result.Add(new Container(
                GetDescriptionFromName(containerInfo.ContainerName, nameMapping),
                ip,
                containerInfo.Port,
                containerInfo.Status));
        }

        return result.ToList();
    }
    
    private static string GetDescriptionFromName(string containerName, List<DockerNameMapping> nameMapping) => 
        nameMapping.FirstOrDefault(mapping => mapping.ContainerName == containerName)?.Description ?? containerName;

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