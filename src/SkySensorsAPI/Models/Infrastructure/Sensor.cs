using System.Net.NetworkInformation;

namespace SkySensorsAPI.Models.Infrastructure;

public record class Sensor(PhysicalAddress MacAddress, SensorType Type);
