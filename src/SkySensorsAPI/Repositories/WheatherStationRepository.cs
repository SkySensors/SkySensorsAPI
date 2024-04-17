using SkySensorsAPI.InfrastureServices;
using Dapper;
using SkySensorsAPI.Models;
using NpgsqlTypes;
using System.Net.NetworkInformation;
namespace SkySensorsAPI.Repositories;

public interface IWheatherStationRepository
{
	Task<WeatherStation> GetWheaterStation(string macAddress);
	Task<IEnumerable<WeatherStation>> GetWheaterStations();
	Task<IEnumerable<Sensor>> GetSensorsByMacAddress(string macAddress);
	Task<IEnumerable<SensorValueDTO>> GetSensorValuesByMacAddress(PhysicalAddress macAddress, string type, long startTime, long endTime);
	Task<bool> UpsertWeatherStation(PhysicalAddress macAddress, float lon, float lat);
	Task<bool> UpsertWeatherStationSensor(PhysicalAddress macAddress, string type);
}

public class WheatherStationRepository(
	IPostgreSqlInfrastureService postgreSqlService) : IWheatherStationRepository
{
	public async Task<WeatherStation> GetWheaterStation(string macAddress)
	{
		return await postgreSqlService.ExecuteQueryAsync(
			(con) => con.QueryFirstAsync<WeatherStation>("SELECT mac_address, lon, lat FROM weather_stations WHERE mac_address = @MacAddr;",
			new { MacAddr = PhysicalAddress.Parse(macAddress), NpgsqlDbType = NpgsqlDbType.MacAddr }));
	}

	public async Task<IEnumerable<WeatherStation>> GetWheaterStations()
	{
		return await postgreSqlService.ExecuteQueryAsync(
			(con) => con.QueryAsync<WeatherStation>("SELECT mac_address, lon, lat FROM weather_stations;"));
	}

	public async Task<IEnumerable<Sensor>> GetSensorsByMacAddress(string macAddress)
	{
		return await postgreSqlService.ExecuteQueryAsync(
			(con) => con.QueryAsync<Sensor>("SELECT mac_address, type FROM sensors WHERE mac_address = @MacAddress;",
				new
				{
					MacAddress = PhysicalAddress.Parse(macAddress),
					NpgsqlDbType = NpgsqlDbType.MacAddr
				}));
	}

	public async Task<IEnumerable<SensorValueDTO>> GetSensorValuesByMacAddress(PhysicalAddress macAddress, string type, long startTime, long endTime)
	{
		return await postgreSqlService.ExecuteQueryAsync(
			(con) => con.QueryAsync<SensorValueDTO>("SELECT unix_time, value FROM calibrated_sensor_values WHERE mac_address = @MacAddress AND type = @Type AND unix_time >= @StartTime AND unix_time <= @EndTime;",
			new
			{
				MacAddress = macAddress,
				Type = type,
				StartTime = startTime,
				EndTime = endTime
			}));
	}

	public async Task<bool> UpsertWeatherStation(PhysicalAddress macAddress, float lon, float lat)
	{
		int succeeded = await postgreSqlService.ExecuteQueryAsync(
			   (con) => con.ExecuteAsync("INSERT INTO public.weather_stations (mac_address, lon, lat) VALUES(@MacAddress, @Longitude, @Latitude) ON CONFLICT (mac_address) DO UPDATE SET lon=EXCLUDED.lon, lat=EXCLUDED.lat;",
			   new
			   {
				   MacAddress = macAddress,
				   Longitude = lon,
				   Latitude = lat
			   }));

		return succeeded != 0;
	}

	public async Task<bool> UpsertWeatherStationSensor(PhysicalAddress macAddress, string type)
	{
		int succeeded = await postgreSqlService.ExecuteQueryAsync(
			   (con) => con.ExecuteAsync("INSERT INTO public.sensors (mac_address, \"type\") VALUES(@MacAddress, @Type) ON CONFLICT (mac_address, \"type\") DO NOTHING;",
			   new
			   {
				   MacAddress = macAddress,
				   Type = type
			   }));

		return succeeded != 0;
	}
}
