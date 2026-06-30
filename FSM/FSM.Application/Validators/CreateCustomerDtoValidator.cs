using FluentValidation;
using FSM.Application.DTOs;

namespace FSM.Application.Validators;

public class CreateCustomerDtoValidator : AbstractValidator<CreateCustomerDto>
{
    public CreateCustomerDtoValidator()
    {
        // Not: Create işleminde Id kontrolü YAPMIYORUZ çünkü Id'yi veritabanı kendisi verecek.

        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("Müşteri adı boş bırakılamaz.")
            .MinimumLength(2).WithMessage("Müşteri adı en az 2 karakter olmalıdır.")
            .MaximumLength(50).WithMessage("Müşteri adı en fazla 50 karakter olabilir.");
            
        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Müşteri soyadı boş bırakılamaz.")
            .MinimumLength(2).WithMessage("Müşteri soyadı en az 2 karakter olmalıdır.")
            .MaximumLength(50).WithMessage("Müşteri soyadı en fazla 50 karakter olabilir.");

        RuleFor(x => x.CompanyName)
            .NotEmpty().WithMessage("Şirket adı boş bırakılamaz.");

       RuleFor(x => x.PhoneNumber)
            .NotEmpty().WithMessage("Telefon numarası boş bırakılamaz.")
            .Matches(@"^\+?\d{10,15}$").WithMessage("Geçerli bir telefon numarası giriniz.");

        RuleFor(x => x.Address)
            .NotEmpty().WithMessage("Adres boş bırakılamaz.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("E-posta adresi boş bırakılamaz.")
            .EmailAddress().WithMessage("Geçerli bir e-posta adresi giriniz.");
       
    }
}