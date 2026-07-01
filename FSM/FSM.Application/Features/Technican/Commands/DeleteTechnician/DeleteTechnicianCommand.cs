using MediatR;

namespace FSM.Application.Features.Technicians.Commands.DeleteTechnician;

public class DeleteTechnicianCommand : IRequest<Unit>
{
    public int Id { get; set; }
}