using FluentValidation;
using Streetcode.BLL.Resources;

namespace Streetcode.BLL.MediatR.Media.Audio.Delete;

public class DeleteAudioCommandValidator : AbstractValidator<DeleteAudioCommand>
{
    public DeleteAudioCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage(ValidatorMessages.IdMustBeGreaterThanZero);
    }
}
