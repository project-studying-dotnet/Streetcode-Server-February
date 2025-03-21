using Microsoft.AspNetCore.Mvc;
using Streetcode.BLL.MediatR.News.GetAll;
using Streetcode.BLL.MediatR.News.GetByUrl;

namespace Streetcode.WebApi.Controllers.News;

public class NewsController : BaseApiController
{
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        return HandleResult(await Mediator.Send(new GetAllNewsQuery()));
    }

    [HttpGet("{url}")]
    public async Task<IActionResult> GetByUrl([FromRoute] string url)
    {
        return HandleResult(await Mediator.Send(new GetNewsByUrlQuery(url)));
    }
}
