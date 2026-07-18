using AutoMapper;
using MediatR;
using FSM.Application.Common;
using FSM.Domain.Entities;
using FSM.Domain.Interfaces;
using FSM.Application.Interfaces;
using Microsoft.Extensions.Caching.Distributed; // 🔥 1. REDIS İÇİN EKLENDİ

namespace FSM.Application.Features.Customers.Commands.UpdateCustomer;

public class UpdateCustomerCommandHandler : IRequestHandler<UpdateCustomerCommand, Unit>
{
    private readonly IGenericRepository<Customer> _repository;
    private readonly IMapper _mapper;
    private readonly INotificationService _notificationService;
    private readonly IDistributedCache _cache; // 🔥 2. EKLENDİ

    public UpdateCustomerCommandHandler(
        IGenericRepository<Customer> repository,
        IMapper mapper,
        INotificationService notificationService,
        IDistributedCache cache) // 🔥 3. EKLENDİ
    {
        _repository = repository;
        _mapper = mapper;
        _notificationService = notificationService;
        _cache = cache; // 🔥 EŞLEŞTİRİLDİ
    }

    public async Task<Unit> Handle(UpdateCustomerCommand request, CancellationToken cancellationToken)
    {
        var customer = await _repository.GetActiveByIdOrThrowAsync(request.Id, "müşteri");

        // Verileri elle atayarak kuryeyi aradan çıkarıyoruz:
        customer.FirstName = request.Name;
        customer.LastName = request.LastName;
        customer.Email = request.Email;
        customer.PhoneNumber = request.Phone;
        customer.Address = request.Address;

        await _repository.UpdateAsync(customer);
        await _repository.SaveChangesAsync(); // Fırın çalıştı 🔥

        // 🔥 YENİ EKLENEN KISIM: ÇEKMECEYİ TEMİZLİYORUZ (Cache Invalidation)
        await _cache.RemoveAsync("all_customers_list", cancellationToken);

        // Sinyali Ateşliyoruz!
        await _notificationService.SendWorkOrderNotification(
            $"🔄 Müşteri Bilgileri Güncellendi: {customer.FirstName} {customer.LastName}"
        );

        return Unit.Value;
    }
}