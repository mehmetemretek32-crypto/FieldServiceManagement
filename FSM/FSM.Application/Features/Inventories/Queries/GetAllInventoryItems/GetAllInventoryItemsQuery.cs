using AutoMapper;
using FSM.Application.DTOs;
using FSM.Domain.Entities;
using FSM.Domain.Interfaces;
using MediatR;

namespace FSM.Application.Features.Inventories.Queries.GetAllInventoryItems;

// 🔥 1. İSTEK (Query) - API'den gelecek listeleme talebi
public class GetAllInventoryItemsQuery : IRequest<IEnumerable<InventoryItemDto>>
{
}

// 🔥 2. İŞLEYİCİ (Handler) - Veritabanından veriyi çekip DTO'ya dönüştüren kısım
public class GetAllInventoryItemsQueryHandler : IRequestHandler<GetAllInventoryItemsQuery, IEnumerable<InventoryItemDto>>
{
    private readonly IGenericRepository<InventoryItem> _repository;
    private readonly IMapper _mapper;

    public GetAllInventoryItemsQueryHandler(IGenericRepository<InventoryItem> repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<InventoryItemDto>> Handle(GetAllInventoryItemsQuery request, CancellationToken cancellationToken)
    {
        // Veritabanından tüm malzemeleri çek (Eğer GetAllAsync gibi bir metodun varsa onu kullan)
        var items = await _repository.GetAllAsync();

        // Çekilen verileri AutoMapper ile DTO'ya dönüştür ve geri dön
        return _mapper.Map<IEnumerable<InventoryItemDto>>(items);
    }
}