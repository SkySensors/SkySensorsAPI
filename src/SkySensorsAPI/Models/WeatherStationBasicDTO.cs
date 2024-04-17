using SkySensorsAPI.Utilities;
using System.Net.NetworkInformation;
using System.Text.Json.Serialization;

namespace SkySensorsAPI.Models
{
	public class WeatherStationBasicDTO
	{
		[JsonConverter(typeof(PhysicalAddressConverter))] // Needed to convert PhysicalAddress to string when used in endpoint result
		public PhysicalAddress MacAddress { get; set; }
		public GpsLocation GpsLocation { get; set; }
		public SensorDataDTO[] Sensors { get; set; }
	}
}
