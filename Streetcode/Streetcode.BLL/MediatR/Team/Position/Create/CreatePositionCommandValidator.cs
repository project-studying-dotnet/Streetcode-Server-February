using FluentValidation;
using Streetcode.BLL.Constants;
using Streetcode.BLL.Extensions;
using Streetcode.BLL.Resources;

namespace Streetcode.BLL.MediatR.Team.Position.Create;

public class CreatePositionCommandValidator : AbstractValidator<CreatePositionCommand>
{
    public CreatePositionCommandValidator()
    {
        RuleFor(x => x.Position.Position)
            .MinimumLength(ValidatorConstants.PositionMinLength)
            .MaximumLength(ValidatorConstants.PositionMaxLength)
            .WithFormatedMessage(ValidatorMessages.PositionMinAndMaxLength, ValidatorConstants.PositionMinLength, ValidatorConstants.PositionMaxLength);
    }
}
