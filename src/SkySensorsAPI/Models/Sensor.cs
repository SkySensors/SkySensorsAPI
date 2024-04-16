using System.Net.NetworkInformation;

namespace SkySensorsAPI.Models
{
    public class Sensor
    {
        public PhysicalAddress MacAddress { get; set; }
        public SensorType Type { get; set; }
    }
}
