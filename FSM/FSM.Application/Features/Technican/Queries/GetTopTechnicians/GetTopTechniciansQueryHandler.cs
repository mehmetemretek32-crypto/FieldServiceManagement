using FSM.Domain.Entities;
using FSM.Domain.Enums; // WorkOrderState enum'ı için
using FSM.Domain.Interfaces; // IGenericRepository için
using MediatR;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FSM.Application.Features.Technicians.Queries.GetTopTechnicians;

public class GetTopTechniciansQueryHandler : IRequestHandler<GetTopTechniciansQuery, List<TopTechnicianDto>>
{
    private readonly IGenericRepository<Technician> _technicianRepository;
    private readonly IGenericRepository<WorkOrder> _workOrderRepository;

    public GetTopTechniciansQueryHandler(
        IGenericRepository<Technician> technicianRepository,
        IGenericRepository<WorkOrder> workOrderRepository)
    {
        _technicianRepository = technicianRepository;
        _workOrderRepository = workOrderRepository;
    }

    public async Task<List<TopTechnicianDto>> Handle(GetTopTechniciansQuery request, CancellationToken cancellationToken)
    {
        // 1. Tüm verileri Repository üzerinden getiriyoruz
        var technicians = await _technicianRepository.GetAllAsync();
        var workOrders = await _workOrderRepository.GetAllAsync();

        // 2. Sadece aktif (silinmemiş) teknisyenleri alıyoruz
        var activeTechnicians = technicians.Where(t => !t.IsDeleted).ToList();

        // 3. Tamamlanan (Completed) işleri teknisyen ID'sine göre gruplayıp sayıyoruz
        var completedJobsCount = workOrders
            .Where(w => !w.IsDeleted && w.TechnicianId != null && w.State == WorkOrderState.Completed)
            .GroupBy(w => w.TechnicianId)
            .ToDictionary(g => (int)g.Key!, g => g.Count());

        // 4. Teknisyenleri DTO'ya map'liyor, iş sayısına göre sıralayıp EN İYİ 5'İ alıyoruz!
        var topTechnicians = activeTechnicians
            .Select(t => new TopTechnicianDto
            {
                Id = t.Id,
                FullName = t.FullName,
                CompletedJobs = completedJobsCount.ContainsKey(t.Id) ? completedJobsCount[t.Id] : 0
            })
            .OrderByDescending(t => t.CompletedJobs) // En çok iş yapandan aza doğru sırala
            .Take(5) // Sadece ilk 5'i al
            .ToList();

        return topTechnicians;
    }
}