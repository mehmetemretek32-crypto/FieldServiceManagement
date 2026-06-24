using FSM.Application.DTOs.Technicians;
using FSM.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FSM.WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TechniciansController : ControllerBase
{
    private readonly ITechnicianService _service;

    public TechniciansController(ITechnicianService service)
    {
        _service = service;
    }

    // GET: api/Technicians
    [HttpGet]
    public async Task<ActionResult<IEnumerable<TechnicianDto>>> GetAll()
    {
        var technicians = await _service.GetAllTechniciansAsync();
        return Ok(technicians);
    }

    // GET: api/Technicians/5
    [HttpGet("{id}")]
    public async Task<ActionResult<TechnicianDto>> GetById(int id)
    {
        var technician = await _service.GetTechnicianByIdAsync(id);

        if (technician == null)
            return NotFound(new { message = $"{id} numaralı teknisyen bulunamadı." });

        return Ok(technician);
    }

    // POST: api/Technicians
    [HttpPost]
    public async Task<ActionResult<int>> Create([FromBody] CreateTechnicianDto dto)
    {
        var newId = await _service.CreateTechnicianAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = newId }, newId);
    }
}