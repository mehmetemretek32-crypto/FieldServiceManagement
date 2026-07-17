using MediatR;
using FSM.Application.Common;
using FSM.Domain.Entities;
using FSM.Domain.Interfaces;
using FSM.Application.Interfaces; // <-- YENİ: Bildirim servisini çağırdık

namespace FSM.Application.Features.Customers.Commands.DeleteCustomer;

public class DeleteCustomerCommandHandler : IRequestHandler<DeleteCustomerCommand, Unit>
{
    private readonly IGenericRepository<Customer> _repository;

    // 1. Bildirim servisi tanımı
    private readonly INotificationService _notificationService; // <-- EKLENDİ

    // 2. Constructor içine enjekte edilmesi
    public DeleteCustomerCommandHandler(
        IGenericRepository<Customer> repository,
        INotificationService notificationService) // <-- EKLENDİ
    {
        _repository = repository;
        _notificationService = notificationService; // <-- EKLENDİ
    }

    public async Task<Unit> Handle(DeleteCustomerCommand request, CancellationToken cancellationToken)
    {
        // 1. Veritabanından (Soft Delete) silme işlemi (Fırın çalıştı)
        await _repository.SoftDeleteAsync(request.Id, "müşteri");

        // 2. 👇 YENİ EKLENEN KISIM: Sinyali Ateşliyoruz!
        await _notificationService.SendWorkOrderNotification(
            $"⚠️ #{request.Id} ID'li müşteri sistemden pasife çekildi/silindi!"
        );

        return Unit.Value;
    }
}