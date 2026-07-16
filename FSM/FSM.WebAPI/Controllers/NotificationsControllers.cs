using FSM.Application.Features.WorkOrders.Queries.GetAllWorkOrders;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FSM.WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class NotificationsController : ControllerBase
{
    private readonly IMediator _mediator;

    public NotificationsController(IMediator mediator)
    {
        _mediator = mediator;
    }
    [HttpGet]
    public async Task<IActionResult> GetRecentNotifications()
    {
        var workOrders = await _mediator.Send(new GetAllWorkOrdersQuery());

        var notifications = workOrders
            .OrderByDescending(w => w.CreatedAt)
            .Take(5)
            .Select(w => new
            {
                Id = w.Id,
                // Enum üzerinden karşılaştırma yapıyoruz (hata veren yer burasıydı)
                Type = w.State == (int)FSM.Domain.Enums.WorkOrderState.Pending ? "critical" : "info",
                Message = w.State == (int)FSM.Domain.Enums.WorkOrderState.Pending
                            ? $"🚨 Yeni Kayıt: #{w.Id} numaralı '{w.Title}' iş emri atama bekliyor!"
                            : $"ℹ️ Bilgi: #{w.Id} numaralı iş emrinin durumu güncellendi.",
                Time = w.CreatedAt.ToString("HH:mm")
            });

        return Ok(notifications);
    }
}