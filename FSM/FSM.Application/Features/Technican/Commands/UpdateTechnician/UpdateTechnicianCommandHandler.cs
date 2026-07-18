using AutoMapper;
using MediatR;
using FSM.Application.Common;
using FSM.Domain.Entities;
using FSM.Domain.Interfaces;
using FSM.Application.Interfaces;
using Microsoft.Extensions.Caching.Distributed; // 🔥 REDIS EKLENDİ

namespace FSM.Application.Features.Technicians.Commands.UpdateTechnician;

public class UpdateTechnicianCommandHandler : IRequestHandler<UpdateTechnicianCommand, Unit>
{
    private readonly IGenericRepository<Technician> _repository;
    private readonly IMapper _mapper;
    private readonly INotificationService _notificationService;
    private readonly IDistributedCache _cache; // 🔥 EKLENDİ

    public UpdateTechnicianCommandHandler(
        IGenericRepository<Technician> repository,
        IMapper mapper,
        INotificationService notificationService,
        IDistributedCache cache) // 🔥 EKLENDİ
    {
        _repository = repository;
        _mapper = mapper;
        _notificationService = notificationService;
        _cache = cache; // 🔥 EŞLEŞTİRİLDİ
    }

    public async Task<Unit> Handle(UpdateTechnicianCommand request, CancellationToken cancellationToken)
    {
        var technician = await _repository.GetActiveByIdOrThrowAsync(request.Id, "teknisyen");

        // Manuel güncellemeler:
        technician.FullName = request.FullName;
        technician.Email = request.Email;
        technician.PhoneNumber = request.PhoneNumber;

        await _repository.UpdateAsync(technician);
        await _repository.SaveChangesAsync(); // Fırın çalıştı 🔥

        // 👇 🔥 ÇEKMECELERİ TEMİZLİYORUZ (Cache Invalidation)
        await _cache.RemoveAsync("all_technicians_list", cancellationToken);
        await _cache.RemoveAsync("top_technicians_list", cancellationToken);

        await _notificationService.SendWorkOrderNotification(
            $"🔄 #{technician.Id} ID'li teknisyen ({technician.FullName}) bilgileri güncellendi!"
        );

        return Unit.Value;
    }
}