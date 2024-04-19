using FluentAssertions;
using SkySensorsAPI.Models.DTO;
using System.Net;
using System.Net.Mail;
using System.Net.NetworkInformation;
using System.Text.Json;

namespace SkySensorsAPI.Tests.IntegrationTests.Controllers;

internal class WeatherStationControllerTests : IntegrationTests
{
	private const string UrlPath = "/api/weatherstation";
	private readonly JsonSerializerOptions defaultOptions = new()
	{
		PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
	};

	[Test]
	public async Task GET_WeatherStation_WhenInputsAreValid_ResponseWith200()
	{
		// Arrange
		string macAddress = "90-A2-DA-10-55-88";

		// Act
		HttpResponseMessage response = await HttpClient.GetAsync(UrlPath + $"?macAddress={macAddress}&startTime=1713355703952&endTime=1713442103952");

		// Assert
		string data = await response.Content.ReadAsStringAsync();
		data.Should().NotBeNullOrEmpty();

		List<WeatherStationDTO>? weatherStations = JsonSerializer.Deserialize<List<WeatherStationDTO>>(data, defaultOptions);
		weatherStations.Should().NotBeNull();
		weatherStations.Count.Should().Be(1);

		weatherStations[0].MacAddress.Should().Be(PhysicalAddress.Parse(macAddress));
		response.StatusCode.Should().Be(HttpStatusCode.OK);
	}

	[Test]
	public async Task GET_WeatherStation_WhenMacAddressDoesntExist_ResponseWith404()
	{
		// Arrange
		// Act
		HttpResponseMessage response = await HttpClient.GetAsync(UrlPath + "?macAddress=00-00-00-00-00-00&startTime=1713355703952&endTime=1713442103952");

		// Assert
		response.StatusCode.Should().Be(HttpStatusCode.NotFound);
	}

	[Test]
	public async Task GET_WeatherStation_WhenStartDateIsBiggerThanEndDate_ResponseWith400()
	{
		// Arrange
		// Act
		HttpResponseMessage response = await HttpClient.GetAsync(UrlPath + "?macAddress=90-A2-DA-10-55-88&startTime=1713442103952&endTime=1713355703952");

		// Assert
		response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
	}

	[Test]
	public async Task GET_GetAllWeatherStations_ResponseWith200()
	{
		// Arrange
		// Act
		HttpResponseMessage response = await HttpClient.GetAsync(UrlPath + "/all?startTime=1713355703952&endTime=1713442103952");

		// Assert
		string data = await response.Content.ReadAsStringAsync();
		data.Should().NotBeNullOrEmpty();

		List<WeatherStationDTO>? weatherStations =
			JsonSerializer.Deserialize<List<WeatherStationDTO>>(data, defaultOptions);
		weatherStations.Should().NotBeNullOrEmpty();

		response.StatusCode.Should().Be(HttpStatusCode.OK);
	}

	[Test]
	public async Task GET_WeatherStationList_ResponseWith200()
	{
		// Arrange
		// Act
		HttpResponseMessage response = await HttpClient.GetAsync(UrlPath + "/list");

		// Assert
		string data = await response.Content.ReadAsStringAsync();
		data.Should().NotBeNullOrEmpty();

		List<WeatherStationLocationAndMacDTO>? weatherStationLocationAndMacs = 
			JsonSerializer.Deserialize<List<WeatherStationLocationAndMacDTO>>(data, defaultOptions);

		weatherStationLocationAndMacs.Should().NotBeNull();
		response.StatusCode.Should().Be(HttpStatusCode.OK);
	}
}
