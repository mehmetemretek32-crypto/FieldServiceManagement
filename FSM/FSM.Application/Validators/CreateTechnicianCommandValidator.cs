using FluentValidation;
using FSM.Application.DTOs;
using FSM.Application.DTOs.Technicians;
using FSM.Application.Features.Technican.Commands.CreateTechnician;

namespace FSM.Application.Validators
{
    public class CreateTechnicianCommandValidator : AbstractValidator<CreateTechnicianCommand>
    {
        public CreateTechnicianCommandValidator()
        {
            // İsim alanı kuralları
            RuleFor(x => x.FullName)
                .NotEmpty().WithMessage("Teknisyen adı boş bırakılamaz.")
                .MinimumLength(2).WithMessage("Teknisyen adı en az 2 karakter olmalıdır.")
                .MaximumLength(50).WithMessage("Teknisyen adı en fazla 50 karakter olabilir.");

            // E-posta kuralları
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("E-posta adresi zorunludur.")
                .EmailAddress().WithMessage("Geçerli bir e-posta adresi giriniz.");

            // Telefon numarası kuralları
            RuleFor(x => x.PhoneNumber)
                .NotEmpty().WithMessage("Telefon numarası zorunludur.")
                .Matches(@"^\+?\d{10,15}$").WithMessage("Geçerli bir telefon numarası giriniz. (10-15 haneli, isteğe bağlı '+' ile başlayabilir)");

        }
    }
}