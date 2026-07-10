using FSM.Domain.Entities;
using FSM.Domain.Interfaces;
using MediatR;

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

    public UpdateInventoryItemCommandHandler(IGenericRepository<InventoryItem> repository)
    {
        _repository = repository;
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
        await _repository.SaveChangesAsync();

        return $"[{entity.SkuCode}] barkodlu malzeme başarıyla güncellendi!";
    }
}