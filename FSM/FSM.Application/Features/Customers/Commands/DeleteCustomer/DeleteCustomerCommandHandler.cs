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

        // 1. Müşteri hiç yoksa
        if (customer == null)
        {
            throw new Exception($"Hata: ID'si {request.Id} olan müşteri bulunamadı.");
        }

        // 2. KRİTİK KONTROL: Müşteri zaten silinmişse
        if (customer.IsDeleted)
        {
            throw new Exception($"Hata: ID'si {request.Id} olan müşteri zaten daha önceden silinmiş!");
        }

        // 3. Silinmemişse pasife çek (Soft Delete)
        customer.IsDeleted = true;

        await _repository.UpdateAsync(customer);
        await _repository.SaveChangesAsync();

        return Unit.Value;
    }
}