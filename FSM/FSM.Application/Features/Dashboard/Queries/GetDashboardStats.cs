using FSM.Application.DTOs.Dashboard;
using MediatR;

namespace FSM.Application.Features.Dashboard.Queries.GetDashboardStats
{
    public class GetDashboardStatsQuery : IRequest<DashboardStatsDto>
    {
    }
}