using SkySensorsAPI.Models.Dto;
using SkySensorsAPI.Models.Infrastructure;
using SkySensorsAPI.Repositories;
using System.Net.NetworkInformation;

namespace SkySensorsAPI.ApplicationServices;

public interface IWheatherStationAppService
{
	public Task<WeatherStationDTO> GetWeatherStation(string macAddress, long startTime, long endTime);
	public Task<List<WeatherStationDTO>> GetWeatherStations(long startTime, long endTime);
	public Task<IEnumerable<WeatherStationLocationAndMacDTO>> GetWeatherStationLists();
	public Task UpsertWeatherStation(WeatherStationBasicDTO weatherStationBasic);
	public Task UpsertWeatherStationSensor(PhysicalAddress macAddress, string type);
	public Task InsertMeasuredSensorValues(MeasuredSensorValuesDTO[] measuredSensorValues);
}

public class WheatherStationAppService(
	IWheatherStationRepository wheatherStationRepository) : IWheatherStationAppService
{
	public async Task<WeatherStationDTO> GetWeatherStation(string macAddress, long startTime, long endTime)
	{
		WeatherStation weatherStation = await wheatherStationRepository.GetWheaterStation(macAddress);
		IEnumerable<Sensor> sensors = await wheatherStationRepository.GetSensorsByMacAddress(macAddress);

		List<MeasuredSensorValuesDTO> measuredSensorValuesDTO = await MapSensorsAndSensorValuesToDTO(sensors, startTime, endTime);
		return WeatherStationDTO.FromWeatherStation(weatherStation, measuredSensorValuesDTO);
	}

	public async Task<List<WeatherStationDTO>> GetWeatherStations(long startTime, long endTime)
	{
		IEnumerable<WeatherStation> weatherStations = await wheatherStationRepository.GetWheaterStations();

		List<WeatherStationDTO> weatherStationsDTO = [];
		foreach (WeatherStation weatherStation in weatherStations)
		{
			IEnumerable<Sensor> sensorDatas = await wheatherStationRepository.GetSensorsByMacAddress(weatherStation.MacAddress.ToString());
			List<MeasuredSensorValuesDTO> sensors = await MapSensorsAndSensorValuesToDTO(sensorDatas, startTime, endTime);
			weatherStationsDTO.Add(WeatherStationDTO.FromWeatherStation(weatherStation, sensors));
		}

		return weatherStationsDTO;
	}

	public async Task<IEnumerable<WeatherStationLocationAndMacDTO>> GetWeatherStationLists()
	{
		IEnumerable<WeatherStation> weatherStations = await wheatherStationRepository.GetWheaterStations();
		return weatherStations.Select(w => WeatherStationLocationAndMacDTO.FromWeatherStation(w));
	}

	public async Task UpsertWeatherStation(WeatherStationBasicDTO weatherStationBasic)
	{
		await wheatherStationRepository.UpsertWeatherStation(weatherStationBasic.MacAddress, weatherStationBasic.GpsLocation.Longitude, weatherStationBasic.GpsLocation.Latitude);
	}

	public async Task UpsertWeatherStationSensor(PhysicalAddress macAddress, string type)
	{
		await wheatherStationRepository.UpsertWeatherStationSensor(macAddress, type);
	}

	public async Task InsertMeasuredSensorValues(MeasuredSensorValuesDTO[] measuredSensorValues)
	{
		SensorValue[] sensorValues = measuredSensorValues.SelectMany(m => m.SensorValues.Select(v => new SensorValue()
		{
			MacAddress = m.MacAddress,
			Type = m.Type.ToString(),
			UnixTime = DateTimeOffset.FromUnixTimeSeconds(v.UnixTime).ToUnixTimeMilliseconds(),
			Value = v.Value
		})).ToArray();
		await wheatherStationRepository.InsertSensorValues(sensorValues);
	}

	private async Task<List<MeasuredSensorValuesDTO>> MapSensorsAndSensorValuesToDTO(IEnumerable<Sensor> sensors, long startTime, long endTime)
	{
		if (!sensors.Any())
		{
			return [];
		}

		List<MeasuredSensorValuesDTO> measuredSensorValuesDTO = [];
		foreach (Sensor sensor in sensors)
		{
			IEnumerable<SensorValue> sensorValues = await wheatherStationRepository.GetSensorValuesByMacAddress(sensor.MacAddress, sensor.Type.ToString(), startTime, endTime);
			measuredSensorValuesDTO.Add(new MeasuredSensorValuesDTO()
			{
				Type = sensor.Type,
				MacAddress = sensor.MacAddress,
				SensorValues = sensorValues.Select(s => new SensorValueDTO { Value = s.Value, UnixTime = s.UnixTime }).ToList()
			});
		}
		return measuredSensorValuesDTO;
	}
}
