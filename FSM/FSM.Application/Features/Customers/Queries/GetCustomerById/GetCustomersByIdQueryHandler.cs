using AutoMapper;
using MediatR;
using FSM.Application.Common;
using FSM.Application.DTOs;
using FSM.Domain.Entities;
using FSM.Domain.Interfaces; // Kendi Repository yoluna göre düzenle

namespace FSM.Application.Features.Customers.Queries.GetCustomerById;

public class GetCustomerByIdQueryHandler : IRequestHandler<GetCustomerByIdQuery, CustomerDto>
{
    private readonly IGenericRepository<Customer> _repository;
    private readonly IMapper _mapper;

    public GetCustomerByIdQueryHandler(IGenericRepository<Customer> repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<CustomerDto> Handle(GetCustomerByIdQuery request, CancellationToken cancellationToken)
    {
        var customer = await _repository.GetActiveByIdOrThrowAsync(request.Id, "müşteri");

        var customerDto = _mapper.Map<CustomerDto>(customer);
        return customerDto;
    }
}