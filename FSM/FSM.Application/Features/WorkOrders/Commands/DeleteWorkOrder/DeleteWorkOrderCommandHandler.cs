using MediatR;
using FSM.Domain.Entities;
using FSM.Domain.Interfaces;

namespace FSM.Application.Features.WorkOrders.Commands.DeleteWorkOrder;

public class DeleteWorkOrderCommandHandler : IRequestHandler<DeleteWorkOrderCommand, Unit>
{
    private readonly IGenericRepository<WorkOrder> _repository;

    public DeleteWorkOrderCommandHandler(IGenericRepository<WorkOrder> repository)
    {
        _repository = repository;
    }

    public async Task<Unit> Handle(DeleteWorkOrderCommand request, CancellationToken cancellationToken)
    {
        var workOrder = await _repository.GetByIdAsync(request.Id);

        // 1. İş emri hiç yoksa
        if (workOrder == null)
        {
            throw new Exception($"Hata: ID'si {request.Id} olan iş emri bulunamadı.");
        }

        // 2. İş emri zaten silinmişse (Sonsuz silme engeli)
        if (workOrder.IsDeleted)
        {
            throw new Exception($"Hata: ID'si {request.Id} olan iş emri zaten daha önceden silinmiş!");
        }

        // 3. Pasife Çek (Soft Delete)
        workOrder.IsDeleted = true;

        await _repository.UpdateAsync(workOrder);
        await _repository.SaveChangesAsync();

        return Unit.Value;
    }
}