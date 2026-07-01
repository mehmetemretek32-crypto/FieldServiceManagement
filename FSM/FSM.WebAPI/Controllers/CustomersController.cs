using MediatR;
using Microsoft.AspNetCore.Mvc;
using FSM.Application.Features.Customers.Commands.CreateCustomer;
using FSM.Application.Features.Customers.Commands.UpdateCustomer;
using FSM.Application.Features.Customers.Commands.DeleteCustomer;
using FSM.Application.Features.Customers.Queries.GetAllCustomers;
using FSM.Application.Features.Customers.Queries.GetCustomerById;

namespace FSM.WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CustomersController : ControllerBase
{
    private readonly IMediator _mediator;

    public CustomersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        // GetAllCustomersQuery sınıfının adını, kendi oluşturduğun Query sınıfının adıyla aynı olduğundan emin ol
        var customers = await _mediator.Send(new GetAllCustomersQuery());
        return Ok(customers);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        // GetCustomerByIdQuery sınıfını gönderiyoruz
        var customer = await _mediator.Send(new GetCustomerByIdQuery { Id = id });
        return Ok(customer);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCustomerCommand command)
    {
        // DTO yerine direkt Command alıyoruz
        var newCustomerId = await _mediator.Send(command);
        return Ok(new { message = "Müşteri başarıyla oluşturuldu.", id = newCustomerId });
    }

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] UpdateCustomerCommand command)
    {
        // Dün WorkOrders'da yaptığımız gibi, Unit dönen işlemlerde değişkene atama (var result =) yapmıyoruz!
        await _mediator.Send(command);
        return Ok(new { message = "Müşteri başarıyla güncellendi." });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        // Delete için de direkt command gönderiyoruz
        await _mediator.Send(new DeleteCustomerCommand { Id = id });
        return Ok(new { message = "Müşteri başarıyla sistemden pasife çekildi." });
    }
}