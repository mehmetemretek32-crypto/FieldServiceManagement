using AutoMapper;
using FSM.Application.DTOs;
using FSM.Domain.Entities;
using FSM.Domain.Interfaces;
using MediatR;

namespace FSM.Application.Features.Customers.Queries.GetAllCustomers;

// Not: İsimdeki küçük 'w' harf hatasını da düzelttik! 🚀
public class GetAllCustomersQueryHandler : IRequestHandler<GetAllCustomersQuery, IEnumerable<CustomerDto>>
{
    private readonly IGenericRepository<Customer> _customerRepository;
    private readonly IGenericRepository<WorkOrder> _workOrderRepository; // 🔥 YENİ: İş Emirleri deposunu aldık!
    private readonly IMapper _mapper;

    public GetAllCustomersQueryHandler(
        IGenericRepository<Customer> customerRepository,
        IGenericRepository<WorkOrder> workOrderRepository,
        IMapper mapper)
    {
        _customerRepository = customerRepository;
        _workOrderRepository = workOrderRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<CustomerDto>> Handle(GetAllCustomersQuery request, CancellationToken cancellationToken)
    {
        // 1. Silinmemiş aktif müşterileri getir
        var customers = await _customerRepository.GetAllAsync();
        var activeCustomers = customers.Where(c => !c.IsDeleted).ToList();

        // 2. Silinmemiş tüm iş emirlerini getir ve Müşteri ID'sine göre grup yapıp sayılarını bul!
        var workOrders = await _workOrderRepository.GetAllAsync();
        var workOrderCounts = workOrders
            .Where(w => !w.IsDeleted)
            .GroupBy(w => w.CustomerId)
            .ToDictionary(g => g.Key, g => g.Count());

        // 3. Senin harika AutoMapper kurgunla listeyi DTO'ya çeviriyoruz
        var customerDtos = _mapper.Map<List<CustomerDto>>(activeCustomers);

        // 4. 🔥 PRO DOKUNUŞ: Her bir DTO'nun içine girip hesapladığımız iş emri sayısını yazıyoruz!
        foreach (var dto in customerDtos)
        {
            dto.TotalWorkOrderCount = workOrderCounts.ContainsKey(dto.Id) ? workOrderCounts[dto.Id] : 0;
        }

        return customerDtos;
    }
}