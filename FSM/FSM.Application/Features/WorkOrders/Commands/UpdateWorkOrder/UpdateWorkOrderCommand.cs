using System.Text.Json.Serialization;
using MediatR;

namespace FSM.Application.Features.WorkOrders.Commands.UpdateWorkOrder;

public class UpdateWorkOrderCommand : IRequest<Unit>
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    // WorkOrderDto.State ile aynı JSON alan adı
    [JsonPropertyName("state")]
    public int State { get; set; }

    public int? TechnicianId { get; set; }
    public int CustomerId { get; set; }
}