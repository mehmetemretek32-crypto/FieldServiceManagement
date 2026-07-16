using MediatR;

namespace FSM.Application.Features.WorkOrders.Commands.UpdateWorkOrder;

public class UpdateWorkOrderCommand : IRequest<Unit>
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int Status { get; set; } // Angular'dan gelen sayı
    public int? TechnicianId { get; set; }
    public int CustomerId { get; set; }
}