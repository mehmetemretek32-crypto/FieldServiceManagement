using FSM.Domain.Entities;
using FSM.Domain.Interfaces;
using MediatR;

namespace FSM.Application.Features.Inventories.Commands.DeleteInventoryItem;

// 🔥 KOMUT: Sadece silinecek malzemenin Id'si yeterli
public sealed record DeleteInventoryItemCommand(int Id) : IRequest<string>;

// 🔥 İŞLEYİCİ
internal sealed class DeleteInventoryItemCommandHandler : IRequestHandler<DeleteInventoryItemCommand, string>
{
    private readonly IGenericRepository<InventoryItem> _repository;

    public DeleteInventoryItemCommandHandler(IGenericRepository<InventoryItem> repository)
    {
        _repository = repository;
    }

    public async Task<string> Handle(DeleteInventoryItemCommand request, CancellationToken cancellationToken)
    {
        var entity = await _repository.GetByIdAsync(request.Id);
        
        if (entity == null)
            throw new Exception("Silinecek malzeme bulunamadı!");

        await _repository.DeleteAsync(entity); // Repository'ndeki silme metodu neyse onu kullan (Örn: Remove)
        await _repository.SaveChangesAsync();

        return "Malzeme sistemden başarıyla silindi!";
    }
}