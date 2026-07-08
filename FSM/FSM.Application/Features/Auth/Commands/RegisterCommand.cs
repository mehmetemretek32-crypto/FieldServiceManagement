using MediatR;

namespace FSM.Application.Features.Auth.Commands;

public class RegisterCommand : IRequest<bool>
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;

    // Rol, istemci tarafından belirlenemez (yetki yükseltme riski). Sunucu
    // tarafında güvenli bir varsayılan atanır; ayrıcalıklı roller yalnızca
    // yetkili bir yönetici uç noktası tarafından verilmelidir.
}