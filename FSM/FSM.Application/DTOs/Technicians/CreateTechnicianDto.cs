namespace FSM.Application.DTOs.Technicians;

public record CreateTechnicianDto(
    string FullName,
    string Email,
    string PhoneNumber
);