using MediatR;
using FSM.Application.Common;
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
        await _repository.SoftDeleteAsync(request.Id, "iş emri");
        return Unit.Value;
    }
}
