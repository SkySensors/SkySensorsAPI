using System.Text.Json;

namespace SkySensorsAPI.Middlewares;

public class ExceptionHandlerMiddleware(RequestDelegate nextMiddleware)
{
	public async Task Invoke(HttpContext context)
	{
		try
		{
			await nextMiddleware.Invoke(context);
		}
		catch (Exception ex) when (
			ex is FormatException || 
			ex is JsonException || 
			ex is ArgumentException)
		{
			context.Response.StatusCode = 400;
			context.Response.ContentType = "text/plain";
			await context.Response.WriteAsync(ex.Message);
		}
		catch (InvalidOperationException ex) when (ex.Message == "Sequence contains no elements")
		{
			context.Response.StatusCode = 404;
		}
	}
}
