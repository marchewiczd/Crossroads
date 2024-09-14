using Microsoft.AspNetCore.Mvc;

namespace Crossroads.WebApi.Controllers;

[ApiController]
[Route("[controller]")]
public class MappingController(ILogger<MappingController> logger) : ControllerBase
{
    //TODO: add stuff related to DB
}