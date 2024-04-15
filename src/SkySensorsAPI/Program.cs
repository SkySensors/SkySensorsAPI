using Serilog;
using SkySensorsAPI.ApplicationServices;
using SkySensorsAPI.InfrastureServices;
using SkySensorsAPI.Repositories;

Log.Logger = new LoggerConfiguration()
	.Enrich.FromLogContext()
	.WriteTo.Console()
	.CreateLogger();

try
{
	var builder = WebApplication.CreateBuilder(args);
	builder.Services.AddSingleton<IWhetherStationAppService, WhetherStationAppService>();
	builder.Services.AddSingleton<IPostgreSqlInfrastureService, PostgreSqlInfrastureService>();
	builder.Services.AddSingleton<IWheatherStationRepository, WheatherStationRepository>();
	builder.Services.AddControllers();
	builder.Services.AddSwaggerGen();
	builder.Services.AddHealthChecks();
	var app = builder.Build();

	app.UseRouting();
	app.MapControllers();
	app.UseDeveloperExceptionPage();
	app.UseSwagger();
	app.MapHealthChecks("healthcheck");
	app.UseSwaggerUI(c =>
	{
		c.SwaggerEndpoint("/swagger/v1/swagger.json", "SkySensorsAPI");
	});

	app.Run();
}
catch(Exception ex)
{
	Log.Fatal(ex, "Fatal error occurred under startup.");
}
