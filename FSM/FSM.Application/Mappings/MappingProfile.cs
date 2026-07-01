using AutoMapper;
using FSM.Application.DTOs;
using FSM.Application.DTOs.Customer;
using FSM.Application.DTOs.Technicians; // Senin klasör adın 'Technicians' idi
using FSM.Application.DTOs.WorkOrders;
using FSM.Application.Features.Customers.Commands.CreateCustomer;
using FSM.Application.Features.Customers.Commands.UpdateCustomer;
using FSM.Application.Features.Technicians.Commands.CreateTechnician;
using FSM.Application.Features.Technicians.Commands.UpdateTechnician;
using FSM.Application.Features.WorkOrders.Commands.CreateWorkOrder;
using FSM.Application.Features.WorkOrders.Commands.UpdateWorkOrder;
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
        CreateMap<CreateWorkOrderDto, CreateWorkOrderCommand>();
        CreateMap<UpdateWorkOrderDto, UpdateWorkOrderCommand>();
        CreateMap<CreateWorkOrderCommand, WorkOrder>();
        CreateMap<UpdateWorkOrderCommand, WorkOrder>();

        // ====================================================
        // 2. TECHNICIAN (TEKNİSYENLER) HARİTASI
        // ====================================================
        CreateMap<Technician, TechnicianDto>();
        CreateMap<CreateTechnicianCommand, Technician>()
    .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FullName))
    .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
    .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber));
        CreateMap<UpdateTechnicianCommand, Technician>()
    .ForMember(dest => dest.Id, opt => opt.Ignore()); // ID'yi güncellemiyoruz!

        // =========================================================
        // 3. CUSTOMER (MÜŞTERİLER) HARİTASI
        // =========================================================
        CreateMap<Customer, CustomerDto>();
        CreateMap<CreateCustomerCommand, Customer>();
        CreateMap<UpdateCustomerCommand, Customer>();

    }
}