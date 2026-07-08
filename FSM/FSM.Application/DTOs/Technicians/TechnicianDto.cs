namespace FSM.Application.DTOs.Technicians;

public record TechnicianDto(
    int Id,
    string FullName,
    string Email,
    string PhoneNumber,
    bool IsAvailable,
    int ActiveWorkOrdersCount
);