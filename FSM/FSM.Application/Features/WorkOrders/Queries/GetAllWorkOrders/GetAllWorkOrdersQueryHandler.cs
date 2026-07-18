using AutoMapper;
using FSM.Application.DTOs.WorkOrders;
using FSM.Domain.Entities;
using FSM.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore; // 🔥 BÜYÜK SIR BURADA! (Include için gerekli)
using Microsoft.Extensions.Caching.Distributed; // 🔥 REDIS İÇİN EKLENDİ
using System.Text.Json; // 🔥 JSON DÖNÜŞÜMLERİ İÇİN EKLENDİ

namespace FSM.Application.Features.WorkOrders.Queries.GetAllWorkOrders;

public class GetAllWorkOrdersQueryHandler : IRequestHandler<GetAllWorkOrdersQuery, IEnumerable<WorkOrderDto>>
{
    private readonly IGenericRepository<WorkOrder> _workOrderRepository;
    private readonly IMapper _mapper;
    private readonly IDistributedCache _cache; // 🔥 1. Redis servisimizi tanımlıyoruz

    // 🔥 2. Constructor'a IDistributedCache eklendi ve içeriye alındı
    public GetAllWorkOrdersQueryHandler(IGenericRepository<WorkOrder> workOrderRepository, IMapper mapper, IDistributedCache cache)
    {
        _workOrderRepository = workOrderRepository;
        _mapper = mapper;
        _cache = cache;
    }

    public async Task<IEnumerable<WorkOrderDto>> Handle(GetAllWorkOrdersQuery request, CancellationToken cancellationToken)
    {
        // 🔥 3. Redis'teki çekmecemizin (Cache) üstünde yazacak etiket ismi
        const string cacheKey = "all_work_orders_list";

        // 🔥 4. AŞAMA 1: ÖNCE REDIS'E (ÇEKMECEYE) BAK
        var cachedData = await _cache.GetStringAsync(cacheKey, cancellationToken);

        if (!string.IsNullOrEmpty(cachedData))
        {
            // Çekmecede veri var! SQL'e hiç gitmeden, JSON'ı DTO listesine çevirip fişek gibi döndürüyoruz.
            return JsonSerializer.Deserialize<IEnumerable<WorkOrderDto>>(cachedData);
        }

        // 🔥 5. AŞAMA 2: ÇEKMECE BOŞSA SQL'E GİT (Senin orijinal, temiz kodun)
        var query = _workOrderRepository.GetAllAsQueryable();

        var workOrders = await query
                 .Include(w => w.Technician)
                 .Where(w => !w.IsDeleted)
                 .ToListAsync(cancellationToken);

        // Veriyi DTO'ya dönüştür
        var mappedWorkOrders = _mapper.Map<IEnumerable<WorkOrderDto>>(workOrders);

        // 🔥 6. AŞAMA 3: SQL'DEN ALINAN VERİYİ BİR SONRAKİ İSTEK İÇİN REDIS'E KAYDET
        var cacheOptions = new DistributedCacheEntryOptions
        {
            // Veri Redis'te 10 dakika taze kalsın, sonra otomatik silinsin
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
        };

        // Elimizdeki DTO listesini JSON metnine çevirip Redis'e atıyoruz
        var serializedData = JsonSerializer.Serialize(mappedWorkOrders);
        await _cache.SetStringAsync(cacheKey, serializedData, cacheOptions, cancellationToken);

        // Arayüze veriyi teslim et
        return mappedWorkOrders;
    }
}