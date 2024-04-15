using SkySensorsAPI.InfrastureServices;
using Dapper;
using SkySensorsAPI.Models;
using NpgsqlTypes;
using System.Net.NetworkInformation;
namespace SkySensorsAPI.Repositories;

public interface IWheatherStationRepository
{
	Task<WeatherStationDB> GetWheaterStation(string macAddress);
	Task<IEnumerable<SensorDB>> GetSensorsByMacAddress(string macAddress);
	Task<IEnumerable<SensorValue>> GetSensorValuesBySensorId(int sensorId);


}

public class WheatherStationRepository(
	IPostgreSqlInfrastureService postgreSqlService) : IWheatherStationRepository
{
	public async Task<WeatherStationDB> GetWheaterStation(string macAddress)
	{
		return await postgreSqlService.ExecuteQueryAsync(
			(con) => con.QueryFirstAsync<WeatherStationDB>("SELECT mac_address, lon, lat FROM weather_stations WHERE mac_address = @MacAddr;",
			new { MacAddr = PhysicalAddress.Parse(macAddress), NpgsqlDbType = NpgsqlDbType.MacAddr }));
	}
	public async Task<IEnumerable<SensorDB>> GetSensorsByMacAddress(string macAddress)
	{
		return await postgreSqlService.ExecuteQueryAsync(
			(con) => con.QueryAsync<SensorDB>("SELECT id, type, calibration_offset FROM sensors WHERE weather_id = @WeatherID;", 
				new {WeatherID = PhysicalAddress.Parse(macAddress), NpgsqlDbType = NpgsqlDbType.MacAddr }));
	}

	public async Task<IEnumerable<SensorValue>> GetSensorValuesBySensorId(int sensorId)
	{
		return await postgreSqlService.ExecuteQueryAsync(
			(con) => con.QueryAsync<SensorValue>("SELECT unix_time, value FROM sensor_values WHERE sensor_id = @SensorId;",
				new {SensorId = sensorId}));
	}
}
