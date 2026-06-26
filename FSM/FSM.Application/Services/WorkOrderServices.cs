using FSM.Application.DTOs;
using FSM.Application.DTOs.WorkOrders;
using FSM.Application.Interfaces;
using FSM.Domain.Entities;
using FSM.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FSM.Domain.Enums;

namespace FSM.Application.Services
{
    public class WorkOrdersService : IWorkOrderService
    {
        // İki tablonun da anahtarını (Repository) tanımlıyoruz
        private readonly IGenericRepository<WorkOrder> _workOrderRepository;
        private readonly IGenericRepository<Technician> _technicianRepository;

        // Constructor (Yapıcı Metot) - Anahtarları içeri alıyoruz
        public WorkOrdersService(
            IGenericRepository<WorkOrder> workOrderRepository,
            IGenericRepository<Technician> technicianRepository)
        {
            _workOrderRepository = workOrderRepository;
            _technicianRepository = technicianRepository;
        }

        public async Task<IEnumerable<WorkOrderDto>> GetAllWorkOrdersAsync()
        {
            var workOrders = await _workOrderRepository.GetAllAsync();

            return workOrders.Select(w => new WorkOrderDto(
                w.Id,
                w.Title,
                w.Description,
                w.State.ToString(), // Entity'ndeki isme göre State veya Status olabilir
                w.CreatedAt
            ));
        }

        public async Task<WorkOrderDto> GetWorkOrderByIdAsync(int id)
        {
            var w = await _workOrderRepository.GetByIdAsync(id);
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
                CreatedAt = DateTime.UtcNow
            };

            await _workOrderRepository.AddAsync(entity);
            // Kendi Repository tasarımına göre aşağıdakini Update/Save olarak düzenleyebilirsin
            await _workOrderRepository.SaveChangesAsync();

            return entity.Id;
        }

        // --- İŞTE YENİ EKLENEN BÜYÜK ATAMA OPERASYONU ---
        public async Task AssignWorkOrderAsync(AssignWorkOrderDto dto)
        {
            // 1. İş Emrini ve Teknisyeni veritabanından bulup getiriyoruz
            var workOrder = await _workOrderRepository.GetByIdAsync(dto.WorkOrderId);
            var technician = await _technicianRepository.GetByIdAsync(dto.TechnicianId);

            // 2. İş Emri veya Teknisyen sistemde yoksa hata fırlat (Exception Handling)
            if (workOrder == null)
                throw new Exception($"ID'si {dto.WorkOrderId} olan iş emri bulunamadı.");

            if (technician == null)
                       throw new Exception($"ID'si {dto.TechnicianId} olan teknisyen bulunamadı.");
            
            if (technician.IsDeleted)
                throw new Exception("Sistemden silinmiş (pasife çekilmiş) bir teknisyene yeni iş atanamaz!");
            
            // 3. Teknisyen şu an müsait değilse (başka işteyse) atamayı reddet
            if (!technician.IsAvailable)
                throw new Exception($"{technician.FullName} adlı teknisyen şu an müsait değil. Başka bir iş üzerinde çalışıyor.");

            // 4. ATAMA İŞLEMİ (Verileri Güncelliyoruz)
            workOrder.TechnicianId = technician.Id; // İşi ustanın üstüne yaptık

            // Not: Senin Entity'nde bu alanın adı "State" ise workOrder.State = "Assigned" yapmalısın.
            workOrder.State = WorkOrderState.Assigned;  // İşin durumunu 'Atandı' yaptık
            technician.IsAvailable = false;         // Usta artık müsait değil!

            // 5. Değişiklikleri Veritabanına Kaydet
            await _workOrderRepository.UpdateAsync(workOrder);
            await _technicianRepository.UpdateAsync(technician);
        }
    }
}