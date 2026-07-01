using AutoMapper;
using MediatR;
using FSM.Domain.Entities;
using FSM.Domain.Interfaces; // Repository arayüzünün olduğu namespace (projedeki yerine göre düzeltirsin)

namespace FSM.Application.Features.Customers.Commands.UpdateCustomer;

public class UpdateCustomerCommandHandler : IRequestHandler<UpdateCustomerCommand, Unit>
{
    private readonly IGenericRepository<Customer> _repository;
    private readonly IMapper _mapper;

    public UpdateCustomerCommandHandler(IGenericRepository<Customer> repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;


    }

    public async Task<Unit> Handle(UpdateCustomerCommand request, CancellationToken cancellationToken)
    {
        var customer = await _repository.GetByIdAsync(request.Id);

        if (customer == null || customer.IsDeleted)
            throw new Exception($"ID'si {request.Id} olan müşteri bulunamadı!");

        // Verileri elle atayarak kuryeyi aradan çıkarıyoruz:
        customer.FirstName = request.Name; // Kendi özellik isimlerini yaz
        customer.LastName = request.LastName;
        customer.Email = request.Email;
        customer.PhoneNumber = request.Phone;
        customer.Address = request.Address;

        await _repository.UpdateAsync(customer);
        await _repository.SaveChangesAsync();
        return Unit.Value;
    }
}