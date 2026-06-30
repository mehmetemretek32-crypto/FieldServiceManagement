using AutoMapper;
using MediatR;
using FSM.Domain.Entities;
using FSM.Domain.Interfaces;

namespace FSM.Application.Features.WorkOrders.Commands.UpdateWorkOrder;

public class UpdateWorkOrderCommandHandler : IRequestHandler<UpdateWorkOrderCommand, Unit>
{
    private readonly IGenericRepository<WorkOrder> _workOrderRepository;
    private readonly IMapper _mapper;

    public UpdateWorkOrderCommandHandler(IGenericRepository<WorkOrder> workOrderRepository, IMapper mapper)
    {
        _workOrderRepository = workOrderRepository;
        _mapper = mapper;
    }

    // Task yerine Task<Unit> olması şart!
    public async Task<Unit> Handle(UpdateWorkOrderCommand request, CancellationToken cancellationToken)
    {
        var workOrder = await _workOrderRepository.GetByIdAsync(request.Id);

        if (workOrder == null)
            throw new Exception($"ID'si {request.Id} olan iş emri bulunamadı!");

        // Mapper, request'teki alanları var olan workOrder içine yazar
        _mapper.Map(request, workOrder);

        await _workOrderRepository.UpdateAsync(workOrder);

        // MediatR'ın beklediği boş dönüş değeri
        return Unit.Value;
    }
}

