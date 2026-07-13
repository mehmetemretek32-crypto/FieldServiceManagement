using AutoMapper;
using FSM.Domain.Entities;
using FSM.Domain.Interfaces;
using MediatR;

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

    public CreateInventoryItemCommandHandler(IGenericRepository<InventoryItem> repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<int> Handle(CreateInventoryItemCommand request, CancellationToken cancellationToken)
    {
        // AutoMapper ile Command nesnesini InventoryItem Entity'sine dönüştür
        var entity = _mapper.Map<InventoryItem>(request);

        // 🔥 GÜVENLİK KİLİDİ: Yeni eklenen malzeme asla silinmiş olarak başlayamaz!
        entity.IsDeleted = false;

        // Generic Repository üzerinden veritabanına ekle
        await _repository.AddAsync(entity);
        await _repository.SaveChangesAsync();

        // Eklenen malzemenin ID'sini geri dön
        return entity.Id;
    }
}