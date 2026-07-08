using AutoMapper;
using FSM.Application.Common;
using FSM.Application.DTOs.Technicians;
using FSM.Domain.Entities;
using FSM.Domain.Interfaces;
using MediatR;

namespace FSM.Application.Features.Technican.Queries.GetAllTechnician;

public class GetAllTechniciansQueryHandler : IRequestHandler<GetAllTechniciansQuery, List<TechnicianDto>>
{
    private readonly IGenericRepository<Technician> _repository;
    private readonly IMapper _mapper;

    public GetAllTechniciansQueryHandler(IGenericRepository<Technician> repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<List<TechnicianDto>> Handle(GetAllTechniciansQuery request, CancellationToken cancellationToken)
    {
        var technicians = await _repository.GetAllAsync();

        // DİKKAT: Burada sadece silinmemiş (IsDeleted == false) olanları filtreliyoruz!
        var activeTechnicians = technicians.OnlyActive().ToList();

        var dtoList = _mapper.Map<List<TechnicianDto>>(activeTechnicians);
        return dtoList;
    }
}