using FSM.Application.Common;
using FSM.Domain.Entities;
using FSM.Domain.Enums;
using FSM.Domain.Interfaces;
using MediatR;

namespace FSM.Application.Features.WorkOrders.Commands.UpdateWorkOrder;

public class UpdateWorkOrderCommandHandler : IRequestHandler<UpdateWorkOrderCommand, Unit>
{
    private readonly IGenericRepository<WorkOrder> _workOrderRepository;

    public UpdateWorkOrderCommandHandler(IGenericRepository<WorkOrder> workOrderRepository)
    {
        _workOrderRepository = workOrderRepository;
    }

    public async Task<Unit> Handle(UpdateWorkOrderCommand request, CancellationToken cancellationToken)
    {
        var workOrder = await _workOrderRepository.GetActiveByIdOrThrowAsync(request.Id, "iş emri");

        workOrder.Title = request.Title;
        workOrder.Description = request.Description;
        workOrder.State = (WorkOrderState)request.State;
        workOrder.TechnicianId = request.TechnicianId;
        workOrder.CustomerId = request.CustomerId;

        await _workOrderRepository.UpdateAsync(workOrder);
        return Unit.Value;
    }
}