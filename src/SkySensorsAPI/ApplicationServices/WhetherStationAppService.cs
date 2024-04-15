using SkySensorsAPI.Models;
using SkySensorsAPI.Repositories;

namespace SkySensorsAPI.ApplicationServices;

public interface IWhetherStationAppService
{
    public Task<bool> GetDummyValue();
    public Task<WeatherStation> GetWeatherStation(string macAddress);
}

public class WhetherStationAppService(
    IWheatherStationRepository wheatherStationRepository,
    ILogger<WhetherStationAppService> logger) : IWhetherStationAppService
{
    public async Task<bool> GetDummyValue()
    {
        logger.LogInformation("GetDummyValue was called");

        object wheaterStation = await wheatherStationRepository.GetWheaterStation("");
        return wheaterStation != null;
    }

    public async Task<WeatherStation> GetWeatherStation(string macAddress)
    {
        WeatherStationDB wsd = await wheatherStationRepository.GetWheaterStation(macAddress);
		IEnumerable<SensorDB> sensorDatas = await wheatherStationRepository.GetSensorsByMacAddress(macAddress);

        List<Sensor> sensors = [];

        // Fetch all sensor values foreach sensor 
		foreach (SensorDB sensorData in sensorDatas)
		{
			IEnumerable<SensorValue> values = await wheatherStationRepository.GetSensorValuesBySensorId(sensorData.Id);

			sensors.Add(new Sensor()
			{
				CalibrationOffset = sensorData.CalibrationOffset,
				Type = sensorData.Type,
				SensorValues = values.ToList(),

			});
		}

        return new WeatherStation() { 
            MacAddress = wsd.MacAddress,
            GpsLocation = new GpsLocation() { Latitude = wsd.Latitude, Longitude = wsd.Longitude },
			Sensors = sensors,
		};
    }

    public async Task<bool> AddWeatherStation(WeatherStation weatherStation)
    {
        return false;
    }

    public async Task<bool> AddSensorValues(string macAddress, SensorValue[] sensorValues)
    {
        return false;
    }
}
