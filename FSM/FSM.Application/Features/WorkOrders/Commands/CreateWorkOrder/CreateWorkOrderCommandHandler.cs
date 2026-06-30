using AutoMapper;
using FSM.Domain.Entities;
using FSM.Domain.Enums;
using FSM.Domain.Interfaces;
using MediatR;

namespace FSM.Application.Features.WorkOrders.Commands.CreateWorkOrder;

public class CreateWorkOrderCommandHandler : IRequestHandler<CreateWorkOrderCommand, int>
{
    private readonly IGenericRepository<WorkOrder> _workOrderRepository;
    private readonly IMapper _mapper;

    public CreateWorkOrderCommandHandler(IGenericRepository<WorkOrder> workOrderRepository, IMapper mapper)
    {
        _workOrderRepository = workOrderRepository;
        _mapper = mapper;
    }

    public async Task<int> Handle(CreateWorkOrderCommand request, CancellationToken cancellationToken)
    {
        var entity = _mapper.Map<WorkOrder>(request);
        entity.CreatedAt = DateTime.UtcNow;
        entity.State = WorkOrderState.Pending;

        await _workOrderRepository.AddAsync(entity); // AddAsync zaten içeride SaveChangesAsync çağırıyor

        return entity.Id;
    }
}