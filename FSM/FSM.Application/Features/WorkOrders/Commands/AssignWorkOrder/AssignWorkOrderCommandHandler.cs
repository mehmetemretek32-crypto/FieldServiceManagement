using FSM.Domain.Entities;
using FSM.Domain.Enums;
using FSM.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Caching.Distributed; // 🔥 REDIS EKLENDİ

namespace FSM.Application.Features.WorkOrders.Commands.AssignWorkOrder;

public class AssignWorkOrderCommandHandler : IRequestHandler<AssignWorkOrderCommand, bool>
{
    private readonly IGenericRepository<WorkOrder> _workOrderRepository;
    private readonly IGenericRepository<Technician> _technicianRepository;
    private readonly IDistributedCache _cache; // 🔥 EKLENDİ

    public AssignWorkOrderCommandHandler(
        IGenericRepository<WorkOrder> workOrderRepository,
        IGenericRepository<Technician> technicianRepository,
        IDistributedCache cache) // 🔥 EKLENDİ
    {
        _workOrderRepository = workOrderRepository;
        _technicianRepository = technicianRepository;
        _cache = cache; // 🔥 EŞLEŞTİRİLDİ
    }

    public async Task<bool> Handle(AssignWorkOrderCommand request, CancellationToken cancellationToken)
    {
        var workOrder = await _workOrderRepository.GetByIdAsync(request.WorkOrderId);
        if (workOrder == null)
            throw new Exception("İş emri bulunamadı!");

        var technician = await _technicianRepository.GetByIdAsync(request.TechnicianId);
        if (technician == null)
            throw new Exception("Teknisyen bulunamadı!");

        workOrder.TechnicianId = request.TechnicianId;
        workOrder.ScheduledStartDate = request.ScheduledStartDate;
        workOrder.ScheduledEndDate = request.ScheduledEndDate;
        workOrder.State = WorkOrderState.Assigned;

        await _workOrderRepository.SaveChangesAsync(); // Fırın çalıştı 🔥

        // 👇 🔥 ZİNCİRLEME ÇEKMECE TEMİZLİĞİ (Cache Invalidation)
        await _cache.RemoveAsync("all_work_orders_list", cancellationToken); // İş emirleri listesini temizle
        await _cache.RemoveAsync("all_technicians_list", cancellationToken); // Teknisyen listesini temizle (Aktif iş sayısı arttı)
        await _cache.RemoveAsync($"active_orders_for_tech_{request.TechnicianId}", cancellationToken); // Teknisyene özel paneli temizle

        return true;
    }
}