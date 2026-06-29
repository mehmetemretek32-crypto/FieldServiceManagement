using FluentValidation;
using FSM.Application.DTOs.Customer;
namespace FSM.Application.Validators;

public class UpdateCustomerDtoValidator : AbstractValidator<UpdateCustomerDto>
{
    public UpdateCustomerDtoValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("Geçerli bir müşteri ID'si girilmelidir.");

        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("Müşteri adı boş bırakılamaz.")
            .MinimumLength(2).WithMessage("Müşteri adı en az 2 karakter olmalıdır.");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Müşteri soyadı boş bırakılamaz.");

        RuleFor(x => x.CompanyName)
            .NotEmpty().WithMessage("Şirket adı boş bırakılamaz.");
    }
}