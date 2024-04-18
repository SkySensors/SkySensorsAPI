using System.Net.NetworkInformation;

namespace SkySensorsAPI.Models;

public class WeatherStation
{
	public PhysicalAddress MacAddress { get; set; }
	public float Lon{ get; set; }
	public float Lat { get; set; }

	public static BasicWeatherStationDTO ToBasicWeatherStationDTO(WeatherStation weatherStationDB)
	{
		return new BasicWeatherStationDTO
		{
			MacAddress = weatherStationDB.MacAddress,
			GpsLocation = new GpsLocation() { Latitude = weatherStationDB.Lat, Longitude = weatherStationDB.Lon },
		};
	}
	public static WeatherStationDTO ToWeatherStationDTO(WeatherStation weatherStationDB, List<SensorDTO> sensors)
	{
		return new WeatherStationDTO()
		{
			MacAddress = weatherStationDB.MacAddress,
			GpsLocation = new GpsLocation() { Latitude = weatherStationDB.Lat, Longitude = weatherStationDB.Lon },
			Sensors = sensors,
		};
	}
}
