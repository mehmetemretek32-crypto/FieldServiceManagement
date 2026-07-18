using AutoMapper;
using FSM.Application.Common;
using FSM.Application.DTOs.WorkOrders;
using FSM.Domain.Entities;
using FSM.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Caching.Distributed; // 🔥 REDIS İÇİN
using System.Text.Json; // 🔥 JSON İÇİN

namespace FSM.Application.Features.WorkOrders.Queries.GetMyActiveWorkOrders;

public class GetMyActiveWorkOrdersQueryHandler : IRequestHandler<GetMyActiveWorkOrdersQuery, IEnumerable<WorkOrderDto>>
{
    private readonly IGenericRepository<WorkOrder> _workOrderRepository;
    private readonly IMapper _mapper;
    private readonly IDistributedCache _cache; // 🔥 1. Cache eklendi

    public GetMyActiveWorkOrdersQueryHandler(IGenericRepository<WorkOrder> workOrderRepository, IMapper mapper, IDistributedCache cache) // 🔥 2. Constructor'a eklendi
    {
        _workOrderRepository = workOrderRepository;
        _mapper = mapper;
        _cache = cache;
    }

    public async Task<IEnumerable<WorkOrderDto>> Handle(GetMyActiveWorkOrdersQuery request, CancellationToken cancellationToken)
    {
        // 🔥 3. DİNAMİK ANAHTAR: Her teknisyene özel bir çekmece açıyoruz!
        string cacheKey = $"active_orders_for_tech_{request.TechnicianId}";

        // 🔥 AŞAMA 1: TEKNİSYENE ÖZEL ÇEKMECEYE BAK
        var cachedData = await _cache.GetStringAsync(cacheKey, cancellationToken);
        if (!string.IsNullOrEmpty(cachedData))
        {
            return JsonSerializer.Deserialize<IEnumerable<WorkOrderDto>>(cachedData)!;
        }

        // 🔥 AŞAMA 2: ÇEKMECE BOŞSA VERİTABANINA GİT
        var allWorkOrders = await _workOrderRepository.GetAllAsync();

        var myActiveOrders = allWorkOrders
            .OnlyActive()
            .Where(w => w.TechnicianId == request.TechnicianId);

        var mappedData = _mapper.Map<IEnumerable<WorkOrderDto>>(myActiveOrders);

        // 🔥 AŞAMA 3: SONUCU O TEKNİSYENİN ÇEKMECESİNE KOY
        var cacheOptions = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
        };
        await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(mappedData), cacheOptions, cancellationToken);

        return mappedData;
    }
}