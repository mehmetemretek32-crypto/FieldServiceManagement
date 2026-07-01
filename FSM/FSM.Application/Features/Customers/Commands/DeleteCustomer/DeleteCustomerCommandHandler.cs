using MediatR;
using FSM.Domain.Entities;
using FSM.Domain.Interfaces; // Kendi namespace'ine göre düzeltirsin

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
        var customer = await _repository.GetByIdAsync(request.Id);

        if (customer == null)
            throw new Exception($"ID'si {request.Id} olan müşteri bulunamadı!");

        // 1. Veriyi silmek yerine durumunu "Silindi" veya "Pasif" olarak işaretliyoruz.
        // DİKKAT: Kendi Customer tablonuzdaki özellik adı neyse (IsActive, Status vb.) onu kullanmalısın!
        customer.IsDeleted = true;

        // 2. Repository'nin Delete metodunu DEĞİL, Update metodunu çağırıyoruz.
        await _repository.UpdateAsync(customer);

        return Unit.Value;
    }
}