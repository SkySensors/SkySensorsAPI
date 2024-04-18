using SkySensorsAPI.Utilities;
using System.Net.NetworkInformation;
using System.Text.Json.Serialization;

namespace SkySensorsAPI.Models.Infrastructure;

public class SensorValue
{
	public required string Type { get; set; }
	[JsonConverter(typeof(PhysicalAddressConverter))] // Needed to convert PhysicalAddress to string when used in endpoint result
	public required PhysicalAddress MacAddress { get; set; }
	public required long UnixTime { get; set; }
	public required float Value { get; set; }
}
