using MediatR;

namespace FSM.Application.Features.Technicians.Commands.UpdateTechnician;

public class UpdateTechnicianCommand : IRequest<Unit>
{
    public int Id { get; set; }
    public string FullName { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
}