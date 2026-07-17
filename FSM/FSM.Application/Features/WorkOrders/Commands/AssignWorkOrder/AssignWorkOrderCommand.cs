using MediatR;
using System;

namespace FSM.Application.Features.WorkOrders.Commands.AssignWorkOrder;

// Arayüzden gelecek olan veriyi taşıyan o jilet gibi Record yapımız:
public record AssignWorkOrderCommand(
    int WorkOrderId,
    int TechnicianId,
    DateTime ScheduledStartDate,
    DateTime ScheduledEndDate
) : IRequest<bool>; // İşlem başarılı olursa geriye true/false döneceğiz.