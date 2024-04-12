using Microsoft.AspNetCore.Mvc;
using SkySensorsAPI.Services;

namespace SkySensorsAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class WhetherStationController(
	IWhetherStationService weatherStationService) : ControllerBase
{
	[HttpGet]
	public async Task<IActionResult> GetDummyValue()
	{
		return await weatherStationService.GetDummyValue() ? Ok() : NotFound();
	}
}
