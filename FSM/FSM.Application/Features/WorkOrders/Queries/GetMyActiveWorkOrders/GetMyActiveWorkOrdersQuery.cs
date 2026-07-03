using FSM.Application.DTOs.WorkOrders;
using FSM.Application.Features.WorkOrders; // WorkOrderDto'nun olduğu namespace (kendi yapına göre kontrol et)
using MediatR;

namespace FSM.Application.Features.WorkOrders.Queries.GetMyActiveWorkOrders;

public class GetMyActiveWorkOrdersQuery : IRequest<IEnumerable<WorkOrderDto>>
{
    public int TechnicianId { get; set; }
}