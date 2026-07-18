using MediatR;
using FSM.Application.Common;
using FSM.Domain.Entities;
using FSM.Domain.Interfaces;
using FSM.Application.Interfaces;
using Microsoft.Extensions.Caching.Distributed; // 🔥 REDIS EKLENDİ

namespace FSM.Application.Features.Technicians.Commands.DeleteTechnician;

public class DeleteTechnicianCommandHandler : IRequestHandler<DeleteTechnicianCommand, Unit>
{
    private readonly IGenericRepository<Technician> _repository;
    private readonly INotificationService _notificationService;
    private readonly IDistributedCache _cache; // 🔥 EKLENDİ

    public DeleteTechnicianCommandHandler(
        IGenericRepository<Technician> repository,
        INotificationService notificationService,
        IDistributedCache cache) // 🔥 EKLENDİ
    {
        _repository = repository;
        _notificationService = notificationService;
        _cache = cache; // 🔥 EŞLEŞTİRİLDİ
    }

    public async Task<Unit> Handle(DeleteTechnicianCommand request, CancellationToken cancellationToken)
    {
        // Veritabanından (Soft Delete) silme işlemi
        await _repository.SoftDeleteAsync(request.Id, "teknisyen");

        // 👇 🔥 ÇEKMECELERİ TEMİZLİYORUZ (Cache Invalidation)
        await _cache.RemoveAsync("all_technicians_list", cancellationToken);
        await _cache.RemoveAsync("top_technicians_list", cancellationToken);

        await _notificationService.SendWorkOrderNotification(
            $"⚠️ #{request.Id} ID'li teknisyen sistemden pasife çekildi/silindi!"
        );

        return Unit.Value;
    }
}