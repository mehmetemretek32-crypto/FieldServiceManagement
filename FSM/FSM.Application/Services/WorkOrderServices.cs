using AutoMapper;
using FSM.Application.DTOs;
using FSM.Application.DTOs.WorkOrders;
using FSM.Application.Interfaces;
using FSM.Domain.Entities;
using FSM.Domain.Enums;
using FSM.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FSM.Application.Services
{
    public class WorkOrdersService : IWorkOrderService
    {
        private readonly IGenericRepository<WorkOrder> _workOrderRepository;
        private readonly IGenericRepository<Technician> _technicianRepository;
        private readonly IMapper _mapper;

        public WorkOrdersService(
            IGenericRepository<WorkOrder> workOrderRepository,
            IGenericRepository<Technician> technicianRepository,
            IMapper mapper)
        {
            _workOrderRepository = workOrderRepository;
            _technicianRepository = technicianRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<WorkOrderDto>> GetAllWorkOrdersAsync()
        {
            var workOrders = await _workOrderRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<WorkOrderDto>>(workOrders);
        }

        public async Task<WorkOrderDto> GetWorkOrderByIdAsync(int id)
        {
            var w = await _workOrderRepository.GetByIdAsync(id);
            if (w == null) return null;

            return _mapper.Map<WorkOrderDto>(w);
        }

        public async Task<int> CreateWorkOrderAsync(CreateWorkOrderDto dto)
        {
            // AutoMapper ile DTO'dan Entity'ye dönüşüm
            var entity = _mapper.Map<WorkOrder>(dto);

            // Otomatik atanan alanlar
            entity.CreatedAt = DateTime.UtcNow;
            entity.State = WorkOrderState.Pending;

            await _workOrderRepository.AddAsync(entity);
            await _workOrderRepository.SaveChangesAsync();

            return entity.Id;
        }

        public async Task AssignWorkOrderAsync(AssignWorkOrderDto dto)
        {
            var workOrder = await _workOrderRepository.GetByIdAsync(dto.WorkOrderId);
            var technician = await _technicianRepository.GetByIdAsync(dto.TechnicianId);

            if (workOrder == null)
                throw new Exception($"ID'si {dto.WorkOrderId} olan iş emri bulunamadı.");

            if (technician == null)
                throw new Exception($"ID'si {dto.TechnicianId} olan teknisyen bulunamadı.");

            if (technician.IsDeleted)
                throw new Exception("Sistemden silinmiş bir teknisyene yeni iş atanamaz!");

            if (!technician.IsAvailable)
                throw new Exception($"{technician.FullName} adlı teknisyen şu an müsait değil.");

            // Atama İşlemi
            workOrder.TechnicianId = technician.Id;
            workOrder.State = WorkOrderState.Assigned;
            technician.IsAvailable = false;

            await _workOrderRepository.UpdateAsync(workOrder);
            await _technicianRepository.UpdateAsync(technician);
        }
    }
}