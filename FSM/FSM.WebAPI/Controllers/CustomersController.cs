using FSM.Application.DTOs;
using FSM.Application.DTOs.Customer;
using FSM.Application.Services; // Veya senin ICustomerService namespace'in hangisiyse
using Microsoft.AspNetCore.Mvc;

namespace FSM.WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CustomersController : ControllerBase
{
    private readonly ICustomerService _customerService;

    public CustomersController(ICustomerService customerService)
    {
        _customerService = customerService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var customers = await _customerService.GetAllCustomersAsync();
        return Ok(customers);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var customer = await _customerService.GetCustomerByIdAsync(id);
        return Ok(customer);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCustomerDto dto)
    {
        var newCustomerId = await _customerService.CreateCustomerAsync(dto);
        return Ok(new { message = "Müşteri başarıyla oluşturuldu.", id = newCustomerId });
    }

    [HttpPut] // WorkOrder ile simetrik hale getirdik
    public async Task<IActionResult> Update([FromBody] UpdateCustomerDto dto)
    {
        await _customerService.UpdateCustomerAsync(dto);
        return Ok(new { message = "Müşteri başarıyla güncellendi." });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _customerService.DeleteCustomerAsync(id);
        return Ok(new { message = "Müşteri başarıyla sistemden pasife çekildi." });
    }
}