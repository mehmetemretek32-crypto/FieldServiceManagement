using AutoMapper;
using FSM.Application.Common;
using FSM.Application.DTOs.Technicians;
using FSM.Domain.Entities;
using FSM.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Caching.Distributed; // 🔥 REDIS İÇİN
using System.Text.Json; // 🔥 JSON İÇİN

namespace FSM.Application.Features.Technican.Queries.GetAllTechnician;

public class GetAllTechniciansQueryHandler : IRequestHandler<GetAllTechniciansQuery, List<TechnicianDto>>
{
    private readonly IGenericRepository<Technician> _technicianRepository;
    private readonly IGenericRepository<WorkOrder> _workOrderRepository;
    private readonly IMapper _mapper;
    private readonly IDistributedCache _cache; // 🔥 1. Cache eklendi

    public GetAllTechniciansQueryHandler(
        IGenericRepository<Technician> technicianRepository,
        IGenericRepository<WorkOrder> workOrderRepository,
        IMapper mapper,
        IDistributedCache cache) // 🔥 2. Constructor'a eklendi
    {
        _technicianRepository = technicianRepository;
        _workOrderRepository = workOrderRepository;
        _mapper = mapper;
        _cache = cache;
    }

    public async Task<List<TechnicianDto>> Handle(GetAllTechniciansQuery request, CancellationToken cancellationToken)
    {
        const string cacheKey = "all_technicians_list"; // 🔥 3. Anahtarımız

        // 🔥 AŞAMA 1: ÇEKMECEYE BAK
        var cachedData = await _cache.GetStringAsync(cacheKey, cancellationToken);
        if (!string.IsNullOrEmpty(cachedData))
        {
            return JsonSerializer.Deserialize<List<TechnicianDto>>(cachedData)!;
        }

        // 🔥 AŞAMA 2: ÇEKMECE BOŞSA VERİTABANINA GİT VE HESAPLA
        var technicians = await _technicianRepository.GetAllAsync();
        var activeTechnicians = technicians.OnlyActive().ToList();

        var workOrders = await _workOrderRepository.GetAllAsync();

        var activeWorkOrdersCount = workOrders
            .Where(w => !w.IsDeleted && w.TechnicianId != null && w.TechnicianId != 0)
            .GroupBy(w => w.TechnicianId)
            .ToDictionary(g => (int)g.Key!, g => g.Count());

        var dtoList = activeTechnicians.Select(t => new TechnicianDto(
            t.Id,
            t.FullName,
            t.Email,
            t.PhoneNumber,
            t.IsAvailable,
            activeWorkOrdersCount.ContainsKey(t.Id) ? activeWorkOrdersCount[t.Id] : 0
        )).ToList();

        // 🔥 AŞAMA 3: SONUCU ÇEKMECEYE KOY (10 Dakika sakla)
        var cacheOptions = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
        };
        await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(dtoList), cacheOptions, cancellationToken);

        return dtoList;
    }
}