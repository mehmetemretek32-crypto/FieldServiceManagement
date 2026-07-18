using MediatR;
using FSM.Application.Common;
using FSM.Domain.Entities;
using FSM.Domain.Interfaces;
using FSM.Application.Interfaces;
using Microsoft.Extensions.Caching.Distributed; // 🔥 1. REDIS İÇİN EKLENDİ

namespace FSM.Application.Features.Customers.Commands.DeleteCustomer;

public class DeleteCustomerCommandHandler : IRequestHandler<DeleteCustomerCommand, Unit>
{
    private readonly IGenericRepository<Customer> _repository;
    private readonly INotificationService _notificationService;
    private readonly IDistributedCache _cache; // 🔥 2. EKLENDİ

    public DeleteCustomerCommandHandler(
        IGenericRepository<Customer> repository,
        INotificationService notificationService,
        IDistributedCache cache) // 🔥 3. EKLENDİ
    {
        _repository = repository;
        _notificationService = notificationService;
        _cache = cache; // 🔥 EŞLEŞTİRİLDİ
    }

    public async Task<Unit> Handle(DeleteCustomerCommand request, CancellationToken cancellationToken)
    {
        // 1. Veritabanından (Soft Delete) silme işlemi (Fırın çalıştı)
        await _repository.SoftDeleteAsync(request.Id, "müşteri");

        // 🔥 2. ÇEKMECEYİ TEMİZLİYORUZ (Cache Invalidation)
        await _cache.RemoveAsync("all_customers_list", cancellationToken);

        // 3. Sinyali Ateşliyoruz!
        await _notificationService.SendWorkOrderNotification(
            $"⚠️ #{request.Id} ID'li müşteri sistemden pasife çekildi/silindi!"
        );

        return Unit.Value;
    }
}