using AutoMapper;
using MediatR;
using FSM.Application.Common;
using FSM.Domain.Entities;
using FSM.Domain.Interfaces;
using FSM.Application.Interfaces; // <-- YENİ: Bildirim servisini çağırdık

namespace FSM.Application.Features.Technicians.Commands.UpdateTechnician;

public class UpdateTechnicianCommandHandler : IRequestHandler<UpdateTechnicianCommand, Unit>
{
    private readonly IGenericRepository<Technician> _repository;
    private readonly IMapper _mapper;

    // 1. Bildirim servisi tanımı
    private readonly INotificationService _notificationService; // <-- EKLENDİ

    // 2. Constructor içine enjekte edilmesi
    public UpdateTechnicianCommandHandler(
        IGenericRepository<Technician> repository,
        IMapper mapper,
        INotificationService notificationService) // <-- EKLENDİ
    {
        _repository = repository;
        _mapper = mapper;
        _notificationService = notificationService; // <-- EKLENDİ
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

        // 👇 3. YENİ EKLENEN KISIM: Sinyali Ateşliyoruz!
        await _notificationService.SendWorkOrderNotification(
            $"🔄 #{technician.Id} ID'li teknisyen ({technician.FullName}) bilgileri güncellendi!"
        );

        return Unit.Value;
    }
}