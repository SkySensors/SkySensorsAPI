using System.Text.Json.Serialization;

namespace SkySensorsAPI.Models
{
	public class SensorDataDTO
	{
		[JsonConverter(typeof(JsonStringEnumConverter))] // Convert the Enum to string 
		public SensorType Type { get; set; }
	}
}
