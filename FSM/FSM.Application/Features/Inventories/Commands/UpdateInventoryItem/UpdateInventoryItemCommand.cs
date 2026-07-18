using FSM.Domain.Entities;
using FSM.Domain.Interfaces;
using MediatR;
using FSM.Application.Interfaces;
using Microsoft.Extensions.Caching.Distributed; // 🔥 REDIS EKLENDİ

namespace FSM.Application.Features.Inventories.Commands.UpdateInventoryItem;

public sealed record UpdateInventoryItemCommand(
    int Id,
    string Name,
    string SkuCode,
    int StockQuantity,
    decimal UnitPrice) : IRequest<string>;

internal sealed class UpdateInventoryItemCommandHandler : IRequestHandler<UpdateInventoryItemCommand, string>
{
    private readonly IGenericRepository<InventoryItem> _repository;
    private readonly INotificationService _notificationService;
    private readonly IDistributedCache _cache; // 🔥 EKLENDİ

    public UpdateInventoryItemCommandHandler(
        IGenericRepository<InventoryItem> repository,
        INotificationService notificationService,
        IDistributedCache cache) // 🔥 EKLENDİ
    {
        _repository = repository;
        _notificationService = notificationService;
        _cache = cache; // 🔥 EŞLEŞTİRİLDİ
    }

    public async Task<string> Handle(UpdateInventoryItemCommand request, CancellationToken cancellationToken)
    {
        var entity = await _repository.GetByIdAsync(request.Id);

        if (entity == null)
            throw new Exception("Güncellenecek malzeme bulunamadı!");

        entity.Name = request.Name;
        entity.SkuCode = request.SkuCode;
        entity.StockQuantity = request.StockQuantity;
        entity.UnitPrice = request.UnitPrice;

        await _repository.UpdateAsync(entity);
        await _repository.SaveChangesAsync(); // Fırın çalıştı 🔥

        // 👇 🔥 ÇEKMECEYİ TEMİZLİYORUZ (Cache Invalidation)
        await _cache.RemoveAsync("all_inventory_items_list", cancellationToken);

        await _notificationService.SendWorkOrderNotification(
            $"🔄 Envanter Güncellendi: [{entity.SkuCode}] {entity.Name} (Yeni Stok: {entity.StockQuantity})"
        );

        return $"[{entity.SkuCode}] barkodlu malzeme başarıyla güncellendi!";
    }
}