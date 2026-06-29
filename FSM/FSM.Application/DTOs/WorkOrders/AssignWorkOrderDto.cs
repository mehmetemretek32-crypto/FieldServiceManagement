namespace FSM.Application.DTOs.WorkOrders
{
    // Sadece İş Emri ID'si ve Teknisyen ID'sini taşıyan hafif bir çanta
    public record AssignWorkOrderDto(int WorkOrderId, int TechnicianId);
}