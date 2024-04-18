using Microsoft.Extensions.Configuration;

namespace SkySensorsAPI.Tests.IntegrationTests;

[SetUpFixture]
internal class IntegrationTestsSetup
{
	public static HttpClient HttpClient { get; private set; } = new();
	public static IConfigurationRoot Configuration { get; private set; } = new ConfigurationBuilder().Build();

	[OneTimeSetUp]
	public static async Task Setup()
	{
		Configuration = new ConfigurationBuilder()
		   .AddJsonFile("appsettings.tests.json")
		   .AddEnvironmentVariables()
		   .Build();

		string apiUrl = Configuration.GetValue<string>("ApiUrl") ?? string.Empty;
		HttpClient = new HttpClient
		{
			BaseAddress = new Uri(apiUrl)
		};

		try
		{
			HttpResponseMessage healthCheckResult = await HttpClient.GetAsync("healthcheck");
			healthCheckResult.EnsureSuccessStatusCode();
		}
		catch (Exception ex)
		{
			throw new Exception("SkySensorsAPI was unreachable did you forget to start the API application?", ex);
		}
	}
}
