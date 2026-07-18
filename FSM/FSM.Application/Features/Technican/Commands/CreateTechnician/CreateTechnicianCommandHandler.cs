using FSM.Domain.Entities;
using FSM.Domain.Interfaces;
using FSM.Application.Interfaces;
using MediatR;
using AutoMapper;
using Microsoft.Extensions.Caching.Distributed; // 🔥 REDIS EKLENDİ

namespace FSM.Application.Features.Technican.Commands.CreateTechnician;

public class CreateTechnicianCommandHandler : IRequestHandler<CreateTechnicianCommand, int>
{
    private readonly IGenericRepository<Technician> _technicianRepository;
    private readonly IMapper _mapper;
    private readonly INotificationService _notificationService;
    private readonly IDistributedCache _cache; // 🔥 EKLENDİ

    public CreateTechnicianCommandHandler(
        IGenericRepository<Technician> technicianRepository,
        IMapper mapper,
        INotificationService notificationService,
        IDistributedCache cache) // 🔥 EKLENDİ
    {
        _technicianRepository = technicianRepository;
        _mapper = mapper;
        _notificationService = notificationService;
        _cache = cache; // 🔥 EŞLEŞTİRİLDİ
    }

    public async Task<int> Handle(CreateTechnicianCommand request, CancellationToken cancellationToken)
    {
        var newTechnician = new Technician
        {
            FullName = request.FullName,
            Email = request.Email,
            PhoneNumber = request.PhoneNumber,
            IsAvailable = request.IsAvailable,
            IsDeleted = false
        };

        await _technicianRepository.AddAsync(newTechnician);
        await _technicianRepository.SaveChangesAsync(); // Fırın çalıştı 🔥

        // 👇 🔥 ÇEKMECELERİ TEMİZLİYORUZ (Cache Invalidation)
        // Teknisyen eklendiği için hem genel listeyi hem de Top 5 listesini siliyoruz!
        await _cache.RemoveAsync("all_technicians_list", cancellationToken);
        await _cache.RemoveAsync("top_technicians_list", cancellationToken);

        await _notificationService.SendWorkOrderNotification(
            $"🔧 Yeni Teknisyen Sisteme Eklendi: {newTechnician.FullName}"
        );

        return newTechnician.Id;
    }
}