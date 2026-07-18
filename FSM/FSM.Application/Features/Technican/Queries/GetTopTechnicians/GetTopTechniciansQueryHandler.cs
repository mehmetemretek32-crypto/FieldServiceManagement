using FSM.Domain.Entities;
using FSM.Domain.Enums;
using FSM.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Caching.Distributed; // 🔥 REDIS İÇİN
using System.Text.Json; // 🔥 JSON İÇİN
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FSM.Application.Features.Technicians.Queries.GetTopTechnicians;

public class GetTopTechniciansQueryHandler : IRequestHandler<GetTopTechniciansQuery, List<TopTechnicianDto>>
{
    private readonly IGenericRepository<Technician> _technicianRepository;
    private readonly IGenericRepository<WorkOrder> _workOrderRepository;
    private readonly IDistributedCache _cache; // 🔥 1. Cache eklendi

    public GetTopTechniciansQueryHandler(
        IGenericRepository<Technician> technicianRepository,
        IGenericRepository<WorkOrder> workOrderRepository,
        IDistributedCache cache) // 🔥 2. Constructor'a eklendi
    {
        _technicianRepository = technicianRepository;
        _workOrderRepository = workOrderRepository;
        _cache = cache;
    }

    public async Task<List<TopTechnicianDto>> Handle(GetTopTechniciansQuery request, CancellationToken cancellationToken)
    {
        const string cacheKey = "top_technicians_list"; // 🔥 3. Anahtarımız (Farklı olmalı!)

        // 🔥 AŞAMA 1: ÇEKMECEYE BAK
        var cachedData = await _cache.GetStringAsync(cacheKey, cancellationToken);
        if (!string.IsNullOrEmpty(cachedData))
        {
            return JsonSerializer.Deserialize<List<TopTechnicianDto>>(cachedData)!;
        }

        // 🔥 AŞAMA 2: ÇEKMECE BOŞSA VERİTABANINA GİT VE HESAPLA
        var technicians = await _technicianRepository.GetAllAsync();
        var workOrders = await _workOrderRepository.GetAllAsync();

        var activeTechnicians = technicians.Where(t => !t.IsDeleted).ToList();

        var completedJobsCount = workOrders
            .Where(w => !w.IsDeleted && w.TechnicianId != null && w.State == WorkOrderState.Completed)
            .GroupBy(w => w.TechnicianId)
            .ToDictionary(g => (int)g.Key!, g => g.Count());

        var topTechnicians = activeTechnicians
            .Select(t => new TopTechnicianDto
            {
                Id = t.Id,
                FullName = t.FullName,
                CompletedJobs = completedJobsCount.ContainsKey(t.Id) ? completedJobsCount[t.Id] : 0
            })
            .OrderByDescending(t => t.CompletedJobs)
            .Take(5)
            .ToList();

        // 🔥 AŞAMA 3: SONUCU ÇEKMECEYE KOY (10 Dakika sakla)
        var cacheOptions = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
        };
        await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(topTechnicians), cacheOptions, cancellationToken);

        return topTechnicians;
    }
}