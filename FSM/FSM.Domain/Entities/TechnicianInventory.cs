using FSM.Domain.Common;

namespace FSM.Domain.Entities;

public class TechnicianInventory : BaseEntity
{
    public int TechnicianId { get; set; }
    public Technician Technician { get; set; } = null!;

    public int InventoryItemId { get; set; }
    public InventoryItem InventoryItem { get; set; } = null!;

    // Teknisyenin aracında/elinde bu malzemeden kaç adet var?
    public int Quantity { get; set; }
}