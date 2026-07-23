using AutoMapper;
using FSM.Application.DTOs.WorkOrders;
using FSM.Domain.Entities;
using FSM.Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.IdentityModel.JsonWebTokens; 
using System.Security.Claims;
using System.Text.Json;

namespace FSM.Application.Features.WorkOrders.Queries.GetAllWorkOrders;

public class GetAllWorkOrdersQueryHandler : IRequestHandler<GetAllWorkOrdersQuery, IEnumerable<WorkOrderDto>>
{
    private readonly IGenericRepository<WorkOrder> _workOrderRepository;
    private readonly IGenericRepository<Technician> _technicianRepository; // 🆕
    private readonly IMapper _mapper;
    private readonly IDistributedCache _cache;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public GetAllWorkOrdersQueryHandler(
        IGenericRepository<WorkOrder> workOrderRepository,
        IGenericRepository<Technician> technicianRepository, // 🆕
        IMapper mapper,
        IDistributedCache cache,
        IHttpContextAccessor httpContextAccessor)
    {
        _workOrderRepository = workOrderRepository;
        _technicianRepository = technicianRepository; // 🆕
        _mapper = mapper;
        _cache = cache;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<IEnumerable<WorkOrderDto>> Handle(GetAllWorkOrdersQuery request, CancellationToken cancellationToken)
    {
        var user = _httpContextAccessor.HttpContext?.User;
        var role = user?.FindFirst(ClaimTypes.Role)?.Value;

        // 🆕 Teknisyen ise: AppUser'daki e-posta ile Technician tablosundaki e-postayı eşleştirip
        // gerçek Technician.Id'yi buluyoruz (AppUser.Id ile Technician.Id birbiriyle ilişkili değil!)
        int? currentTechnicianId = null;
        if (role == "Technician")
        {
            var email = user?.FindFirst("email")?.Value
              ?? user?.FindFirst(ClaimTypes.Email)?.Value;

            if (!string.IsNullOrEmpty(email))
            {
                var technician = await _technicianRepository.GetAsync(t => t.Email == email);
                currentTechnicianId = technician?.Id;
            }
        }

        string cacheKey = role == "Technician"
            ? $"work_orders_list_tech_{currentTechnicianId}"
            : "all_work_orders_list";

        var cachedData = await _cache.GetStringAsync(cacheKey, cancellationToken);

        if (!string.IsNullOrEmpty(cachedData))
        {
            return JsonSerializer.Deserialize<IEnumerable<WorkOrderDto>>(cachedData);
        }

        var query = _workOrderRepository.GetAllAsQueryable();

        var filteredQuery = query
                 .Include(w => w.Technician)
                 .Where(w => !w.IsDeleted);

        if (role == "Technician")
        {
            // currentTechnicianId null ise (eşleşme bulunamadıysa) kasıtlı olarak boş liste dönsün —
            // yanlışlıkla tüm listeyi göstermesin.
            filteredQuery = filteredQuery.Where(w => w.TechnicianId == currentTechnicianId);
        }

        var workOrders = await filteredQuery.ToListAsync(cancellationToken);

        var mappedWorkOrders = _mapper.Map<IEnumerable<WorkOrderDto>>(workOrders);

        var cacheOptions = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
        };

        var serializedData = JsonSerializer.Serialize(mappedWorkOrders);
        await _cache.SetStringAsync(cacheKey, serializedData, cacheOptions, cancellationToken);

        return mappedWorkOrders;
    }
}