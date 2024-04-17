using SkySensorsAPI.Utilities;
using System.Net.NetworkInformation;
using System.Text.Json.Serialization;

namespace SkySensorsAPI.Models
{
	public class SensorValue
	{
		public string Type { get; set; }
		[JsonConverter(typeof(PhysicalAddressConverter))] // Needed to convert PhysicalAddress to string when used in endpoint result
		public PhysicalAddress MacAddress { get; set; }
		public long UnixTime { get; set; }
		public float Value { get; set; }

	}
}
