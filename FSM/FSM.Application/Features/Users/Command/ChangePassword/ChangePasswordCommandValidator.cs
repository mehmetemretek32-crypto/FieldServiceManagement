using FluentValidation;

namespace FSM.Application.Features.Users.Commands.ChangePassword;

public class ChangePasswordCommandValidator : AbstractValidator<ChangePasswordCommand>
{
    public ChangePasswordCommandValidator()
    {
        RuleFor(x => x.UserId)
            .GreaterThan(0).WithMessage("Geçersiz kullanıcı kimliği.");

        RuleFor(x => x.CurrentPassword)
            .NotEmpty().WithMessage("Mevcut şifrenizi girmelisiniz.");

        RuleFor(x => x.NewPassword)
            .NotEmpty().WithMessage("Yeni şifre alanı boş bırakılamaz.")
            .MinimumLength(6).WithMessage("Yeni şifreniz en az 6 karakter olmalıdır.")
            .Matches("[A-Z]").WithMessage("Yeni şifreniz en az bir büyük harf içermelidir.")
            .Matches("[0-9]").WithMessage("Yeni şifreniz en az bir rakam içermelidir.");

        RuleFor(x => x.ConfirmNewPassword)
            .Equal(x => x.NewPassword).WithMessage("Yeni şifreler birbiriyle uyuşmuyor.");
    }
}