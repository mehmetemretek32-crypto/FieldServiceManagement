using AutoMapper;
using FSM.Application.DTOs;
using FSM.Application.DTOs.Customer;
using FSM.Application.DTOs.Technicians; // Senin klasör adın 'Technicians' idi
using FSM.Application.DTOs.WorkOrders;
using FSM.Application.Features.Customers.Commands.CreateCustomer;
using FSM.Application.Features.Customers.Commands.UpdateCustomer;
using FSM.Application.Features.Inventories.Commands.CreateInventoryItem;
using FSM.Application.Features.Technican.Commands.CreateTechnician;
using FSM.Application.Features.Technicians.Commands.UpdateTechnician;
using FSM.Application.Features.WorkOrders.Commands.CreateWorkOrder;
using FSM.Application.Features.WorkOrders.Commands.UpdateWorkOrder;
using FSM.Domain.Entities;
namespace FSM.Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // 1. WORK ORDER (İŞ EMİRLERİ) HARİTASI
        // ====================================================

        // Okuma (Read) işlemleri için: Veritabanından DTO'ya özel eşleme
        CreateMap<WorkOrder, WorkOrderDto>()
            .ForMember(dest => dest.State, opt => opt.MapFrom(src => (int)src.State))
            .ForMember(dest => dest.TechnicianName, opt => opt.MapFrom(src =>
                src.Technician != null ? src.Technician.FullName : "Henüz Atanmadı"));

        // Yazma (Write/Update) işlemleri için:
        CreateMap<CreateWorkOrderDto, CreateWorkOrderCommand>();
        CreateMap<UpdateWorkOrderDto, UpdateWorkOrderCommand>();

        // Command'den Entity'ye:
        CreateMap<CreateWorkOrderCommand, WorkOrder>();
        CreateMap<UpdateWorkOrderCommand, WorkOrder>()
            .ForMember(dest => dest.State, opt => opt.MapFrom(src => (FSM.Domain.Enums.WorkOrderState)src.State));

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


        // =========================================================
        // 4. INVENTORY (EŞYALAR) HARİTASI
        // =========================================================
        CreateMap<CreateInventoryItemCommand, InventoryItem>();
        CreateMap<InventoryItem, InventoryItemDto>();
    }
}