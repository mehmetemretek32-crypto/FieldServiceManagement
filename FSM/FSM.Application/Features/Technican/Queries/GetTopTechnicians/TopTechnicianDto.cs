namespace FSM.Application.Features.Technicians.Queries.GetTopTechnicians;

public class TopTechnicianDto
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public int CompletedJobs { get; set; } // Tamamladığı iş sayısı
}