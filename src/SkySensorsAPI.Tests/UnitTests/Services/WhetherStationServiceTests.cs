using AutoFixture.NUnit3;
using FluentAssertions;
using NSubstitute;
using SkySensorsAPI.ApplicationServices;
using SkySensorsAPI.Repositories;

namespace SkySensorsAPI.Tests.UnitTests.Services;

internal class WheatherStationServiceTests
{
	[Test, AutoDomainData]
	public async Task GetDummyValue_WhenEverythingIsValid_ReturnsTrue(
		[Frozen] IWheatherStationRepository wheatherStationRepository,
		WheatherStationAppService sut)
	{
		// Arrange
		wheatherStationRepository.GetWheaterStations().Returns(new object());

		// Act
		bool result = await sut.GetDummyValue();

		// Assert
		result.Should().BeTrue();
		wheatherStationRepository.Received().GetWheaterStations().Should().NotBeNull();
	}
}
