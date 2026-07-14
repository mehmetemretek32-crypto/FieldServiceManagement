using FSM.Domain.Entities;
using FSM.Domain.Enums;
using FSM.Domain.Interfaces;
using MediatR;

// Not: IGenericRepository için Ctrl + . (Nokta) yaparak using'i eklemeyi unutma!

namespace FSM.Application.Features.WorkOrders.Commands.AssignWorkOrder;

public class AssignWorkOrderCommandHandler(
    IGenericRepository<WorkOrder> _workOrderRepository,
    IGenericRepository<Technician> _technicianRepository)
    : IRequestHandler<AssignWorkOrderCommand, bool>
{
    public async Task<bool> Handle(AssignWorkOrderCommand request, CancellationToken cancellationToken)
    {
        // 1. Veritabanından o iş emrini buluyoruz (Senin sistemindeki GetByIdAsync)
        var workOrder = await _workOrderRepository.GetByIdAsync(request.WorkOrderId);
        if (workOrder == null)
            throw new Exception("İş emri bulunamadı!");

        // 2. Teknisyen gerçekten var mı diye teyit ediyoruz
        var technician = await _technicianRepository.GetByIdAsync(request.TechnicianId);
        if (technician == null)
            throw new Exception("Teknisyen bulunamadı!");

        // 3. Sürükle-bırak ekranından gelen verileri iş emrine işliyoruz
        workOrder.TechnicianId = request.TechnicianId;
        workOrder.ScheduledStartDate = request.ScheduledStartDate;
        workOrder.ScheduledEndDate = request.ScheduledEndDate;

        // İşin durumu "Bekliyor"dan "Atandı" (Assigned) konumuna geçiyor. 
        // Eğer WorkOrderState enum'un içinde Assigned yoksa InProgress veya uygun olanı yazabilirsin.
        workOrder.State = WorkOrderState.Assigned;

        // 4. Senin sisteminin kayıt metodu olan SaveChangesAsync'i çağırıyoruz
       
        await _workOrderRepository.SaveChangesAsync();

        return true;
    }
}