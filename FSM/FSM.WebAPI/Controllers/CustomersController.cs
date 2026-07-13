using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FSM.Application.Features.Customers.Commands.CreateCustomer;
using FSM.Application.Features.Customers.Commands.UpdateCustomer;
using FSM.Application.Features.Customers.Commands.DeleteCustomer;
using FSM.Application.Features.Customers.Queries.GetAllCustomers;
using FSM.Application.Features.Customers.Queries.GetCustomerById;

namespace FSM.WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class CustomersController : ControllerBase
{
    private readonly IMediator _mediator;

    public CustomersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // --- GENEL MÜŞTERİ OPERASYONLARI ---

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var customers = await _mediator.Send(new GetAllCustomersQuery());
        return Ok(customers);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var customer = await _mediator.Send(new GetCustomerByIdQuery { Id = id });
        return Ok(customer);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCustomerCommand command)
    {
        var newCustomerId = await _mediator.Send(command);
        return Ok(new { message = "Müşteri başarıyla oluşturuldu.", id = newCustomerId });
    }

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] UpdateCustomerCommand command)
    {
        await _mediator.Send(command);
        return Ok(new { message = "Müşteri başarıyla güncellendi." });
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        await _mediator.Send(new DeleteCustomerCommand { Id = id });
        return Ok(new { message = "Müşteri başarıyla sistemden pasife çekildi." });
    }

    // --- 🔥 PRO FSM EKLEMELERİ (TARİHÇE VE RAPORLAMA) ---

    // MÜŞTERİ KRONOLOJİK İŞ EMRİ GEÇMİŞİ (Timeline Modalı İçin)
    // GET: api/Customers/5/workorders
    [HttpGet("{id}/workorders")]
    public async Task<IActionResult> GetWorkOrderHistoryByCustomer(int id)
    {
        // İleride buraya: var history = await _mediator.Send(new GetWorkOrdersByCustomerIdQuery { CustomerId = id });
        return Ok(new { Message = $"{id} ID'li müşterinin geçmiş tüm arıza, bakım ve saha iş emirleri listeleniyor..." });
    }
}