﻿using SkySensorsAPI.Utilities;
using System.Net.NetworkInformation;
using System.Text.Json.Serialization;

namespace SkySensorsAPI.Models;

// This model represents the data which the endpoints response with
public class WeatherStationDTO
{
	[JsonConverter(typeof(PhysicalAddressConverter))] // Needed to convert PhysicalAddress to string when used in endpoint result
	public PhysicalAddress MacAddress { get; set; }
    public GpsLocation GpsLocation { get; set; }
    public List<SensorDTO> Sensors { get; set; }
}