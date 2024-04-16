using Microsoft.AspNetCore.Mvc;
using SkySensorsAPI.ApplicationServices;
using SkySensorsAPI.Models;

namespace SkySensorsAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class WhetherStationController(
	IWhetherStationAppService weatherStationService) : ControllerBase
{
	[HttpGet]
	public async Task<IActionResult> GetWeatherStation(string macAddress = "00-b0-d0-63-c2-26")
	{
		WeatherStation? weatherStation = await weatherStationService.GetWeatherStation(macAddress);

		return weatherStation == null ? NotFound() : Ok(weatherStation);
	}
	[HttpGet]
	public async Task<IActionResult> GetWeatherStations()
	{
		List<WeatherStation> weatherStations = await weatherStationService.GetWeatherStations();

		return weatherStations == null ? NotFound() : Ok(weatherStations);
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
