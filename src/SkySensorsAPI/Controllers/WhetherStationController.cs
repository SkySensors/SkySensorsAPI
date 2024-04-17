using Microsoft.AspNetCore.Mvc;
using SkySensorsAPI.ApplicationServices;
using SkySensorsAPI.Models;

namespace SkySensorsAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WheatherStationController(
	IWheatherStationAppService weatherStationService) : ControllerBase
{
	[HttpGet]
	public async Task<IActionResult> GetWeatherStation(string? macAddress, long startTime = 1713260957000, long endTime = 1713260957000)
	{
		if (macAddress == null)
		{
			List<WeatherStationDTO> weatherStations = await weatherStationService.GetWeatherStations(startTime, endTime);

			return weatherStations == null ? NotFound() : Ok(weatherStations);
		}

		// For compatability with frontend send the station in an array
		List<WeatherStationDTO> weatherStation = [];

		weatherStation.Add(await weatherStationService.GetWeatherStation(macAddress, startTime, endTime));

		return weatherStation == null ? NotFound() : Ok(weatherStation);
	}

	[HttpPost]
	public async Task<IActionResult> AddSensorValues()
	{
		return await weatherStationService.GetDummyValue() ? Ok() : NotFound();
	}

	[HttpPost("handshake")]
	public async Task<IActionResult> MakeWeatherStationHandshake(WeatherStationBasicDTO weatherStation)
	{
		// Insert or update weather station
		bool weatherStationOK = await weatherStationService.UpsertWeatherStation(weatherStation);

		if (!weatherStationOK)
		{
			return StatusCode(500);
		}

		// Add all sensors that does not exist
		foreach (var sensor in weatherStation.Sensors)
		{
			bool sensorOK = await weatherStationService.UpsertWeatherStationSensor(weatherStation.MacAddress, sensor.Type.ToString());

			if (!sensorOK)
			{
				return StatusCode(500);
			}
		}

		// Find the time schedule that would fit for this device

		return Ok();
	}

	[HttpGet("list")]
	public async Task<IActionResult> GetWeatherStationList()
	{
		IEnumerable<BasicWeatherStationDTO> weatherStations = await weatherStationService.GetWeatherStationLists();

		return weatherStations == null ? NotFound() : Ok(weatherStations);
	}

}
