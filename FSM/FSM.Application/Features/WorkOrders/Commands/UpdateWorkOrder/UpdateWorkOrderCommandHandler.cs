using FSM.Application.Common;
using FSM.Domain.Entities;
using FSM.Domain.Enums;
using FSM.Domain.Interfaces;
using MediatR;

namespace FSM.Application.Features.WorkOrders.Commands.UpdateWorkOrder;

public class UpdateWorkOrderCommandHandler : IRequestHandler<UpdateWorkOrderCommand, Unit>
{
    private readonly IGenericRepository<WorkOrder> _workOrderRepository;

    // 1. Teknisyen tablosuna erişmek için yeni Repository tanımı
    private readonly IGenericRepository<Technician> _technicianRepository;

    // 2. Constructor (Yapıcı Metot) içine Teknisyen Repository'sinin enjekte edilmesi
    public UpdateWorkOrderCommandHandler(
        IGenericRepository<WorkOrder> workOrderRepository,
        IGenericRepository<Technician> technicianRepository)
    {
        _workOrderRepository = workOrderRepository;
        _technicianRepository = technicianRepository; // Atamanın yapılması
    }

    public async Task<Unit> Handle(UpdateWorkOrderCommand request, CancellationToken cancellationToken)
    {
        var workOrder = await _workOrderRepository.GetActiveByIdOrThrowAsync(request.Id, "iş emri");

        // 3. İş Kuralı (Business Rule): Teknisyen uygunluk kontrolü
        if (request.TechnicianId.HasValue && request.TechnicianId.Value > 0)
        {
            var technician = await _technicianRepository.GetByIdAsync(request.TechnicianId.Value);

            if (technician != null && !technician.IsAvailable)
            {
                throw new Exception($"Seçilen teknisyen ({technician.FullName}) şu anda başka bir işte meşgul. Lütfen müsait bir teknisyen seçin.");
            }
        }

        // 4. Verilerin Güncellenmesi
        workOrder.Title = request.Title;
        workOrder.Description = request.Description;
        workOrder.State = (WorkOrderState)request.State;
        workOrder.TechnicianId = request.TechnicianId;
        workOrder.CustomerId = request.CustomerId;

        // 5. Kaydetme
        await _workOrderRepository.SaveChangesAsync();

        return Unit.Value;
    }
}