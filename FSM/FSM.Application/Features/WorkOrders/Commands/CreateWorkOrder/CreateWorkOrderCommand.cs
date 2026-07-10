using MediatR;

namespace FSM.Application.Features.WorkOrders.Commands.CreateWorkOrder;

public class CreateWorkOrderCommand : IRequest<int>
{
    public string Title { get; set; } = string.Empty; // DTO'dan gelen başlık
    public int CustomerId { get; set; }
    public string Description { get; set; } = string.Empty;
    public string Status { get; set; } = "Açık";
    public int? TechnicianId { get; set; }
}