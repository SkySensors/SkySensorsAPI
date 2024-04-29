using System.Net.NetworkInformation;

namespace SkySensorsAPI.Models.Infrastructure;

public record struct Sensor(PhysicalAddress MacAddress, SensorType Type);
