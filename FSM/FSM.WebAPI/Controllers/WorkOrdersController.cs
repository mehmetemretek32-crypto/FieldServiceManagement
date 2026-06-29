using FSM.Application.DTOs;
using FSM.Application.DTOs.WorkOrders;
using FSM.Application.Interfaces;
using FSM.Application.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FSM.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WorkOrdersController : ControllerBase
    {
        // İsim hem Program.cs ile uyumlu (çoğul) hale getirildi, 
        // hem de değişken adı senin alıştığın gibi '_service' olarak bırakıldı.
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

        // --- YENİ EKLENEN ATAMA ENDPOINT'İ ---
        [HttpPost("assign")]
        public async Task<IActionResult> AssignWorkOrder([FromBody] AssignWorkOrderDto dto)
        {
            // Try-Catch yok! Kod patlarsa kapıdaki Bodyguard (Middleware) yakalayacak.
            await _service.AssignWorkOrderAsync(dto);

            return Ok(new { Message = "İş emri başarıyla teknisyene atandı!" });
        }
    }
}