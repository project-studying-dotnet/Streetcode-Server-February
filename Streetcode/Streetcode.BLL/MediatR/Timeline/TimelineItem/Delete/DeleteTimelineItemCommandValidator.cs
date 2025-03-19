using FluentValidation;
using Streetcode.BLL.Resources;

namespace Streetcode.BLL.MediatR.Timeline.TimelineItem.Delete;

public class DeleteTimelineItemCommandValidator
    : AbstractValidator<DeleteTimelineItemCommand>
{
    public DeleteTimelineItemCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage(ValidatorMessages.IdIsRequired)
            .GreaterThan(0).WithMessage(ValidatorMessages.IdMustBeGreaterThanZero);
    }
}
