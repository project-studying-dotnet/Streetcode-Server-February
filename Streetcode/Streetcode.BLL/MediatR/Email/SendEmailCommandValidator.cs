using FluentValidation;
using Streetcode.BLL.Constants;

namespace Streetcode.BLL.MediatR.Email;

public class SendEmailCommandValidator : AbstractValidator<SendEmailCommand>
{
    public SendEmailCommandValidator()
    {
        RuleFor(x => x.Email.From)
            .NotEmpty().WithMessage("Sender email is required.")
            .MaximumLength(ValidatorsConstants.EmailFromMaxLength).WithMessage($"Sender email must not exceed {ValidatorsConstants.EmailFromMaxLength} characters.")
            .EmailAddress().WithMessage("Invalid email format.");

        RuleFor(x => x.Email.Content)
            .NotEmpty().WithMessage("Email content is required.")
            .MaximumLength(ValidatorsConstants.EmailContentMaxLength).WithMessage($"Email content must not exceed {ValidatorsConstants.EmailContentMaxLength} characters.");
    }
}