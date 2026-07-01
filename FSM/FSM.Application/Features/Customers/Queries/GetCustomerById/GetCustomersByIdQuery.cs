using FSM.Application.DTOs;
using FSM.Application.DTOs.Customer; // DTO'nun olduğu namespace
using MediatR;

namespace FSM.Application.Features.Customers.Queries.GetCustomerById;

// Geriye bir CustomerDto döneceğini belirtiyoruz
public class GetCustomerByIdQuery : IRequest<CustomerDto>
{
    public int Id { get; set; }
}