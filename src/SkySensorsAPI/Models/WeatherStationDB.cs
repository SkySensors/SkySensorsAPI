using System.Net.NetworkInformation;

namespace SkySensorsAPI.Models;

public class WeatherStationDB
{
	public PhysicalAddress MacAddress { get; set; }
	public float Longitude{ get; set; }
	public float Latitude { get; set; }
}
