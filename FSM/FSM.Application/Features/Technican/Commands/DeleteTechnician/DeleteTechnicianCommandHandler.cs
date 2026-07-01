using MediatR;
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
        var technician = await _repository.GetByIdAsync(request.Id);

        if (technician == null || technician.IsDeleted)
            throw new Exception($"ID'si {request.Id} olan aktif teknisyen bulunamadı!");

        // Soft Delete işlemi
        technician.IsDeleted = true;

        await _repository.UpdateAsync(technician);
        await _repository.SaveChangesAsync();

        return Unit.Value;
    }
}