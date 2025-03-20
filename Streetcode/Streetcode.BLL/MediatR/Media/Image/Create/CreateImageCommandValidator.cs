using FluentValidation;
using Streetcode.BLL.Constants;
using Streetcode.BLL.Extensions;
using Streetcode.BLL.Resources;

namespace Streetcode.BLL.MediatR.Media.Image.Create;

public class CreateImageCommandValidator : AbstractValidator<CreateImageCommand>
{
    public CreateImageCommandValidator()
    {
        RuleFor(x => x.Image.Title)
            .NotEmpty().WithMessage(ValidatorMessages.TitleIsRequired)
            .MaximumLength(ValidatorConstants.TitleMaxLength).WithFormatedMessage(ValidatorMessages.TitleMaxLength, ValidatorConstants.TitleMaxLength);

        RuleFor(x => x.Image.BaseFormat)
            .NotEmpty().WithMessage(ValidatorMessages.BaseFormatIsRequired);

        RuleFor(x => x.Image.MimeType)
            .NotEmpty().WithMessage(ValidatorMessages.MimeTypeIsRequired)
            .Matches(ValidatorConstants.MimeTypeRegularExpression).WithMessage(ValidatorMessages.MimeTypeFormat);

        RuleFor(x => x.Image.Extension)
            .NotEmpty().WithMessage(ValidatorMessages.ExtensionIsRequired)
            .Matches(ValidatorConstants.ExtensionRegularExpression).WithMessage(ValidatorMessages.ExtensionFormat);

        RuleFor(x => x.Image.Alt)
            .MaximumLength(ValidatorConstants.ImageAltMaxLength).WithFormatedMessage(ValidatorMessages.ImageAltMaxLength, ValidatorConstants.ImageAltMaxLength);
    }
}
