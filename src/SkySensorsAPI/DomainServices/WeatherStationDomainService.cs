using SkySensorsAPI.Models.DTO;
using SkySensorsAPI.Models.Infrastructure;
using SkySensorsAPI.Repositories;
using System.Net.NetworkInformation;

namespace SkySensorsAPI.DomainServices;

public interface IWeatherStationDomainService
{
	public Task<WeatherStationDTO> GetWeatherStation(PhysicalAddress macAddress, long startTime, long endTime, bool isCalibrated);
	public Task<List<WeatherStationDTO>> GetWeatherStations(long startTime, long endTime, bool isCalibrated);
	public Task<IEnumerable<WeatherStationLocationAndMacDTO>> GetAllLocationsAndMacAddresses();
	public Task UpsertWeatherStation(WeatherStationBasicDTO weatherStationBasic);
	public Task InsertMeasuredSensorValues(MeasuredSensorValuesDTO[] measuredSensorValues);
}

public class WeatherStationDomainService(IWeatherStationRepository weatherStationRepository) : IWeatherStationDomainService
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
		List<MeasuredSensorValuesDTO> measuredSensorValuesDTO = await GetMeasuredSensorValuesList(weatherStation.MacAddress, startTime, endTime, isCalibrated);
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
			List<MeasuredSensorValuesDTO> sensors = await GetMeasuredSensorValuesList(weatherStation.MacAddress, startTime, endTime, isCalibrated);
			weatherStationsDTO.Add(WeatherStationDTO.FromWeatherStation(weatherStation, sensors));
		}

		return weatherStationsDTO;
	}

	/// <summary>
	/// Used to get gps location and mac address for all weather stations
	/// </summary>
	/// <returns>collection of WeatherStationLocationAndMacDTO</returns>
	public async Task<IEnumerable<WeatherStationLocationAndMacDTO>> GetAllLocationsAndMacAddresses()
	{
		IEnumerable<WeatherStation> weatherStations = await weatherStationRepository.GetWeatherStations();
		return weatherStations.Select(WeatherStationLocationAndMacDTO.FromWeatherStation);
	}

	/// <summary>
	/// Used to upsert weather station
	/// </summary>
	public async Task UpsertWeatherStation(WeatherStationBasicDTO weatherStation)
	{
		await weatherStationRepository.UpsertWeatherStation(weatherStation.MacAddress, weatherStation.GpsLocation.Longitude, weatherStation.GpsLocation.Latitude);
		foreach (var sensor in weatherStation.Sensors)
		{
			await weatherStationRepository.UpsertWeatherStationSensor(weatherStation.MacAddress, sensor.Type.ToString());
		}
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

	/// <summary>
	/// Used to get a collection of measured sensor values for a weather station
	/// </summary>
	/// <returns>collection of MeasuredSensorValuesDTO</returns>
	private async Task<List<MeasuredSensorValuesDTO>> GetMeasuredSensorValuesList(PhysicalAddress macAddress, long startTime, long endTime, bool isCalibrated)
	{
		IEnumerable<Sensor> sensors = await weatherStationRepository.GetSensorsByMacAddress(macAddress);
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
