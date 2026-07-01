using MediatR;

namespace FSM.Application.Features.WorkOrders.Commands.DeleteWorkOrder;

public class DeleteWorkOrderCommand : IRequest<Unit>
{
    public int Id { get; set; }
}