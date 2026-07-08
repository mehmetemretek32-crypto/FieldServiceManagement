using AutoMapper;
using FSM.Domain.Entities;
using FSM.Domain.Interfaces;
using MediatR;

namespace FSM.Application.Features.Customers.Commands.CreateCustomer;

public class CreateCustomerCommandHandler : IRequestHandler<CreateCustomerCommand, int>
{
    private readonly IGenericRepository<Customer> _repository;
    private readonly IMapper _mapper;

    public CreateCustomerCommandHandler(IGenericRepository<Customer> repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<int> Handle(CreateCustomerCommand request, CancellationToken cancellationToken)
    {
        var entity = _mapper.Map<Customer>(request);

        // 🔥 GÜVENLİK KİLİDİ: Yeni eklenen bir müşteri asla silinmiş olarak başlayamaz!
        entity.IsDeleted = false;

        await _repository.AddAsync(entity);
        await _repository.SaveChangesAsync();

        return entity.Id; // Eklenen müşterinin ID'sini dön
    }
}