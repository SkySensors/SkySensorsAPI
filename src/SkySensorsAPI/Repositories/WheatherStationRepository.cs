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
			(con) => con.QueryAsync<SensorValueDTO>("SELECT unix_time, value FROM sensor_values WHERE mac_address = @MacAddress AND type = @Type AND unix_time >= @StartTime AND unix_time <= @EndTime;",
			new
			{
				MacAddress = macAddress,
				Type = type,
				StartTime = startTime,
				EndTime = endTime
			}));
	}
}
