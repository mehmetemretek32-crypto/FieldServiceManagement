using FluentValidation;
using FSM.Application.DTOs.Technicians;
using FSM.Application.Features.Technicians.Commands.UpdateTechnician;

public class UpdateTechnicianCommandValidator : AbstractValidator<UpdateTechnicianCommand>
{
    public UpdateTechnicianCommandValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0).WithMessage("Geçerli bir teknisyen ID giriniz.");
        RuleFor(x => x.FullName).NotEmpty().MinimumLength(3).WithMessage("İsim boş olamaz.");
        RuleFor(x => x.Email).NotEmpty().EmailAddress().WithMessage("Geçerli bir e-posta adresi giriniz.");
        RuleFor(x => x.PhoneNumber).NotEmpty().Matches(@"^\+?\d{10,15}$").WithMessage("Geçerli bir telefon numarası giriniz.");
    }
}