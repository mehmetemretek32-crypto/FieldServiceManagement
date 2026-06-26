using FSM.Application.DTOs.Technicians;
using FSM.Application.Interfaces;
using FSM.Application.Services;
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

    // TEKNİSYEN GÜNCELLEME (UPDATE)
    [HttpPut]
    public async Task<IActionResult> UpdateTechnician([FromBody] UpdateTechnicianDto dto)
    {
        // 1. Gelen DTO'yu servise (mutfağa) gönder
        var isUpdated = await _service.UpdateTechnicianAsync(dto);

        // 2. Eğer servis 'false' dönerse, veritabanında böyle bir ID yok demektir
        if (!isUpdated)
        {
            return NotFound(new { message = $"ID'si {dto.Id} olan teknisyen bulunamadı." });
        }

        // 3. İşlem başarılıysa müşteriye bilgi ver
        return Ok(new { message = "Teknisyen bilgileri başarıyla güncellendi." });

        
    }
    // TEKNİSYEN YUMUŞAK SİLME (SOFT DELETE)
    // TEKNİSYEN YUMUŞAK SİLME (SOFT DELETE)
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTechnician(int id)
    {
        try
        {
            var isDeleted = await _service.DeleteTechnicianAsync(id);

            if (!isDeleted)
            {
                return NotFound(new { message = $"ID'si {id} olan aktif bir teknisyen bulunamadı." });
            }

            return Ok(new { message = "Teknisyen başarıyla pasife çekildi (Soft Delete)." });
        }
        catch (InvalidOperationException ex)
        {
          
            return BadRequest(new { message = ex.Message });
        }
    }
}