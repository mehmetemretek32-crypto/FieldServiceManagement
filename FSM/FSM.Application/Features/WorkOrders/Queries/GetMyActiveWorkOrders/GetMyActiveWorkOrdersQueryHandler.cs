using AutoMapper;
using FSM.Application.Common;
using FSM.Application.DTOs.WorkOrders;
using FSM.Application.Features.WorkOrders;
using FSM.Domain.Entities;
using FSM.Domain.Interfaces;
using MediatR;

namespace FSM.Application.Features.WorkOrders.Queries.GetMyActiveWorkOrders;

public class GetMyActiveWorkOrdersQueryHandler : IRequestHandler<GetMyActiveWorkOrdersQuery, IEnumerable<WorkOrderDto>>
{
    private readonly IGenericRepository<WorkOrder> _workOrderRepository;
    private readonly IMapper _mapper;

    public GetMyActiveWorkOrdersQueryHandler(IGenericRepository<WorkOrder> workOrderRepository, IMapper mapper)
    {
        _workOrderRepository = workOrderRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<WorkOrderDto>> Handle(GetMyActiveWorkOrdersQuery request, CancellationToken cancellationToken)
    {
        // Tüm iş emirlerini çekip filtrelüyoruz (Eğer repository'nde Where/Find metodu varsa direkt onu da kullanabilirsin)
        var allWorkOrders = await _workOrderRepository.GetAllAsync();

        var myActiveOrders = allWorkOrders
            .OnlyActive()
            .Where(w => w.TechnicianId == request.TechnicianId);

        return _mapper.Map<IEnumerable<WorkOrderDto>>(myActiveOrders);
    }
}