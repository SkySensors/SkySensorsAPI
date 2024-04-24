using SkySensorsAPI.Utilities;
using System.Net.NetworkInformation;
using System.Text.Json.Serialization;

namespace SkySensorsAPI.Models.DTO;

public class WeatherStationBasicDTO
{
	[JsonConverter(typeof(PhysicalAddressConverter))] // Needed to convert PhysicalAddress to string when used in endpoint result
	public required PhysicalAddress MacAddress { get; set; }
	public required GpsLocationDTO GpsLocation { get; set; }
	public SensorDataDTO[] Sensors { get; set; } = [];
}
