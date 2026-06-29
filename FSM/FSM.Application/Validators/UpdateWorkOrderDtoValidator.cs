using FluentValidation;
using FSM.Application.DTOs.WorkOrders;


namespace FSM.Application.Validators;

public class UpdateWorkOrderDtoValidator : AbstractValidator<UpdateWorkOrderDto>
{
    public UpdateWorkOrderDtoValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("Güncellenecek iş emri için geçerli bir ID girilmelidir.");

        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Başlık alanı boş bırakılamaz.")
            .MinimumLength(3).WithMessage("Başlık en az 3 karakter olmalıdır.")
            .MaximumLength(100).WithMessage("Başlık 100 karakteri geçemez.");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Açıklama alanı boş bırakılamaz.")
            .MinimumLength(5).WithMessage("Açıklama çok kısa, lütfen detay giriniz.");
    }
}