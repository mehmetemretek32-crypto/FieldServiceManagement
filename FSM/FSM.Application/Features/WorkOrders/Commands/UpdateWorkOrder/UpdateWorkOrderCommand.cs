using MediatR;

namespace FSM.Application.Features.WorkOrders.Commands.UpdateWorkOrder;

public class UpdateWorkOrderCommand : IRequest<Unit>
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}