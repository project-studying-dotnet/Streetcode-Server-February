using FluentResults;
using MediatR;
using Streetcode.BLL.DTO.Timeline.TimelineItem;

namespace Streetcode.BLL.MediatR.Timeline.TimelineItem.Create;

public record CreateTimelineItemCommand
    (TimelineItemCreateDTO TimelineItemDTO)
    : IRequest<Result<Unit>>
{
}
