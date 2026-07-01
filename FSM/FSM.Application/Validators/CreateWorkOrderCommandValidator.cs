using FluentValidation;
using FSM.Application.DTOs;
using FSM.Application.DTOs.WorkOrders;
using FSM.Application.Features.WorkOrders.Commands.CreateWorkOrder;

namespace FSM.Application.Validators;

public class CreateWorkOrderCommandValidator : AbstractValidator<CreateWorkOrderCommand>
{
    public CreateWorkOrderCommandValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Başlık alanı boş bırakılamaz!")
            .MinimumLength(3).WithMessage("Başlık en az 3 karakter olmalıdır!");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Açıklama alanı boş bırakılamaz!")
            .MaximumLength(500).WithMessage("Açıklama 500 karakteri geçemez!");

        RuleFor(x => x.CustomerId)
            .GreaterThan(0).WithMessage("Geçerli bir müşteri seçmelisiniz!");
            
    }
}