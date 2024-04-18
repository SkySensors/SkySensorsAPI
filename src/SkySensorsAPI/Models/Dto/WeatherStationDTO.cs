﻿using SkySensorsAPI.Models.Infrastructure;
using SkySensorsAPI.Utilities;
using System.Net.NetworkInformation;
using System.Text.Json.Serialization;

namespace SkySensorsAPI.Models.Dto;

public class WeatherStationDTO
{
	[JsonConverter(typeof(PhysicalAddressConverter))] // Needed to convert PhysicalAddress to string when used in endpoint result
	public required PhysicalAddress MacAddress { get; set; }
	public required GpsLocation GpsLocation { get; set; }
	public List<MeasuredSensorValuesDTO> Sensors { get; set; } = [];

	public static WeatherStationDTO FromWeatherStation(WeatherStation weatherStation, List<MeasuredSensorValuesDTO> measuredSensorValuesDTO)
	{
		return new WeatherStationDTO()
		{
			MacAddress = weatherStation.MacAddress,
			GpsLocation = new GpsLocation() { Latitude = weatherStation.Lat, Longitude = weatherStation.Lon },
			Sensors = measuredSensorValuesDTO,
		};
	}
}
