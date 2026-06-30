using AutoMapper;
using FSM.Application.DTOs.WorkOrders;
using FSM.Domain.Entities;
using FSM.Domain.Interfaces;
using MediatR;

namespace FSM.Application.Features.WorkOrders.Queries.GetWorkOrderById;

public class GetWorkOrderByIdQueryHandler : IRequestHandler<GetWorkOrderByIdQuery, WorkOrderDto?>
{
    private readonly IGenericRepository<WorkOrder> _workOrderRepository;
    private readonly IMapper _mapper;

    public GetWorkOrderByIdQueryHandler(IGenericRepository<WorkOrder> workOrderRepository, IMapper mapper)
    {
        _workOrderRepository = workOrderRepository;
        _mapper = mapper;
    }

    public async Task<WorkOrderDto?> Handle(GetWorkOrderByIdQuery request, CancellationToken cancellationToken)
    {
        var workOrder = await _workOrderRepository.GetByIdAsync(request.Id);
        return workOrder == null ? null : _mapper.Map<WorkOrderDto>(workOrder);
    }
}