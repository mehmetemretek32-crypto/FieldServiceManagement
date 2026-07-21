using MediatR;

namespace FSM.Application.Features.Users.Commands.ChangePassword;

public record ChangePasswordCommand(
    int UserId,
    string CurrentPassword,
    string NewPassword,
    string ConfirmNewPassword
) : IRequest<bool>;