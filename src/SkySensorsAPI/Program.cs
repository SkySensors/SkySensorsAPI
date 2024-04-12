using Serilog;
using SkySensorsAPI.Services;

Log.Logger = new LoggerConfiguration()
	.Enrich.FromLogContext()
	.WriteTo.Console()
	.CreateLogger();

try
{
	var builder = WebApplication.CreateBuilder(args);
	builder.Services.AddSingleton<IWhetherStationService, WhetherStationService>();
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