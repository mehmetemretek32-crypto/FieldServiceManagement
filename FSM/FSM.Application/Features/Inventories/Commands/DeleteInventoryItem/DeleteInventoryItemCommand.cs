using FSM.Domain.Entities;
using FSM.Domain.Interfaces;
using MediatR;
using FSM.Application.Common;
using FSM.Application.Interfaces;
using Microsoft.Extensions.Caching.Distributed; // 🔥 REDIS EKLENDİ

namespace FSM.Application.Features.Inventories.Commands.DeleteInventoryItem;

public sealed record DeleteInventoryItemCommand(int Id) : IRequest<string>;

internal sealed class DeleteInventoryItemCommandHandler : IRequestHandler<DeleteInventoryItemCommand, string>
{
    private readonly IGenericRepository<InventoryItem> _repository;
    private readonly INotificationService _notificationService;
    private readonly IDistributedCache _cache; // 🔥 EKLENDİ

    public DeleteInventoryItemCommandHandler(
        IGenericRepository<InventoryItem> repository,
        INotificationService notificationService,
        IDistributedCache cache) // 🔥 EKLENDİ
    {
        _repository = repository;
        _notificationService = notificationService;
        _cache = cache; // 🔥 EŞLEŞTİRİLDİ
    }

    public async Task<string> Handle(DeleteInventoryItemCommand request, CancellationToken cancellationToken)
    {
        var entity = await _repository.GetByIdAsync(request.Id);

        if (entity == null)
            throw new Exception("Silinecek malzeme bulunamadı!");

        string itemName = entity.Name;

        // Diğer modüllerle (Customer, WorkOrder, Technician) tutarlı olması için
        // artık kalıcı silme yerine soft-delete kullanılıyor.
        await _repository.SoftDeleteAsync(request.Id, "malzeme");

        // 👇 🔥 ÇEKMECEYİ TEMİZLİYORUZ (Cache Invalidation)
        await _cache.RemoveAsync("all_inventory_items_list", cancellationToken);

        await _notificationService.SendWorkOrderNotification(
            $"🗑️ Envanterden Malzeme Silindi: {itemName}"
        );

        return "Malzeme sistemden başarıyla silindi!";
    }
}