using AutoMapper;
using FSM.Domain.Entities;
using FSM.Domain.Interfaces;
using MediatR;

namespace FSM.Application.Features.Technican.Commands.CreateTechnician;

public class CreateTechnicianCommandHandler : IRequestHandler<CreateTechnicianCommand, int>
{
    private readonly IGenericRepository<Technician> _technicianRepository;
    private readonly IMapper _mapper;

    public CreateTechnicianCommandHandler(IGenericRepository<Technician> technicianRepository, IMapper mapper)
    {
        _technicianRepository = technicianRepository;
        _mapper = mapper;
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

        // 4. Yeni oluşan ID'yi geriye döndür
        return newTechnician.Id;
    }
}