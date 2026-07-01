using AutoMapper;
using MediatR;
using FSM.Domain.Entities;
using FSM.Domain.Interfaces;

namespace FSM.Application.Features.Technicians.Commands.UpdateTechnician;

public class UpdateTechnicianCommandHandler : IRequestHandler<UpdateTechnicianCommand, Unit>
{
    private readonly IGenericRepository<Technician> _repository;
    private readonly IMapper _mapper;

    public UpdateTechnicianCommandHandler(IGenericRepository<Technician> repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<Unit> Handle(UpdateTechnicianCommand request, CancellationToken cancellationToken)
    {
        var technician = await _repository.GetByIdAsync(request.Id);

        if (technician == null || technician.IsDeleted)
            throw new Exception($"ID'si {request.Id} olan aktif teknisyen bulunamadı!");

        // Manuel güncellemeler:
        technician.FullName = request.FullName;
        technician.Email = request.Email;
        technician.PhoneNumber = request.PhoneNumber;

        await _repository.UpdateAsync(technician);
        await _repository.SaveChangesAsync();

        return Unit.Value;
    }
}