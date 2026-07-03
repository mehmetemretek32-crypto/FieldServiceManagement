using FSM.Application.Interfaces;
using FSM.Domain.Entities;
using FSM.Domain.Enums;
using FSM.Domain.Interfaces;
using MediatR;

namespace FSM.Application.Features.WorkOrders.Commands.UpdateWorkOrderStatus;

public class UpdateWorkOrderStatusCommandHandler : IRequestHandler<UpdateWorkOrderStatusCommand, bool>
{
    private readonly IGenericRepository<WorkOrder> _workOrderRepository;
    private readonly INotificationService _notificationService;

    public UpdateWorkOrderStatusCommandHandler(IGenericRepository<WorkOrder> workOrderRepository, INotificationService notificationService)
    {
        _workOrderRepository = workOrderRepository;
        _notificationService = notificationService;
    }

    public async Task<bool> Handle(UpdateWorkOrderStatusCommand request, CancellationToken cancellationToken)
    {
        var workOrder = await _workOrderRepository.GetByIdAsync(request.WorkOrderId);
        if (workOrder == null || workOrder.IsDeleted)
            throw new Exception("İş emri bulunamadı!");

        // Durumu güvenli bir şekilde güncelleyelim:
        // (true parametresi büyük/küçük harf duyarlılığını ortadan kaldırır)
        if (Enum.TryParse<WorkOrderState>(request.NewStatus, true, out var parsedState))
        {
            workOrder.State = parsedState; // <--- İşte eksik olan eşitleme (Atama) burası!
        }
        else
        {
            throw new Exception($"Geçersiz iş emri durumu: '{request.NewStatus}'. Lütfen geçerli bir durum adı giriniz.");
        }

        await _workOrderRepository.UpdateAsync(workOrder);
        await _notificationService.SendWorkOrderNotification($"İş emri #{workOrder.Id} durumu değişti: {request.NewStatus}");

        return true;
    }
}