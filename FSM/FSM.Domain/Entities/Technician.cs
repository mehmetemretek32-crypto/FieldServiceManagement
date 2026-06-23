using FSM.Domain.Common;

namespace FSM.Domain.Entities;

public class Technician : BaseEntity
{
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public bool IsAvailable { get; set; } = true; // Teknisyen o an boşta mı?
}