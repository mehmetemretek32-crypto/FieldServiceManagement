using FSM.Application.DTOs.Dashboard;
using FSM.Domain.Entities;
using FSM.Domain.Interfaces;
using FSM.Application.Common; // .OnlyActive() için gerekli
using MediatR;
using Microsoft.Extensions.Caching.Distributed; // Redis için
using System.Text.Json; // JSON serileştirme için

namespace FSM.Application.Features.Dashboard.Queries.GetDashboardStats
{
    public class GetDashboardStatsQueryHandler : IRequestHandler<GetDashboardStatsQuery, DashboardStatsDto>
    {
        private readonly IGenericRepository<WorkOrder> _workOrderRepository;
        private readonly IGenericRepository<Technician> _technicianRepository;
        private readonly IDistributedCache _cache;

        public GetDashboardStatsQueryHandler(
            IGenericRepository<WorkOrder> workOrderRepository,
            IGenericRepository<Technician> technicianRepository,
            IDistributedCache cache)
        {
            _workOrderRepository = workOrderRepository;
            _technicianRepository = technicianRepository;
            _cache = cache;
        }

        public async Task<DashboardStatsDto> Handle(GetDashboardStatsQuery request, CancellationToken cancellationToken)
        {
            const string cacheKey = "dashboard_stats_data";

            // 🔥 AŞAMA 1: ÖNCE ÇEKMECEYE (REDIS) BAK
            var cachedData = await _cache.GetStringAsync(cacheKey, cancellationToken);
            if (!string.IsNullOrEmpty(cachedData))
            {
                // Veri varsa SQL'i yorma, JSON'dan DTO'ya çevir ve gönder!
                return JsonSerializer.Deserialize<DashboardStatsDto>(cachedData)!;
            }

            // 🔥 AŞAMA 2: ÇEKMECE BOŞSA VERİTABANI (REPOSITORY) ÜZERİNDEN HESAPLA
            var workOrders = await _workOrderRepository.GetAllAsync();
            var activeWorkOrders = workOrders.OnlyActive().ToList();

            var technicians = await _technicianRepository.GetAllAsync();
            var activeTechnicians = technicians.OnlyActive().ToList();

            var stats = new DashboardStatsDto
            {
                TotalWorkOrders = activeWorkOrders.Count,
                ActiveTechnicians = activeTechnicians.Count,
                // Not: Eğer State property'si Enum ise (int)w.State şeklinde karşılaştırma garanti olur.
                PendingAssignments = activeWorkOrders.Count(w => (int)w.State == 1),
                CompletedJobs = activeWorkOrders.Count(w => (int)w.State == 3)
            };

            // 🔥 AŞAMA 3: SONUCU REDIS'E KAYDET
            // Dashboard verileri genelde sık değiştiği için süreyi 2 dakika olarak ayarladım
            var cacheOptions = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(2)
            };
            await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(stats), cacheOptions, cancellationToken);

            return stats;
        }
    }
}