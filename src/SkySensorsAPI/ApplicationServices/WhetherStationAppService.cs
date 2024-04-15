using SkySensorsAPI.Repositories;

namespace SkySensorsAPI.ApplicationServices;

public interface IWhetherStationAppService
{
    public Task<bool> GetDummyValue();
}

public class WhetherStationAppService(
    IWheatherStationRepository wheatherStationRepository,
    ILogger<WhetherStationAppService> logger) : IWhetherStationAppService
{
    public async Task<bool> GetDummyValue()
    {
        logger.LogInformation("GetDummyValue was called");

        object wheaterStation = await wheatherStationRepository.GetWheaterStation();
        return wheaterStation != null;
    }
}
