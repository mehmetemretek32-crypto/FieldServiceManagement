using AutoMapper;
using MediatR;
using FSM.Domain.Entities;
using FSM.Domain.Interfaces; // Kendi Repository namespace'ine göre düzelt

namespace FSM.Application.Features.Technicians.Commands.CreateTechnician;

public class CreateTechnicianCommandHandler : IRequestHandler<CreateTechnicianCommand, int>
{
    private readonly IGenericRepository<Technician> _repository;
    private readonly IMapper _mapper;

    public CreateTechnicianCommandHandler(IGenericRepository<Technician> repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<int> Handle(CreateTechnicianCommand request, CancellationToken cancellationToken)
    {
        // Kurye (AutoMapper) gelen request'i Technician nesnesine çeviriyor
        var technician = _mapper.Map<Technician>(request);

        await _repository.AddAsync(technician);
        await _repository.SaveChangesAsync(); // Kaydetmeyi unutmuyoruz!

        return technician.Id;
    }
}