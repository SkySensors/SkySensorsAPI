using System.Net.NetworkInformation;

namespace SkySensorsAPI.Models.Infrastructure;

public record struct TimeSlot(PhysicalAddress MacAddress, int SecondsNumber = 0);
