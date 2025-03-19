using FluentValidation;
using Streetcode.BLL.Resources;
using Streetcode.BLL.Validators.Timeline.TimelineItem;

namespace Streetcode.BLL.MediatR.Timeline.TimelineItem.Update;

public class UpdateTimelineItemCommandValidator
    : AbstractValidator<UpdateTimelineItemCommand>
{
    public UpdateTimelineItemCommandValidator(
        BaseTimelineItemValidator timelineItemRules)
    {
        RuleFor(x => x.TimelineItemDTO)
            .SetValidator(timelineItemRules);

        RuleFor(x => x.TimelineItemDTO.Id)
            .NotEmpty().WithMessage(ValidatorMessages.IdIsRequired)
            .GreaterThan(0).WithMessage(ValidatorMessages.IdMustBeGreaterThanZero);
    }
}
