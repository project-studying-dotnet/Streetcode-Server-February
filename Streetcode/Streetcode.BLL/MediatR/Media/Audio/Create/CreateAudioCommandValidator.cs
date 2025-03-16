using FluentValidation;
using Streetcode.BLL.Constants;
using Streetcode.BLL.Extensions;
using Streetcode.BLL.Resources;

namespace Streetcode.BLL.MediatR.Media.Audio.Create;

public class CreateAudioCommandValidator : AbstractValidator<CreateAudioCommand>
{
    public CreateAudioCommandValidator()
    {
        RuleFor(x => x.Audio.Title)
            .NotEmpty().WithMessage(ValidatorMessages.TitleIsRequired)
            .MaximumLength(ValidatorConstants.TitleMaxLength).WithFormatedMessage(ValidatorMessages.TitleMaxLength, ValidatorConstants.TitleMaxLength);

        RuleFor(x => x.Audio.BaseFormat)
            .NotEmpty().WithMessage(ValidatorMessages.BaseFormatIsRequired)
            .MaximumLength(ValidatorConstants.BaseFormatMaxLength).WithFormatedMessage(ValidatorMessages.BaseFormatMaxLength, ValidatorConstants.BaseFormatMaxLength);

        RuleFor(x => x.Audio.MimeType)
            .NotEmpty().WithMessage(ValidatorMessages.MimeTypeIsRequired)
            .Matches(ValidatorConstants.MimeTypeRegularExpression).WithMessage(ValidatorMessages.MimeTypeFormat);

        RuleFor(x => x.Audio.Extension)
            .NotEmpty().WithMessage(ValidatorMessages.ExtensionIsRequired)
            .Matches(ValidatorConstants.ExtensionRegularExpression).WithMessage(ValidatorMessages.ExtensionFormat);

        RuleFor(x => x.Audio.Description)
            .MaximumLength(ValidatorConstants.DescriptionMaxLength).WithFormatedMessage(ValidatorMessages.DescriptionMaxLength, ValidatorConstants.DescriptionMaxLength);
    }
}
