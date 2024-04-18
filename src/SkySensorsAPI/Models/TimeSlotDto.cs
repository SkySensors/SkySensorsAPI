namespace SkySensorsAPI.Models;

public class TimeSlotDto
{
	public required int SecondsNumber { get; set; }
	public int IntervalSeconds { get; } = 10; // Controls how often data should be sent
}
