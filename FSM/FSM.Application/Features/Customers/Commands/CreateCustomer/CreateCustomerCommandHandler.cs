using AutoMapper;
using FSM.Domain.Entities;
using FSM.Domain.Interfaces;
using MediatR;
using FSM.Application.Interfaces; // <-- YENİ: Bildirim servisini çağırdık

namespace FSM.Application.Features.Customers.Commands.CreateCustomer;

public class CreateCustomerCommandHandler : IRequestHandler<CreateCustomerCommand, int>
{
    private readonly IGenericRepository<Customer> _repository;
    private readonly IMapper _mapper;

    // 1. Bildirim servisi tanımı
    private readonly INotificationService _notificationService; // <-- EKLENDİ

    // 2. Constructor içine enjekte edilmesi
    public CreateCustomerCommandHandler(
        IGenericRepository<Customer> repository,
        IMapper mapper,
        INotificationService notificationService) // <-- EKLENDİ
    {
        _repository = repository;
        _mapper = mapper;
        _notificationService = notificationService; // <-- EKLENDİ
    }

    public async Task<int> Handle(CreateCustomerCommand request, CancellationToken cancellationToken)
    {
        var entity = _mapper.Map<Customer>(request);

        // 🔥 GÜVENLİK KİLİDİ: Yeni eklenen bir müşteri asla silinmiş olarak başlayamaz!
        entity.IsDeleted = false;

        await _repository.AddAsync(entity);
        await _repository.SaveChangesAsync(); // Fırın çalıştı 🔥

        // 👇 3. YENİ EKLENEN KISIM: Sinyali Ateşliyoruz!
        await _notificationService.SendWorkOrderNotification(
            $"👤 Yeni Müşteri Kaydı Oluşturuldu: {entity.FirstName} {entity.LastName}"
        );

        return entity.Id; // Eklenen müşterinin ID'sini dön
    }
}