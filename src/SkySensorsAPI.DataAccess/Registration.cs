using SkySensorsAPI.DataAccess.Repositories;
using SkySensorsAPI.DataAccess.Services;

namespace ImageBot.Core;

public static class Registration
{
	public static void AddDataAccess(this IServiceCollection services)
	{
		services.AddTransient<IPostgreSqlService, PostgreSqlService>();
		services.AddSingleton<IWheatherStationRepository, WheatherStationRepository>();
	}

	public static void UseDataAccess(this WebApplication app)
	{
	}
}