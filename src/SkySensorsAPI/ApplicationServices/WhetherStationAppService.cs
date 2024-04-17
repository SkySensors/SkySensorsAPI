using SkySensorsAPI.Models;
using SkySensorsAPI.Repositories;
using System.Net.NetworkInformation;

namespace SkySensorsAPI.ApplicationServices;

public interface IWheatherStationAppService
{
	public Task<bool> GetDummyValue();
	public Task<WeatherStationDTO> GetWeatherStation(string macAddress, long startTime, long endTime);

	public Task<List<WeatherStationDTO>> GetWeatherStations(long startTime, long endTime);

	public Task<IEnumerable<BasicWeatherStationDTO>> GetWeatherStationLists();
	public Task UpsertWeatherStation(WeatherStationBasicDTO weatherStationBasic);
	public Task UpsertWeatherStationSensor(PhysicalAddress macAddress, string type);
	public Task InsertMeasuredSensorValues(MeasuredSensorValuesDTO[] measureds);
}

public class WheatherStationAppService(
	IWheatherStationRepository wheatherStationRepository,
	ILogger<WheatherStationAppService> logger) : IWheatherStationAppService
{
	public async Task<bool> GetDummyValue()
	{
		logger.LogInformation("GetDummyValue was called");

		object wheaterStation = await wheatherStationRepository.GetWheaterStation("");
		return wheaterStation != null;
	}

	public async Task<WeatherStationDTO> GetWeatherStation(string macAddress, long startTime, long endTime)
	{
		WeatherStation wsd = await wheatherStationRepository.GetWheaterStation(macAddress);
		IEnumerable<Sensor> sensorDatas = await wheatherStationRepository.GetSensorsByMacAddress(macAddress);

		List<SensorDTO> sensors = await MapSensorsAndSensorValuesToDTO(sensorDatas, startTime, endTime);


		return WeatherStation.ToWeatherStationDTO(wsd, sensors);
	}
	public async Task<List<WeatherStationDTO>> GetWeatherStations(long startTime, long endTime)
	{
		IEnumerable<WeatherStation> weatherStations = await wheatherStationRepository.GetWheaterStations();

		List<WeatherStationDTO> weatherStationsDTO = [];
		foreach (WeatherStation weatherStation in weatherStations)
		{
			IEnumerable<Sensor> sensorDatas = await wheatherStationRepository.GetSensorsByMacAddress(weatherStation.MacAddress.ToString());
			List<SensorDTO> sensors = await MapSensorsAndSensorValuesToDTO(sensorDatas, startTime, endTime);
			weatherStationsDTO.Add(WeatherStation.ToWeatherStationDTO(weatherStation, sensors));
		}

		return weatherStationsDTO;
	}

	public async Task<bool> AddWeatherStation(WeatherStationDTO weatherStation)
	{
		return false;
	}

	public async Task<bool> AddSensorValues(List<SensorDTO> sensors)
	{
		return false;
	}

	private async Task<List<SensorDTO>> MapSensorsAndSensorValuesToDTO(IEnumerable<Sensor> sensorDBs, long startTime, long endTime)
	{
		List<SensorDTO> sensors = [];

		if (sensorDBs != null)
		{
			// Fetch all sensor values foreach sensor 
			foreach (Sensor sensorData in sensorDBs)
			{
				IEnumerable<SensorValueDTO> values = await wheatherStationRepository.GetSensorValuesByMacAddress(sensorData.MacAddress, sensorData.Type.ToString(), startTime, endTime);

				sensors.Add(new SensorDTO()
				{
					//CalibrationOffset = sensorData.CalibrationOffset,
					Type = sensorData.Type,
					SensorValues = values.ToList(),

				});
			}
		}
		return sensors;
	}

	public async Task<IEnumerable<BasicWeatherStationDTO>> GetWeatherStationLists()
	{
		IEnumerable<WeatherStation> wsds = await wheatherStationRepository.GetWheaterStations();
		return wsds.Select(w => WeatherStation.ToBasicWeatherStationDTO(w));
	}

	public async Task UpsertWeatherStation(WeatherStationBasicDTO weatherStationBasic)
	{
		await wheatherStationRepository.UpsertWeatherStation(weatherStationBasic.MacAddress, weatherStationBasic.GpsLocation.Longitude, weatherStationBasic.GpsLocation.Latitude);
	}

	public async Task UpsertWeatherStationSensor(PhysicalAddress macAddress, string type)
	{
		await wheatherStationRepository.UpsertWeatherStationSensor(macAddress, type);
	}

	public async Task InsertMeasuredSensorValues(MeasuredSensorValuesDTO[] measureds)
	{
		SensorValue[] sensorValues = measureds.SelectMany(m => m.SensorValues.Select(v => new SensorValue()
		{
			MacAddress = m.MacAddress,
			Type = m.Type.ToString(),
			UnixTime = v.UnixTime * 1000,
			Value = v.Value
		})).ToArray();
		await wheatherStationRepository.InsertSensorValues(sensorValues);
	}
}
