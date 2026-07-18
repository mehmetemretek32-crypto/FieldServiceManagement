using AutoMapper;
using FSM.Domain.Entities;
using FSM.Domain.Interfaces;
using MediatR;
using FSM.Application.Interfaces;
using Microsoft.Extensions.Caching.Distributed; // 🔥 REDIS EKLENDİ

namespace FSM.Application.Features.Inventories.Commands.CreateInventoryItem;

// 🔥 1. KOMUT (Command)
public sealed record CreateInventoryItemCommand(
    string Name,
    string SkuCode,
    int StockQuantity,
    decimal UnitPrice) : IRequest<int>;

// 🔥 2. İŞLEYİCİ (Handler)
internal sealed class CreateInventoryItemCommandHandler : IRequestHandler<CreateInventoryItemCommand, int>
{
    private readonly IGenericRepository<InventoryItem> _repository;
    private readonly IMapper _mapper;
    private readonly INotificationService _notificationService;
    private readonly IDistributedCache _cache; // 🔥 EKLENDİ

    public CreateInventoryItemCommandHandler(
        IGenericRepository<InventoryItem> repository,
        IMapper mapper,
        INotificationService notificationService,
        IDistributedCache cache) // 🔥 EKLENDİ
    {
        _repository = repository;
        _mapper = mapper;
        _notificationService = notificationService;
        _cache = cache; // 🔥 EŞLEŞTİRİLDİ
    }

    public async Task<int> Handle(CreateInventoryItemCommand request, CancellationToken cancellationToken)
    {
        var entity = _mapper.Map<InventoryItem>(request);
        entity.IsDeleted = false;

        await _repository.AddAsync(entity);
        await _repository.SaveChangesAsync(); // Fırın çalıştı 🔥

        // 👇 🔥 ÇEKMECEYİ TEMİZLİYORUZ (Cache Invalidation)
        await _cache.RemoveAsync("all_inventory_items_list", cancellationToken);

        await _notificationService.SendWorkOrderNotification(
            $"📦 Yeni Malzeme Eklendi: {entity.Name} (Stok: {entity.StockQuantity})"
        );

        return entity.Id;
    }
}