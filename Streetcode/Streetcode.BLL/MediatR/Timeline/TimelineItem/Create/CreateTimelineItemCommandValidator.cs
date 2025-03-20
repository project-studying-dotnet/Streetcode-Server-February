using FluentValidation;
using Streetcode.BLL.Validators.Timeline.TimelineItem;

namespace Streetcode.BLL.MediatR.Timeline.TimelineItem.Create;

public class CreateTimelineItemCommandValidator
    : AbstractValidator<CreateTimelineItemCommand>
{
    public CreateTimelineItemCommandValidator(
        BaseTimelineItemValidator timelineItemRules)
    {
        RuleFor(x => x.TimelineItemDTO).SetValidator(timelineItemRules);
    }
}
