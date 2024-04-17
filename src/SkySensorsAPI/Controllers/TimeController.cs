using Microsoft.AspNetCore.Mvc;

namespace SkySensorsAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TimeController : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<long>> GetCurrentTime()
    {
        return DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
    }
}

