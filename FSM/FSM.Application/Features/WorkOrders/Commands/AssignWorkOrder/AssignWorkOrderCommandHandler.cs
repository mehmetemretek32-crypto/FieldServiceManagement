using FSM.Domain.Entities;
using FSM.Domain.Enums;
using FSM.Domain.Interfaces;
using MediatR;

namespace FSM.Application.Features.WorkOrders.Commands.AssignWorkOrder;

public class AssignWorkOrderCommandHandler : IRequestHandler<AssignWorkOrderCommand>
{
    private readonly IGenericRepository<WorkOrder> _workOrderRepository;
    private readonly IGenericRepository<Technician> _technicianRepository;

    public AssignWorkOrderCommandHandler(
        IGenericRepository<WorkOrder> workOrderRepository,
        IGenericRepository<Technician> technicianRepository)
    {
        _workOrderRepository = workOrderRepository;
        _technicianRepository = technicianRepository;
    }

    public async Task Handle(AssignWorkOrderCommand request, CancellationToken cancellationToken)
    {
        var workOrder = await _workOrderRepository.GetByIdAsync(request.WorkOrderId);
        var technician = await _technicianRepository.GetByIdAsync(request.TechnicianId);

        if (workOrder == null)
            throw new Exception($"ID'si {request.WorkOrderId} olan iş emri bulunamadı.");
        if (technician == null)
            throw new Exception($"ID'si {request.TechnicianId} olan teknisyen bulunamadı.");
        if (technician.IsDeleted)
            throw new Exception("Sistemden silinmiş bir teknisyene yeni iş atanamaz!");
        if (!technician.IsAvailable)
            throw new Exception($"{technician.FullName} adlı teknisyen şu an müsait değil.");

        workOrder.TechnicianId = technician.Id;
        workOrder.State = WorkOrderState.Assigned;
        technician.IsAvailable = false;

        // İkisi de aynı DbContext'e (Scoped) bağlı olduğu için tek SaveChanges ikisini de yazar.
        await _workOrderRepository.UpdateAsync(workOrder);
    }
}