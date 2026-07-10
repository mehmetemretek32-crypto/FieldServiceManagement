using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
[ApiController]
public class DashboardController : ControllerBase
{
    [HttpGet("stats")] // <-- Angular tam burayı arıyor!
    public IActionResult GetStats()
    {
        return Ok(new { totalWorkOrders = 10, activeTechnicians = 5 });
    }
}