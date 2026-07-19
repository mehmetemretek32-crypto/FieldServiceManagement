using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using FSM.Domain.Entities;
using FSM.Domain.Interfaces;

namespace FSM.Application.Features.WorkOrders.Commands.AddMaterialToWorkOrder;

public class AddMaterialToWorkOrderCommandHandler : IRequestHandler<AddMaterialToWorkOrderCommand, int>
{
    private readonly IGenericRepository<WorkOrder> _workOrderRepository;
    private readonly IGenericRepository<InventoryItem> _inventoryRepository;
    private readonly IGenericRepository<WorkOrderInventory> _workOrderInventoryRepository;
    private readonly IDistributedCache _cache;

    public AddMaterialToWorkOrderCommandHandler(
        IGenericRepository<WorkOrder> workOrderRepository,
        IGenericRepository<InventoryItem> inventoryRepository,
        IGenericRepository<WorkOrderInventory> workOrderInventoryRepository,
        IDistributedCache cache)
    {
        _workOrderRepository = workOrderRepository;
        _inventoryRepository = inventoryRepository;
        _workOrderInventoryRepository = workOrderInventoryRepository;
        _cache = cache;
    }

    public async Task<int> Handle(AddMaterialToWorkOrderCommand request, CancellationToken cancellationToken)
    {
        // 1. İş Emri ve Malzemeyi veritabanından bul
        var workOrder = await _workOrderRepository.GetByIdAsync(request.WorkOrderId);
        if (workOrder == null)
            throw new Exception("İş emri bulunamadı!");

        var inventoryItem = await _inventoryRepository.GetByIdAsync(request.InventoryItemId);
        if (inventoryItem == null)
            throw new Exception("Envanter kalemi bulunamadı!");

        // 2. Stok Kontrolü (İstenen miktar stoktan fazla olamaz)
        if (inventoryItem.StockQuantity < request.QuantityUsed)
            throw new Exception($"Yetersiz stok! Depoda sadece {inventoryItem.StockQuantity} adet {inventoryItem.Name} bulunuyor.");

        // 3. Stok Miktarını Düş
        inventoryItem.StockQuantity -= request.QuantityUsed;
       
        await _inventoryRepository.UpdateAsync(inventoryItem);

        // 4. Ara Tabloya Kayıt At (İş Emrine Malzemeyi Bağla)
        var workOrderInventory = new WorkOrderInventory
        {
            WorkOrderId = request.WorkOrderId,
            InventoryItemId = request.InventoryItemId,
            QuantityUsed = request.QuantityUsed
        };
        await _workOrderInventoryRepository.AddAsync(workOrderInventory);

        // 5. Veritabanına Değişiklikleri Kaydet
        await _workOrderRepository.SaveChangesAsync();

        // 6. Redis Cache Temizliği (Hem iş emirleri hem envanter güncellendiği için ikisinin de vitrinini yeniliyoruz)
        await _cache.RemoveAsync("WorkOrdersList", cancellationToken);
        await _cache.RemoveAsync("all_inventory_items_list", cancellationToken);

        return workOrderInventory.Id;
    }
}