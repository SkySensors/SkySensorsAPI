using Serilog;
using SkySensorsAPI.DomainServices;
using SkySensorsAPI.InfrastureServices;
using SkySensorsAPI.Middlewares;
using SkySensorsAPI.Repositories;

Log.Logger = new LoggerConfiguration()
	.Enrich.FromLogContext()
	.WriteTo.Console()
	.CreateLogger();

try
{
	var builder = WebApplication.CreateBuilder(args);
	builder.Services.AddSingleton<IWeatherStationDomainService, WeatherStationDomainService>();
	builder.Services.AddSingleton<ITimeSlotDomainService, TimeSlotDomainService>();
	builder.Services.AddSingleton<IPostgreSqlInfrastureService, PostgreSqlInfrastureService>();
	builder.Services.AddSingleton<IWeatherStationRepository, WeatherStationRepository>();
	builder.Services.AddSingleton<ITimeSlotRepository, TimeSlotRepository>();
	builder.Services.AddControllers();
	builder.Services.AddSwaggerGen();
	builder.Services.AddHealthChecks();
    builder.Services.AddCors(o => o.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    }));

    var app = builder.Build();

	app.UseRouting();
    app.UseHttpsRedirection();
    app.UseCors("AllowAll");

    app.MapControllers();
	app.UseDeveloperExceptionPage();
	app.UseMiddleware<ExceptionHandlerMiddleware>();
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
