using FSM.Application.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace FSM.WebAPI.Hubs;

public class NotificationService : INotificationService
{
    private readonly IHubContext<NotificationHub> _hubContext;

    public NotificationService(IHubContext<NotificationHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task SendWorkOrderNotification(string message)
    {
        // İsim Angular'daki dinleyici ile aynı olmalı
        await _hubContext.Clients.All.SendAsync("ReceiveWorkOrderUpdate", message);
    }

    // YENİ METOT: Sadece ilgili teknisyenin grubuna mesaj gönderir
    public async Task SendNotificationToTechnician(int technicianId, string message)
    {
        string groupName = $"Technician_{technicianId}";
        await _hubContext.Clients.Group(groupName).SendAsync("ReceiveWorkOrder", message);
    }
}