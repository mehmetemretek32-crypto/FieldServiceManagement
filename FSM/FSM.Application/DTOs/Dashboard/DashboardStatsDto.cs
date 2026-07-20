namespace FSM.Application.DTOs.Dashboard
{
    public class DashboardStatsDto
    {
        public int TotalWorkOrders { get; set; }
        public int ActiveTechnicians { get; set; }
        public int PendingAssignments { get; set; }
        public int CompletedJobs { get; set; }
    }
}