using MediatR;
using System.Collections.Generic;

namespace FSM.Application.Features.WorkOrders.Queries.GetAllWorkOrders
{
    // Angular tablosuna göndereceğimiz veri paketi (Eğer zaten bir DTO'n varsa ona da bu alanları ekleyebilirsin)
    public class WorkOrderListDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
        public string TechnicianName { get; set; } = string.Empty;
        public int Status { get; set; }
        public int Priority { get; set; }
        public string Location { get; set; } = string.Empty;
        public string ScheduledDate { get; set; } = string.Empty;
    }

    // Status parametresi (int?) alır, böylece hem tümünü hem filtrelileri getirebilir
    public record GetAllWorkOrdersQuery(int? Status = null) : IRequest<List<WorkOrderListDto>>;
}