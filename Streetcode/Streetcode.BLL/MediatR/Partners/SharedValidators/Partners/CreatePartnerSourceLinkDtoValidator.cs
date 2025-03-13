using FluentValidation;
using Streetcode.BLL.DTO.Partners.Create;

namespace Streetcode.BLL.SharedValidators.Partners;

public class CreatePartnerSourceLinkDtoValidator : AbstractValidator<CreatePartnerSourceLinkDTO>
{
    public CreatePartnerSourceLinkDtoValidator()
    {
        RuleFor(x => x.TargetUrl)
            .NotEmpty().WithMessage("URL is required.")
            .Must(url => Uri.TryCreate(url, UriKind.Absolute, out _))
            .WithMessage("Invalid URL format.");
    }
}