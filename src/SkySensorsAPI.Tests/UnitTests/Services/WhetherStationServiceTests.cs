using FluentAssertions;
using SkySensorsAPI.Services;

namespace SkySensorsAPI.Tests.UnitTests.Services;

internal class WhetherStationServiceTests
{
	[Test, AutoDomainData]
	public async Task GetDummyValue_WhenEverythingIsValid_ReturnsTrue(
		WhetherStationService sut)
	{
		// Arrange
		// Act
		bool result = await sut.GetDummyValue();

		// Assert
		result.Should().BeTrue();
	}
}
