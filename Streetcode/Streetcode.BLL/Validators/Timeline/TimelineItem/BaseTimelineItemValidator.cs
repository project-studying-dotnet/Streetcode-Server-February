using FluentValidation;
using Streetcode.BLL.Constants;
using Streetcode.BLL.DTO.Timeline.TimelineItem;
using Streetcode.BLL.Extensions;
using Streetcode.BLL.Resources;
using Streetcode.DAL.Enums;

namespace Streetcode.BLL.Validators.Timeline.TimelineItem;

public class BaseTimelineItemValidator
    : AbstractValidator<TimelineItemCreateUpdateDTO>
{
    public BaseTimelineItemValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage(ValidatorMessages.TitleIsRequired)
            .MaximumLength(ValidatorConstants.TimelineItemTitleMaxLength)
            .WithFormatedMessage(
            ValidatorMessages.TitleMaxLength,
            ValidatorConstants.TimelineItemTitleMaxLength);

        RuleFor(x => x.Description)
            .MaximumLength(ValidatorConstants.TimelineItemDescriptionMaxLength)
            .WithFormatedMessage(
            ValidatorMessages.DescriptionMaxLength,
            ValidatorConstants.TimelineItemDescriptionMaxLength);

        RuleFor(x => x.Date)
            .NotEmpty().WithMessage(ValidatorMessages.DateIsRequired);

        int enumCount = Enum.GetNames<DateViewPattern>().Length - 1;
        RuleFor(x => x.DateViewPattern)
            .NotEmpty().WithMessage(ValidatorMessages.DateViewPatternIsRequired)
            .Must(value => Enum.IsDefined(value))
            .WithFormatedMessage(ValidatorMessages.InvalidDateViewPattern, enumCount);

        RuleFor(x => x.StreetcodeId)
            .NotEmpty().WithMessage(ValidatorMessages.StreetcodeIdIsRequired)
            .GreaterThan(0).WithMessage(ValidatorMessages.IdMustBeGreaterThanZero);
    }
}
