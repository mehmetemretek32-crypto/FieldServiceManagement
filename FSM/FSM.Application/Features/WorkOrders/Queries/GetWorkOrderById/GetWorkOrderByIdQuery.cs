using FSM.Application.DTOs.WorkOrders;
using MediatR;

namespace FSM.Application.Features.WorkOrders.Queries.GetWorkOrderById;

public class GetWorkOrderByIdQuery : IRequest<WorkOrderDto?>
{
    public int Id { get; set; }

    public GetWorkOrderByIdQuery(int id)
    {
        Id = id;
    }
}