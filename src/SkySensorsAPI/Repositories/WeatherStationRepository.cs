using SkySensorsAPI.InfrastureServices;
using Dapper;
using NpgsqlTypes;
using System.Net.NetworkInformation;
using Faithlife.Utility.Dapper;
using SkySensorsAPI.Models.Infrastructure;
namespace SkySensorsAPI.Repositories;

public interface IWeatherStationRepository
{
	Task<WeatherStation> GetWeatherStation(PhysicalAddress macAddress);
	Task<IEnumerable<WeatherStation>> GetWeatherStations();
	Task<IEnumerable<Sensor>> GetSensorsByMacAddress(PhysicalAddress macAddress);
	Task<IEnumerable<SensorValue>> GetCalibratedSensorValuesByMacAddress(PhysicalAddress macAddress, string type, long startTime, long endTime);
	Task UpsertWeatherStation(PhysicalAddress macAddress, float lon, float lat);
	Task UpsertWeatherStationSensor(PhysicalAddress macAddress, string type);
	Task InsertSensorValues(SensorValue[] sensorValues);
	Task<IEnumerable<SensorValue>> GetSensorValuesByMacAddress(PhysicalAddress macAddress, string type, long startTime, long endTime);
}

public class WeatherStationRepository(
	IPostgreSqlInfrastureService postgreSqlService) : IWeatherStationRepository
{
	public async Task<WeatherStation> GetWeatherStation(PhysicalAddress macAddress)
	{
		return await postgreSqlService.ExecuteQueryAsync(
			(con) => con.QueryFirstAsync<WeatherStation>("SELECT mac_address, lon, lat FROM weather_stations WHERE mac_address = @MacAddr;",
			new { MacAddr = macAddress, NpgsqlDbType = NpgsqlDbType.MacAddr }));
	}

	public async Task<IEnumerable<WeatherStation>> GetWeatherStations()
	{
		return await postgreSqlService.ExecuteQueryAsync(
			(con) => con.QueryAsync<WeatherStation>("SELECT mac_address, lon, lat FROM weather_stations;"));
	}

	public async Task<IEnumerable<Sensor>> GetSensorsByMacAddress(PhysicalAddress macAddress)
	{
		return await postgreSqlService.ExecuteQueryAsync(
			(con) => con.QueryAsync<Sensor>("SELECT mac_address, type FROM sensors WHERE mac_address = @MacAddress;",
				new
				{
					MacAddress = macAddress,
					NpgsqlDbType = NpgsqlDbType.MacAddr
				}));
	}

	public async Task<IEnumerable<SensorValue>> GetCalibratedSensorValuesByMacAddress(PhysicalAddress macAddress, string type, long startTime, long endTime)
	{
		return await postgreSqlService.ExecuteQueryAsync(
			(con) => con.QueryAsync<SensorValue>("SELECT unix_time, value FROM calibrated_sensor_values WHERE mac_address = @MacAddress AND type = @Type AND unix_time >= @StartTime AND unix_time <= @EndTime;",
			new
			{
				MacAddress = macAddress,
				Type = type,
				StartTime = startTime,
				EndTime = endTime
			}));
	}

	public async Task<IEnumerable<SensorValue>> GetSensorValuesByMacAddress(PhysicalAddress macAddress, string type, long startTime, long endTime)
	{
		return await postgreSqlService.ExecuteQueryAsync(
			(con) => con.QueryAsync<SensorValue>("SELECT unix_time, value FROM sensor_values WHERE mac_address = @MacAddress AND type = @Type AND unix_time >= @StartTime AND unix_time <= @EndTime;",
			new
			{
				MacAddress = macAddress,
				Type = type,
				StartTime = startTime,
				EndTime = endTime
			}));
	}

	public async Task UpsertWeatherStation(PhysicalAddress macAddress, float lon, float lat)
	{
		int succeeded = await postgreSqlService.ExecuteQueryAsync(
			   (con) => con.ExecuteAsync("INSERT INTO public.weather_stations (mac_address, lon, lat) VALUES(@MacAddress, @Longitude, @Latitude) ON CONFLICT (mac_address) DO UPDATE SET lon=EXCLUDED.lon, lat=EXCLUDED.lat;",
			   new
			   {
				   MacAddress = macAddress,
				   Longitude = lon,
				   Latitude = lat
			   }));

	}

	public async Task UpsertWeatherStationSensor(PhysicalAddress macAddress, string type)
	{
		int succeeded = await postgreSqlService.ExecuteQueryAsync(
			   (con) => con.ExecuteAsync("INSERT INTO public.sensors (mac_address, \"type\") VALUES(@MacAddress, @Type) ON CONFLICT (mac_address, \"type\") DO NOTHING;",
			   new
			   {
				   MacAddress = macAddress,
				   Type = type
			   }));
	}

	public async Task InsertSensorValues(SensorValue[] sensorValues)
	{
		await postgreSqlService.ExecuteQueryAsync((con) =>
			con.BulkInsertAsync("INSERT INTO sensor_values(mac_address, type, unix_time, value) VALUES(@MacAddress, @Type, @UnixTime, @Value) ...", sensorValues));
	}
}
