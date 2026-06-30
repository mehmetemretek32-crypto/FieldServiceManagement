using MediatR;

namespace FSM.Application.Features.WorkOrders.Commands.CreateWorkOrder;

public class CreateWorkOrderCommand : IRequest<int>
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int CustomerId { get; set; }
}