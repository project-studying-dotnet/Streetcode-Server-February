using FluentValidation;
using Streetcode.BLL.Constants;
using Streetcode.BLL.SharedValidators.Partners;

namespace Streetcode.BLL.MediatR.Partners.Create;

public class CreatePartnerCommandValidator : AbstractValidator<CreatePartnerCommand>
{
    public CreatePartnerCommandValidator()
    {
        RuleFor(x => x.NewPartner.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MaximumLength(ValidatorsConstants.TitleMaxLength).WithMessage($"Title must not exceed {ValidatorsConstants.TitleMaxLength} characters.");

        RuleFor(x => x.NewPartner.Description)
            .MaximumLength(ValidatorsConstants.DescriptionMaxLength).WithMessage($"Description must not exceed {ValidatorsConstants.DescriptionMaxLength} characters.");

        RuleFor(x => x.NewPartner.TargetUrl)
            .Must(url => Uri.TryCreate(url, UriKind.Absolute, out _))
            .When(x => !string.IsNullOrEmpty(x.NewPartner.TargetUrl)).WithMessage("TargetUrl must be a valid URL.");

        RuleFor(x => x.NewPartner.LogoId)
            .GreaterThan(0).WithMessage("LogoId must be greater than 0.");

        RuleFor(x => x.NewPartner.UrlTitle)
            .MaximumLength(ValidatorsConstants.UrlTitleMaxLength).WithMessage($"UrlTitle must not exceed {ValidatorsConstants.UrlTitleMaxLength} characters.")
            .When(x => !string.IsNullOrEmpty(x.NewPartner.UrlTitle));

        RuleFor(x => x.NewPartner.PartnerSourceLinks)
            .NotEmpty().WithMessage("PartnerSourceLinks cannot be empty.")
            .When(x => x.NewPartner.PartnerSourceLinks != null);

        RuleForEach(x => x.NewPartner.PartnerSourceLinks)
            .SetValidator(new CreatePartnerSourceLinkDtoValidator());

        RuleFor(x => x.NewPartner.Streetcodes)
            .NotEmpty().WithMessage("At least one Streetcode is required.");

        RuleForEach(x => x.NewPartner.Streetcodes)
            .SetValidator(new StreetcodeShortDtoValidator());
    }
}
