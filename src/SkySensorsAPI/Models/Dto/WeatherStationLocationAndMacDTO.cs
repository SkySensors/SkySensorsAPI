using SkySensorsAPI.Models.Infrastructure;
using SkySensorsAPI.Utilities;
using System.Net.NetworkInformation;
using System.Text.Json.Serialization;

namespace SkySensorsAPI.Models.Dto;

public class WeatherStationLocationAndMacDTO
{
    [JsonConverter(typeof(PhysicalAddressConverter))] // Needed to convert PhysicalAddress to string when used in endpoint result
    public required PhysicalAddress MacAddress { get; set; }
    public required GpsLocation GpsLocation { get; set; }

	public static WeatherStationLocationAndMacDTO FromWeatherStation(WeatherStation weatherStationDB)
	{
		return new WeatherStationLocationAndMacDTO
		{
			MacAddress = weatherStationDB.MacAddress,
			GpsLocation = new GpsLocation() { Latitude = weatherStationDB.Lat, Longitude = weatherStationDB.Lon },
		};
	}
}
