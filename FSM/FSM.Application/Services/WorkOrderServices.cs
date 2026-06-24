using FSM.Application.DTOs.WorkOrders;
using FSM.Application.Interfaces;
using FSM.Domain;
using FSM.Domain.Entities;
using FSM.Domain.Interfaces;

namespace FSM.Application.Services;

public class WorkOrderService : IWorkOrderService
{
    private readonly IGenericRepository<WorkOrder> _repository;

    public WorkOrderService(IGenericRepository<WorkOrder> repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<WorkOrderDto>> GetAllWorkOrdersAsync()
    {
        var workOrders = await _repository.GetAllAsync();

        // Veritabanı varlığını (Entity), sunum tabağına (DTO) dönüştürüyoruz
        return workOrders.Select(w => new WorkOrderDto(
            w.Id,
            w.Title,
            w.Description,
            w.State.ToString(),
            w.CreatedAt
        ));
    }

    public async Task<WorkOrderDto?> GetWorkOrderByIdAsync(int id)
    {
        var w = await _repository.GetByIdAsync(id);
        if (w == null) return null;

        return new WorkOrderDto(
            w.Id,
            w.Title,
            w.Description,
            w.State.ToString(),
            w.CreatedAt
        );
    }

    public async Task<int> CreateWorkOrderAsync(CreateWorkOrderDto dto)
    {
        var entity = new WorkOrder
        {
            Title = dto.Title,
            Description = dto.Description,
            CreatedAt = DateTime.UtcNow // <-- Müşterinin bilmediği o otomatik saati biz bastık
        };

        await _repository.AddAsync(entity);
        await _repository.SaveChangesAsync(); // <-- Fırının tetiği çekildi, diske yazıldı

        return entity.Id;
    }
}