namespace FSM.Application.DTOs.Technicians;

// C# 9.0 Record tipi ile değiştirilemez (immutable) DTO
public record UpdateTechnicianDto(
    int Id,
    string PhoneNumber,
    bool IsAvailable
);