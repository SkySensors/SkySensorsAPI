using FluentAssertions;
using System.Net;

namespace SkySensorsAPI.Tests.IntegrationTests.Controllers;

internal class WheaterStationControllerTests : IntegrationTests
{
	private const string UrlPath = "/wheatherstation";

	[Test, Order(2)]
	public async Task GET_DummyValue_ReturnsTrue()
	{
		// Arrange
		// Act
		HttpResponseMessage response = await HttpClient.GetAsync(UrlPath);

		// Assert
		response.StatusCode.Should().Be(HttpStatusCode.OK);
	}
}
