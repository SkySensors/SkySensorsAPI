namespace SkySensorsAPI.Models
{
    public class SensorDTO
    {
        public SensorType Type { get; set; }
        public float CalibrationOffset { get; set; }
        public SensorValueDTO[] SensorValue { get; set; }
    }
}
