using AutoMapper;
using FSM.Domain.Entities;
using FSM.Domain.Interfaces;
using MediatR;
using FSM.Application.Interfaces; // <-- YENİ: Bildirim servisini çağırdık

namespace FSM.Application.Features.Inventories.Commands.CreateInventoryItem;

// 🔥 1. KOMUT (Command) - Dışarıdan (API'den) gelecek veriler
public sealed record CreateInventoryItemCommand(
    string Name,
    string SkuCode,
    int StockQuantity,
    decimal UnitPrice) : IRequest<int>; // Eklenen malzemenin Id'sini dönüyoruz

// 🔥 2. İŞLEYİCİ (Handler) - Repository Pattern ile Veritabanına Kayıt
internal sealed class CreateInventoryItemCommandHandler : IRequestHandler<CreateInventoryItemCommand, int>
{
    private readonly IGenericRepository<InventoryItem> _repository;
    private readonly IMapper _mapper;

    // 1. Bildirim servisi tanımı
    private readonly INotificationService _notificationService; // <-- EKLENDİ

    // 2. Constructor içine enjekte edilmesi
    public CreateInventoryItemCommandHandler(
        IGenericRepository<InventoryItem> repository,
        IMapper mapper,
        INotificationService notificationService) // <-- EKLENDİ
    {
        _repository = repository;
        _mapper = mapper;
        _notificationService = notificationService; // <-- EKLENDİ
    }

    public async Task<int> Handle(CreateInventoryItemCommand request, CancellationToken cancellationToken)
    {
        // AutoMapper ile Command nesnesini InventoryItem Entity'sine dönüştür
        var entity = _mapper.Map<InventoryItem>(request);

        // 🔥 GÜVENLİK KİLİDİ: Yeni eklenen malzeme asla silinmiş olarak başlayamaz!
        entity.IsDeleted = false;

        // Generic Repository üzerinden veritabanına ekle
        await _repository.AddAsync(entity);
        await _repository.SaveChangesAsync(); // Fırın çalıştı 🔥

        // 👇 3. YENİ EKLENEN KISIM: Sinyali Ateşliyoruz!
        await _notificationService.SendWorkOrderNotification(
            $"📦 Yeni Malzeme Eklendi: {entity.Name} (Stok: {entity.StockQuantity})"
        );

        // Eklenen malzemenin ID'sini geri dön
        return entity.Id;
    }
}