using FSM.Application.DTOs.Technicians;
using FSM.Application.Interfaces;
using FSM.Domain.Entities;
using FSM.Domain.Enums;
using FSM.Domain.Interfaces;

namespace FSM.Application.Services;

public class TechnicianService : ITechnicianService
{
    private readonly IGenericRepository<Technician> _repository;
    private readonly IGenericRepository<WorkOrder> _workOrderRepository;

    public TechnicianService(IGenericRepository<Technician> repository,
        IGenericRepository<WorkOrder> workOrderRepository)

    {
        _workOrderRepository = workOrderRepository;

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

        var activeTechnicians = technicians.Where(t => !t.IsDeleted);

        return activeTechnicians.Select(t => new TechnicianDto(
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
        if (technician.IsDeleted)
            return null;

        return new TechnicianDto(
            technician.Id,
            technician.FullName,
            technician.Email,
            technician.PhoneNumber,
            technician.IsAvailable
        );
    }
    public async Task<bool> UpdateTechnicianAsync(UpdateTechnicianDto dto)
    {
        // 1. Teknisyeni veritabanından bul
        var technician = await _repository.GetByIdAsync(dto.Id);

        // Eğer böyle bir teknisyen yoksa false dön (Controller'da 404 NotFound vereceğiz)
        if (technician == null)
            return false;

        // 2. Yeni bilgileri var olan teknisyenin üzerine yaz
        technician.PhoneNumber = dto.PhoneNumber;
        technician.IsAvailable = dto.IsAvailable;

        // 3. Repository üzerinden güncelleme işlemini başlat
        _repository.Update(technician);

        await _repository.SaveChangesAsync();

        return true; // Başarıyla güncellendi
    }
    public async Task<bool> DeleteTechnicianAsync(int id)
    {
        var technician = await _repository.GetByIdAsync(id);

        // Teknisyen yoksa veya zaten silinmişse (IsDeleted == true) false dön
        if (technician == null || technician.IsDeleted)
            return false;

        // --- YENİ EKLENEN İŞ KURALI (BUSINESS RULE) BAŞLANGICI ---

        // 1. Sistemdeki iş emirlerini getir
        var workOrders = await _workOrderRepository.GetAllAsync();

        // 2. Bu teknisyene atanmış ve henüz "Tamamlanmamış" (Pending, InProgress vb.) iş emri var mı diye bak
       
        bool hasActiveWorkOrders = workOrders.Any(w =>
            w.TechnicianId == id &&
            w.State != WorkOrderState.Completed); // Eğer enum'ında Completed yoksa Done vb. yazabilirsin

        // 3. Eğer aktif işi varsa silinmesini ENGELLİYORUZ!
        if (hasActiveWorkOrders)
        {
            // return false; YERİNE AŞAĞIDAKİ GİBİ HATA FIRLATIYORUZ:
            throw new InvalidOperationException("Bu teknisyenin üzerinde aktif iş emirleri bulunduğu için silinemez!");
        }

        // Gerçekten SİLMİYORUZ, sadece pasife çekiyoruz (Soft Delete)
        technician.IsDeleted = true;
        technician.IsAvailable = false; // Sistemden silinen adam iş de alamaz

        _repository.Update(technician);
        await _repository.SaveChangesAsync();

        return true;
    }
}