using SkySensorsAPI.Models.DTO;
using SkySensorsAPI.Models.Infrastructure;
using SkySensorsAPI.Repositories;
using System.Net.NetworkInformation;

namespace SkySensorsAPI.ApplicationServices;

public interface IWeatherStationAppService
{
	public Task<WeatherStationDTO> GetWeatherStation(PhysicalAddress macAddress, long startTime, long endTime, bool isCalibrated);
	public Task<List<WeatherStationDTO>> GetWeatherStations(long startTime, long endTime, bool isCalibrated);
	public Task<IEnumerable<WeatherStationLocationAndMacDTO>> GetWeatherStationLists();
	public Task UpsertWeatherStation(WeatherStationBasicDTO weatherStationBasic);
	public Task UpsertWeatherStationSensor(PhysicalAddress macAddress, string type);
	public Task InsertMeasuredSensorValues(MeasuredSensorValuesDTO[] measuredSensorValues);
}

public class WeatherStationAppService(
	IWeatherStationRepository weatherStationRepository) : IWeatherStationAppService
{
	/// <summary>
	/// Used to get weather station
	/// </summary>
	/// <returns>WeatherStationDTO</returns>
	public async Task<WeatherStationDTO> GetWeatherStation(PhysicalAddress macAddress, long startTime, long endTime, bool isCalibrated)
	{
		if (startTime > endTime)
		{
			throw new ArgumentException("Start time is bigger than end time");
		}

		WeatherStation weatherStation = await weatherStationRepository.GetWeatherStation(macAddress);
		IEnumerable<Sensor> sensors = await weatherStationRepository.GetSensorsByMacAddress(macAddress);

		List<MeasuredSensorValuesDTO> measuredSensorValuesDTO = await MapSensorsAndSensorValuesToDTO(sensors, startTime, endTime, isCalibrated);
		return WeatherStationDTO.FromWeatherStation(weatherStation, measuredSensorValuesDTO);
	}

	/// <summary>
	/// Used to get weather stations
	/// </summary>
	/// <returns>collection of WeatherStationDTO</returns>
	public async Task<List<WeatherStationDTO>> GetWeatherStations(long startTime, long endTime, bool isCalibrated)
	{
		if (startTime > endTime)
		{
			throw new ArgumentException("Start time is bigger than end time");
		}

		IEnumerable<WeatherStation> weatherStations = await weatherStationRepository.GetWeatherStations();

		List<WeatherStationDTO> weatherStationsDTO = [];
		foreach (WeatherStation weatherStation in weatherStations)
		{
			IEnumerable<Sensor> sensorDatas = await weatherStationRepository.GetSensorsByMacAddress(weatherStation.MacAddress);
			List<MeasuredSensorValuesDTO> sensors = await MapSensorsAndSensorValuesToDTO(sensorDatas, startTime, endTime, isCalibrated);
			weatherStationsDTO.Add(WeatherStationDTO.FromWeatherStation(weatherStation, sensors));
		}

		return weatherStationsDTO;
	}

	/// <summary>
	/// Used to get gps location and mac address for all weather stations
	/// </summary>
	/// <returns>collection of WeatherStationLocationAndMacDTO</returns>
	public async Task<IEnumerable<WeatherStationLocationAndMacDTO>> GetWeatherStationLists()
	{
		IEnumerable<WeatherStation> weatherStations = await weatherStationRepository.GetWeatherStations();
		return weatherStations.Select(w => WeatherStationLocationAndMacDTO.FromWeatherStation(w));
	}

	/// <summary>
	/// Used to upsert weather station
	/// </summary>
	public async Task UpsertWeatherStation(WeatherStationBasicDTO weatherStationBasic)
	{
		await weatherStationRepository.UpsertWeatherStation(weatherStationBasic.MacAddress, weatherStationBasic.GpsLocation.Longitude, weatherStationBasic.GpsLocation.Latitude);
	}

	/// <summary>
	/// Used to upsert weather station sensor
	/// </summary>
	public async Task UpsertWeatherStationSensor(PhysicalAddress macAddress, string type)
	{
		await weatherStationRepository.UpsertWeatherStationSensor(macAddress, type);
	}

	/// <summary>
	/// Insert measured sensor values
	/// </summary>
	public async Task InsertMeasuredSensorValues(MeasuredSensorValuesDTO[] measuredSensorValues)
	{
		SensorValue[] sensorValues = measuredSensorValues.SelectMany(m => m.SensorValues.Select(v => new SensorValue()
		{
			MacAddress = m.MacAddress,
			Type = m.Type.ToString(),
			UnixTime = DateTimeOffset.FromUnixTimeSeconds(v.UnixTime).ToUnixTimeMilliseconds(),
			Value = v.Value
		})).ToArray();
		await weatherStationRepository.InsertSensorValues(sensorValues);
	}

	private async Task<List<MeasuredSensorValuesDTO>> MapSensorsAndSensorValuesToDTO(IEnumerable<Sensor> sensors, long startTime, long endTime, bool isCalibrated)
	{
		if (!sensors.Any())
		{
			return [];
		}

		List<MeasuredSensorValuesDTO> measuredSensorValuesDTO = [];
		foreach (Sensor sensor in sensors)
		{
			IEnumerable<SensorValue> sensorValues = isCalibrated
				? await weatherStationRepository.GetCalibratedSensorValuesByMacAddress(sensor.MacAddress, sensor.Type.ToString(), startTime, endTime)
				: await weatherStationRepository.GetSensorValuesByMacAddress(sensor.MacAddress, sensor.Type.ToString(), startTime, endTime);
			measuredSensorValuesDTO.Add(new MeasuredSensorValuesDTO()
			{
				Type = sensor.Type,
				MacAddress = sensor.MacAddress,
				SensorValues = sensorValues.Select(s => new SensorValueDTO(s.UnixTime, s.Value)).ToList()
			});
		}
		return measuredSensorValuesDTO;
	}
}
