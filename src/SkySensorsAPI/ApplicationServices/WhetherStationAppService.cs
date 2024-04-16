using SkySensorsAPI.Models;
using SkySensorsAPI.Repositories;
using System.Collections.Generic;

namespace SkySensorsAPI.ApplicationServices;

public interface IWhetherStationAppService
{
    public Task<bool> GetDummyValue();
    public Task<WeatherStation> GetWeatherStation(string macAddress);

	public Task<List<WeatherStation>> GetWeatherStations();
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

        List<SensorDTO> sensors = await MapSensorsAndSensorValuesToDTO(sensorDatas);


        return MapWeatherStationToDTO(wsd, sensors);
    }
	public async Task<List<WeatherStation>> GetWeatherStations()
	{
		IEnumerable<WeatherStationDB> wsds = await wheatherStationRepository.GetWheaterStations();

		List<WeatherStation> weatherStations = [];
		foreach (WeatherStationDB wsd in wsds)
		{
			IEnumerable<SensorDB> sensorDatas = await wheatherStationRepository.GetSensorsByMacAddress(wsd.MacAddress.ToString());
			List<SensorDTO> sensors = await MapSensorsAndSensorValuesToDTO(sensorDatas);
			weatherStations.Add(MapWeatherStationToDTO(wsd,sensors));
		}

		return weatherStations;
	}

	public async Task<bool> AddWeatherStation(WeatherStation weatherStation)
    {
        return false;
    }

    public async Task<bool> AddSensorValues(List<SensorDTO> sensors)
    {
        return false;
    }

	private async Task<List<SensorDTO>> MapSensorsAndSensorValuesToDTO(IEnumerable<SensorDB> sensorDBs)
	{
		List<SensorDTO> sensors = [];

		// Fetch all sensor values foreach sensor 
		foreach (SensorDB sensorData in sensorDBs)
		{
			IEnumerable<SensorValueDTO> values = await wheatherStationRepository.GetSensorValuesBySensorId(sensorData.Id);

			sensors.Add(new SensorDTO()
			{
				//CalibrationOffset = sensorData.CalibrationOffset,
				Type = sensorData.Type,
				SensorValues = values.ToList(),

			});
		}
		return sensors;
	}

	private static WeatherStation MapWeatherStationToDTO(WeatherStationDB weatherStation, List<SensorDTO> sensors)
	{
		return new WeatherStation()
		{
			MacAddress = weatherStation.MacAddress,
			GpsLocation = new GpsLocation() { Latitude = weatherStation.Latitude, Longitude = weatherStation.Longitude },
			Sensors = sensors,
		};
	}
}
