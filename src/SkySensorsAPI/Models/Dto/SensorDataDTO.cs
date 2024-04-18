using System.Text.Json.Serialization;

namespace SkySensorsAPI.Models.Dto;

public class SensorDataDTO
{
	[JsonConverter(typeof(JsonStringEnumConverter))] // Convert the Enum to string 
	public SensorType Type { get; set; }
}
