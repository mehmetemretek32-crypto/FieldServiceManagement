using AutoMapper;
using MediatR;
using FSM.Application.Common;
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

        // Verileri doğrudan aktar (AutoMapper'ı devre dışı bırakıp manuel yapıyoruz ki hata payı kalmasın)
        workOrder.Title = request.Title;
        workOrder.Description = request.Description;
        workOrder.State = (FSM.Domain.Enums.WorkOrderState)request.Status;
        workOrder.TechnicianId = request.TechnicianId;
        workOrder.CustomerId = request.CustomerId;

        await _workOrderRepository.UpdateAsync(workOrder);
        return Unit.Value;
    }
}