using FSM.Application.DTOs.WorkOrders;
using FSM.Application.DTOs;

namespace FSM.Application.Interfaces;

// Aşçının yapabileceği yemeklerin listesi (Sözleşme)
public interface IWorkOrderService
{
    // Dışarıya "Sunum Tabağı" (WorkOrderDto) listesi döner
    Task<IEnumerable<WorkOrderDto>> GetAllWorkOrdersAsync();

    // EKSİK OLAN SATIR: Sadece tek bir "Sunum Tabağı" döner (Bulamazsa null '?' döner)
    Task<WorkOrderDto?> GetWorkOrderByIdAsync(int id);

    // Dışarıdan "Sipariş Fişi" (CreateWorkOrderDto) alır, veritabanına ekleyip yeni ID'sini döner
    Task<int> CreateWorkOrderAsync(CreateWorkOrderDto dto);

    Task AssignWorkOrderAsync(AssignWorkOrderDto dto);

    Task UpdateWorkOrderAsync(UpdateWorkOrderDto dto);
}


  