using FluentValidation;
using FSM.Application.Features.Inventories.Commands.CreateInventoryItem;

namespace FSM.Application.Validators;

public class CreateInventoryItemCommandValidator : AbstractValidator<CreateInventoryItemCommand>
{
    public CreateInventoryItemCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Malzeme adı boş bırakılamaz.")
            .MinimumLength(2).WithMessage("Malzeme adı en az 2 karakter olmalıdır.")
            .MaximumLength(100).WithMessage("Malzeme adı en fazla 100 karakter olabilir.");

        RuleFor(x => x.SkuCode)
            .NotEmpty().WithMessage("Stok (SKU) kodu boş bırakılamaz.")
            .MinimumLength(3).WithMessage("Stok kodu en az 3 karakter olmalıdır.")
            .MaximumLength(50).WithMessage("Stok kodu en fazla 50 karakter olabilir.");

        // 🔥 GÜVENLİK DOKUNUŞU: Stok eksiye düşemez!
        RuleFor(x => x.StockQuantity)
            .GreaterThanOrEqualTo(0).WithMessage("Stok adedi 0'dan küçük olamaz. Geçerli bir stok giriniz.");

        // 🔥 GÜVENLİK DOKUNUŞU: Bedava veya üzerine para verdiğimiz malzeme olmaz!
        RuleFor(x => x.UnitPrice)
            .GreaterThan(0).WithMessage("Birim fiyatı 0'dan büyük olmalıdır.");
    }
}