namespace FSM.Application.DTOs;

public class CustomerDto
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? CompanyName { get; set; }
    public string PhoneNumber { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string Address { get; set; } = string.Empty;

    // 🔥 PRO DOKUNUŞ: Müşterinin bugüne kadar açtırdığı toplam iş emri sayısı!
    public int TotalWorkOrderCount { get; set; }
}