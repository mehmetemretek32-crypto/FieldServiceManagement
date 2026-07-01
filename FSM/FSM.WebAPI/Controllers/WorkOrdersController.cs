using AutoMapper;
using FSM.Application.DTOs;
using FSM.Application.DTOs.WorkOrders;
using FSM.Application.Features.WorkOrders.Commands.AssignWorkOrder;
using FSM.Application.Features.WorkOrders.Commands.CreateWorkOrder;
using FSM.Application.Features.WorkOrders.Commands.DeleteWorkOrder;
using FSM.Application.Features.WorkOrders.Commands.UpdateWorkOrder;
using FSM.Application.Features.WorkOrders.Queries.GetAllWorkOrders;
using FSM.Application.Features.WorkOrders.Queries.GetWorkOrderById;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FSM.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WorkOrdersController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;

        public WorkOrdersController(IMediator mediator, IMapper mapper)
        {
            _mediator = mediator;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<WorkOrderDto>>> GetAll()
        {
            var result = await _mediator.Send(new GetAllWorkOrdersQuery());
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<WorkOrderDto>> GetById(int id)
        {
            var result = await _mediator.Send(new GetWorkOrderByIdQuery(id));
            if (result == null)
                return NotFound(new { message = $"{id} numaralı iş emri sistemde bulunamadı." });
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<int>> Create([FromBody] CreateWorkOrderDto dto)
        {
            var command = _mapper.Map<CreateWorkOrderCommand>(dto);
            var newId = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetById), new { id = newId }, newId);
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpdateWorkOrderCommand command)
        {
            // Artık 'var result' yok, çünkü Unit hiçbir şey döndürmez.
            // İşlem başarılıysa buraya kadar gelir, hata varsa zaten Exception fırlatır.
            await _mediator.Send(command);

            return Ok(new { message = "İş emri başarıyla güncellendi." });
        }

        [HttpPost("assign")]
        public async Task<IActionResult> AssignWorkOrder([FromBody] AssignWorkOrderDto dto)
        {
            var command = new AssignWorkOrderCommand { WorkOrderId = dto.WorkOrderId, TechnicianId = dto.TechnicianId };
            await _mediator.Send(command);
            return Ok(new { Message = "İş emri başarıyla teknisyene atandı!" });
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _mediator.Send(new DeleteWorkOrderCommand { Id = id });
            return Ok(new { message = "İş emri başarıyla iptal edildi/pasife çekildi." });
        }
    }
}