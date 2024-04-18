using System.Net.NetworkInformation;
using System.Text.Json.Serialization;

namespace SkySensorsAPI.Models.Dto;

public class MeasuredSensorValuesDTO
{
	[JsonConverter(typeof(JsonStringEnumConverter))] // Convert the Enum to string 
	public SensorType Type { get; set; }
	public required PhysicalAddress MacAddress { get; set; }
	public List<SensorValueDTO> SensorValues { get; set; } = [];
}
