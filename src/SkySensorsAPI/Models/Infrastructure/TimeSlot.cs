using System.Net.NetworkInformation;

namespace SkySensorsAPI.Models.Infrastructure;

public record class TimeSlot(int SecondsNumber, PhysicalAddress MacAddress);
