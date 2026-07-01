namespace FSM.Application.Interfaces;

public interface INotificationService
{
    Task SendWorkOrderNotification(string message);
}