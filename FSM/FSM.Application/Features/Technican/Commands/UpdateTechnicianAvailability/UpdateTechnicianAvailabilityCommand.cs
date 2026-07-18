using FSM.Domain.Entities;
using FSM.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Caching.Distributed; // 🔥 1. REDIS İÇİN EKLENDİ

namespace FSM.Application.Features.Technicians.Commands.UpdateTechnicianAvailability;

public sealed record UpdateTechnicianAvailabilityCommand(int Id, bool IsAvailable) : IRequest<string>;

internal sealed class UpdateTechnicianAvailabilityCommandHandler : IRequestHandler<UpdateTechnicianAvailabilityCommand, string>
{
    private readonly IGenericRepository<Technician> _repository;
    private readonly IDistributedCache _cache; // 🔥 2. EKLENDİ

    public UpdateTechnicianAvailabilityCommandHandler(
        IGenericRepository<Technician> repository,
        IDistributedCache cache) // 🔥 3. EKLENDİ
    {
        _repository = repository;
        _cache = cache; // 🔥 EŞLEŞTİRİLDİ
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

        // 👇 🔥 ÇEKMECELERİ TEMİZLİYORUZ (Cache Invalidation)
        // Müsaitlik durumu değiştiği için liste artık eskidi, temizliyoruz.
        await _cache.RemoveAsync("all_technicians_list", cancellationToken);
        await _cache.RemoveAsync("top_technicians_list", cancellationToken);

        return $"Teknisyen durumu başarıyla güncellendi.";
    }
}