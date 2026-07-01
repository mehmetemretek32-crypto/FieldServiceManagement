using MediatR;

namespace FSM.Application.Features.Customers.Commands.DeleteCustomer;

public class DeleteCustomerCommand : IRequest<Unit>
{
    // Silmek için sadece ID'ye ihtiyacımız var
    public int Id { get; set; }
}