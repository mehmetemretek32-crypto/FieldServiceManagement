using MediatR;
using FSM.Domain.Entities;
using FSM.Domain.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace FSM.Application.Features.WorkOrders.Commands.ScheduleWorkOrder;

public class ScheduleWorkOrderCommandHandler : IRequestHandler<ScheduleWorkOrderCommand, bool>
{
    private readonly IGenericRepository<WorkOrder> _workOrderRepository;
    private readonly IDistributedCache _cache;

    public ScheduleWorkOrderCommandHandler(
        IGenericRepository<WorkOrder> workOrderRepository,
        IDistributedCache cache)
    {
        _workOrderRepository = workOrderRepository;
        _cache = cache;
    }

    public async Task<bool> Handle(ScheduleWorkOrderCommand request, CancellationToken cancellationToken)
    {
        // 1. İş Emrini Bul
        var workOrder = await _workOrderRepository.GetByIdAsync(request.WorkOrderId);
        if (workOrder == null)
            throw new Exception("İş emri bulunamadı!");

        // 2. Yeni Tarihleri ve Teknisyeni Güncelle
        workOrder.ScheduledStartDate = request.ScheduledStartDate;
        workOrder.ScheduledEndDate = request.ScheduledEndDate;

        if (request.TechnicianId.HasValue)
        {
            workOrder.TechnicianId = request.TechnicianId;
        }

        await _workOrderRepository.UpdateAsync(workOrder);
        await _workOrderRepository.SaveChangesAsync();

        // 3. Vitrini (Cache) Temizle ki Liste Güncel Kalsın
        await _cache.RemoveAsync("all_work_orders_list", cancellationToken);

        return true;
    }
}