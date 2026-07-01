using FSM.Application.DTOs.Technicians;
using MediatR;

namespace FSM.Application.Features.Technican.Queries.GetAllTechnician;

public class GetAllTechniciansQuery : IRequest<List<TechnicianDto>>
{
}