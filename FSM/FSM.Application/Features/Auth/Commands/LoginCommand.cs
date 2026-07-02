using MediatR;

namespace FSM.Application.Features.Auth.Commands;

// IRequest<string> diyoruz çünkü başarılı giriş sonucunda geriye Token (string) dönecek
public class LoginCommand : IRequest<string>
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}