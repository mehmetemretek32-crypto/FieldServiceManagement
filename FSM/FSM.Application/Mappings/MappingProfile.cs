using AutoMapper;
using FSM.Application.DTOs;
using FSM.Application.DTOs.Customer;
using FSM.Application.DTOs.Technicians; // Senin klasör adın 'Technicians' idi
using FSM.Application.DTOs.WorkOrders;
using FSM.Domain.Entities;

namespace FSM.Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // ====================================================
        // 1. WORK ORDER (İŞ EMİRLERİ) HARİTASI
        // ====================================================
        CreateMap<WorkOrder, WorkOrderDto>();
        CreateMap<CreateWorkOrderDto, WorkOrder>();
        CreateMap<UpdateWorkOrderDto, WorkOrder>();

        // ====================================================
        // 2. TECHNICIAN (TEKNİSYENLER) HARİTASI
        // ====================================================
        CreateMap<Technician, TechnicianDto>();
        CreateMap<CreateTechnicianDto, Technician>();
        CreateMap<UpdateTechnicianDto, Technician>();

        // ====================================================
        // 3. CUSTOMER (MÜŞTERİLER) HARİTASI
        // ====================================================
        CreateMap<Customer, CustomerDto>();
        CreateMap<CreateCustomerDto, Customer>();
        CreateMap<UpdateCustomerDto, Customer>();
    }
}