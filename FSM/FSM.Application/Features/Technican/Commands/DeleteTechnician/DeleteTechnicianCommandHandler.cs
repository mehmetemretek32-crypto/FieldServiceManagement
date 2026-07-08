using MediatR;
using FSM.Application.Common;
using FSM.Domain.Entities;
using FSM.Domain.Interfaces;

namespace FSM.Application.Features.Technicians.Commands.DeleteTechnician;

public class DeleteTechnicianCommandHandler : IRequestHandler<DeleteTechnicianCommand, Unit>
{
    private readonly IGenericRepository<Technician> _repository;

    public DeleteTechnicianCommandHandler(IGenericRepository<Technician> repository)
    {
        _repository = repository;
    }

    public async Task<Unit> Handle(DeleteTechnicianCommand request, CancellationToken cancellationToken)
    {
        await _repository.SoftDeleteAsync(request.Id, "teknisyen");
        return Unit.Value;
    }
}
