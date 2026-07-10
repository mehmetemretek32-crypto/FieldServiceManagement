using FSM.Domain.Common;
using System.ComponentModel.DataAnnotations.Schema;

namespace FSM.Domain.Entities;

public class InventoryItem : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string SkuCode { get; set; } = string.Empty; // Stok Barkodu (Örn: KBL-001)
    public int StockQuantity { get; set; } = 0; // Depodaki Adet

    [Column(TypeName = "decimal(18,2)")] // Toplam 18 basamak, virgülden sonra 2 basamak (Kuruş)
    public decimal UnitPrice { get; set; }

    public bool IsDeleted { get; set; } = false;

}