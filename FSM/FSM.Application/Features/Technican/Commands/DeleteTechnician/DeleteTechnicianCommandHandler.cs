using MediatR;
using FSM.Application.Common;
using FSM.Domain.Entities;
using FSM.Domain.Interfaces;
using FSM.Application.Interfaces; // <-- YENİ: Bildirim servisini çağırdık

namespace FSM.Application.Features.Technicians.Commands.DeleteTechnician;

public class DeleteTechnicianCommandHandler : IRequestHandler<DeleteTechnicianCommand, Unit>
{
    private readonly IGenericRepository<Technician> _repository;

    // 1. Bildirim servisi tanımı
    private readonly INotificationService _notificationService; // <-- EKLENDİ

    // 2. Constructor içine enjekte edilmesi
    public DeleteTechnicianCommandHandler(
        IGenericRepository<Technician> repository,
        INotificationService notificationService) // <-- EKLENDİ
    {
        _repository = repository;
        _notificationService = notificationService; // <-- EKLENDİ
    }

    public async Task<Unit> Handle(DeleteTechnicianCommand request, CancellationToken cancellationToken)
    {
        // 1. Veritabanından (Soft Delete) silme işlemi (Fırın kendi içinde çalışıyor)
        await _repository.SoftDeleteAsync(request.Id, "teknisyen");

        // 2. 👇 YENİ EKLENEN KISIM: Sinyali Ateşliyoruz!
        await _notificationService.SendWorkOrderNotification(
            $"⚠️ #{request.Id} ID'li teknisyen sistemden pasife çekildi/silindi!"
        );

        return Unit.Value;
    }
}