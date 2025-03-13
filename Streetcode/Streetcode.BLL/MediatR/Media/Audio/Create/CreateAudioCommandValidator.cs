using FluentValidation;
using Streetcode.BLL.Constants;

namespace Streetcode.BLL.MediatR.Media.Audio.Create;

public class CreateAudioCommandValidator : AbstractValidator<CreateAudioCommand>
{
    public CreateAudioCommandValidator()
    {
        RuleFor(x => x.Audio.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MaximumLength(ValidatorsConstants.TitleMaxLength).WithMessage($"Title must be at most {ValidatorsConstants.TitleMaxLength} characters.");

        RuleFor(x => x.Audio.BaseFormat)
            .NotEmpty().WithMessage("BaseFormat is required.")
            .MaximumLength(ValidatorsConstants.BaseFormatMaxLength).WithMessage($"BaseFormat must be at most {ValidatorsConstants.BaseFormatMaxLength} characters.");

        RuleFor(x => x.Audio.MimeType)
            .NotEmpty().WithMessage("MimeType is required.")
            .Matches(ValidatorsConstants.MimeTypeRegularExpression).WithMessage("MimeType must be a valid format (e.g., 'image/png').");

        RuleFor(x => x.Audio.Extension)
            .NotEmpty().WithMessage("Extension is required.")
            .Matches(ValidatorsConstants.ExtensionRegularExpression).WithMessage("Extension must start with a dot followed by alphanumeric characters (e.g., '.jpg').");

        RuleFor(x => x.Audio.Description)
            .MaximumLength(ValidatorsConstants.DescriptionMaxLength).WithMessage($"Description must be at most {ValidatorsConstants.DescriptionMaxLength} characters.");
    }
}