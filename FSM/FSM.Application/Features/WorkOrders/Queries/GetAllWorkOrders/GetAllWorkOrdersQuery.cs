using FSM.Application.DTOs.WorkOrders;
using MediatR;

namespace FSM.Application.Features.WorkOrders.Queries.GetAllWorkOrders;

public class GetAllWorkOrdersQuery : IRequest<IEnumerable<WorkOrderDto>>
{
}