using MediatR;

namespace FSM.Application.Features.Auth.Commands;

public class RegisterCommand : IRequest<bool>
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;

    // Şimdilik varsayılan olarak Technician atayabiliriz veya dışarıdan alabiliriz
    public string Role { get; set; } = "Technician";
}