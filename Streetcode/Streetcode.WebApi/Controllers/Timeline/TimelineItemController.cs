using Microsoft.AspNetCore.Mvc;
using Streetcode.BLL.DTO.Timeline.TimelineItem;
using Streetcode.BLL.MediatR.Timeline.TimelineItem.Create;
using Streetcode.BLL.MediatR.Timeline.TimelineItem.GetAll;
using Streetcode.BLL.MediatR.Timeline.TimelineItem.GetById;
using Streetcode.BLL.MediatR.Timeline.TimelineItem.GetByStreetcodeId;
using Streetcode.BLL.MediatR.Timeline.TimelineItem.Update;
using Streetcode.BLL.MediatR.Timeline.TimelineItem.Delete;

namespace Streetcode.WebApi.Controllers.Timeline;

public class TimelineItemController : BaseApiController
{
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        return HandleResult(await Mediator.Send(new GetAllTimelineItemsQuery()));
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById([FromRoute] int id)
    {
        return HandleResult(await Mediator.Send(new GetTimelineItemByIdQuery(id)));
    }

    [HttpGet("{streetcodeId:int}")]
    public async Task<IActionResult> GetByStreetcodeId([FromRoute] int streetcodeId)
    {
        return HandleResult(await Mediator.Send(new GetTimelineItemsByStreetcodeIdQuery(streetcodeId)));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] TimelineItemCreateDTO timelineItemCreateDTO)
    {
        return HandleResult(await Mediator.Send(
            new CreateTimelineItemCommand(timelineItemCreateDTO)));
    }

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] TimelineItemUpdateDTO timelineItemDTO)
    {
        return HandleResult(await Mediator.Send(
            new UpdateTimelineItemCommand(timelineItemDTO)));
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteTimelineItem(int id)
    {
        return HandleResult(await Mediator.Send(new DeleteTimelineItemCommand(id)));
    }
}
