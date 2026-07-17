using FSM.Application.Common;
using FSM.Application.Interfaces; // <-- YENİ: Bildirim servisi için interface'i çağırdık
using FSM.Domain.Entities;
using FSM.Domain.Enums;
using FSM.Domain.Interfaces;
using MediatR;

namespace FSM.Application.Features.WorkOrders.Commands.UpdateWorkOrder;

public class UpdateWorkOrderCommandHandler : IRequestHandler<UpdateWorkOrderCommand, Unit>
{
    private readonly IGenericRepository<WorkOrder> _workOrderRepository;
    private readonly IGenericRepository<Technician> _technicianRepository;

    // 1. Bildirim servisi tanımı
    private readonly INotificationService _notificationService; // <-- EKLENDİ

    // 2. Constructor içine enjekte edilmesi
    public UpdateWorkOrderCommandHandler(
        IGenericRepository<WorkOrder> workOrderRepository,
        IGenericRepository<Technician> technicianRepository,
        INotificationService notificationService) // <-- EKLENDİ
    {
        _workOrderRepository = workOrderRepository;
        _technicianRepository = technicianRepository;
        _notificationService = notificationService; // <-- EKLENDİ
    }

    public async Task<Unit> Handle(UpdateWorkOrderCommand request, CancellationToken cancellationToken)
    {
        var workOrder = await _workOrderRepository.GetActiveByIdOrThrowAsync(request.Id, "iş emri");

        // İş Kuralı: Teknisyen uygunluk kontrolü
        if (request.TechnicianId.HasValue && request.TechnicianId.Value > 0)
        {
            var technician = await _technicianRepository.GetByIdAsync(request.TechnicianId.Value);

            if (technician != null && !technician.IsAvailable)
            {
                throw new Exception($"Seçilen teknisyen ({technician.FullName}) şu anda başka bir işte meşgul. Lütfen müsait bir teknisyen seçin.");
            }
        }

        // Verilerin Güncellenmesi
        workOrder.Title = request.Title;
        workOrder.Description = request.Description;
        workOrder.State = (WorkOrderState)request.State;
        workOrder.TechnicianId = request.TechnicianId;
        workOrder.CustomerId = request.CustomerId;

        // Kaydetme
        await _workOrderRepository.SaveChangesAsync();

        // 👇 3. YENİ EKLENEN KISIM: Sinyali Ateşliyoruz!
        // Veritabanına başarıyla yazıldıktan sonra tüm bağlı istemcilere haber veriyoruz.
        await _notificationService.SendWorkOrderNotification(
            $"#{workOrder.Id} numaralı iş emri güncellendi!"
        );

        return Unit.Value;
    }
}