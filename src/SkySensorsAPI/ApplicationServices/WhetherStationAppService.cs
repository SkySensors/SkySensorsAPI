using SkySensorsAPI.Models;
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

    public async Task<WeatherStation> GetWeatherStation(string macAddress, DateTime measurementStartDate, DateTime measurementEndDate)
    {
        return new WeatherStation { };
    }

    public async Task<bool> AddWeatherStation(WeatherStation weatherStation)
    {
        return false;
    }

    public async Task<bool> AddSensorValues(string macAddress, SensorValue[] sensorValues)
    {
        return false;
    }
}
