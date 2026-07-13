using FSM.Domain.Entities;
using FSM.Domain.Interfaces;
using MediatR;

namespace FSM.Application.Features.Technicians.Commands.UpdateTechnicianAvailability;

// 🔥 KOMUT: Sadece ID ve yeni müsaitlik durumunu taşıyor
public sealed record UpdateTechnicianAvailabilityCommand(int Id, bool IsAvailable) : IRequest<string>;

// 🔥 İŞLEYİCİ: Veritabanına gidip sadece o alanı güncelliyor
internal sealed class UpdateTechnicianAvailabilityCommandHandler : IRequestHandler<UpdateTechnicianAvailabilityCommand, string>
{
    private readonly IGenericRepository<Technician> _repository;

    public UpdateTechnicianAvailabilityCommandHandler(IGenericRepository<Technician> repository)
    {
        _repository = repository;
    }

    public async Task<string> Handle(UpdateTechnicianAvailabilityCommand request, CancellationToken cancellationToken)
    {
        var technician = await _repository.GetByIdAsync(request.Id);

        if (technician == null || technician.IsDeleted)
            throw new Exception("Güncellenecek teknisyen bulunamadı!");

        // Sadece müsaitlik durumunu değiştiriyoruz
        technician.IsAvailable = request.IsAvailable;

        await _repository.UpdateAsync(technician);
        await _repository.SaveChangesAsync(); // 🔥 İŞTE O HAYAT KURTARAN KAYDETME ADIMI!

        return $"Teknisyen durumu başarıyla güncellendi.";
    }
}