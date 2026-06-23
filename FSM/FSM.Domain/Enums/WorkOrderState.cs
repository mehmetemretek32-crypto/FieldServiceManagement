namespace FSM.Domain.Enums;

public enum WorkOrderState
{
    Pending = 1,     // Bekliyor
    Assigned = 2,    // Teknisyene Atandı
    InProgress = 3,  // Sahada İşlemde
    Completed = 4,   // Tamamlandı
    Cancelled = 5    // İptal Edildi
}