using MediatR;

namespace FSM.Application.Features.WorkOrders.Commands.AssignWorkOrder;

public class AssignWorkOrderCommand : IRequest
{
    public int WorkOrderId { get; set; }
    public int TechnicianId { get; set; }
}