using FSM.Domain.Common;
using FSM.Domain.Enums;
using System.Collections.Generic;
using System; // DateTime için gerekli

namespace FSM.Domain.Entities;

public class WorkOrder : BaseEntity, ISoftDeletable
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public WorkOrderState State { get; set; } = WorkOrderState.Pending;

    public int? TechnicianId { get; set; }

    // --- TAKVİM (SCHEDULER) İÇİN YENİ EKLENEN KISIM ---
    public DateTime? ScheduledStartDate { get; set; } // Planlanan Başlangıç Saati
    public DateTime? ScheduledEndDate { get; set; }   // Planlanan Bitiş Saati
    // --------------------------------------------------

    public int CustomerId { get; set; }
    public Customer Customer { get; set; } = null!;
    public bool IsDeleted { get; set; } = false;
    public ICollection<WorkOrderInventory> UsedParts { get; set; } = new List<WorkOrderInventory>();
}