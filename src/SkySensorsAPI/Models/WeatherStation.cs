using System.Net.NetworkInformation;

namespace SkySensorsAPI.Models;

public class WeatherStation
{
	public PhysicalAddress MacAddress { get; set; }
	public float Longitude{ get; set; }
	public float Latitude { get; set; }

	public static BasicWeatherStationDTO ToBasicWeatherStationDTO(WeatherStation weatherStationDB)
	{
		return new BasicWeatherStationDTO
		{
			MacAddress = weatherStationDB.MacAddress,
			GpsLocation = new GpsLocation() { Latitude = weatherStationDB.Latitude, Longitude = weatherStationDB.Longitude },
		};
	}
	public static WeatherStationDTO ToWeatherStationDTO(WeatherStation weatherStationDB, List<SensorDTO> sensors)
	{
		return new WeatherStationDTO()
		{
			MacAddress = weatherStationDB.MacAddress,
			GpsLocation = new GpsLocation() { Latitude = weatherStationDB.Latitude, Longitude = weatherStationDB.Longitude },
			Sensors = sensors,
		};
	}
}
