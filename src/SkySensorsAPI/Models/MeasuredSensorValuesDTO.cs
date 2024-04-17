using System.Net.NetworkInformation;

namespace SkySensorsAPI.Models
{
    public class MeasuredSensorValuesDTO
    {
        public SensorType Type { get; set; }
        public PhysicalAddress MacAddress { get; set; }
        public SensorValueDTO[] SensorValues { get; set; }
    }
}
