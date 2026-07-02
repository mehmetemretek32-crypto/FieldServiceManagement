using FSM.Application.Features.Auth.Commands;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FSM.WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterCommand command)
    {
        // MediatR, RegisterCommand'i alır ve otomatik olarak RegisterCommandHandler'a iletir.
        var result = await _mediator.Send(command);

        return Ok(new { Success = result, Message = "Kullanıcı başarıyla kaydedildi." });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginCommand command)
    {
        // MediatR, LoginCommand'i alır ve sonucu (Token) geri döner.
        var token = await _mediator.Send(command);

        return Ok(new { Token = token, Message = "Giriş başarılı." });
    }

    [Authorize] // İŞTE BU BİZİM KİLİDİMİZ!
    [HttpGet("guvenli-alan")]
    public IActionResult ProtectedArea()
    {
        return Ok(new { Message = "Tebrikler! Güvenlik duvarını aştın ve içeridesin." });
    }
}