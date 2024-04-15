using Microsoft.AspNetCore.Mvc;
using SkySensorsAPI.ApplicationServices;

namespace SkySensorsAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class WhetherStationController(
	IWhetherStationAppService weatherStationService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetWeatherStation()
    {
        return await weatherStationService.GetDummyValue() ? Ok() : NotFound();
    }

    [HttpPost]
    public async Task<IActionResult> AddSensorValues()
    {
        return await weatherStationService.GetDummyValue() ? Ok() : NotFound();
    }

    [HttpGet("handshake")]
    public async Task<IActionResult> MakeWeatherStationHandshake()
    {
        return await weatherStationService.GetDummyValue() ? Ok() : NotFound();
    }

}
