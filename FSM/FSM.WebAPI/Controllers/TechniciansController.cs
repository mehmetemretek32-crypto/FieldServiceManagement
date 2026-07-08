using FSM.Application.Features.Technican.Queries.GetAllTechnician;
using FSM.Application.Features.Technican.Commands.CreateTechnician;
using FSM.Application.Features.Technicians.Commands.DeleteTechnician;
using FSM.Application.Features.Technicians.Commands.UpdateTechnician;
using FSM.Application.Features.Technicians.Queries.GetTechnicianById;
using MediatR;
using Microsoft.AspNetCore.Authorization;
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

    // --- GENEL TEKNİSYEN OPERASYONLARI ---

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _mediator.Send(new GetAllTechniciansQuery());
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _mediator.Send(new GetTechnicianByIdQuery { Id = id });
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateTechnicianCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] UpdateTechnicianCommand command)
    {
        await _mediator.Send(command);
        return Ok(new { message = "Teknisyen bilgileri başarıyla güncellendi." });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _mediator.Send(new DeleteTechnicianCommand { Id = id });
        return Ok(new { message = "Teknisyen başarıyla pasife çekildi (Soft Delete)." });
    }

    // --- 🔥 PRO FSM EKLEMELERİ (OPERASYON & YETKİLENDİRME) ---

    // 1. TEKNİSYEN HIZLI DURUM DEĞİŞTİRME (Müsait <-> Meşgul)
    // PATCH: api/Technicians/5/availability?isAvailable=true
    [HttpPatch("{id}/availability")]
    public async Task<IActionResult> UpdateAvailability(int id, [FromQuery] bool isAvailable)
    {
        // Not: Bunun için küçük bir UpdateTechnicianAvailabilityCommand yazacağız veya UpdateCommand kullanacağız
        return Ok(new { message = $"Teknisyen statüsü '(Müsait: {isAvailable})' olarak güncellendi." });
    }

    // 2. TEKNİSYENE ATANMIŞ İŞ EMİRLERİNİ GETİR (Teknisyen Kendi İşlerini Görür)
    // GET: api/Technicians/5/workorders
    [Authorize(Roles = "Technician,Admin")]
    [HttpGet("{id}/workorders")]
    public async Task<IActionResult> GetWorkOrdersByTechnician(int id)
    {
        // İleride buraya: var result = await _mediator.Send(new GetWorkOrdersByTechnicianIdQuery { TechnicianId = id });
        return Ok(new { Message = $"{id} ID'li teknisyene ait aktif saha iş emirleri listeleniyor..." });
    }

    // 3. ADMİN ÖZEL: TÜM TEKNİSYENLER (Silinmişler / Pasifler Dahil Performans Raporu)
    [Authorize(Roles = "Admin")]
    [HttpGet("admin/all-with-performance")]
    public IActionResult GetAllTechniciansAdmin()
    {
        // İleride buraya: Adminlere özel silinmiş teknisyenleri veya tamamladığı toplam iş sayısını çeken sorgu gelecek
        return Ok(new { Message = "Admin yetkisiyle tüm teknisyen performans ve pasiflik kayıtları görüntüleniyor." });
    }
}