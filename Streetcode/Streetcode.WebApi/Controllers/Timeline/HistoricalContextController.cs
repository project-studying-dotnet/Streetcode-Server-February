using Microsoft.AspNetCore.Mvc;
using Streetcode.BLL.MediatR.Timeline.HistoricalContext.GetAll;
using Streetcode.BLL.MediatR.Timeline.HistoricalContext.Delete;

namespace Streetcode.WebApi.Controllers.Timeline;

public class HistoricalContextController : BaseApiController
{
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        return HandleResult(await Mediator.Send(new GetAllHistoricalContextQuery()));
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteHistoricalContext(int id)
    {
        return HandleResult(await Mediator.Send(new DeleteHistoryEventCommand(id)));
    }
}