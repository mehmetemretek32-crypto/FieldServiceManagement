using FSM.Application.Common;
using FSM.Application.Interfaces;
using FSM.Domain.Entities;
using FSM.Domain.Enums;
using FSM.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Caching.Distributed; // 🔥 1. REDIS EKLENDİ

namespace FSM.Application.Features.WorkOrders.Commands.UpdateWorkOrder;

public class UpdateWorkOrderCommandHandler : IRequestHandler<UpdateWorkOrderCommand, Unit>
{
    private readonly IGenericRepository<WorkOrder> _workOrderRepository;
    private readonly IGenericRepository<Technician> _technicianRepository;
    private readonly INotificationService _notificationService;
    private readonly IDistributedCache _cache; // 🔥 2. EKLENDİ

    public UpdateWorkOrderCommandHandler(
        IGenericRepository<WorkOrder> workOrderRepository,
        IGenericRepository<Technician> technicianRepository,
        INotificationService notificationService,
        IDistributedCache cache) // 🔥 3. EKLENDİ
    {
        _workOrderRepository = workOrderRepository;
        _technicianRepository = technicianRepository;
        _notificationService = notificationService;
        _cache = cache; // 🔥 EŞLEŞTİRİLDİ
    }

    public async Task<Unit> Handle(UpdateWorkOrderCommand request, CancellationToken cancellationToken)
    {
        var workOrder = await _workOrderRepository.GetActiveByIdOrThrowAsync(request.Id, "iş emri");

        // İş Kuralı: Teknisyen uygunluk kontrolü
        if (request.TechnicianId.HasValue && request.TechnicianId.Value > 0)
        {
            var technician = await _technicianRepository.GetByIdAsync(request.TechnicianId.Value);

            if (technician != null && !technician.IsAvailable)
            {
                throw new Exception($"Seçilen teknisyen ({technician.FullName}) şu anda başka bir işte meşgul. Lütfen müsait bir teknisyen seçin.");
            }
        }

        // Verilerin Güncellenmesi
        workOrder.Title = request.Title;
        workOrder.Description = request.Description;
        workOrder.State = (WorkOrderState)request.State;
        workOrder.TechnicianId = request.TechnicianId;
        workOrder.CustomerId = request.CustomerId;

        // Kaydetme
        await _workOrderRepository.SaveChangesAsync();

        // 👇 🔥 ZİNCİRLEME ÇEKMECE TEMİZLİĞİ (Cache Invalidation)
        await _cache.RemoveAsync("dashboard_stats_data", cancellationToken);
        await _cache.RemoveAsync("all_work_orders_list", cancellationToken);
        await _cache.RemoveAsync("all_customers_list", cancellationToken);
        await _cache.RemoveAsync("all_technicians_list", cancellationToken);
        await _cache.RemoveAsync("top_technicians_list", cancellationToken);

        if (request.TechnicianId.HasValue && request.TechnicianId.Value > 0)
        {
            await _cache.RemoveAsync($"active_orders_for_tech_{request.TechnicianId.Value}", cancellationToken);
        }

        // Sinyali Ateşliyoruz!
        await _notificationService.SendWorkOrderNotification(
            $"#{workOrder.Id} numaralı iş emri güncellendi!"
        );

        return Unit.Value;
    }
}