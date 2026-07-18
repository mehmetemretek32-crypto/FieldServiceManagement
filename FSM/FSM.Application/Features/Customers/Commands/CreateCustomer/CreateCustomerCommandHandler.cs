using AutoMapper;
using FSM.Domain.Entities;
using FSM.Domain.Interfaces;
using MediatR;
using FSM.Application.Interfaces;
using Microsoft.Extensions.Caching.Distributed; // 🔥 1. YENİ: Redis kütüphanesi

namespace FSM.Application.Features.Customers.Commands.CreateCustomer;

public class CreateCustomerCommandHandler : IRequestHandler<CreateCustomerCommand, int>
{
    private readonly IGenericRepository<Customer> _repository;
    private readonly IMapper _mapper;
    private readonly INotificationService _notificationService;
    private readonly IDistributedCache _cache; // 🔥 2. YENİ: Cache tanımı eklendi

    public CreateCustomerCommandHandler(
        IGenericRepository<Customer> repository,
        IMapper mapper,
        INotificationService notificationService,
        IDistributedCache cache) // 🔥 3. YENİ: Constructor'a eklendi
    {
        _repository = repository;
        _mapper = mapper;
        _notificationService = notificationService;
        _cache = cache; // 🔥 YENİ: Eşleştirildi
    }

    public async Task<int> Handle(CreateCustomerCommand request, CancellationToken cancellationToken)
    {
        var entity = _mapper.Map<Customer>(request);

        // GÜVENLİK KİLİDİ: Yeni eklenen bir müşteri asla silinmiş olarak başlayamaz!
        entity.IsDeleted = false;

        await _repository.AddAsync(entity);
        await _repository.SaveChangesAsync(); // Fırın çalıştı ve veritabanına kaydedildi!

        // 👇 🔥 4. YENİ EKLENEN KISIM: ÇEKMECEYİ TEMİZLİYORUZ (Cache Invalidation)
        // Müşteri eklendiğine göre eski liste artık çöp oldu, Redis'ten siliyoruz.
        await _cache.RemoveAsync("all_customers_list", cancellationToken);

        // Sinyali Ateşliyoruz!
        await _notificationService.SendWorkOrderNotification(
            $"👤 Yeni Müşteri Kaydı Oluşturuldu: {entity.FirstName} {entity.LastName}"
        );

        return entity.Id; // Eklenen müşterinin ID'sini dön
    }
}