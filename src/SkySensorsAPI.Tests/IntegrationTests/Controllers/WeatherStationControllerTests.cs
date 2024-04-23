using FluentAssertions;
using SkySensorsAPI.Models.DTO;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Http.Json;
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

	private static readonly DateTimeOffset now = DateTimeOffset.UtcNow;
	private readonly long validStartTime = now.ToUnixTimeMilliseconds();
	private readonly long validEndTime = now.AddMinutes(20).ToUnixTimeMilliseconds();
	private const string validMacAddressStr = "00-00-00-00-00-00";
	private static readonly SensorDataDTO[] validSensorDatas =
	[
		new SensorDataDTO() {Type = Models.SensorType.Temperature}
	];

	private readonly WeatherStationBasicDTO weatherStationBasicDTO = new WeatherStationBasicDTO()
	{
		MacAddress = PhysicalAddress.Parse(validMacAddressStr),
		GpsLocation = new Models.GpsLocation() { Longitude = 55.00000F, Latitude = 12.0000F },
		Sensors = validSensorDatas

	};

	private readonly MeasuredSensorValuesDTO[] validMeasuredSensorValues =
	[
		new MeasuredSensorValuesDTO() {
			MacAddress = PhysicalAddress.Parse(validMacAddressStr),
			Type = Models.SensorType.Temperature,
			SensorValues = [
				new() {
					UnixTime = now.AddSeconds(2).ToUnixTimeSeconds(),
					Value = 20
				}
			]
		}
	];

	[Test, Order(2)]
	public async Task POST_MakeWeatherStationHandshake_WhenInputsAreValid_ResponseWithTimeSlot()
	{
		// Arrange
		// Act
		HttpResponseMessage response = await HttpClient.PostAsJsonAsync<WeatherStationBasicDTO>(UrlPath + $"/handshake", weatherStationBasicDTO);

		// Assert
		string data = await response.Content.ReadAsStringAsync();
		data.Should().NotBeNullOrEmpty();

		TimeSlotDTO? timeSlot = JsonSerializer.Deserialize<TimeSlotDTO>(data, defaultOptions);
		timeSlot.Should().NotBeNull();
		timeSlot.IntervalSeconds.Should().BeOfType(typeof(int));
		timeSlot.SecondsNumber.Should().BeOfType(typeof(int));

		response.StatusCode.Should().Be(HttpStatusCode.OK);
	}

	[Test, Order(3)]
	public async Task POST_AddSensorValues_WhenInputsAreValid_ResponseWith200()
	{
		// Arrange
		// Act
		HttpResponseMessage response = await HttpClient.PostAsJsonAsync<MeasuredSensorValuesDTO[]>(UrlPath, validMeasuredSensorValues);

		// Assert
		response.StatusCode.Should().Be(HttpStatusCode.OK);
	}

	[Test]
	public async Task GET_WeatherStation_WhenInputsAreValid_ResponseWith200()
	{
		// Arrange
		//string macAddress = "90-A2-DA-10-55-88";

		// Act
		HttpResponseMessage response = await HttpClient.GetAsync(UrlPath + $"?macAddress={validMacAddressStr}&startTime={validStartTime}&endTime={validEndTime}");

		// Assert
		string data = await response.Content.ReadAsStringAsync();
		data.Should().NotBeNullOrEmpty();

		List<WeatherStationDTO>? weatherStations = JsonSerializer.Deserialize<List<WeatherStationDTO>>(data, defaultOptions);
		weatherStations.Should().NotBeNull();
		weatherStations.Count.Should().Be(1);

		weatherStations[0].MacAddress.Should().Be(PhysicalAddress.Parse(validMacAddressStr));
		response.StatusCode.Should().Be(HttpStatusCode.OK);
	}

	[Test, Order(1)]
	public async Task GET_WeatherStation_WhenMacAddressDoesntExist_ResponseWith404()
	{
		// Arrange
		// Act
		//var t = ;
		HttpResponseMessage response = await HttpClient.GetAsync(UrlPath + $"?macAddress={validMacAddressStr}&startTime={validStartTime}&endTime={validEndTime}");
		// Assert
		response.StatusCode.Should().Be(HttpStatusCode.NotFound);
	}

	[Test]
	public async Task GET_WeatherStation_WhenStartDateIsBiggerThanEndDate_ResponseWith400()
	{
		// Arrange
		// Act
		HttpResponseMessage response = await HttpClient.GetAsync(UrlPath + $"?macAddress={validMacAddressStr}&startTime={validEndTime}&endTime={validStartTime}");

		// Assert
		response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
	}

	[Test]
	public async Task GET_GetAllWeatherStations_ResponseWith200()
	{
		// Arrange
		// Act
		HttpResponseMessage response = await HttpClient.GetAsync(UrlPath + $"/all?startTime={validStartTime}&endTime={validEndTime}");

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
