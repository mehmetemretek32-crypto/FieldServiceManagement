using AutoMapper;
using FSM.Application.DTOs;
using FSM.Application.DTOs.WorkOrders;
using FSM.Domain.Entities;

namespace FSM.Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // WorkOrder -> WorkOrderDto dönüşümü
        CreateMap<WorkOrder, WorkOrderDto>();

        // CreateWorkOrderDto -> WorkOrder dönüşümü
        CreateMap<CreateWorkOrderDto, WorkOrder>();
    }
}