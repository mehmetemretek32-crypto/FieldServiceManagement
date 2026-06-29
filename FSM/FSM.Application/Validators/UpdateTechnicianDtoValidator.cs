using FluentValidation;
using FSM.Application.DTOs.Technicians;

public class UpdateTechnicianDtoValidator : AbstractValidator<UpdateTechnicianDto>
{
    public UpdateTechnicianDtoValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0).WithMessage("Geçerli bir teknisyen ID giriniz.");
        RuleFor(x => x.FullName).NotEmpty().MinimumLength(3).WithMessage("İsim boş olamaz.");
    }
}