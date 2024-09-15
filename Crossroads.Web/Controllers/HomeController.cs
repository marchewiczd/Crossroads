using System.Diagnostics;
using Crossroads.Utils.Database.Models;
using Crossroads.Utils.Docker;
using Crossroads.Utils.Docker.Models;
using Crossroads.Utils.Services;
using Microsoft.AspNetCore.Mvc;
using Crossroads.Web.Models;

namespace Crossroads.Web.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly WebApiService _apiService;
    private readonly DockerStatus _dockerStatus;

    public HomeController(ILogger<HomeController> logger, WebApiService apiService)
    {
        _logger = logger;
        _apiService = apiService;
        _dockerStatus = new DockerStatus();
    }

    public async Task<IActionResult> Index() => 
        View(await GetHomeModel());

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    private async Task<List<Container>> GetHomeModel()
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
}