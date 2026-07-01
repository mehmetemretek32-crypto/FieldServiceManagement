using Microsoft.AspNetCore.SignalR;
using FSM.Application.Interfaces;
using FSM.WebAPI.Hubs;

public class NotificationService : INotificationService
{
    private readonly IHubContext<NotificationHub> _hubContext;

    public NotificationService(IHubContext<NotificationHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task SendWorkOrderNotification(string message)
    {
        await _hubContext.Clients.All.SendAsync("ReceiveWorkOrder", message);
    }
}