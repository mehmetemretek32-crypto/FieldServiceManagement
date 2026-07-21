using FSM.Application.Features.Users.Command.UpdateUSerProfile;
using FSM.Application.Features.Users.Commands.ChangePassword;
using FSM.Application.Features.Users.Commands.UpdateUserProfile;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FSM.WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize] // Bu sınıftaki işlemleri sadece giriş yapmış (Token'ı olan) kişiler yapabilir
public class UsersController : ControllerBase
{
    private readonly IMediator _mediator;

    public UsersController(IMediator mediator)
    {
        _mediator = mediator;
    }
    

    public class UpdateUserProfileRequest
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }

    public class ChangePasswordRequest
    {
        public string CurrentPassword { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
        public string ConfirmNewPassword { get; set; } = string.Empty;
    }

    [HttpPut("update-profile")]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateUserProfileRequest request)
    {
        // 1. Token içinden giriş yapan kullanıcının ID'sini güvenli bir şekilde alıyoruz
        var userIdStr = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (!int.TryParse(userIdStr, out int userId))
            return Unauthorized("Kullanıcı kimliği doğrulanamadı.");

        // 2. Dışarıdan gelen Request'i ve Token'dan gelen ID'yi birleştirip asıl Command'ı oluşturuyoruz!
        var command = new UpdateUserProfileCommand(
            userId,
            request.FirstName,
            request.LastName,
            request.Email
        );

        // 3. İşi Handler'a devret
        var result = await _mediator.Send(command);
        return Ok(new { Message = "Profil bilgileriniz başarıyla güncellendi." });
    }

    [HttpPut("change-password")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
    {
        // 1. Token içinden giriş yapan kullanıcının ID'sini alıyoruz
        var userIdStr = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (!int.TryParse(userIdStr, out int userId))
            return Unauthorized("Kullanıcı kimliği doğrulanamadı.");

        // 2. Gerçek Command nesnesini burada güvenli bir şekilde inşa ediyoruz
        var command = new ChangePasswordCommand(
            userId,
            request.CurrentPassword,
            request.NewPassword,
            request.ConfirmNewPassword
        );

        // 3. İşi Handler'a devret
        var result = await _mediator.Send(command);
        return Ok(new { Message = "Şifreniz başarıyla değiştirildi." });
    }
}