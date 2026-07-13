using FSM.Application.Common;
using FSM.Application.Interfaces;
using FSM.Domain.Entities;
using FSM.Domain.Enums;
using FSM.Domain.Interfaces;
using MediatR;

namespace FSM.Application.Features.WorkOrders.Commands.AssignWorkOrder;

public class AssignWorkOrderCommandHandler : IRequestHandler<AssignWorkOrderCommand>
{
    private readonly IGenericRepository<WorkOrder> _workOrderRepository;
    private readonly IGenericRepository<Technician> _technicianRepository;
    private readonly INotificationService _notificationService; // 1. Bildirim servisi eklendi

    public AssignWorkOrderCommandHandler(
        IGenericRepository<WorkOrder> workOrderRepository,
        IGenericRepository<Technician> technicianRepository,
        INotificationService notificationService) // 2. Constructor'da inject edildi
    {
        _workOrderRepository = workOrderRepository;
        _technicianRepository = technicianRepository;
        _notificationService = notificationService;
    }

    public async Task Handle(AssignWorkOrderCommand request, CancellationToken cancellationToken)
    {
        var workOrder = await _workOrderRepository.GetByIdOrThrowAsync(request.WorkOrderId, "iş emri");
        var technician = await _technicianRepository.GetByIdOrThrowAsync(request.TechnicianId, "teknisyen");

        // Mükemmel kontrollerin aynen duruyor
        if (technician.IsDeleted)
            throw new Exception("Sistemden silinmiş bir teknisyene yeni iş atanamaz!");
        if (!technician.IsAvailable)
            throw new Exception($"{technician.FullName} adlı teknisyen şu an müsait değil.");

        // Durum güncellemeleri
        workOrder.TechnicianId = technician.Id;
        workOrder.State = WorkOrderState.Assigned;
        technician.IsAvailable = false;

        // İkisi de aynı DbContext'e (Scoped) bağlı olduğu için tek SaveChanges ikisini de yazar.
        await _workOrderRepository.UpdateAsync(workOrder);
        await _workOrderRepository.SaveChangesAsync(); // KRİTİK: Veritabanına işlemi yansıtıyoruz

        // 3. SADECE İLGİLİ TEKNİSYENE ÖZEL ANLIK BİLDİRİM
        string notificationMessage = $"Yeni Görev: {workOrder.Title} başlıklı iş emri size atandı!";
        await _notificationService.SendNotificationToTechnician(technician.Id, notificationMessage);
    }
}