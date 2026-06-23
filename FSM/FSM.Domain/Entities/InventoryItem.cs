using FSM.Domain.Common;

namespace FSM.Domain.Entities;

public class InventoryItem : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string SkuCode { get; set; } = string.Empty; // Stok Barkodu (Örn: KBL-001)
    public int StockQuantity { get; set; } = 0; // Depodaki Adet
    public decimal UnitPrice { get; set; } // Parça Fiyatı
}