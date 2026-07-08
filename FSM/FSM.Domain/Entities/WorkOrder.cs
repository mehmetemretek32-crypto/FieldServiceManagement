using FSM.Domain.Common;
using FSM.Domain.Enums;

namespace FSM.Domain.Entities;

public class WorkOrder : BaseEntity, ISoftDeletable
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public WorkOrderState State { get; set; } = WorkOrderState.Pending;
    // Diğer özelliklerin (Id, Title vs.) altına şunu ekle:
    public int? TechnicianId { get; set; }//? boş olabilir amlamında

    public int CustomerId { get; set; }
    public Customer Customer { get; set; } = null!;
    public bool IsDeleted { get; set; } = false;
}