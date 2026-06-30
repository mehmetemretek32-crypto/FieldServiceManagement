
using MediatR;
using FSM.Application.DTOs;

namespace FSM.Application.Features.Customers.Queries.GetAllCustomers;

public class GetAllCustomersQuery : IRequest<IEnumerable<CustomerDto>>
{
}