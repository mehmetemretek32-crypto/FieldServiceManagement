using AutoMapper;
using FSM.Application.DTOs;
using FSM.Domain.Entities;
using AutoMapper;
using MediatR;
using FSM.Domain.Entities;
using FSM.Domain.Interfaces; // Kendi Repository yoluna göre düzenle
using FSM.Application.DTOs.Customer;

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
        var customer = await _repository.GetByIdAsync(request.Id);

        // Hem veritabanında hiç yoksa (null) hem de silinmişse (IsDeleted == true) hata fırlatıyoruz
        if (customer == null || customer.IsDeleted)
        {
            throw new Exception($"ID'si {request.Id} olan aktif bir müşteri bulunamadı!");
        }

        var customerDto = _mapper.Map<CustomerDto>(customer);
        return customerDto;
    }
}