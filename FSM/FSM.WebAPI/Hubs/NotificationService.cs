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

    // Genel bildirim gönderme (Bütün sisteme)
    public async Task SendWorkOrderNotification(string message)
    {
        await _hubContext.Clients.All.SendAsync("ReceiveNotification", message);
    }

    // İşte senin gruplama mantığına özel harika metot! 
    // Sadece işin atandığı teknisyene bildirim gider:
    public async Task SendNotificationToTechnician(int technicianId, string message)
    {
        string groupName = $"Technician_{technicianId}";
        await _hubContext.Clients.Group(groupName).SendAsync("ReceiveNotification", message);
    }
}