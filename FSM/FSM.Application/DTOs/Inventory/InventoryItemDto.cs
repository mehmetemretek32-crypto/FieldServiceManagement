namespace FSM.Application.DTOs;

public class InventoryItemDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string SkuCode { get; set; } = string.Empty;
    public int StockQuantity { get; set; }
    public decimal UnitPrice { get; set; }

    // 🔥 PRO DOKUNUŞ: Bu malzemenin bugüne kadar kaç farklı iş emrinde (sahada) kullanıldığı bilgisi! 
    // (Bunu ileride raporlama ekranları için dolduracağız)
    public int TotalUsageCount { get; set; }
}