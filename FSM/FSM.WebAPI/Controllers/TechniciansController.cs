using FSM.Application.Features.Technican.Queries.GetAllTechnician;
using FSM.Application.Features.Technicians.Commands.CreateTechnician;
using FSM.Application.Features.Technicians.Commands.DeleteTechnician;
using FSM.Application.Features.Technicians.Commands.UpdateTechnician;
using FSM.Application.Features.Technicians.Queries.GetTechnicianById;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FSM.WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TechniciansController : ControllerBase
{
    private readonly IMediator _mediator;

    public TechniciansController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // GET: api/Technicians
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _mediator.Send(new GetAllTechniciansQuery());
        return Ok(result);
    }

    // GET: api/Technicians/5
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _mediator.Send(new GetTechnicianByIdQuery { Id = id });
        return Ok(result);
    }

    // POST: api/Technicians
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateTechnicianCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    // PUT: api/Technicians
    [HttpPut]
    public async Task<IActionResult> Update([FromBody] UpdateTechnicianCommand command)
    {
        await _mediator.Send(command);
        return Ok(new { message = "Teknisyen bilgileri başarıyla güncellendi." });
    }

    // DELETE: api/Technicians/5 (Soft Delete)
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _mediator.Send(new DeleteTechnicianCommand { Id = id });
        return Ok(new { message = "Teknisyen başarıyla pasife çekildi (Soft Delete)." });
    }
}