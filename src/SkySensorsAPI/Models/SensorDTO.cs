using System.Text.Json.Serialization;

namespace SkySensorsAPI.Models
{
    public class SensorDTO
    {
		[JsonConverter(typeof(JsonStringEnumConverter))] // Convert the Enum to string 
		public SensorType Type { get; set; }
        //public float CalibrationOffset { get; set; }
        public List<SensorValueDTO> SensorValues { get; set; }
    }
}
