using FSM.Application.Common;
using FSM.Application.Interfaces;
using FSM.Domain.Entities;
using FSM.Domain.Enums;
using FSM.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Caching.Distributed; // 🔥 1. REDIS EKLENDİ

namespace FSM.Application.Features.WorkOrders.Commands.UpdateWorkOrderStatus;

public class UpdateWorkOrderStatusCommandHandler : IRequestHandler<UpdateWorkOrderStatusCommand, bool>
{
    private readonly IGenericRepository<WorkOrder> _workOrderRepository;
    private readonly INotificationService _notificationService;
    private readonly IDistributedCache _cache; // 🔥 2. EKLENDİ

    public UpdateWorkOrderStatusCommandHandler(
        IGenericRepository<WorkOrder> workOrderRepository,
        INotificationService notificationService,
        IDistributedCache cache) // 🔥 3. EKLENDİ
    {
        _workOrderRepository = workOrderRepository;
        _notificationService = notificationService;
        _cache = cache; // 🔥 EŞLEŞTİRİLDİ
    }

    public async Task<bool> Handle(UpdateWorkOrderStatusCommand request, CancellationToken cancellationToken)
    {
        var workOrder = await _workOrderRepository.GetActiveByIdOrThrowAsync(request.WorkOrderId, "iş emri");

        // Durumu güvenli bir şekilde güncelleyelim:
        if (Enum.TryParse<WorkOrderState>(request.NewStatus, true, out var parsedState))
        {
            workOrder.State = parsedState;
        }
        else
        {
            throw new Exception($"Geçersiz iş emri durumu: '{request.NewStatus}'. Lütfen geçerli bir durum adı giriniz.");
        }

        await _workOrderRepository.UpdateAsync(workOrder);
        // Not: Eğer UpdateAsync kendi içinde kaydetmiyorsa buraya await _workOrderRepository.SaveChangesAsync(); eklenebilir.

        // 👇 🔥 ZİNCİRLEME ÇEKMECE TEMİZLİĞİ (Durum değiştiği için istatistikler etkilendi)
        await _cache.RemoveAsync("all_work_orders_list", cancellationToken);
        await _cache.RemoveAsync("all_technicians_list", cancellationToken);
        await _cache.RemoveAsync("top_technicians_list", cancellationToken);

        if (workOrder.TechnicianId.HasValue)
        {
            await _cache.RemoveAsync($"active_orders_for_tech_{workOrder.TechnicianId.Value}", cancellationToken);
        }

        await _notificationService.SendWorkOrderNotification($"İş emri #{workOrder.Id} durumu değişti: {request.NewStatus}");

        return true;
    }
}