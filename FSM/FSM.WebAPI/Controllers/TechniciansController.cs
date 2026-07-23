using FSM.Application.Features.Technican.Commands.CreateTechnician;
using FSM.Application.Features.Technican.Queries.GetAllTechnician;
using FSM.Application.Features.Technicians.Commands.DeleteTechnician;
using FSM.Application.Features.Technicians.Commands.UpdateTechnician;
using FSM.Application.Features.Technicians.Commands.UpdateTechnicianAvailability;
using FSM.Application.Features.Technicians.Queries.GetTechnicianById;
using FSM.Application.Features.Technicians.Queries.GetTopTechnicians;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FSM.WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class TechniciansController : ControllerBase
{
    private readonly IMediator _mediator;

    public TechniciansController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // --- GENEL TEKNİSYEN OPERASYONLARI ---
    [Authorize(Roles = "Admin,Dispatcher")]
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
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] CreateTechnicianCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpPut]
   [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update([FromBody] UpdateTechnicianCommand command)
    {
        await _mediator.Send(command);
        return Ok(new { message = "Teknisyen bilgileri başarıyla güncellendi." });
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        await _mediator.Send(new DeleteTechnicianCommand { Id = id });
        return Ok(new { message = "Teknisyen başarıyla pasife çekildi (Soft Delete)." });
    }

    // --- 🔥 PRO FSM EKLEMELERİ (OPERASYON & YETKİLENDİRME) ---

    // 1. TEKNİSYEN HIZLI DURUM DEĞİŞTİRME (Müsait <-> Meşgul)
    // PATCH: api/Technicians/5/availability?isAvailable=true
    // 1. TEKNİSYEN HIZLI DURUM DEĞİŞTİRME (Müsait <-> Meşgul)
    // PATCH: api/Technicians/5/availability?isAvailable=true
    [HttpPatch("{id}/availability")]
    public async Task<IActionResult> UpdateAvailability(int id, [FromQuery] bool isAvailable)
    {
        // Artık taslak değil, gerçek MediatR komutunu ateşliyoruz!
        var message = await _mediator.Send(new UpdateTechnicianAvailabilityCommand(id, isAvailable));

        return Ok(new { message = message });
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
    [HttpGet("top-performance")]
    public async Task<IActionResult> GetTopPerformance()
    {
        var result = await _mediator.Send(new GetTopTechniciansQuery());
        return Ok(result);
    }
}