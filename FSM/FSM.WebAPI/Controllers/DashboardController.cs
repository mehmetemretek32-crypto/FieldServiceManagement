using FSM.Application.DTOs.Dashboard;
using FSM.Application.Features.Dashboard.Queries.GetDashboardStats;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FSM.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        private readonly IMediator _mediator;

        public DashboardController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("stats")]
        public async Task<ActionResult<DashboardStatsDto>> GetStats()
        {
            // Mediator aracılığıyla az önce yazdığımız Handler'ı tetikliyoruz
            var result = await _mediator.Send(new GetDashboardStatsQuery());
            return Ok(result);
        }
    }
}