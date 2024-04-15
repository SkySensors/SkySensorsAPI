using System.Text.Json.Serialization;

namespace SkySensorsAPI.Models
{
    public class Sensor
    {
		[JsonConverter(typeof(JsonStringEnumConverter))] // Convert the Enum to string 
		public SensorType Type { get; set; }
        public float CalibrationOffset { get; set; }
        public List<SensorValue> SensorValues { get; set; }
    }
}
