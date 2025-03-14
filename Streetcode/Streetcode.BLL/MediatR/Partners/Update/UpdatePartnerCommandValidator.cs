using FluentValidation;
using Streetcode.BLL.Constants;
using Streetcode.BLL.Extensions;
using Streetcode.BLL.MediatR.Partners.SharedValidators;
using Streetcode.BLL.Resources;

namespace Streetcode.BLL.MediatR.Partners.Update;

public class UpdatePartnerCommandValidator : AbstractValidator<UpdatePartnerCommand>
{
    public UpdatePartnerCommandValidator()
    {
        RuleFor(x => x.Partner.Id)
            .GreaterThan(0).WithMessage(ValidatorMessages.IdMustBeGreaterThanZero);

        RuleFor(x => x.Partner.Title)
            .NotEmpty().WithMessage(ValidatorMessages.TitleIsRequired)
            .MaximumLength(ValidatorConstants.TitleMaxLength).WithFormatedMessage(ValidatorMessages.TitleMaxLength, ValidatorConstants.TitleMaxLength);

        RuleFor(x => x.Partner.Description)
            .MaximumLength(ValidatorConstants.DescriptionMaxLength).WithFormatedMessage(ValidatorMessages.DescriptionMaxLength, ValidatorConstants.DescriptionMaxLength);

        RuleFor(x => x.Partner.TargetUrl)
            .Must(url => Uri.TryCreate(url, UriKind.Absolute, out _))
            .When(x => !string.IsNullOrEmpty(x.Partner.TargetUrl))
            .WithMessage(ValidatorMessages.TargetUrlMustBeValid);

        RuleFor(x => x.Partner.LogoId)
            .GreaterThan(0).WithMessage(ValidatorMessages.LogoIdMustBeGreaterThanZero);

        RuleFor(x => x.Partner.UrlTitle)
            .MaximumLength(ValidatorConstants.UrlTitleMaxLength).WithFormatedMessage(ValidatorMessages.UrlTitleMaxLength, ValidatorConstants.UrlTitleMaxLength)
            .When(x => !string.IsNullOrEmpty(x.Partner.UrlTitle));

        RuleFor(x => x.Partner.PartnerSourceLinks)
            .NotEmpty().WithMessage(ValidatorMessages.PartnerSourceLinksIsRequired)
            .When(x => x.Partner.PartnerSourceLinks != null);

        RuleForEach(x => x.Partner.PartnerSourceLinks)
            .SetValidator(new CreatePartnerSourceLinkDtoValidator());

        RuleFor(x => x.Partner.Streetcodes)
            .NotEmpty().WithMessage(ValidatorMessages.StreetCodeIsRequired);

        RuleForEach(x => x.Partner.Streetcodes)
            .SetValidator(new StreetcodeShortDtoValidator());
    }
}
