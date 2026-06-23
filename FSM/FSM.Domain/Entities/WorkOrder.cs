using FSM.Domain.Common;
using FSM.Domain.Enums;

namespace FSM.Domain.Entities;

public class WorkOrder : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public WorkOrderState State { get; set; } = WorkOrderState.Pending;
    public int? AssignedTechnicianId { get; set; }
}