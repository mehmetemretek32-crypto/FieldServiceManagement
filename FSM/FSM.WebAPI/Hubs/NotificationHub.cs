using Microsoft.AspNetCore.SignalR;

namespace FSM.WebAPI.Hubs;

// İstemcilerin (teknisyenlerin) bağlanacağı ana merkez
public class NotificationHub : Hub
{
    // Bağlanan kullanıcıyı loglayabilir veya belirli gruplara ekleyebiliriz
    public override async Task OnConnectedAsync()
    {
        await Clients.Caller.SendAsync("Connected", "Sisteme başarıyla bağlandın!");
        await base.OnConnectedAsync();
    }
    public async Task JoinTechnicianGroup(int technicianId)
    {
        string groupName = $"Technician_{technicianId}";
        await Groups.AddToGroupAsync(Context.ConnectionId, groupName);

        // Sadece test amaçlı, bağlandığını anlaması için geri mesaj yollayalım
        await Clients.Caller.SendAsync("Connected", $"{technicianId} ID'li teknisyen grubuna katıldın!");
    }
}