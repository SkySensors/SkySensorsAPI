namespace SkySensorsAPI.Services;

public interface IWhetherStationService
{
	public Task<bool> GetDummyValue();
}

public class WhetherStationService(
	ILogger<WhetherStationService> logger) : IWhetherStationService
{
	public async Task<bool> GetDummyValue()
	{
		logger.LogInformation("GetDummyValue was called");
		return await Task.FromResult(true);
	}
}
