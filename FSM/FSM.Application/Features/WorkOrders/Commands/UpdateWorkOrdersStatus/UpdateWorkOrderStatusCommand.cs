using MediatR;

namespace FSM.Application.Features.WorkOrders.Commands.UpdateWorkOrderStatus;

public class UpdateWorkOrderStatusCommand : IRequest<bool>
{
    public int WorkOrderId { get; set; }
    public string NewStatus { get; set; } = string.Empty; // Yolda, İşlemde, Tamamlandı, İptal
}