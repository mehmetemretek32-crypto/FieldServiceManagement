namespace FSM.Application.DTOs;

public class CreateWorkOrderDto
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    // Müşteriyle bağlantı kurmamızı sağlayacak kapı:
    public int CustomerId { get; set; }
}