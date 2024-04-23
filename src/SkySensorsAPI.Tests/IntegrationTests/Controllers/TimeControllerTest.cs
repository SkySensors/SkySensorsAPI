using FluentAssertions;
using System.Net;

namespace SkySensorsAPI.Tests.IntegrationTests.Controllers;

internal class TimeControllerTest : IntegrationTests
{
	private const string UrlPath = "/api/time";

	[Test]
	public async Task GET_GetCurrentTime_ResponseWith200()
	{
		// Arrange
		DateTimeOffset now = DateTimeOffset.UtcNow;
		long rangeStart = now.AddMinutes(-1).ToUnixTimeMilliseconds();
		long rangeEnd = now.AddMinutes(1).ToUnixTimeMilliseconds();

		// Act
		HttpResponseMessage response = await HttpClient.GetAsync(UrlPath );

		// Assert
		response.StatusCode.Should().Be(HttpStatusCode.OK);
		string data = await response.Content.ReadAsStringAsync();
		data.Should().NotBeNullOrEmpty();
		
		Assert.True(long.TryParse(data, out long unixTimeResponse));

		unixTimeResponse.Should().BeInRange(rangeStart, rangeEnd);
	}
}
