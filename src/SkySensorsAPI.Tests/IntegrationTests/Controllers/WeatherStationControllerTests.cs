using FluentAssertions;
using System.Net;

namespace SkySensorsAPI.Tests.IntegrationTests.Controllers;

internal class WeatherStationControllerTests : IntegrationTests
{
	private const string UrlPath = "/weatherstation";

	[Test]
	public async Task GET_WeatherStation_ResponseWith200()
	{
		// Arrange
		// Act
		HttpResponseMessage response = await HttpClient.GetAsync(UrlPath);

		// Assert
		response.StatusCode.Should().Be(HttpStatusCode.OK);
	}
}
