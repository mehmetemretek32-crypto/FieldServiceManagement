using AutoMapper;
using FSM.Application.Common;
using FSM.Application.DTOs;
using FSM.Domain.Entities;
using FSM.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Caching.Distributed; // 🔥 REDIS İÇİN EKLENDİ
using System.Text.Json; // 🔥 JSON İÇİN EKLENDİ

namespace FSM.Application.Features.Customers.Queries.GetAllCustomers;

// Not: İsimdeki küçük 'w' harf hatasını da düzelttik! 🚀
public class GetAllCustomersQueryHandler : IRequestHandler<GetAllCustomersQuery, IEnumerable<CustomerDto>>
{
    private readonly IGenericRepository<Customer> _customerRepository;
    private readonly IGenericRepository<WorkOrder> _workOrderRepository;
    private readonly IMapper _mapper;
    private readonly IDistributedCache _cache; // 🔥 1. Cache servisi eklendi

    public GetAllCustomersQueryHandler(
        IGenericRepository<Customer> customerRepository,
        IGenericRepository<WorkOrder> workOrderRepository,
        IMapper mapper,
        IDistributedCache cache) // 🔥 2. Constructor'a eklendi
    {
        _customerRepository = customerRepository;
        _workOrderRepository = workOrderRepository;
        _mapper = mapper;
        _cache = cache;
    }

    public async Task<IEnumerable<CustomerDto>> Handle(GetAllCustomersQuery request, CancellationToken cancellationToken)
    {
        // 🔥 3. Sabit Anahtarımız
        const string cacheKey = "all_customers_list";

        // 🔥 AŞAMA 1: ÖNCE ÇEKMECEYE (REDIS) BAK
        var cachedData = await _cache.GetStringAsync(cacheKey, cancellationToken);
        if (!string.IsNullOrEmpty(cachedData))
        {
            // Veri varsa SQL'i hiç yorma, JSON'dan DTO listesine çevir ve gönder!
            return JsonSerializer.Deserialize<IEnumerable<CustomerDto>>(cachedData)!;
        }

        // 🔥 AŞAMA 2: ÇEKMECE BOŞSA VERİTABANINA GİT (Senin harika kodların)
        var customers = await _customerRepository.GetAllAsync();
        var activeCustomers = customers.OnlyActive().ToList();

        var workOrders = await _workOrderRepository.GetAllAsync();
        var workOrderCounts = workOrders
            .OnlyActive()
            .GroupBy(w => w.CustomerId)
            .ToDictionary(g => g.Key, g => g.Count());

        var customerDtos = _mapper.Map<List<CustomerDto>>(activeCustomers);

        foreach (var dto in customerDtos)
        {
            dto.TotalWorkOrderCount = workOrderCounts.ContainsKey(dto.Id) ? workOrderCounts[dto.Id] : 0;
        }

        // 🔥 AŞAMA 3: HESAPLANAN SONUCU BİR SONRAKİ İSTEK İÇİN ÇEKMECEYE KOY
        var cacheOptions = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10) // 10 dakika boyunca taze kalacak
        };
        await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(customerDtos), cacheOptions, cancellationToken);

        return customerDtos;
    }
}