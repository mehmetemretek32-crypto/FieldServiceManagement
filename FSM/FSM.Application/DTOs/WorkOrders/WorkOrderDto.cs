public class WorkOrderDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int State { get; set; } // int olarak tutuyoruz
    public DateTime CreatedAt { get; set; }
    public int CustomerId { get; set; }
    public int? TechnicianId { get; set; }
    public string TechnicianName { get; set; } = string.Empty;
    public DateTime? ScheduledStartDate { get; set; }
    public DateTime? ScheduledEndDate { get; set; }
}