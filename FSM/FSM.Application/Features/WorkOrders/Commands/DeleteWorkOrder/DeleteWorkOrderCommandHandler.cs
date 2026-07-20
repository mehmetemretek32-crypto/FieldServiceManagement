using MediatR;
using FSM.Application.Common;
using FSM.Domain.Entities;
using FSM.Domain.Interfaces;
using FSM.Application.Interfaces;
using Microsoft.Extensions.Caching.Distributed; // 🔥 REDIS EKLENDİ

namespace FSM.Application.Features.WorkOrders.Commands.DeleteWorkOrder;

public class DeleteWorkOrderCommandHandler : IRequestHandler<DeleteWorkOrderCommand, Unit>
{
    private readonly IGenericRepository<WorkOrder> _repository;
    private readonly INotificationService _notificationService;
    private readonly IDistributedCache _cache; // 🔥 EKLENDİ

    public DeleteWorkOrderCommandHandler(
        IGenericRepository<WorkOrder> repository,
        INotificationService notificationService,
        IDistributedCache cache) // 🔥 EKLENDİ
    {
        _repository = repository;
        _notificationService = notificationService;
        _cache = cache; // 🔥 EŞLEŞTİRİLDİ
    }

    public async Task<Unit> Handle(DeleteWorkOrderCommand request, CancellationToken cancellationToken)
    {
        // Silmeden önce iş emrini bulalım ki, hangi teknisyene veya müşteriye ait olduğunu bilelim
        var workOrder = await _repository.GetByIdAsync(request.Id);

        // Veritabanından (Soft Delete) silme/pasife çekme işlemi
        await _repository.SoftDeleteAsync(request.Id, "iş emri");

        // 👇 🔥 ZİNCİRLEME ÇEKMECE TEMİZLİĞİ
        await _cache.RemoveAsync("dashboard_stats_data", cancellationToken);
        await _cache.RemoveAsync("all_work_orders_list", cancellationToken);
        await _cache.RemoveAsync("all_customers_list", cancellationToken); // Müşterinin iş sayısı azaldı
        await _cache.RemoveAsync("all_technicians_list", cancellationToken); // Teknisyenin aktif iş sayısı azaldı
        await _cache.RemoveAsync("top_technicians_list", cancellationToken); // Tamamlanmış iş silindiyse sıralama değişebilir

        if (workOrder != null && workOrder.TechnicianId.HasValue)
        {
            await _cache.RemoveAsync($"active_orders_for_tech_{workOrder.TechnicianId.Value}", cancellationToken);
        }

        // Sinyali Ateşliyoruz!
        await _notificationService.SendWorkOrderNotification(
            $"🗑️ #{request.Id} numaralı iş emri sistemden silindi/iptal edildi!"
        );

        return Unit.Value;
    }
}