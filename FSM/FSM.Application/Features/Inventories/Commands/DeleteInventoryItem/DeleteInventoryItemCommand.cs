using FSM.Domain.Entities;
using FSM.Domain.Interfaces;
using MediatR;
using FSM.Application.Interfaces; // <-- YENİ: Bildirim servisini çağırdık

namespace FSM.Application.Features.Inventories.Commands.DeleteInventoryItem;

// 🔥 KOMUT: Sadece silinecek malzemenin Id'si yeterli
public sealed record DeleteInventoryItemCommand(int Id) : IRequest<string>;

// 🔥 İŞLEYİCİ
internal sealed class DeleteInventoryItemCommandHandler : IRequestHandler<DeleteInventoryItemCommand, string>
{
    private readonly IGenericRepository<InventoryItem> _repository;

    // 1. Bildirim servisi tanımı
    private readonly INotificationService _notificationService; // <-- EKLENDİ

    // 2. Constructor içine enjekte edilmesi
    public DeleteInventoryItemCommandHandler(
        IGenericRepository<InventoryItem> repository,
        INotificationService notificationService) // <-- EKLENDİ
    {
        _repository = repository;
        _notificationService = notificationService; // <-- EKLENDİ
    }

    public async Task<string> Handle(DeleteInventoryItemCommand request, CancellationToken cancellationToken)
    {
        var entity = await _repository.GetByIdAsync(request.Id);

        if (entity == null)
            throw new Exception("Silinecek malzeme bulunamadı!");

        // Bildirimde kullanmak üzere malzemenin adını bir kenara not alıyoruz
        string itemName = entity.Name;

        await _repository.DeleteAsync(entity); // Repository'ndeki silme metodu neyse onu kullan (Örn: Remove)
        await _repository.SaveChangesAsync(); // Fırın çalıştı 🔥

        // 👇 3. YENİ EKLENEN KISIM: Sinyali Ateşliyoruz!
        await _notificationService.SendWorkOrderNotification(
            $"🗑️ Envanterden Malzeme Silindi: {itemName}"
        );

        return "Malzeme sistemden başarıyla silindi!";
    }
}