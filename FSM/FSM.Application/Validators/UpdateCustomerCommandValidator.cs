using FluentValidation;
using FSM.Application.DTOs.Customer;
using FSM.Application.Features.Customers.Commands.UpdateCustomer;
namespace FSM.Application.Validators;

public class UpdateCustomerCommandValidator : AbstractValidator<UpdateCustomerCommand>
{
    public UpdateCustomerCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("Geçerli bir müşteri ID'si girilmelidir.");

        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("Müşteri adı boş bırakılamaz.")
            .MinimumLength(2).WithMessage("Müşteri adı en az 2 karakter olmalıdır.");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Müşteri soyadı boş bırakılamaz.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("E-posta adresi boş bırakılamaz.")
            .EmailAddress().WithMessage("Geçerli bir e-posta adresi giriniz.");

        RuleFor(x => x.PhoneNumber)
            .NotEmpty().WithMessage("Telefon numarası boş bırakılamaz.")
            .Matches(@"^\+?\d{10,15}$").WithMessage("Geçerli bir telefon numarası giriniz.");

        RuleFor(x => x.CompanyName)
            .NotEmpty().WithMessage("Şirket adı boş bırakılamaz.");

        RuleFor(x => x.Address)
            .NotEmpty().WithMessage("Adres boş bırakılamaz.");
    }
}