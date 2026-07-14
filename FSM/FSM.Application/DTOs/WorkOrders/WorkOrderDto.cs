namespace FSM.Application.DTOs.WorkOrders;

public record WorkOrderDto(
    int Id,
    string Title,
    string Description,
    string State,
    DateTime CreatedAt,
    int CustomerId,
    DateTime? ScheduledStartDate, // 👈 İŞTE TAKVİMİ DOLDURACAK OLAN EKSİK KAN
    DateTime? ScheduledEndDate,   // 👈 BİTİŞ TARİHİ
    int? TechnicianId             // 👈 Hazır el atmışken bunu da ekleyelim, teknisyenleri filtrelerken lazım olacak
);