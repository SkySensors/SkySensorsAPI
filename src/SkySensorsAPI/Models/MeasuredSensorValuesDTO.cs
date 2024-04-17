using SkySensorsAPI.Utilities;
using System.Net.NetworkInformation;
using System.Text.Json.Serialization;

namespace SkySensorsAPI.Models;

public class MeasuredSensorValuesDTO
{
	[JsonConverter(typeof(JsonStringEnumConverter))] // Convert the Enum to string 
	public SensorType Type { get; set; }
	[JsonConverter(typeof(PhysicalAddressConverter))] // Needed to convert PhysicalAddress to string when used in endpoint result
	public PhysicalAddress MacAddress { get; set; }
	public SensorValueDTO[] SensorValues { get; set; }
}
