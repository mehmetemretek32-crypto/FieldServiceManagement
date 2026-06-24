using FSM.Application.DTOs.Technicians;
using FSM.Application.Interfaces;
using FSM.Domain.Entities;
using FSM.Domain.Interfaces;

namespace FSM.Application.Services;

public class TechnicianService : ITechnicianService
{
    private readonly IGenericRepository<Technician> _repository;

    public TechnicianService(IGenericRepository<Technician> repository)
    {
        _repository = repository;
    }

    // 1. YENİ TEKNİSYEN İŞE ALMA (CREATE)
    public async Task<int> CreateTechnicianAsync(CreateTechnicianDto dto)
    {
        var newTechnician = new Technician
        {
            FullName = dto.FullName,
            Email = dto.Email,
            PhoneNumber = dto.PhoneNumber,
            IsAvailable = true // Yeni kayıt olduğu için otomatik müsait
        };

        await _repository.AddAsync(newTechnician);
        return newTechnician.Id;
    }

    // 2. TÜM TEKNİSYENLERİ GETİRME (GET ALL)
    public async Task<IEnumerable<TechnicianDto>> GetAllTechniciansAsync()
    {
        var technicians = await _repository.GetAllAsync();

        return technicians.Select(t => new TechnicianDto(
            t.Id,
            t.FullName,
            t.Email,
            t.PhoneNumber,
            t.IsAvailable
        ));
    }

    // 3. ID'YE GÖRE TEK BİR TEKNİSYEN GETİRME (GET BY ID)
    public async Task<TechnicianDto?> GetTechnicianByIdAsync(int id)
    {
        var technician = await _repository.GetByIdAsync(id);

        if (technician == null)
            return null;

        return new TechnicianDto(
            technician.Id,
            technician.FullName,
            technician.Email,
            technician.PhoneNumber,
            technician.IsAvailable
        );
    }
}