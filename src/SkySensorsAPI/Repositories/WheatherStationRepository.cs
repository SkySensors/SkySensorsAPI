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
	Task<IEnumerable<SensorValueDTO>> GetSensorValuesBySensorId(int sensorId);
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
			(con) => con.QueryAsync<Sensor>("SELECT id, type, calibration_offset FROM sensors WHERE weather_id = @WeatherID;", 
				new {WeatherID = PhysicalAddress.Parse(macAddress), NpgsqlDbType = NpgsqlDbType.MacAddr }));
	}

	public async Task<IEnumerable<SensorValueDTO>> GetSensorValuesBySensorId(int sensorId)
	{
		return await postgreSqlService.ExecuteQueryAsync(
			(con) => con.QueryAsync<SensorValueDTO>("SELECT unix_time, value FROM sensor_values WHERE sensor_id = @SensorId;",
				new {SensorId = sensorId}));
	}
}
