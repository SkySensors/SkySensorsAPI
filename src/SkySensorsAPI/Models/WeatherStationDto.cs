namespace SkySensorsAPI.Models;

// This model represents the data which the endpoints response with
public class WeatherStationDTO
{
    public string MacAddress { get; set; }
    public GpsLocation GpsLocation { get; set; }
    public SensorDTO Sensors { get; set; }
}
