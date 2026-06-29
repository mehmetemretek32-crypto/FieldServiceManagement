using FluentValidation;
using FSM.Application.DTOs.WorkOrders;

namespace FSM.Application.Validators
{
    public class AssignWorkOrderDtoValidator : AbstractValidator<AssignWorkOrderDto>
    {
        public AssignWorkOrderDtoValidator()
        {
            RuleFor(x => x.WorkOrderId)
                .GreaterThan(0).WithMessage("Geçerli bir İş Emri ID'si gereklidir.");

            RuleFor(x => x.TechnicianId)
                .GreaterThan(0).WithMessage("Geçerli bir Teknisyen ID'si gereklidir.");
        }
    }
}