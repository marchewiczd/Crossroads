using System.Diagnostics;
using Crossroads.Utils.Docker.Enums;
using Crossroads.Utils.Docker.Models;

namespace Crossroads.Utils.Docker;

public class DockerStatus(string ipv4)
{
    public List<Container> GetContainerInformation(string containersInfoString)
    {
        var list = SplitContainersInfo(containersInfoString);
        var result = new List<Container>();

        foreach (var containerStatusString in list)
        {
            Debug.WriteLine($"Parsing container information: {containerStatusString}");
            var containerStatusStringList = containerStatusString.Split(',');
            Container? container;

            switch (containerStatusStringList.Length)
            {
                case 3:
                    Debug.WriteLine("Detected string with length 3");
                    container =
                        new Container(
                            containerStatusStringList[0], string.Empty, string.Empty, Status.Exited);
                    
                    result.Add(container);
                    Debug.WriteLine($"Created container record: {container}");
                    break;
                
                case 4:
                    Debug.WriteLine("Detected string with length 4");
                    container = new Container(
                        containerStatusStringList[0],
                        ipv4,
                        GetPort(containerStatusStringList[1]),
                        GetContainerStatus(containerStatusStringList[3]));
                    
                    result.Add(container);
                    Debug.WriteLine($"Created container record: {container}");
                    break;
                
                default:
                    Debug.WriteLine($"Unknown string, length of {containerStatusStringList.Length} cannot be parsed");
                    break;
            }
        }

        return result;
    }

    private static List<string> SplitContainersInfo(string containersInfo) =>
        containersInfo.Split("\n").ToList();

    private static Status GetContainerStatus(string statusString) =>
        statusString switch
        {
            not null when statusString.Contains("exited", StringComparison.InvariantCultureIgnoreCase) => Status.Exited,
            not null when statusString.Contains("up", StringComparison.InvariantCultureIgnoreCase) => Status.Running,
            not null when statusString.Contains("created", StringComparison.InvariantCultureIgnoreCase) => Status.Created,
            not null when statusString.Contains("exited", StringComparison.InvariantCultureIgnoreCase) => Status.Exited,
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