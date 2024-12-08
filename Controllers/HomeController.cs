using Microsoft.AspNetCore.Mvc;

namespace Blogue.Controllers;

[ApiController]
[Route("")]
public class HomeController : ControllerBase
{
    // [ApiKeyAttributes]
    [HttpGet("")]
    public IActionResult Get(
        [FromServices] IConfiguration configuration)
    {
        var env = configuration.GetValue<string>("Env");
        return Ok(new
        {
            environment = env
        });
    }
}