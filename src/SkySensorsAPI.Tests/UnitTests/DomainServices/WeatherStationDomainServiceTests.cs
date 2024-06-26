﻿using AutoFixture.NUnit3;
using FluentAssertions;
using NSubstitute;
using SkySensorsAPI.DomainServices;
using SkySensorsAPI.Models;
using SkySensorsAPI.Models.DTO;
using SkySensorsAPI.Models.Infrastructure;
using SkySensorsAPI.Repositories;
using System.Net.NetworkInformation;

namespace SkySensorsAPI.Tests.UnitTests.DomainServices;

internal class WeatherStationServiceDomainTests
{
	private const string validMacAddressStr = "00:00:00:00:00:00";
	private readonly PhysicalAddress validMacAddress = PhysicalAddress.Parse(validMacAddressStr);
	private readonly WeatherStation validWeatherStation = new()
	{
		Lat = 0,
		Lon = 0,
		MacAddress = PhysicalAddress.Parse(validMacAddressStr)
	};
	private readonly List<SensorValue> validSensorValues =
	[
		new SensorValue
		{
			MacAddress = PhysicalAddress.Parse(validMacAddressStr),
			Type = "Temperature",
			UnixTime = 1713355703952,
			Value = 10,
		}
	];


	[Test, AutoDomainData]
	public async Task GetWeatherStation_WhenEverythingIsValid_ReturnsWeatherStation(
		[Frozen] IWeatherStationRepository weatherStationRepository,
		WeatherStationDomainService sut)
	{
		// Arrange
		weatherStationRepository.GetWeatherStation(validMacAddress)
			.Returns(validWeatherStation);
		weatherStationRepository.GetSensorsByMacAddress(validMacAddress)
			.Returns([new Sensor(validMacAddress, SensorType.Temperature)]);
		weatherStationRepository.GetCalibratedSensorValuesByMacAddress(validMacAddress, "Temperature", 1713355703952, 1713442103952)
			.Returns(validSensorValues);

		// Act
		WeatherStationDTO result = await sut.GetWeatherStation(validMacAddress, 1713355703952, 1713442103952, true);

		// Assert
		result.Should().NotBeNull();
		result.MacAddress.Should().Be(validMacAddress);
		_ = weatherStationRepository.Received(1).GetWeatherStation(validMacAddress);
	}

	[Test, AutoDomainData]
	public void GetWeatherStation_WhenMacAddressIsInBadFormat_ThrowsException(
	[Frozen] IWeatherStationRepository weatherStationRepository,
	WeatherStationDomainService sut)
	{

		// Act and Assert
		FormatException ex = Assert.ThrowsAsync<FormatException>(async () => await sut.GetWeatherStation(PhysicalAddress.Parse("123"), 1713355703952, 1713442103952, true))
			?? throw new NullReferenceException();
		_ = weatherStationRepository.Received(0).GetWeatherStation(validMacAddress);
	}

	[Test, AutoDomainData]
	public void GetWeatherStation_WhenStartDateIsLargerThanEndDate_ThrowsException(
		[Frozen] IWeatherStationRepository weatherStationRepository,
		WeatherStationDomainService sut)
	{
		// Act and Assert
		ArgumentException ex = Assert.ThrowsAsync<ArgumentException>(async () => await sut.GetWeatherStation(validMacAddress, 1713442103952, 1713355703952, true))
			?? throw new NullReferenceException();
		ex.Message.Should().Be("Start time is bigger than end time");
		_ = weatherStationRepository.Received(0).GetWeatherStation(validMacAddress);
	}

	[Test, AutoDomainData]
	public async Task InsertMeasuredSensorValues_WhenEverythingIsValid_ReturnsVoid(
		[Frozen] IWeatherStationRepository weatherStationAppService,
		WeatherStationDomainService sut)
	{
		// Arrange
		weatherStationAppService.InsertSensorValues(validSensorValues.ToArray())
			.Returns(Task.CompletedTask);
		MeasuredSensorValuesDTO[] measuredSensorValuesDTO =
		[
			new MeasuredSensorValuesDTO
			{
				MacAddress = validMacAddress,
				Type = SensorType.Temperature,
				SensorValues = [new SensorValueDTO(1, DateTimeOffset.UtcNow.ToUnixTimeSeconds())]
			}
		];

		// Act
		await sut.InsertMeasuredSensorValues(measuredSensorValuesDTO);

		// Assert
		weatherStationAppService.Received(1);
	}

	[Test, AutoDomainData]
	public async Task GetWeatherStations_WhenEverythingIsValid_ReturnsWeatherStations(
		[Frozen] IWeatherStationRepository weatherStationRepository,
		WeatherStationDomainService sut)
	{
		// Arrange
		weatherStationRepository.GetWeatherStations().Returns(new List<WeatherStation> { validWeatherStation });

		weatherStationRepository.GetSensorsByMacAddress(validMacAddress)
			.Returns([new Sensor(validMacAddress, SensorType.Temperature)]);

		weatherStationRepository.GetCalibratedSensorValuesByMacAddress(validMacAddress, "Temperature", 1713355703952, 1713442103952)
			.Returns(validSensorValues);

		// Act
		List<WeatherStationDTO> result = await sut.GetWeatherStations(1713355703952, 1713442103952, true);

		// Assert
		result.Should().NotBeNull();
		_ = weatherStationRepository.Received(1).GetWeatherStations();
	}

	[Test, AutoDomainData]
	public void GetWeatherStations_WhenStartDateIsLargerThanEndDate_ThrowsException(
	[Frozen] IWeatherStationRepository weatherStationRepository,
	WeatherStationDomainService sut)
	{
		// Act and assert
		ArgumentException ex = Assert.ThrowsAsync<ArgumentException>(async () => await sut.GetWeatherStations(1713442103952, 1713355703952, true))
			?? throw new NullReferenceException();
		ex.Message.Should().Be("Start time is bigger than end time");
		_ = weatherStationRepository.Received(0).GetWeatherStations();
	}
}
