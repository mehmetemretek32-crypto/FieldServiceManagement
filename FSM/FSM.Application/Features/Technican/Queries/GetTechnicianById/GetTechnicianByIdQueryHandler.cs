using AutoMapper;
using FSM.Application.DTOs.Technicians;
using FSM.Domain.Entities;
using FSM.Domain.Interfaces;
using MediatR;

namespace FSM.Application.Features.Technicians.Queries.GetTechnicianById;

public class GetTechnicianByIdQueryHandler : IRequestHandler<GetTechnicianByIdQuery, TechnicianDto>
{
    private readonly IGenericRepository<Technician> _repository;
    private readonly IMapper _mapper;

    public GetTechnicianByIdQueryHandler(IGenericRepository<Technician> repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<TechnicianDto> Handle(GetTechnicianByIdQuery request, CancellationToken cancellationToken)
    {
        var technician = await _repository.GetByIdAsync(request.Id);

        // Hem nesne yoksa hem de Soft Delete ile silinmişse hata fırlatıyoruz!
        if (technician == null || technician.IsDeleted)
        {
            throw new Exception($"ID'si {request.Id} olan aktif bir teknisyen bulunamadı!");
        }

        var dto = _mapper.Map<TechnicianDto>(technician);
        return dto;
    }
}