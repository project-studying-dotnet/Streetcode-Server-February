using FluentValidation;

namespace Streetcode.BLL.MediatR.Payment;

public class CreateInvoiceCommandValidator : AbstractValidator<CreateInvoiceCommand>
{
    public CreateInvoiceCommandValidator()
    {
        RuleFor(x => x.Payment.Amount)
            .GreaterThan(0).WithMessage("Payment amount must be greater than zero.");
    }
}