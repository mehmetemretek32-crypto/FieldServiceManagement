using FluentValidation;

namespace FSM.Application.Features.WorkOrders.Commands.CreateWorkOrder;

public class CreateWorkOrderCommandValidator : AbstractValidator<CreateWorkOrderCommand>
{
    public CreateWorkOrderCommandValidator()
    {
        RuleFor(x => x.CustomerId).GreaterThan(0).WithMessage("Lütfen geçerli bir müşteri seçin.");

        // Teknisyen kuralını sildik, yerine başlık kuralı ekledik
        RuleFor(x => x.Title).NotEmpty().WithMessage("İş emri başlığı boş bırakılamaz.");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("İş emri açıklaması boş bırakılamaz.")
            .MinimumLength(10).WithMessage("Açıklama çok kısa, lütfen en az 10 karakterlik bir detay girin.");
    }
}