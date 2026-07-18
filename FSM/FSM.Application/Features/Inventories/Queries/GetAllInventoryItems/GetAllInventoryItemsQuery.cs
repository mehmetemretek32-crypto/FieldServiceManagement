using AutoMapper;
using FSM.Application.DTOs;
using FSM.Domain.Entities;
using FSM.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Caching.Distributed; // 🔥 REDIS İÇİN EKLENDİ
using System.Text.Json; // 🔥 JSON İÇİN EKLENDİ

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
    private readonly IDistributedCache _cache; // 🔥 1. Cache servisi eklendi

    public GetAllInventoryItemsQueryHandler(
        IGenericRepository<InventoryItem> repository,
        IMapper mapper,
        IDistributedCache cache) // 🔥 2. Constructor'a eklendi
    {
        _repository = repository;
        _mapper = mapper;
        _cache = cache;
    }

    public async Task<IEnumerable<InventoryItemDto>> Handle(GetAllInventoryItemsQuery request, CancellationToken cancellationToken)
    {
        // 🔥 3. Sabit Anahtarımız
        const string cacheKey = "all_inventory_items_list";

        // 🔥 AŞAMA 1: ÖNCE ÇEKMECEYE (REDIS) BAK
        var cachedData = await _cache.GetStringAsync(cacheKey, cancellationToken);
        if (!string.IsNullOrEmpty(cachedData))
        {
            // Veri varsa SQL'i hiç yorma, JSON'dan çevir ve geri dön!
            return JsonSerializer.Deserialize<IEnumerable<InventoryItemDto>>(cachedData)!;
        }

        // 🔥 AŞAMA 2: ÇEKMECE BOŞSA VERİTABANINA GİT
        var items = await _repository.GetAllAsync();
        var dtoList = _mapper.Map<IEnumerable<InventoryItemDto>>(items);

        // 🔥 AŞAMA 3: SONUCU ÇEKMECEYE KOY (10 Dakika taze kalsın)
        var cacheOptions = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
        };
        await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(dtoList), cacheOptions, cancellationToken);

        return dtoList;
    }
}