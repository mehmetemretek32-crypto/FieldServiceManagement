using MediatR;

namespace FSM.Application.Features.WorkOrders.Commands.AddMaterialToWorkOrder;

public class AddMaterialToWorkOrderCommand : IRequest<int>
{
    public int WorkOrderId { get; set; }
    public int InventoryItemId { get; set; }
    public int QuantityUsed { get; set; }
}