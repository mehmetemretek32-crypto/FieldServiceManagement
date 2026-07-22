using System;
using System.Text.Json.Serialization;
using MediatR;

namespace FSM.Application.Features.WorkOrders.Commands.UpdateWorkOrder;

public class UpdateWorkOrderCommand : IRequest<Unit>
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    [JsonPropertyName("state")]
    public int State { get; set; }

    public int? TechnicianId { get; set; }
    public int CustomerId { get; set; }

    public DateTime? ScheduledStartDate { get; set; }
    public DateTime? ScheduledEndDate { get; set; }
}