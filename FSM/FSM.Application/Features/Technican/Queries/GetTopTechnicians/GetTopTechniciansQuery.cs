using MediatR;
using System.Collections.Generic;

namespace FSM.Application.Features.Technicians.Queries.GetTopTechnicians;

public class GetTopTechniciansQuery : IRequest<List<TopTechnicianDto>>
{
}