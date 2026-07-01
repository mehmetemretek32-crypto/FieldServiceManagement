
using FSM.Application.DTOs.Technicians;
using MediatR;

namespace FSM.Application.Features.Technicians.Queries.GetTechnicianById;

public class GetTechnicianByIdQuery : IRequest<TechnicianDto>
{
    public int Id { get; set; }
}