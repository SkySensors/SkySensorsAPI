using SkySensorsAPI.DataAccess.Services;

namespace SkySensorsAPI.DataAccess.Repositories;

public interface IWheatherStationRepository
{
	Task<object> GetWheaterStation();
}

public class WheatherStationRepository(
	IPostgreSqlService postgreSqlService) : IWheatherStationRepository
{
	public async Task<object> GetWheaterStation()
	{
		return await Task.FromResult(new object());
	}
}
