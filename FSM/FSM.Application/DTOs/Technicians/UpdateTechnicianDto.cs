namespace FSM.Application.DTOs.Technicians;

// C# 9.0 Record tipi ile değiştirilemez (immutable) DTO
public record UpdateTechnicianDto(
    int Id,
    string FullName,
    string PhoneNumber,
    bool IsAvailable
);