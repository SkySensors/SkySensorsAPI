using System.Net.NetworkInformation;

namespace SkySensorsAPI.Models.Infrastructure;

public record class TimeSlot(PhysicalAddress MacAddress, int SecondsNumber = 0);
