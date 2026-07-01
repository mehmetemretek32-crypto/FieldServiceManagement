using FluentValidation;
using FSM.Application.DTOs.WorkOrders;
using FSM.Application.Features.WorkOrders.Commands.AssignWorkOrder;

namespace FSM.Application.Validators
{
    public class AssignWorkOrderCommandValidator : AbstractValidator<AssignWorkOrderCommand>
    {
        public AssignWorkOrderCommandValidator()
        {
            RuleFor(x => x.WorkOrderId)
                .GreaterThan(0).WithMessage("Geçerli bir İş Emri ID'si gereklidir.");

            RuleFor(x => x.TechnicianId)
                .GreaterThan(0).WithMessage("Geçerli bir Teknisyen ID'si gereklidir.");
        }
    }
}