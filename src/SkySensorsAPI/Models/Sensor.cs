namespace SkySensorsAPI.Models
{
    public class Sensor
    {
        public SensorType Type { get; set; }
        public float CalibrationOffset { get; set; }
        public SensorValue[] SensorValue { get; set; }
    }
}
