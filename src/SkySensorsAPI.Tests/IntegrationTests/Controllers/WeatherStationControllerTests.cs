using FluentAssertions;
using System.Net;

namespace SkySensorsAPI.Tests.IntegrationTests.Controllers;

internal class WeatherStationControllerTests : IntegrationTests
{
	private const string UrlPath = "/api/weatherstation";

	[Test]
	public async Task GET_WeatherStation_ResponseWith200()
	{
		// Arrange
		// Act
		HttpResponseMessage response = await HttpClient.GetAsync(UrlPath + "?macAddress=90-A2-DA-10-55-88&startTime=1713355703952&endTime=1713442103952");

		// Assert
		var a = await response.Content.ReadAsStringAsync();
		a.Should().Be("a");
		response.StatusCode.Should().Be(HttpStatusCode.OK);
	}
}
