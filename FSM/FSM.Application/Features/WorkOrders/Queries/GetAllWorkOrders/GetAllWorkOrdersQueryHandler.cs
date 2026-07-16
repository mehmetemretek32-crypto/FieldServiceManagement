using AutoMapper;
using FSM.Application.DTOs.WorkOrders;
using FSM.Domain.Entities;
using FSM.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore; // 🔥 BÜYÜK SIR BURADA! (Include için gerekli)

namespace FSM.Application.Features.WorkOrders.Queries.GetAllWorkOrders;

public class GetAllWorkOrdersQueryHandler : IRequestHandler<GetAllWorkOrdersQuery, IEnumerable<WorkOrderDto>>
{
    private readonly IGenericRepository<WorkOrder> _workOrderRepository;
    private readonly IMapper _mapper;

    public GetAllWorkOrdersQueryHandler(IGenericRepository<WorkOrder> workOrderRepository, IMapper mapper)
    {
        _workOrderRepository = workOrderRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<WorkOrderDto>> Handle(GetAllWorkOrdersQuery request, CancellationToken cancellationToken)
    {
        // 1. Veriyi IQueryable olarak al ve Include işlemini yap
        var query = _workOrderRepository.GetAllAsQueryable();

        var workOrders = await query
                 .Include(w => w.Technician)
                 .Where(w => !w.IsDeleted)
                 .ToListAsync(cancellationToken);

        // 2. AutoMapper profile içindeki kurallara göre tek satırda dönüştür
        // (Az önce MappingProfile'a eklediğimiz kurallar burada otomatik çalışacak)
        return _mapper.Map<IEnumerable<WorkOrderDto>>(workOrders);
    }
}