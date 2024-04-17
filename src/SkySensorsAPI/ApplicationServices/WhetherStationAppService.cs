using SkySensorsAPI.Models;
using SkySensorsAPI.Repositories;
using System.Collections;
using System.Collections.Generic;

namespace SkySensorsAPI.ApplicationServices;

public interface IWheatherStationAppService
{
    public Task<bool> GetDummyValue();
    public Task<WeatherStationDTO> GetWeatherStation(string macAddress, long startTime, long endTime);

    public Task<List<WeatherStationDTO>> GetWeatherStations(long startTime, long endTime);

    public Task<IEnumerable<BasicWeatherStationDTO>> GetWeatherStationLists();

}

public class WheatherStationAppService(
    IWheatherStationRepository wheatherStationRepository,
    ILogger<WheatherStationAppService> logger) : IWheatherStationAppService
{
    public async Task<bool> GetDummyValue()
    {
        logger.LogInformation("GetDummyValue was called");

        object wheaterStation = await wheatherStationRepository.GetWheaterStation("");
        return wheaterStation != null;
    }

    public async Task<WeatherStationDTO> GetWeatherStation(string macAddress, long startTime, long endTime)
    {
        WeatherStation wsd = await wheatherStationRepository.GetWheaterStation(macAddress);
        IEnumerable<Sensor> sensorDatas = await wheatherStationRepository.GetSensorsByMacAddress(macAddress);

        List<SensorDTO> sensors = await MapSensorsAndSensorValuesToDTO(sensorDatas, startTime, endTime);


        return WeatherStation.ToWeatherStationDTO(wsd, sensors);
    }
    public async Task<List<WeatherStationDTO>> GetWeatherStations(long startTime, long endTime)
    {
        IEnumerable<WeatherStation> wsds = await wheatherStationRepository.GetWheaterStations();

        List<WeatherStationDTO> weatherStations = [];
        foreach (WeatherStation wsd in wsds)
        {
            IEnumerable<Sensor> sensorDatas = await wheatherStationRepository.GetSensorsByMacAddress(wsd.MacAddress.ToString());
            List<SensorDTO> sensors = await MapSensorsAndSensorValuesToDTO(sensorDatas, startTime, endTime);
            weatherStations.Add(WeatherStation.ToWeatherStationDTO(wsd, sensors));
        }

        return weatherStations;
    }

    public async Task<bool> AddWeatherStation(WeatherStationDTO weatherStation)
    {
        return false;
    }

    public async Task<bool> AddSensorValues(List<SensorDTO> sensors)
    {
        return false;
    }

    private async Task<List<SensorDTO>> MapSensorsAndSensorValuesToDTO(IEnumerable<Sensor> sensorDBs, long startTime, long endTime)
    {
        List<SensorDTO> sensors = [];

        if (sensorDBs != null)
        {
            // Fetch all sensor values foreach sensor 
            foreach (Sensor sensorData in sensorDBs)
            {
                IEnumerable<SensorValueDTO> values = await wheatherStationRepository.GetSensorValuesByMacAddress(sensorData.MacAddress, sensorData.Type.ToString(), startTime, endTime);

                sensors.Add(new SensorDTO()
                {
                    //CalibrationOffset = sensorData.CalibrationOffset,
                    Type = sensorData.Type,
                    SensorValues = values.ToList(),

                });
            }
        }
        return sensors;
    }

    public async Task<IEnumerable<BasicWeatherStationDTO>> GetWeatherStationLists()
    {
        IEnumerable<WeatherStation> wsds = await wheatherStationRepository.GetWheaterStations();
        return wsds.Select(w => WeatherStation.ToBasicWeatherStationDTO(w));
    }
}
