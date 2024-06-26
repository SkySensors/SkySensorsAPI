﻿using SkySensorsAPI.Models.Infrastructure;
using SkySensorsAPI.Utilities;
using System.Net.NetworkInformation;
using System.Text.Json.Serialization;

namespace SkySensorsAPI.Models.DTO;

public class WeatherStationLocationAndMacDTO
{
	[JsonConverter(typeof(PhysicalAddressConverter))] // Needed to convert PhysicalAddress to string when used in endpoint result
	public required PhysicalAddress MacAddress { get; set; }
	public required GpsLocationDTO GpsLocation { get; set; }

	public static WeatherStationLocationAndMacDTO FromWeatherStation(WeatherStation weatherStation)
	{
		return new WeatherStationLocationAndMacDTO
		{
			MacAddress = weatherStation.MacAddress,
			GpsLocation = new GpsLocationDTO() { Latitude = weatherStation.Lat, Longitude = weatherStation.Lon },
		};
	}
}
