using Microsoft.Extensions.Configuration;

namespace SkySensorsAPI.Tests.IntegrationTests;

internal class IntegrationTests
{
	protected static IConfigurationRoot Configuration => IntegrationTestsSetup.Configuration;
	protected static HttpClient HttpClient => IntegrationTestsSetup.HttpClient;
}
