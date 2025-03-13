using FluentValidation;
using Streetcode.BLL.Constants;

namespace Streetcode.BLL.MediatR.Team.Position.Create;

public class CreatePositionCommandValidator : AbstractValidator<CreatePositionCommand>
{
    public CreatePositionCommandValidator()
    {
        RuleFor(x => x.Position.Position)
            .MinimumLength(ValidatorsConstants.PositionMinLength)
            .MaximumLength(ValidatorsConstants.PositionMaxLength)
            .WithMessage(
                $"Position length must be between {ValidatorsConstants.PositionMinLength} and {ValidatorsConstants.PositionMaxLength} characters");
    }
}