using FluentValidation;
using Streetcode.BLL.Constants;
using Streetcode.BLL.SharedValidators.Partners;

namespace Streetcode.BLL.MediatR.Partners.Update;

public class UpdatePartnerCommandValidator : AbstractValidator<UpdatePartnerCommand>
{
    public UpdatePartnerCommandValidator()
    {
        RuleFor(x => x.Partner.Id)
            .GreaterThan(0).WithMessage("Id must be greater than 0 for an update.");

        RuleFor(x => x.Partner.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MaximumLength(ValidatorsConstants.TitleMaxLength).WithMessage($"Title must not exceed {ValidatorsConstants.TitleMaxLength} characters.");

        RuleFor(x => x.Partner.Description)
            .MaximumLength(ValidatorsConstants.DescriptionMaxLength).WithMessage($"Description must not exceed {ValidatorsConstants.DescriptionMaxLength} characters.");

        RuleFor(x => x.Partner.TargetUrl)
            .Must(url => Uri.TryCreate(url, UriKind.Absolute, out _))
            .When(x => !string.IsNullOrEmpty(x.Partner.TargetUrl))
            .WithMessage("TargetUrl must be a valid URL.");

        RuleFor(x => x.Partner.LogoId)
            .GreaterThan(0).WithMessage("LogoId must be greater than 0.");

        RuleFor(x => x.Partner.UrlTitle)
            .MaximumLength(ValidatorsConstants.UrlTitleMaxLength).WithMessage($"UrlTitle must not exceed {ValidatorsConstants.UrlTitleMaxLength} characters.")
            .When(x => !string.IsNullOrEmpty(x.Partner.UrlTitle));

        RuleFor(x => x.Partner.PartnerSourceLinks)
            .NotEmpty().WithMessage("PartnerSourceLinks cannot be empty.")
            .When(x => x.Partner.PartnerSourceLinks != null);

        RuleForEach(x => x.Partner.PartnerSourceLinks)
            .SetValidator(new CreatePartnerSourceLinkDtoValidator());

        RuleFor(x => x.Partner.Streetcodes)
            .NotEmpty().WithMessage("At least one Streetcode is required.");

        RuleForEach(x => x.Partner.Streetcodes)
            .SetValidator(new StreetcodeShortDtoValidator());
    }
}