namespace FSM.Application.DTOs.WorkOrders;

public record WorkOrderDto(
    int Id,
    string Title,
    string Description,
    string State,
    DateTime CreatedAt
);