// 1. Angular'a göndereceğimiz veri paketi (DTO)
using MediatR;

public class DashboardStatsDto
{
    public int TotalWorkOrders { get; set; }
    public int ActiveTechnicians { get; set; }
    public int PendingAssignments { get; set; }
    public int CompletedJobs { get; set; }
}

// 2. MediatR isteğimiz (Query)
public record GetDashboardStatsQuery : IRequest<DashboardStatsDto>;