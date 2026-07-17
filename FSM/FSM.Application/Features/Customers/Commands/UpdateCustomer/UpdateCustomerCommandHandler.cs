using AutoMapper;
using MediatR;
using FSM.Application.Common;
using FSM.Domain.Entities;
using FSM.Domain.Interfaces;
using FSM.Application.Interfaces; // <-- YENİ: Bildirim servisini çağırdık

namespace FSM.Application.Features.Customers.Commands.UpdateCustomer;

public class UpdateCustomerCommandHandler : IRequestHandler<UpdateCustomerCommand, Unit>
{
    private readonly IGenericRepository<Customer> _repository;
    private readonly IMapper _mapper;

    // 1. Bildirim servisi tanımı
    private readonly INotificationService _notificationService; // <-- EKLENDİ

    // 2. Constructor içine enjekte edilmesi
    public UpdateCustomerCommandHandler(
        IGenericRepository<Customer> repository,
        IMapper mapper,
        INotificationService notificationService) // <-- EKLENDİ
    {
        _repository = repository;
        _mapper = mapper;
        _notificationService = notificationService; // <-- EKLENDİ
    }

    public async Task<Unit> Handle(UpdateCustomerCommand request, CancellationToken cancellationToken)
    {
        var customer = await _repository.GetActiveByIdOrThrowAsync(request.Id, "müşteri");

        // Verileri elle atayarak kuryeyi aradan çıkarıyoruz:
        customer.FirstName = request.Name; // Kendi özellik isimlerini yaz
        customer.LastName = request.LastName;
        customer.Email = request.Email;
        customer.PhoneNumber = request.Phone;
        customer.Address = request.Address;

        await _repository.UpdateAsync(customer);
        await _repository.SaveChangesAsync(); // Fırın çalıştı 🔥

        // 👇 3. YENİ EKLENEN KISIM: Sinyali Ateşliyoruz!
        await _notificationService.SendWorkOrderNotification(
            $"🔄 Müşteri Bilgileri Güncellendi: {customer.FirstName} {customer.LastName}"
        );

        return Unit.Value;
    }
}