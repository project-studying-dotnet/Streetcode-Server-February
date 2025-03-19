using FluentValidation;
using Streetcode.BLL.Resources;

namespace Streetcode.BLL.MediatR.Timeline.TimelineItem.Delete;

public class DeleteTimelineItemValidaror
    : AbstractValidator<DeleteTimelineItemCommand>
{
    public DeleteTimelineItemValidaror()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage(ValidatorMessages.IdIsRequired)
            .GreaterThan(0).WithMessage(ValidatorMessages.IdMustBeGreaterThanZero);
    }
}
