using SkySensorsAPI.InfrastureServices;

namespace SkySensorsAPI.Repositories;

public interface IWheatherStationRepository
{
	Task<object> GetWheaterStation();
}

public class WheatherStationRepository(
	IPostgreSqlInfrastureService postgreSqlService) : IWheatherStationRepository
{
	public async Task<object> GetWheaterStation()
	{
		return await Task.FromResult(new object());
	}
}
