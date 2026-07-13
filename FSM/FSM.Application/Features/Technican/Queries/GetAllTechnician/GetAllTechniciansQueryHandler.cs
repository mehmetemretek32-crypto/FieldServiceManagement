using AutoMapper;
using FSM.Application.Common;
using FSM.Application.DTOs.Technicians;
using FSM.Domain.Entities;
using FSM.Domain.Interfaces;
using MediatR;

namespace FSM.Application.Features.Technican.Queries.GetAllTechnician;

public class GetAllTechniciansQueryHandler : IRequestHandler<GetAllTechniciansQuery, List<TechnicianDto>>
{
    private readonly IGenericRepository<Technician> _technicianRepository;
    private readonly IGenericRepository<WorkOrder> _workOrderRepository; // 🔥 YENİ: İş Emirleri deposunu içeri aldık!
    private readonly IMapper _mapper;

    public GetAllTechniciansQueryHandler(
        IGenericRepository<Technician> technicianRepository,
        IGenericRepository<WorkOrder> workOrderRepository,
        IMapper mapper)
    {
        _technicianRepository = technicianRepository;
        _workOrderRepository = workOrderRepository;
        _mapper = mapper;
    }

    public async Task<List<TechnicianDto>> Handle(GetAllTechniciansQuery request, CancellationToken cancellationToken)
    {
        // 1. Silinmemiş teknisyenleri getir
        var technicians = await _technicianRepository.GetAllAsync();
        var activeTechnicians = technicians.OnlyActive().ToList();

        // 2. Silinmemiş ve bir teknisyene atanmış iş emirlerini getir
        var workOrders = await _workOrderRepository.GetAllAsync();

        // Teknisyen ID'ye göre grup yapıp açık iş sayılarını bir sözlükte (Dictionary) tutuyoruz
        var activeWorkOrdersCount = workOrders
            .Where(w => !w.IsDeleted && w.TechnicianId != null && w.TechnicianId != 0)
            .GroupBy(w => w.TechnicianId)
            .ToDictionary(g => (int)g.Key!, g => g.Count());

        // 3. DTO listemizi oluşturuyoruz! 
        // Not: ActiveWorkOrderCount'u dinamik hesapladığımız için burada AutoMapper yerine
        // LINQ Select ile (en hızlı ve en güvenli yöntemle) map'leme yapıyoruz.
        var dtoList = activeTechnicians.Select(t => new TechnicianDto(
            t.Id,
            t.FullName,
            t.Email,
            t.PhoneNumber,
            t.IsAvailable,
            // Sözlükte teknisyenin ID'si varsa iş sayısını al, yoksa 0 yaz! 
            activeWorkOrdersCount.ContainsKey(t.Id) ? activeWorkOrdersCount[t.Id] : 0
        )).ToList();

        return dtoList;
    }
}