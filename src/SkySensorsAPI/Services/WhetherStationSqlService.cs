using SkySensorsAPI.DataAccess.Repositories;

namespace SkySensorsAPI.Services;

public interface IWhetherStationService
{
	public Task<bool> GetDummyValue();
}

public class WhetherStationSqlService(
	IWheatherStationRepository wheatherStationRepository,
	ILogger<WhetherStationSqlService> logger) : IWhetherStationService
{
	public async Task<bool> GetDummyValue()
	{
		logger.LogInformation("GetDummyValue was called");

		object wheaterStation = await wheatherStationRepository.GetWheaterStation();
		return wheaterStation != null;
	}
}
