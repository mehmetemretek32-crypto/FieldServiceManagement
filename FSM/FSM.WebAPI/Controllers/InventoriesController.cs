using FSM.Application.Features.Inventories.Commands.CreateInventoryItem;
using FSM.Application.Features.Inventories.Commands.DeleteInventoryItem;
using FSM.Application.Features.Inventories.Commands.UpdateInventoryItem;
using FSM.Application.Features.Inventories.Queries.GetAllInventoryItems;
using FSM.Application.Features.Inventories.Queries.GetInventoryItemById;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FSM.WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class InventoriesController : ControllerBase
{
    private readonly IMediator _mediator;

    public InventoriesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // 1. LİSTELEME
    [Authorize(Roles = "Admin,Dispatcher,Technician")]
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _mediator.Send(new GetAllInventoryItemsQuery());
        return Ok(result);
    }

    // 2. TEKİL GETİRME
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _mediator.Send(new GetInventoryItemByIdQuery(id));
        return Ok(result);
    }

    // 3. OLUŞTURMA
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateInventoryItemCommand command)
    {
        var resultId = await _mediator.Send(command);
        return Ok(new { Message = "Malzeme başarıyla eklendi!", Id = resultId });
    }

    // 4. GÜNCELLEME
    [HttpPut]
    public async Task<IActionResult> Update([FromBody] UpdateInventoryItemCommand command)
    {
        var message = await _mediator.Send(command);
        return Ok(new { Message = message });
    }

    // 5. SİLME
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var message = await _mediator.Send(new DeleteInventoryItemCommand(id));
        return Ok(new { Message = message });
    }
}