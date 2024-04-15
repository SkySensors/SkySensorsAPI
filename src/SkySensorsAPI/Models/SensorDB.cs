namespace SkySensorsAPI.Models
{
    public class SensorDB
    {
        public int Id { get; set; }
        public SensorType Type { get; set; }
        public float CalibrationOffset { get; set; }
    }
}
