using FSM.Domain.Common;

namespace FSM.Domain.Entities;

public class WorkOrderInventory : BaseEntity
{
    public int WorkOrderId { get; set; }
    public WorkOrder WorkOrder { get; set; } = null!;

    public int InventoryItemId { get; set; }
    public InventoryItem InventoryItem { get; set; } = null!;

    // Bu iş emrinde bu malzemeden kaç adet kullanıldı/stoktan düşüldü?
    public int QuantityUsed { get; set; }
}