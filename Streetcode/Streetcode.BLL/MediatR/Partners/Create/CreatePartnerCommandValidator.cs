using FluentValidation;
using Streetcode.BLL.Constants;
using Streetcode.BLL.Extensions;
using Streetcode.BLL.MediatR.Partners.SharedValidators;
using Streetcode.BLL.Resources;

namespace Streetcode.BLL.MediatR.Partners.Create;

public class CreatePartnerCommandValidator : AbstractValidator<CreatePartnerCommand>
{
    public CreatePartnerCommandValidator()
    {
        RuleFor(x => x.NewPartner.Title)
            .NotEmpty().WithMessage(ValidatorMessages.TitleIsRequired)
            .MaximumLength(ValidatorConstants.TitleMaxLength).WithFormatedMessage(ValidatorMessages.TitleMaxLength, ValidatorConstants.TitleMaxLength);

        RuleFor(x => x.NewPartner.Description)
            .MaximumLength(ValidatorConstants.DescriptionMaxLength).WithFormatedMessage(ValidatorMessages.DescriptionMaxLength, ValidatorConstants.DescriptionMaxLength);

        RuleFor(x => x.NewPartner.TargetUrl)
            .Must(url => Uri.TryCreate(url, UriKind.Absolute, out _))
            .When(x => !string.IsNullOrEmpty(x.NewPartner.TargetUrl)).WithMessage(ValidatorMessages.TargetUrlMustBeValid);

        RuleFor(x => x.NewPartner.LogoId)
            .GreaterThan(0).WithMessage(ValidatorMessages.LogoIdMustBeGreaterThanZero);

        RuleFor(x => x.NewPartner.UrlTitle)
            .MaximumLength(ValidatorConstants.UrlTitleMaxLength).WithFormatedMessage(ValidatorMessages.UrlTitleMaxLength, ValidatorConstants.UrlTitleMaxLength)
            .When(x => !string.IsNullOrEmpty(x.NewPartner.UrlTitle));

        RuleFor(x => x.NewPartner.PartnerSourceLinks)
            .NotEmpty().WithMessage(ValidatorMessages.PartnerSourceLinksIsRequired)
            .When(x => x.NewPartner.PartnerSourceLinks != null);

        RuleForEach(x => x.NewPartner.PartnerSourceLinks)
            .SetValidator(new CreatePartnerSourceLinkDtoValidator());

        RuleFor(x => x.NewPartner.Streetcodes)
            .NotEmpty().WithMessage(ValidatorMessages.StreetCodeIsRequired);

        RuleForEach(x => x.NewPartner.Streetcodes)
            .SetValidator(new StreetcodeShortDtoValidator());
    }
}
