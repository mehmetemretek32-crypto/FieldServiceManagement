using FSM.Domain.Entities;
using FSM.Domain.Interfaces;
using MediatR;
using FSM.Application.Interfaces; // <-- YENİ: Bildirim servisini çağırdık

namespace FSM.Application.Features.Inventories.Commands.UpdateInventoryItem;

// 🔥 KOMUT: Güncellenecek veriler (Id zorunlu)
public sealed record UpdateInventoryItemCommand(
    int Id,
    string Name,
    string SkuCode,
    int StockQuantity,
    decimal UnitPrice) : IRequest<string>;

// 🔥 İŞLEYİCİ
internal sealed class UpdateInventoryItemCommandHandler : IRequestHandler<UpdateInventoryItemCommand, string>
{
    private readonly IGenericRepository<InventoryItem> _repository;

    // 1. Bildirim servisi tanımı
    private readonly INotificationService _notificationService; // <-- EKLENDİ

    // 2. Constructor içine enjekte edilmesi
    public UpdateInventoryItemCommandHandler(
        IGenericRepository<InventoryItem> repository,
        INotificationService notificationService) // <-- EKLENDİ
    {
        _repository = repository;
        _notificationService = notificationService; // <-- EKLENDİ
    }

    public async Task<string> Handle(UpdateInventoryItemCommand request, CancellationToken cancellationToken)
    {
        var entity = await _repository.GetByIdAsync(request.Id);

        if (entity == null)
            throw new Exception("Güncellenecek malzeme bulunamadı!");

        // Verileri güncelle
        entity.Name = request.Name;
        entity.SkuCode = request.SkuCode;
        entity.StockQuantity = request.StockQuantity;
        entity.UnitPrice = request.UnitPrice;

        await _repository.UpdateAsync(entity);
        await _repository.SaveChangesAsync(); // Fırın çalıştı 🔥

        // 👇 3. YENİ EKLENEN KISIM: Sinyali Ateşliyoruz!
        await _notificationService.SendWorkOrderNotification(
            $"🔄 Envanter Güncellendi: [{entity.SkuCode}] {entity.Name} (Yeni Stok: {entity.StockQuantity})"
        );

        return $"[{entity.SkuCode}] barkodlu malzeme başarıyla güncellendi!";
    }
}