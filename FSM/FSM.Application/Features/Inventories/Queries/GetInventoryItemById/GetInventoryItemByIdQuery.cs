using AutoMapper;
using FSM.Application.DTOs;
using FSM.Domain.Entities;
using FSM.Domain.Interfaces;
using MediatR;

namespace FSM.Application.Features.Inventories.Queries.GetInventoryItemById;

// 🔥 KOMUT: Dışarıdan sadece "Id" alıyoruz
public sealed record GetInventoryItemByIdQuery(int Id) : IRequest<InventoryItemDto>;

// 🔥 İŞLEYİCİ
internal sealed class GetInventoryItemByIdQueryHandler : IRequestHandler<GetInventoryItemByIdQuery, InventoryItemDto>
{
    private readonly IGenericRepository<InventoryItem> _repository;
    private readonly IMapper _mapper;

    public GetInventoryItemByIdQueryHandler(IGenericRepository<InventoryItem> repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<InventoryItemDto> Handle(GetInventoryItemByIdQuery request, CancellationToken cancellationToken)
    {
        var item = await _repository.GetByIdAsync(request.Id);

        if (item == null || item.IsDeleted)
            throw new Exception("Malzeme bulunamadı!"); // İleride buraya özel NotFoundException yazılabilir

        return _mapper.Map<InventoryItemDto>(item);
    }
}