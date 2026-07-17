using FSM.Domain.Entities;
using FSM.Domain.Interfaces;
using FSM.Application.Interfaces; // <-- YENİ: Bildirim servisini çağırdık
using MediatR;
using AutoMapper; // IMapper için gerekli olabilir

namespace FSM.Application.Features.Technican.Commands.CreateTechnician;

public class CreateTechnicianCommandHandler : IRequestHandler<CreateTechnicianCommand, int>
{
    private readonly IGenericRepository<Technician> _technicianRepository;
    private readonly IMapper _mapper;

    // 1. Bildirim servisi tanımı
    private readonly INotificationService _notificationService; // <-- EKLENDİ

    // 2. Constructor içine enjekte edilmesi
    public CreateTechnicianCommandHandler(
        IGenericRepository<Technician> technicianRepository,
        IMapper mapper,
        INotificationService notificationService) // <-- EKLENDİ
    {
        _technicianRepository = technicianRepository;
        _mapper = mapper;
        _notificationService = notificationService; // <-- EKLENDİ
    }

    public async Task<int> Handle(CreateTechnicianCommand request, CancellationToken cancellationToken)
    {
        // 1. Gelen Command verisini yeni bir Technician entity nesnesine çeviriyoruz
        var newTechnician = new Technician
        {
            FullName = request.FullName,
            Email = request.Email,
            PhoneNumber = request.PhoneNumber,
            IsAvailable = request.IsAvailable,
            IsDeleted = false
        };

        // 2. Veritabanına ekle
        await _technicianRepository.AddAsync(newTechnician);

        // 3. Fırını çalıştır (Değişiklikleri kaydet) 🔥
        await _technicianRepository.SaveChangesAsync();

        // 4. 👇 YENİ EKLENEN KISIM: Sinyali Ateşliyoruz!
        await _notificationService.SendWorkOrderNotification(
            $"🔧 Yeni Teknisyen Sisteme Eklendi: {newTechnician.FullName}"
        );

        // 5. Yeni oluşan ID'yi geriye döndür
        return newTechnician.Id;
    }
}
