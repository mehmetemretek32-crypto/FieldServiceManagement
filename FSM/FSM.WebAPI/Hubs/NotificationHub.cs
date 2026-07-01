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
}