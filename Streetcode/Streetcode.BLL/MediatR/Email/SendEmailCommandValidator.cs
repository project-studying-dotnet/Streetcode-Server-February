using System.Globalization;
using FluentValidation;
using Streetcode.BLL.Constants;
using Streetcode.BLL.Extensions;
using Streetcode.BLL.Resources;

namespace Streetcode.BLL.MediatR.Email;

public class SendEmailCommandValidator : AbstractValidator<SendEmailCommand>
{
    public SendEmailCommandValidator()
    {
        RuleFor(x => x.Email.From)
            .NotEmpty().WithMessage(ValidatorMessages.EmailSenderIsRequired)
            .MaximumLength(ValidatorConstants.EmailFromMaxLength).WithFormatedMessage(ValidatorMessages.EmailFromMaxLength, ValidatorConstants.EmailFromMaxLength)
            .EmailAddress().WithMessage(ValidatorMessages.EmailFormat);

        RuleFor(x => x.Email.Content)
            .NotEmpty().WithMessage(ValidatorMessages.EmailContentIsRequired)
            .MaximumLength(ValidatorConstants.EmailContentMaxLength).WithFormatedMessage(ValidatorMessages.EmailContentMaxLength, ValidatorConstants.EmailContentMaxLength);
    }
}
