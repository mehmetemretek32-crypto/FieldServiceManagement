using FSM.Application.DTOs;
using FSM.Application.Services;
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
        try
        {
            var customer = await _customerService.GetCustomerByIdAsync(id);
            return Ok(customer);
        }
        catch (Exception ex)
        {
            // Servisten fırlattığımız "Müşteri bulunamadı" hatalarını burada 400 Bad Request ile yakalıyoruz
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCustomerDto dto)
    {
        var newCustomerId = await _customerService.CreateCustomerAsync(dto);
        return Ok(new { message = "Müşteri başarıyla oluşturuldu.", id = newCustomerId });
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] CreateCustomerDto dto)
    {
        try
        {
            await _customerService.UpdateCustomerAsync(id, dto);
            return Ok(new { message = "Müşteri başarıyla güncellendi." });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await _customerService.DeleteCustomerAsync(id);
            return Ok(new { message = "Müşteri başarıyla sistemden pasife çekildi." });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}