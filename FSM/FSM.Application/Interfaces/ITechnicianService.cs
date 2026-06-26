using FSM.Application.DTOs.Technicians;

namespace FSM.Application.Interfaces;

public interface ITechnicianService
{
    Task<IEnumerable<TechnicianDto>> GetAllTechniciansAsync();
    Task<TechnicianDto?> GetTechnicianByIdAsync(int id);
    Task<int> CreateTechnicianAsync(CreateTechnicianDto dto);
    Task<bool> UpdateTechnicianAsync(UpdateTechnicianDto dto);
    Task<bool> DeleteTechnicianAsync(int id);
}