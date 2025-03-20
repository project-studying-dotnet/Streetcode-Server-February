using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using FluentResults;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Streetcode.BLL.MediatR.Timeline.TimelineItem.Delete;
using Xunit;
using MediatR;

namespace Streetcode.XUnitTest.MediatRTests.Timeline.TimelineItem.Delete;

public class TimelineItemControllerTests
{
    private readonly Mock<IMediator> _mockMediator;
    private readonly MockTimelineItemController _controller;

    public TimelineItemControllerTests()
    {
        _mockMediator = new Mock<IMediator>();
        _controller = new MockTimelineItemController(_mockMediator.Object);
    }

    [Fact]
    public async Task DeleteTimelineItem_ShouldReturnNoContent_WhenSuccessful()
    {
        // Arrange
        _mockMediator
            .Setup(m => m.Send(
                It.IsAny<DeleteTimelineItemCommand>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Ok(Unit.Value));

        // Act
        var result = await _controller.DeleteTimelineItem(1);

        // Assert
        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task DeleteTimelineItem_ShouldCallMediator_WithCorrectCommand()
    {
        // Arrange
        _mockMediator
            .Setup(m => m.Send(
                It.IsAny<DeleteTimelineItemCommand>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Ok(Unit.Value));

        // Act
        await _controller.DeleteTimelineItem(1);

        // Assert
        _mockMediator.Verify(
            x => x.Send(
                It.Is<DeleteTimelineItemCommand>(cmd => cmd.Id == 1),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task DeleteTimelineItem_ShouldReturnNotFound_WhenTimelineItemDoesNotExist()
    {
        // Arrange
        var errorMessage = "Cannot find timeline item";
        _mockMediator
            .Setup(m => m.Send(
                It.IsAny<DeleteTimelineItemCommand>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Fail(new Error(errorMessage)));

        // Act
        var result = await _controller.DeleteTimelineItem(1);

        // Assert
        result.Should().BeOfType<NotFoundObjectResult>();
        (result as NotFoundObjectResult) !.Value.Should().Be(errorMessage);
    }

    private class MockTimelineItemController : ControllerBase
    {
        private readonly IMediator _mediator;

        public MockTimelineItemController(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<IActionResult> DeleteTimelineItem(int id)
        {
            return HandleResult(await _mediator.Send(new DeleteTimelineItemCommand(id)));
        }

        protected IActionResult HandleResult<T>(Result<T> result)
        {
            if (result.IsSuccess)
            {
                if (result.Value == null)
                {
                    return NotFound();
                }

                if (typeof(T) == typeof(Unit))
                {
                    return NoContent();
                }

                return Ok(result.Value);
            }

            return NotFound(result.Errors.First().Message);
        }
    }
}
