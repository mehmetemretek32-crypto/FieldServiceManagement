using MediatR;

namespace FSM.Application.Features.Customers.Commands.CreateCustomer;

public class CreateCustomerCommand : IRequest<int>
{
    // Kendi eski CreateCustomerDto'nun içindeki özellikleri buraya kopyala.
    // Örnek olarak aşağıdakileri yazdım, kendi tablona göre düzelt:
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public string Address { get; set; }
}