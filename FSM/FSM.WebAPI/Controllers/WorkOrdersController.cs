using FSM.Application.DTOs.WorkOrders;
using FSM.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FSM.WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class WorkOrdersController : ControllerBase
{
    private readonly IWorkOrderService _service;

    public WorkOrdersController(IWorkOrderService service)
    {
        _service = service;
    }

    // GET: api/WorkOrders
    [HttpGet]
    public async Task<ActionResult<IEnumerable<WorkOrderDto>>> GetAll()
    {
        var workOrders = await _service.GetAllWorkOrdersAsync();

        return Ok(workOrders); // HTTP 200 OK (Garson tabağı masaya koydu)
    }

    // GET: api/WorkOrders/5
    [HttpGet("{id}")]
    public async Task<ActionResult<WorkOrderDto>> GetById(int id)
    {
        var workOrder = await _service.GetWorkOrderByIdAsync(id);

        if (workOrder == null)
            return NotFound(new { message = $"{id} numaralı iş emri sistemde bulunamadı." }); // HTTP 404

        return Ok(workOrder);
    }

    // POST: api/WorkOrders
    [HttpPost]
    public async Task<ActionResult<int>> Create([FromBody] CreateWorkOrderDto dto)
    {
        var newId = await _service.CreateWorkOrderAsync(dto);

        // BÜYÜ BURADA: 200 OK yerine "201 Created" döner ve faturaya yeni kaydın adresini yazar!
        return CreatedAtAction(nameof(GetById), new { id = newId }, newId);
    }
}