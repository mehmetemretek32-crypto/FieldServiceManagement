namespace FSM.Application.Interfaces;

public interface INotificationService
{
    // Eski metot (İstersen tüm sisteme duyuru yapmak için kalabilir)
    Task SendWorkOrderNotification(string message);

    // YENİ METOT: Sadece belirli bir teknisyene özel bildirim
    Task SendNotificationToTechnician(int technicianId, string message);
}