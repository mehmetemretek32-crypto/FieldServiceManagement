using AutoMapper;
using FSM.Application.Common;
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
        var technician = await _repository.GetActiveByIdOrThrowAsync(request.Id, "teknisyen");

        var dto = _mapper.Map<TechnicianDto>(technician);
        return dto;
    }
}