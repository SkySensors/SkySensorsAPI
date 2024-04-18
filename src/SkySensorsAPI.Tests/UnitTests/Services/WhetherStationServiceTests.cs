using AutoFixture.NUnit3;
using FluentAssertions;
using NSubstitute;
using SkySensorsAPI.ApplicationServices;
using SkySensorsAPI.Models;
using SkySensorsAPI.Models.DTO;
using SkySensorsAPI.Models.Infrastructure;
using SkySensorsAPI.Repositories;
using System.Net.NetworkInformation;

namespace SkySensorsAPI.Tests.UnitTests.Services;

internal class WheatherStationServiceTests
{
	[Test, AutoDomainData]
	public async Task GetWeatherStation_WhenEverythingIsValid_ReturnsWeatherStation(
		[Frozen] IWeatherStationRepository wheatherStationRepository,
		WeatherStationAppService sut)
	{
		// Arrange
		string mac = "00:00:00:00:00:00";
		wheatherStationRepository.GetWheaterStation(mac)
			.Returns(new WeatherStation()
		{
			Lat = 0,
			Lon = 0,
			MacAddress = PhysicalAddress.Parse(mac)
		});
		wheatherStationRepository.GetSensorsByMacAddress(mac)
			.Returns(new List<Sensor>()
		{
			new Sensor(PhysicalAddress.Parse(mac), SensorType.Temperature)
		});
		wheatherStationRepository.GetSensorValuesByMacAddress(PhysicalAddress.Parse(mac), "Temperature", Arg.Any<long>(), Arg.Any<long>())
			.Returns(new List<SensorValue>()
		{
			new SensorValue
			{
				MacAddress = PhysicalAddress.Parse(mac),
				Type = "Temperature",
				UnixTime = 1713355703952,
				Value = 10,
			}
		});

		// Act
		WeatherStationDTO result = await sut.GetWeatherStation(mac, 1713355703952, 1713442103952);

		// Assert
		result.Should().NotBeNull();
		result.MacAddress.Should().Be(PhysicalAddress.Parse(mac));
		_ = wheatherStationRepository.Received(1).GetWheaterStation(mac);
	}
}
