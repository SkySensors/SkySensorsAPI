using Microsoft.AspNetCore.Mvc;
using SkySensorsAPI.ApplicationServices;
using SkySensorsAPI.Models;

namespace SkySensorsAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class WheatherStationController(
	IWheatherStationAppService weatherStationService) : ControllerBase
{
	[HttpGet]
	public async Task<IActionResult> GetWeatherStation(string macAddress = "00-b0-d0-63-c2-26")
	{
        if (macAddress == null)
        {
			List<WeatherStationDTO> weatherStations = await weatherStationService.GetWeatherStations();

			return weatherStations == null ? NotFound() : Ok(weatherStations);
		}		
		
		WeatherStationDTO? weatherStation = await weatherStationService.GetWeatherStation(macAddress);
		return weatherStation == null ? NotFound() : Ok(weatherStation);
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

	[HttpGet("list")]
	public async Task<IActionResult> GetWeatherStationLists()
	{
		IEnumerable<BasicWeatherStationDTO> weatherStations = await weatherStationService.GetWeatherStationLists();

		return weatherStations == null ? NotFound() : Ok(weatherStations);
	}

}
