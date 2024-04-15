using Microsoft.AspNetCore.Mvc;

namespace SkySensorsAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class TimeController : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<long>> GetCurrentTime()
    {
        return DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
    }
}

