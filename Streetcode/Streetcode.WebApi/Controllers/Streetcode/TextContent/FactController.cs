using Microsoft.AspNetCore.Mvc;
using Streetcode.BLL.MediatR.Streetcode.Fact.GetAll;
using Streetcode.BLL.MediatR.Streetcode.Fact.GetById;
using Streetcode.BLL.MediatR.Streetcode.Fact.GetByStreetcodeId;
using Streetcode.BLL.DTO.Streetcode.TextContent.Fact;
using Streetcode.BLL.MediatR.Streetcode.Fact.Reorder;
using Moq;

namespace Streetcode.WebApi.Controllers.Streetcode.TextContent;

public class FactController : BaseApiController
{
    private readonly Mock<IMapper> _mockMapper;

    public FactController(Mock<IMapper> mockMapper)
    {
        _mockMapper = mockMapper;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        return HandleResult(await Mediator.Send(new GetAllFactsQuery()));
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById([FromRoute] int id)
    {
        return HandleResult(await Mediator.Send(new GetFactByIdQuery(id)));
    }

    [HttpGet("{streetcodeId:int}")]
    public async Task<IActionResult> GetByStreetcodeId([FromRoute] int streetcodeId)
    {
        return HandleResult(await Mediator.Send(new GetFactByStreetcodeIdQuery(streetcodeId)));
    }

    [HttpPut("reorder")]
    public async Task<IActionResult> ReorderFacts(
        [FromBody] IEnumerable<ReorderFactDto> facts)
    {
        return HandleResult(await Mediator.Send(new ReorderFactsCommand(facts)));
    }

    private static List<ReorderFactDTO> GetTestFacts()
    {
        return new List<ReorderFactDTO>
        {
            new() { Id = 1, Index = 2 },
            new() { Id = 2, Index = 0 },
            new() { Id = 3, Index = 1 }
        };
    }

    private void SetupMocks()
    {
        _mockMapper.Setup(m => m.Map<Fact>(It.IsAny<ReorderFactDTO>()))
            .Returns<ReorderFactDTO>(dto => new Fact { Id = dto.Id, Index = dto.Index });
    }
}
