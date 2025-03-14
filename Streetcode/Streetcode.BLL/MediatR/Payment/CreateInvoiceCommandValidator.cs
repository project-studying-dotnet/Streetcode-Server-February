using FluentValidation;
using Streetcode.BLL.Resources;

namespace Streetcode.BLL.MediatR.Payment;

public class CreateInvoiceCommandValidator : AbstractValidator<CreateInvoiceCommand>
{
    public CreateInvoiceCommandValidator()
    {
        RuleFor(x => x.Payment.Amount)
            .GreaterThan(0).WithMessage(ValidatorMessages.PaymentAmountMustBeGreaterThanZero);
    }
}
