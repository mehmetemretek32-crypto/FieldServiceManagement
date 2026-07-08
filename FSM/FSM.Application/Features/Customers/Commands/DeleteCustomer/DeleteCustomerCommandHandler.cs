using MediatR;
using FSM.Application.Common;
using FSM.Domain.Entities;
using FSM.Domain.Interfaces;

namespace FSM.Application.Features.Customers.Commands.DeleteCustomer;

public class DeleteCustomerCommandHandler : IRequestHandler<DeleteCustomerCommand, Unit>
{
    private readonly IGenericRepository<Customer> _repository;

    public DeleteCustomerCommandHandler(IGenericRepository<Customer> repository)
    {
        _repository = repository;
    }

    public async Task<Unit> Handle(DeleteCustomerCommand request, CancellationToken cancellationToken)
    {
        await _repository.SoftDeleteAsync(request.Id, "müşteri");
        return Unit.Value;
    }
}
