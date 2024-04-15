namespace SkySensorsAPI.Models;

// This model represents the data which the endpoints response with
public class WeatherStation
{
    public string MacAddress { get; set; }
    public GpsLocation GpsLocation { get; set; }
    public Sensor Sensors { get; set; }
}
