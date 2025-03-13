using FluentValidation;

namespace Streetcode.BLL.MediatR.Media.Audio.Delete;

public class DeleteAudioCommandValidator : AbstractValidator<DeleteAudioCommand>
{
    public DeleteAudioCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("Id must be greater than 0.");
    }
}