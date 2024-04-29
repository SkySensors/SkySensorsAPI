using System.Net.NetworkInformation;

namespace SkySensorsAPI.Models.Infrastructure;

public class WeatherStation
{
	public required PhysicalAddress MacAddress { get; set; }
	public required float Lon { get; set; }
	public required float Lat { get; set; }
}
