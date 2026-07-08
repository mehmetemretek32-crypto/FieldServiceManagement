using AutoMapper;
using FSM.Application.Common;
using FSM.Application.DTOs;
using FSM.Application.Interfaces;
using FSM.Domain.Entities;
using FSM.Domain.Interfaces;
using MediatR;

namespace FSM.Application.Features.Customers.Queries.GetAllCustomers;

public class GetAllCustomerwQueryHandler : IRequestHandler<GetAllCustomersQuery, IEnumerable<CustomerDto>>
{
    private readonly IGenericRepository<Customer> _repository;
    private readonly IMapper _mapper;

    public GetAllCustomerwQueryHandler(IGenericRepository<Customer> repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<CustomerDto>> Handle(GetAllCustomersQuery request, CancellationToken cancellationToken)
    {
        var customers = await _repository.GetAllAsync();
        var activeCustomers = customers.OnlyActive().ToList();
        var customerDtos = _mapper.Map<List<CustomerDto>>(activeCustomers);
        return customerDtos;
    }
}