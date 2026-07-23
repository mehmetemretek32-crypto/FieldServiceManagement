namespace FSM.Application.DTOs.Technicians;

public record TechnicianDto(
    int Id,
    string FullName,
    string Email,
    string PhoneNumber,
    bool IsAvailable,
    int ActiveWorkOrdersCount
)
{
    // AutoMapper'ın nesneyi üretebilmesi için gereken boş yapıcı metot (Parametresiz Constructor).
    // Projenin geri kalanında yazılmış hiçbir kodu BOZMAZ.
    public TechnicianDto() : this(default, string.Empty, string.Empty, string.Empty, default, default) { }
}