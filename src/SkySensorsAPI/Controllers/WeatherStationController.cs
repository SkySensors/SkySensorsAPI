using Microsoft.AspNetCore.Mvc;
using SkySensorsAPI.DomainServices;
using SkySensorsAPI.Models.DTO;
using System.Net.NetworkInformation;

namespace SkySensorsAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WeatherStationController(
	ITimeSlotDomainService timeSlotDomainService,
	IWeatherStationDomainService weatherStationDomainService) : ControllerBase
{
	[HttpGet]
	public async Task<ActionResult<List<WeatherStationDTO>>> GetWeatherStation(string macAddress, long startTime, long endTime, bool isCalibrated = true)
	{
		return new List<WeatherStationDTO>()
		{
			await weatherStationDomainService.GetWeatherStation(PhysicalAddress.Parse(macAddress), startTime, endTime, isCalibrated)
		};
	}

	[HttpGet("all")]
	public async Task<ActionResult<List<WeatherStationDTO>>> GetAllWeatherStations(long startTime, long endTime, bool isCalibrated = true)
	{
		List<WeatherStationDTO> weatherStations = await weatherStationDomainService.GetWeatherStations(startTime, endTime, isCalibrated);
		return weatherStations == null ? NotFound() : Ok(weatherStations);
	}

	[HttpPost]
	public async Task<ActionResult> AddSensorValues(MeasuredSensorValuesDTO[] measuredSensorValuesDTOs)
	{
		await weatherStationDomainService.InsertMeasuredSensorValues(measuredSensorValuesDTOs);
		return Ok();
	}

	[HttpPost("handshake")]
	public async Task<ActionResult<TimeSlotDTO>> MakeWeatherStationHandshake(WeatherStationBasicDTO weatherStation)
	{
		await weatherStationDomainService.UpsertWeatherStation(weatherStation);
		return await timeSlotDomainService.UpsertTimeSlot(weatherStation.MacAddress);
	}

	[HttpGet("list")]
	public async Task<ActionResult<IEnumerable<WeatherStationLocationAndMacDTO>>> GetAllLocationsAndMacAddresses()
	{
		IEnumerable<WeatherStationLocationAndMacDTO> weatherStations = await weatherStationDomainService.GetAllLocationsAndMacAddresses();
		return weatherStations == null ? NotFound() : Ok(weatherStations);
	}
}
