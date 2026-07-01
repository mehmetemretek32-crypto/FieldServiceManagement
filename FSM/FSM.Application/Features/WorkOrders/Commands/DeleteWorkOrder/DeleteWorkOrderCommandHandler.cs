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

        if (workOrder == null)
            throw new Exception($"ID'si {request.Id} olan iş emri bulunamadı!");

        // Fiziksel silme yerine pasife/iptale çekiyoruz
        workOrder.IsDeleted = true;

        await _repository.UpdateAsync(workOrder);
        await _repository.SaveChangesAsync(); // Değişikliği kaydetmeyi unutmuyoruz!

        return Unit.Value;
    }
}