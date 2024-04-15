using Microsoft.AspNetCore.Mvc;
using SkySensorsAPI.ApplicationServices;

namespace SkySensorsAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class WhetherStationController(
	IWhetherStationAppService weatherStationService) : ControllerBase
{
	[HttpGet]
	public async Task<IActionResult> GetDummyValue()
	{
		return await weatherStationService.GetDummyValue() ? Ok() : NotFound();
	}
}
