using FluentValidation;
using Streetcode.BLL.DTO.Streetcode;

namespace Streetcode.BLL.SharedValidators.Partners;

public class StreetcodeShortDtoValidator : AbstractValidator<StreetcodeShortDTO>
{
    public StreetcodeShortDtoValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("Streetcode Id must be greater than 0.");
    }
}