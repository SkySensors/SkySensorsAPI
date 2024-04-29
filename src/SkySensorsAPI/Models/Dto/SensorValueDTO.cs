namespace SkySensorsAPI.Models.DTO;

public record struct SensorValueDTO(long UnixTime, float Value);
