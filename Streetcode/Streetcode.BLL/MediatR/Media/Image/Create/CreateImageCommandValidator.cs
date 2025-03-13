using FluentValidation;
using Streetcode.BLL.Constants;

namespace Streetcode.BLL.MediatR.Media.Image.Create;

public class CreateImageCommandValidator : AbstractValidator<CreateImageCommand>
{
    public CreateImageCommandValidator()
    {
        RuleFor(x => x.Image.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MaximumLength(ValidatorsConstants.TitleMaxLength).WithMessage($"Title must be at most {ValidatorsConstants.TitleMaxLength} characters.");

        RuleFor(x => x.Image.BaseFormat)
            .NotEmpty().WithMessage("BaseFormat is required.")
            .MaximumLength(ValidatorsConstants.BaseFormatMaxLength).WithMessage($"BaseFormat must be at most {ValidatorsConstants.BaseFormatMaxLength} characters.");

        RuleFor(x => x.Image.MimeType)
            .NotEmpty().WithMessage("MimeType is required.")
            .Matches(ValidatorsConstants.MimeTypeRegularExpression).WithMessage("MimeType must be a valid format (e.g., 'image/png').");

        RuleFor(x => x.Image.Extension)
            .NotEmpty().WithMessage("Extension is required.")
            .Matches(ValidatorsConstants.ExtensionRegularExpression).WithMessage("Extension must start with a dot followed by alphanumeric characters (e.g., '.jpg').");

        RuleFor(x => x.Image.Alt)
            .NotEmpty().WithMessage("Alt text is required.")
            .MaximumLength(ValidatorsConstants.ImageAltMaxLength).WithMessage($"Alt text must be at most {ValidatorsConstants.ImageAltMaxLength} characters.");
    }
}