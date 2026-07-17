using MediatR;
using FSM.Application.Common;
using FSM.Domain.Entities;
using FSM.Domain.Interfaces;
using FSM.Application.Interfaces; // <-- YENİ: Bildirim servisini çağırdık

namespace FSM.Application.Features.WorkOrders.Commands.DeleteWorkOrder;

public class DeleteWorkOrderCommandHandler : IRequestHandler<DeleteWorkOrderCommand, Unit>
{
    private readonly IGenericRepository<WorkOrder> _repository;

    // 1. Bildirim servisi tanımı
    private readonly INotificationService _notificationService; // <-- EKLENDİ

    // 2. Constructor içine enjekte edilmesi
    public DeleteWorkOrderCommandHandler(
        IGenericRepository<WorkOrder> repository,
        INotificationService notificationService) // <-- EKLENDİ
    {
        _repository = repository;
        _notificationService = notificationService; // <-- EKLENDİ
    }

    public async Task<Unit> Handle(DeleteWorkOrderCommand request, CancellationToken cancellationToken)
    {
        // 1. Veritabanından (Soft Delete) silme/pasife çekme işlemi
        await _repository.SoftDeleteAsync(request.Id, "iş emri");

        // 2. 👇 YENİ EKLENEN KISIM: Sinyali Ateşliyoruz!
        await _notificationService.SendWorkOrderNotification(
            $"🗑️ #{request.Id} numaralı iş emri sistemden silindi/iptal edildi!"
        );

        return Unit.Value;
    }
}