using MediatR;
using System;

namespace FSM.Application.Features.WorkOrders.Commands.ScheduleWorkOrder;

public class ScheduleWorkOrderCommand : IRequest<bool>
{
    public int WorkOrderId { get; set; }
    public DateTime ScheduledStartDate { get; set; }
    public DateTime ScheduledEndDate { get; set; }

    // Eğer takvimde işi başka bir teknisyenin üzerine sürüklerse diye bunu da ekliyoruz
    public int? TechnicianId { get; set; }
}